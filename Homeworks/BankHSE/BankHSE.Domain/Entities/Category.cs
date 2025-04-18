using BankHSE.Domain.Enums;
using BankHSE.Domain.Abstractions;

namespace BankHSE.Domain.Entities;

public class Category : IIdentifiable, ICoreEntityVisitable
{
    public Guid Id { get; }
    public TransactionType Type { get; private set; }
    public string Name { get; private set; }

    public Category(Guid id, TransactionType type, string name)
    {
        Id = id;
        Type = type;
        Name = name;
    }

    public Category(TransactionType type, string name) : this(Guid.NewGuid(), type, name)
    {
    }

    public void UpdateCategoryName(string name) => Name = name;
    public void UpdateCategoryType(TransactionType type) => Type = type;

    public void Accept(ICoreEntityVisitor visitor) => visitor.Visit(this);
}