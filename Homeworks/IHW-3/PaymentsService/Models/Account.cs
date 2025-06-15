namespace PaymentsService.Models;

public class Account
{
    public string UserId { get; set; } = string.Empty; // Первичный ключ
    public decimal Balance { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int Version { get; set; }
}
