﻿using OrdersService.Services;

namespace OrdersService.Services;

public class OutboxBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxBackgroundService> _logger;

    public OutboxBackgroundService(IServiceProvider serviceProvider, ILogger<OutboxBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var outboxProcessor = scope.ServiceProvider.GetRequiredService<IOutboxProcessor>();
                
                var processedCount = await outboxProcessor.ProcessOutboxMessagesAsync(stoppingToken);
                
                if (processedCount > 0)
                {
                    _logger.LogInformation("Processed {Count} outbox messages", processedCount);
                }

                await Task.Delay(5000, stoppingToken); // Проверяем каждые 5 секунд
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing outbox messages");
                await Task.Delay(10000, stoppingToken); // При ошибке ждем 10 секунд
            }
        }
    }
}