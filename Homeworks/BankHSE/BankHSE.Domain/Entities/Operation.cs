﻿using BankHSE.Domain.Enums;
using BankHSE.Domain.Abstractions;

namespace BankHSE.Domain.Entities;

public class Operation : IIdentifiable, ICoreEntityVisitable
{
    public Guid Id { get; }
    public TransactionType Type { get; }
    public Guid BankAccountId { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime Date { get; private set; }
    public string Description { get; private set; }
    public Guid CategoryId { get; private set; }

    public Operation(Guid id, TransactionType type, Guid bankAccountId, decimal amount, DateTime date,
        string description, Guid categoryId)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be positive.");
        Id = id;
        Type = type;
        BankAccountId = bankAccountId;
        Amount = amount;
        Date = date;
        Description = description;
        CategoryId = categoryId;
    }

    public Operation(TransactionType type, Guid bankAccountId, decimal amount, DateTime date, string description,
        Guid categoryId)
        : this(Guid.NewGuid(), type, bankAccountId, amount, date, description, categoryId)
    {
    }

    public void UpdateOpeationDescription(string description) => Description = description;

    public void Accept(ICoreEntityVisitor visitor) => visitor.Visit(this);
}