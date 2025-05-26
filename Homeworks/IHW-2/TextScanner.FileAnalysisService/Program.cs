using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using TextScanner.FileAnalysisService.Data;
using Serilog;

Env.Load();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/fileanalysis-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
    var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
    var database = Environment.GetEnvironmentVariable("DB_DATABASE") ?? "fileanalysis_db";
    var username = Environment.GetEnvironmentVariable("DB_USERNAME") ?? "textscanner";
    var password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "password123";

    connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password}";
}

builder.Services.AddDbContext<AnalysisDbContext>(options =>
    options.UseNpgsql(connectionString));

var fileStoringServiceUrl =
    Environment.GetEnvironmentVariable("SERVICES_FILESTORINGSERVICE") ?? "http://localhost:5001";

builder.Services.AddHttpClient("FileStoringService", client =>
{
    client.BaseAddress = new Uri(fileStoringServiceUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new()
        {
            Title = "File Analysis Service", Version = "v1", Description = "A service for analyzing text files"
        });
});

builder.Services.AddHealthChecks()
    .AddCheck("database", () =>
    {
        try
        {
            using var scope = builder.Services.BuildServiceProvider().CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AnalysisDbContext>();
            context.Database.CanConnect();
            return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy();
        }
        catch
        {
            return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy();
        }
    });

builder.Host.UseSerilog();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AnalysisDbContext>();
    var maxRetries = 10;
    var delay = TimeSpan.FromSeconds(5);
    
    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            context.Database.EnsureCreated();
            Log.Information("Database ensured created for FileStoringService");
            break;
        }
        catch (Exception ex)
        {
            Log.Warning(ex, $"Attempt {i + 1} failed to create database. Retrying in {delay.TotalSeconds} seconds...");
            if (i == maxRetries - 1)
            {
                Log.Error(ex, "Failed to create database after {MaxRetries} attempts", maxRetries);
                throw;
            }
            await Task.Delay(delay);
        }
    }
}

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "File Analysis Service v1"));
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();