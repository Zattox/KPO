using Microsoft.EntityFrameworkCore;
using PaymentsService.Data;
using RabbitMQ.Client;
using System.Text;

namespace PaymentsService.Services;

public interface IOutboxProcessor
{
    Task<int> ProcessOutboxMessagesAsync(CancellationToken cancellationToken = default);
}

public class OutboxProcessor : IOutboxProcessor
{
    private readonly PaymentDbContext _context;
    private readonly ConnectionFactory _connectionFactory;
    private const int BatchSize = 10;

    public OutboxProcessor(PaymentDbContext context, ConnectionFactory connectionFactory)
    {
        _context = context;
        _connectionFactory = connectionFactory;
    }

    public async Task<int> ProcessOutboxMessagesAsync(CancellationToken cancellationToken = default)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var messages = await _context.OutboxMessages
                .Where(m => m.ProcessedOn == null)
                .OrderBy(m => m.OccurredOn)
                .Take(BatchSize)
                .ToListAsync(cancellationToken);

            if (!messages.Any())
                return 0;

            using var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
            using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

            await channel.ExchangeDeclareAsync(
                exchange: "payments", 
                type: ExchangeType.Topic, 
                durable: true,
                cancellationToken: cancellationToken);

            foreach (var message in messages)
            {
                var body = Encoding.UTF8.GetBytes(message.Content);
                await channel.BasicPublishAsync(
                    exchange: "payments",
                    routingKey: message.Type,
                    body: body,
                    cancellationToken: cancellationToken);

                message.ProcessedOn = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return messages.Count;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
