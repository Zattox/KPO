using BankHSE.Application.Commands;
using BankHSE.Application.Facades;
using BankHSE.Domain.Abstractions;
using BankHSE.Domain.Enums;

namespace BankHSE.Application.Factories;

public class CommandFactory
{
    private readonly BankAccountFacade _accFacade;
    private readonly CategoryFacade _catFacade;
    private readonly OperationFacade _opFacade;

    public CommandFactory(BankAccountFacade accFacade, CategoryFacade catFacade, OperationFacade opFacade)
    {
        _accFacade = accFacade ?? throw new ArgumentNullException(nameof(accFacade));
        _catFacade = catFacade ?? throw new ArgumentNullException(nameof(catFacade));
        _opFacade = opFacade ?? throw new ArgumentNullException(nameof(opFacade));
    }

    public ICommand CreateBankAccountCommand(string name, decimal balance)
    {
        var command = new CreateBankAccountCommand(_accFacade, name, balance);
        return new TimingCommand(command); // Оборачиваем в TimingCommand
    }

    public ICommand CreateCategoryCommand(TransactionType type, string name)
    {
        var command = new CreateCategoryCommand(_catFacade, name, type);
        return new TimingCommand(command); // Оборачиваем в TimingCommand
    }

    public ICommand CreateOperationCommand(TransactionType type, Guid accId, decimal amount, DateTime date, string desc,
        Guid catId)
    {
        var command = new CreateOperationCommand(_opFacade, type, accId, amount, date, desc, catId);
        return new TimingCommand(command); // Оборачиваем в TimingCommand
    }

    public ICommand CreateTimedCommand(ICommand innerCommand)
    {
        return new TimingCommand(innerCommand);
    }
}