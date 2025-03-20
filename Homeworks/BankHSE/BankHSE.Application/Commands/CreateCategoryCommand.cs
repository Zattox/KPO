using BankHSE.Domain.Abstractions;
using BankHSE.Application.Facades;
using BankHSE.Domain.Enums;

namespace BankHSE.Application.Commands;

public class CreateCategoryCommand : ICommand
{
    private readonly CategoryFacade _categoryFacade;
    private string _name;
    private TransactionType _type;

    public CreateCategoryCommand(CategoryFacade categoryFacade)
    {
        _categoryFacade = categoryFacade;
    }

    public void Create(string name, TransactionType type)
    {
        _name = name;
        _type = type;
    }

    public void Execute()
    {
        _categoryFacade.CreateCategory(_type, _name);
    }
}