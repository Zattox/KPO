using DotNetEnv;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;

Env.Load();

// Configure Serilog for logging to console and file
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/apigateway-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add controllers
builder.Services.AddControllers();

// Configure Swagger for API documentation
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "API Gateway",
            Version = "v1",
            Description = "API Gateway для управления файлами и их анализом"
        });
    c.DocInclusionPredicate((docName, apiDesc) =>
    {
        if (apiDesc.ActionDescriptor == null)
            return false;

        var controllerActionDescriptor = apiDesc.ActionDescriptor as ControllerActionDescriptor;
        if (controllerActionDescriptor == null || controllerActionDescriptor.ControllerTypeInfo == null)
            return false;

        var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        var controllerAssembly = controllerActionDescriptor.ControllerTypeInfo.Assembly.GetName().Name;
        return controllerAssembly == assemblyName;
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Configure HTTP clients for FileStoringService and FileAnalysisService
var fileStoringServiceUrl = Environment.GetEnvironmentVariable("SERVICES_FILESTORINGSERVICE");
var fileAnalysisServiceUrl = Environment.GetEnvironmentVariable("SERVICES_FILEANALYSISSERVICE");

if (string.IsNullOrEmpty(fileStoringServiceUrl) || string.IsNullOrEmpty(fileAnalysisServiceUrl))
{
    Log.Error("Environment variables SERVICES_FILESTORINGSERVICE or SERVICES_FILEANALYSISSERVICE are not set.");
    throw new InvalidOperationException(
        "FileStoringService or FileAnalysisService URL must be specified in the environment variables.");
}

builder.Services.AddHttpClient("FileStoringService",
    client => { client.BaseAddress = new Uri(fileStoringServiceUrl); });

builder.Services.AddHttpClient("FileAnalysisService",
    client => { client.BaseAddress = new Uri(fileAnalysisServiceUrl); });

// Use Serilog as the logging provider
builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Gateway v1"));
app.UseAuthorization();
app.MapControllers();

// Log application startup
Log.Information("Starting API Gateway...");

app.Run();