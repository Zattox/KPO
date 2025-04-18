using BankHSE.Domain.Entities;

namespace BankHSE.Domain.Abstractions;

public interface ICoreEntityVisitor
{
    void Visit(BankAccount bankAccount);
    void Visit(Category category);
    void Visit(Operation operation);
}