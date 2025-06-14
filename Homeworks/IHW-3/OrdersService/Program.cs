using Microsoft.EntityFrameworkCore;
using OrdersService.Data;
using OrdersService.Services;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:80");
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<ConnectionFactory>(sp =>
{
    return new ConnectionFactory()
    {
        HostName = builder.Configuration.GetValue<string>("RabbitMQ:HostName") ?? "localhost",
        UserName = builder.Configuration.GetValue<string>("RabbitMQ:UserName") ?? "guest",
        Password = builder.Configuration.GetValue<string>("RabbitMQ:Password") ?? "guest"
    };
});

builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOutboxProcessor, OutboxProcessor>();
builder.Services.AddHostedService<OutboxBackgroundService>();

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    await context.Database.MigrateAsync();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.MapHealthChecks("/health");
app.MapControllers();


app.Run();