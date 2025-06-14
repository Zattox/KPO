using Microsoft.EntityFrameworkCore;
using PaymentsService.Data;
using PaymentsService.Models;
using System.Text.Json;

namespace PaymentsService.Services;

public interface IInboxProcessor
{
    Task<int> ProcessInboxMessagesAsync(CancellationToken cancellationToken = default);
}

public class InboxProcessor : IInboxProcessor
{
    private readonly PaymentDbContext _context;
    private readonly IPaymentService _paymentService;
    private const int BatchSize = 10;

    public InboxProcessor(PaymentDbContext context, IPaymentService paymentService)
    {
        _context = context;
        _paymentService = paymentService;
    }

    public async Task<int> ProcessInboxMessagesAsync(CancellationToken cancellationToken = default)
    {
        var messages = await _context.InboxMessages
            .Where(m => m.ProcessedOn == null)
            .OrderBy(m => m.OccurredOn)
            .Take(BatchSize)
            .ToListAsync(cancellationToken);

        if (!messages.Any())
            return 0;

        int processedCount = 0;
        foreach (var message in messages)
        {
            try
            {
                await ProcessMessageAsync(message, cancellationToken);
                processedCount++;
            }
            catch (Exception ex)
            {
                message.ErrorMessage = ex.Message;
                message.RetryCount++;
                
                if (message.RetryCount >= 3)
                {
                    message.ProcessedOn = DateTime.UtcNow;
                }
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return processedCount;
    }

    private async Task ProcessMessageAsync(InboxMessage message, CancellationToken cancellationToken)
    {
        switch (message.Type)
        {
            case nameof(OrderCreatedEvent):
                await ProcessOrderCreatedEventAsync(message, cancellationToken);
                break;
            default:
                throw new NotSupportedException($"Message type {message.Type} is not supported");
        }
    }

    private async Task ProcessOrderCreatedEventAsync(InboxMessage message, CancellationToken cancellationToken)
    {
        var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(message.Content);
        if (orderEvent != null)
        {
            await _paymentService.ProcessOrderPaymentAsync(
                orderEvent.OrderId,
                orderEvent.UserId.ToString(),
                orderEvent.Amount);
            
            message.ProcessedOn = DateTime.UtcNow;
        }
    }
}
