using BankHSE.Domain.Abstractions;
using BankHSE.Application.Facades;
using BankHSE.Domain.Enums;

namespace BankHSE.Application.Commands;

public class CreateOperationCommand : ICommand
{
    private readonly OperationFacade _operationFacade;
    private TransactionType _operationType;
    private Guid _bankAccountId;
    private decimal _amount;
    private DateTime _date;
    private string _description;
    private Guid _categoryId;

    public CreateOperationCommand(OperationFacade operationFacade)
    {
        _operationFacade = operationFacade;
    }

    public void Create(TransactionType operationType, Guid bankAccountId, decimal amount, DateTime date,
        string description, Guid categoryId)
    {
        _operationType = operationType;
        _bankAccountId = bankAccountId;
        _amount = amount;
        _date = date;
        _description = description;
        _categoryId = categoryId;
    }

    public void Execute()
    {
        _operationFacade.CreateOperation(_operationType, _bankAccountId, _amount, _date, _description, _categoryId);
    }
}