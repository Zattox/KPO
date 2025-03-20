using BankHSE.Domain.Abstractions;
using BankHSE.Application.Facades;
using BankHSE.Domain.Enums;

namespace BankHSE.Application.Commands;

public class CreateOperationCommand : ICommand
{
    private readonly OperationFacade _operationFacade;
    private readonly TransactionType _operationType;
    private readonly Guid _bankAccountId;
    private readonly decimal _amount;
    private readonly DateTime _date;
    private readonly string _description;
    private readonly Guid _categoryId;

    public CreateOperationCommand(OperationFacade operationFacade, TransactionType operationType, Guid bankAccountId, 
        decimal amount, DateTime date, string description, Guid categoryId)
    {
        _operationFacade = operationFacade;
        _operationType = operationType;
        _bankAccountId = bankAccountId;
        _amount = amount;
        _date = date;
        _description = description;
        _categoryId = categoryId;
    }

    public void Execute() => _operationFacade.CreateOperation(_operationType, _bankAccountId, _amount, _date, _description, _categoryId);
}