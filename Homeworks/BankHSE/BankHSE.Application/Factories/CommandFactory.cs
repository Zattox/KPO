using BankHSE.Application.Commands;
using BankHSE.Application.Facades;
using BankHSE.Domain.Abstractions;
using BankHSE.Domain.Enums;

namespace BankHSE.Application.Factories;

public class CommandFactory
{
    private readonly OperationFacade _operationFacade;
    private readonly BankAccountFacade _bankAccountFacade;
    private readonly CategoryFacade _categoryFacade;

    public CommandFactory(OperationFacade operationFacade, BankAccountFacade bankAccountFacade, CategoryFacade categoryFacade)
    {
        _operationFacade = operationFacade;
        _bankAccountFacade = bankAccountFacade;
        _categoryFacade = categoryFacade;
    }

    public ICommand CreateBankAccountCommand(string name, decimal balance)
    {
        return new CreateBankAccountCommand(_bankAccountFacade, name, balance);
    }

    public ICommand CreateCategoryCommand(TransactionType type, string name)
    {
        return new CreateCategoryCommand(_categoryFacade, name, type);
    }

    public ICommand CreateOperationCommand(TransactionType type, Guid accountId, decimal amount, DateTime date,
        string description, Guid categoryId)
    {
        return new CreateOperationCommand(_operationFacade, type, accountId, amount, date, description, categoryId);
    }

    public ICommand CreateTimedCommand(ICommand innerCommand)
    {
        return new TimingCommand(innerCommand);
    }
}