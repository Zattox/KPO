using DotNetEnv;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Gateway", Version = "v1" });
    
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

var fileStoringServiceUrl = Environment.GetEnvironmentVariable("SERVICES_FILESTORINGSERVICE");
if (string.IsNullOrEmpty(fileStoringServiceUrl))
{
    throw new InvalidOperationException("FileStoringService URL must be specified in the environment variables.");
}

var fileAnalysisServiceUrl = Environment.GetEnvironmentVariable("SERVICES_FILEANALYSISSERVICE");
if (string.IsNullOrEmpty(fileAnalysisServiceUrl))
{
    throw new InvalidOperationException("FileAnalysisService URL must be specified in the environment variables.");
}

builder.Services.AddHttpClient("FileStoringService", client =>
{
    client.BaseAddress = new Uri(fileStoringServiceUrl!);
});

builder.Services.AddHttpClient("FileAnalysisService", client =>
{
    client.BaseAddress = new Uri(fileAnalysisServiceUrl!);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Gateway v1"));
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();