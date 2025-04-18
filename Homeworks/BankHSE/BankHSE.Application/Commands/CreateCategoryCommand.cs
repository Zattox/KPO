using BankHSE.Domain.Abstractions;
using BankHSE.Application.Facades;
using BankHSE.Domain.Enums;

namespace BankHSE.Application.Commands;

public class CreateCategoryCommand : ICommand
{
    private readonly CategoryFacade _categoryFacade;
    private readonly string _name;
    private readonly TransactionType _type;

    public CreateCategoryCommand(CategoryFacade categoryFacade, string name, TransactionType type)
    {
        _categoryFacade = categoryFacade;
        _name = name;
        _type = type;
    }

    public void Execute() => _categoryFacade.CreateCategory(_type, _name);
}