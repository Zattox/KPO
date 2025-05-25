using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using TextScanner.FileAnalysisService.Data;
using Serilog;


Env.Load();

// Настройка Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/fileanalysis-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<AnalysisDbContext>(options =>
    options.UseNpgsql($"Host={Environment.GetEnvironmentVariable("DB_HOST")};" +
                      $"Port={Environment.GetEnvironmentVariable("DB_PORT")};" +
                      $"Database={Environment.GetEnvironmentVariable("DB_DATABASE")};" +
                      $"Username={Environment.GetEnvironmentVariable("DB_USERNAME")};" +
                      $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")}"));

var fileStoringServiceUrl = Environment.GetEnvironmentVariable("SERVICES_FILESTORINGSERVICE");
if (string.IsNullOrEmpty(fileStoringServiceUrl))
{
    throw new InvalidOperationException("FileStoringService URL must be specified in the environment variables.");
}

builder.Services.AddHttpClient("FileStoringService",
    client => { client.BaseAddress = new Uri(fileStoringServiceUrl!); });

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new()
        {
            Title = "File Analysis Service", Version = "v1", Description = "Сервис для анализа текстовых файлов"
        });
});

builder.Host.UseSerilog();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "File Analysis Service v1"));
app.UseAuthorization();
app.MapControllers();

app.Run();