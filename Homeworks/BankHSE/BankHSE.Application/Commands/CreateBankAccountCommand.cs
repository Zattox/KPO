using BankHSE.Domain.Abstractions;
using BankHSE.Application.Facades;

namespace BankHSE.Application.Commands;

public class CreateBankAccountCommand : ICommand
{
    private readonly BankAccountFacade _bankAccountFacade;
    private readonly string _name;
    private readonly decimal _balance;

    public CreateBankAccountCommand(BankAccountFacade bankAccountFacade, string name, decimal balance)
    {
        _bankAccountFacade = bankAccountFacade;
        _name = name;
        _balance = balance;
    }

    public void Execute() => _bankAccountFacade.CreateAccount(_name, _balance);
}