namespace PaymentsService.Models;

public class PaymentTransaction
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public TransactionStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? ErrorMessage { get; set; }
}

public enum TransactionType
{
    DEBIT,
    CREDIT
}

public enum TransactionStatus
{
    PENDING,
    COMPLETED,
    FAILED
}