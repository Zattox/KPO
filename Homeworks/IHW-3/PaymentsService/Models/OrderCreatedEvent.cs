﻿namespace PaymentsService.Models;

public class OrderCreatedEvent
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime OccurredOn { get; set; }
}