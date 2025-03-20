using BankHSE.Domain.Entities;
using BankHSE.Domain.Enums;

namespace BankHSE.Application.Factories;

public class CoreEntitiesFactory
{
    public BankAccount CreateBankAccount(string name, decimal balance = decimal.Zero)
    {
        return new BankAccount(name, balance);
    }

    public Category CreateCategory(TransactionType type, string name)
    {
        return new Category(type, name);
    }

    public Operation CreateOperation(TransactionType type, BankAccount bankAccount, decimal amount, DateTime date,
        string description, Category category)
    {
        return new Operation(type, bankAccount.Id, amount, date, description, category.Id);
    }
}