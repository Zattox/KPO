using BankHSE.Domain.Abstractions;
using BankHSE.Application.Facades;

namespace BankHSE.Application.Commands;

public class CreateBankAccountCommand : ICommand
{
    private readonly BankAccountFacade _bankAccountFacade;
    private string _name;
    private decimal _balance;
    
    public CreateBankAccountCommand(BankAccountFacade bankAccountFacade)
    {
        _bankAccountFacade = bankAccountFacade;
    }

    public void Create(string name, decimal balance)
    {
        _name = name;
        _balance = balance;
    }
    
    public void Execute()
    {
        _bankAccountFacade.CreateAccount(_name, _balance);
    }
}