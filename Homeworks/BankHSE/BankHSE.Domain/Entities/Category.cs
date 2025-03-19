using BankHSE.Domain.Enums;
using BankHSE.Domain.Abstractions;

namespace BankHSE.Domain.Entities;

public class Category : IIdentifiable, ICoreEntityVisitable
{
    public Guid Id { get;}
    public TransactionType Type { get; private set; }
    public string Name { get; private set; }

    private Category(TransactionType type, string name)
    {
        Id = Guid.NewGuid();
        Type = type;
        Name = name;
    }
    
    public void Accept(ICoreEntityVisitor visitor)
    {
        visitor.Visit(this);
    }
}