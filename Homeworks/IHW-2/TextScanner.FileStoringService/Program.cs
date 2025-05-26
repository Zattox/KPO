using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using TextScanner.FileStoringService.Data;
using Serilog;

Env.Load();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/filestoring-.txt", rollingInterval: RollingInterval.Day)
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
    var database = Environment.GetEnvironmentVariable("DB_DATABASE") ?? "filestoring_db";
    var username = Environment.GetEnvironmentVariable("DB_USERNAME") ?? "textscanner";
    var password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "password123";

    connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password}";
}

builder.Services.AddDbContext<FileStorageDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new() { Title = "File Storing Service", Version = "v1", Description = "Сервис для хранения файлов" });
});

builder.Services.AddHealthChecks()
    .AddCheck("database", () =>
    {
        try
        {
            using var scope = builder.Services.BuildServiceProvider().CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<FileStorageDbContext>();
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
    var context = scope.ServiceProvider.GetRequiredService<FileStorageDbContext>();
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
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "File Storing Service v1"));
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();