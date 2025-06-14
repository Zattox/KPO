using Microsoft.EntityFrameworkCore;
using PaymentsService.Data;
using PaymentsService.Models;
using System.Text.Json;

namespace PaymentsService.Services;

public interface IPaymentService
{
    Task<Account> CreateAccountAsync(string userId, decimal initialBalance = 0);
    Task<decimal?> GetBalanceAsync(string userId);
    Task DepositAsync(string userId, decimal amount);
    Task ProcessOrderPaymentAsync(Guid orderId, string userId, decimal amount);
    // Добавляем недостающие методы для совместимости
    Task<Account?> GetAccountByUserIdAsync(string userId);
}

public class PaymentService : IPaymentService
{
    private readonly PaymentDbContext _context;

    public PaymentService(PaymentDbContext context)
    {
        _context = context;
    }
    
    public async Task<Account?> GetAccountByUserIdAsync(string userId)
    {
        return await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == userId);
    }

    public async Task<Account> CreateAccountAsync(string userId, decimal initialBalance = 0)
    {
        var existingAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == userId);
        if (existingAccount != null)
        {
            throw new InvalidOperationException("Account already exists for this user");
        }

        var account = new Account
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Balance = initialBalance,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Version = 1
        };

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        return account;
    }

    public async Task<decimal?> GetBalanceAsync(string userId)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == userId);
        return account?.Balance;
    }

    public async Task DepositAsync(string userId, decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be positive");

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == userId);
            if (account == null)
                throw new InvalidOperationException("Account not found");

            account.Balance += amount;
            account.UpdatedAt = DateTime.UtcNow;
            account.Version++;

            var paymentTransaction = new PaymentTransaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                OrderId = Guid.Empty,
                Amount = amount,
                Type = TransactionType.CREDIT,
                Status = TransactionStatus.COMPLETED,
                CreatedAt = DateTime.UtcNow
            };

            _context.PaymentTransactions.Add(paymentTransaction);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task ProcessOrderPaymentAsync(Guid orderId, string userId, decimal amount)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Проверяем, не обрабатывали ли мы уже этот заказ (идемпотентность)
            var existingTransaction = await _context.PaymentTransactions
                .FirstOrDefaultAsync(t => t.OrderId == orderId && t.Type == TransactionType.DEBIT);
            
            if (existingTransaction != null)
            {
                return; // Уже обработан
            }

            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == userId);
            if (account == null)
            {
                await CreatePaymentFailedEventAsync(orderId, userId, "Account not found");
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return;
            }

            if (account.Balance < amount)
            {
                await CreatePaymentFailedEventAsync(orderId, userId, "Insufficient funds");
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return;
            }

            // Списываем средства
            account.Balance -= amount;
            account.UpdatedAt = DateTime.UtcNow;
            account.Version++;

            var paymentTransaction = new PaymentTransaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                OrderId = orderId,
                Amount = amount,
                Type = TransactionType.DEBIT,
                Status = TransactionStatus.COMPLETED,
                CreatedAt = DateTime.UtcNow
            };

            _context.PaymentTransactions.Add(paymentTransaction);

            // Создаем событие об успешной оплате
            await CreatePaymentSuccessEventAsync(orderId, userId, amount);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            await CreatePaymentFailedEventAsync(orderId, userId, ex.Message);
            throw;
        }
    }

    private async Task CreatePaymentSuccessEventAsync(Guid orderId, string userId, decimal amount)
    {
        var paymentEvent = new PaymentProcessedEvent
        {
            OrderId = orderId,
            UserId = userId,
            Amount = amount,
            Success = true,
            ErrorMessage = null,
            OccurredOn = DateTime.UtcNow
        };

        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = nameof(PaymentProcessedEvent),
            Content = JsonSerializer.Serialize(paymentEvent),
            OccurredOn = DateTime.UtcNow
        };

        _context.OutboxMessages.Add(outboxMessage);
        await Task.CompletedTask;
    }

    private async Task CreatePaymentFailedEventAsync(Guid orderId, string userId, string errorMessage)
    {
        var paymentEvent = new PaymentProcessedEvent
        {
            OrderId = orderId,
            UserId = userId,
            Amount = 0,
            Success = false,
            ErrorMessage = errorMessage,
            OccurredOn = DateTime.UtcNow
        };

        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = nameof(PaymentProcessedEvent),
            Content = JsonSerializer.Serialize(paymentEvent),
            OccurredOn = DateTime.UtcNow
        };

        _context.OutboxMessages.Add(outboxMessage);
        await Task.CompletedTask;
    }
}
