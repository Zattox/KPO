using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using TextScanner.FileAnalysisService.Data;

Env.Load();

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

builder.Services.AddHttpClient("FileStoringService", client =>
{
    client.BaseAddress =
        new Uri(fileStoringServiceUrl!); // Добавляем !, чтобы указать компилятору, что значение не null
});

builder.Services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new() { Title = "File Analysis Service", Version = "v1" }); });

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "File Analysis Service v1"));
app.UseAuthorization();
app.MapControllers();

app.Run();