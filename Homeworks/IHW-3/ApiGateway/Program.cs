using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "API Gateway", Version = "v1" });
});

builder.WebHost.UseUrls("http://0.0.0.0:80");

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseCors("AllowAll");


app.UseSwagger();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API Gateway");
        options.SwaggerEndpoint("/proxy/orders/swagger/v1/swagger.json", "Orders Service");
        options.SwaggerEndpoint("/proxy/payments/swagger/v1/swagger.json", "Payments Service");
        options.RoutePrefix = string.Empty;
    });
}


app.MapHealthChecks("/health");
app.MapReverseProxy();

app.Run();