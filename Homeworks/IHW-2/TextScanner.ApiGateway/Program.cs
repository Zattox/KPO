using DotNetEnv;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "API Gateway", Version = "v1" });
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
    client.BaseAddress = new Uri(fileStoringServiceUrl!); // Указываем, что значение не null
});

builder.Services.AddHttpClient("FileAnalysisService", client =>
{
    client.BaseAddress = new Uri(fileAnalysisServiceUrl!); // Указываем, что значение не null
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Gateway v1"));
app.UseAuthorization();
app.MapControllers();

app.Run();