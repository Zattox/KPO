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

// Configure connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    // Fallback to environment variables if connection string is not in config
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
            Title = "File Analysis Service", Version = "v1", Description = "Сервис для анализа текстовых файлов"
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

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AnalysisDbContext>();
    try
    {
        context.Database.EnsureCreated();
        Log.Information("Database ensured created for FileAnalysisService");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Error ensuring database creation");
    }
}

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "File Analysis Service v1"));
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();