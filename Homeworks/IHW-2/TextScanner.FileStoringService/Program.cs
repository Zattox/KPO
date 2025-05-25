using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using TextScanner.FileStoringService.Data;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<FileStorageDbContext>(options =>
    options.UseNpgsql($"Host={Environment.GetEnvironmentVariable("DB_HOST")};" +
                      $"Port={Environment.GetEnvironmentVariable("DB_PORT")};" +
                      $"Database={Environment.GetEnvironmentVariable("DB_DATABASE")};" +
                      $"Username={Environment.GetEnvironmentVariable("DB_USERNAME")};" +
                      $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")}"));

builder.Services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new() { Title = "File Storing Service", Version = "v1" }); });

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "File Storing Service v1"));
app.UseAuthorization();
app.MapControllers();

app.Run();