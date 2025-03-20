using BankHSE.Domain.Abstractions;
using BankHSE.Domain.Enums;

namespace BankHSE.Domain.Entities;

public class BankAccount : IIdentifiable, ICoreEntityVisitable
{
    public Guid Id { get; }
    public string Name { get; private set; }
    public decimal Balance { get; private set; }

    public BankAccount(string name, decimal balance)
    {
        Id = Guid.NewGuid();
        Name = name;
        Balance = balance;
    }

    public void ApplyOperation(Operation operation)
    {
        if (operation.BankAccountId != Id)
            throw new ArgumentException("Operation does not belong to this account.");
        if (operation.Type == TransactionType.Income)
            IncreaseBalance(operation.Amount);
        else
            DecreaseBalance(operation.Amount);
    }

    public void IncreaseBalance(decimal amount) => Balance += amount;
    public void DecreaseBalance(decimal amount) => Balance -= amount;
    public void UpdateAccountName(string name) => Name = name;

    public void Accept(ICoreEntityVisitor visitor)
    {
        visitor.Visit(this);
    }
}