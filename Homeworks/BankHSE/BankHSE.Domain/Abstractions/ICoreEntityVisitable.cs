namespace BankHSE.Domain.Abstractions;

public interface ICoreEntityVisitable
{
    void Accept(ICoreEntityVisitor visitor);
}