using DotNetEnv;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;

Env.Load();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/apigateway-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "API Gateway",
            Version = "v1",
            Description = "API Gateway для управления файлами и их анализом"
        });
});

var fileStoringServiceUrl = Environment.GetEnvironmentVariable("SERVICES_FILESTORINGSERVICE") ?? "http://localhost:5001";
var fileAnalysisServiceUrl = Environment.GetEnvironmentVariable("SERVICES_FILEANALYSISSERVICE") ?? "http://localhost:5002";

builder.Services.AddHttpClient("FileStoringService", client =>
{
    client.BaseAddress = new Uri(fileStoringServiceUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient("FileAnalysisService", client =>
{
    client.BaseAddress = new Uri(fileAnalysisServiceUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHealthChecks();
builder.Host.UseSerilog();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Gateway v1"));
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

Log.Information("Starting API Gateway...");
app.Run();