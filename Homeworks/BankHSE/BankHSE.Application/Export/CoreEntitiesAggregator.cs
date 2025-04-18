using BankHSE.Domain.Abstractions;
using BankHSE.Domain.Entities;

namespace BankHSE.Application.Export;

public class CoreEntitiesAggregator : ICoreEntitiesAggregator
{
    private readonly IRepository<BankAccount> _accounts;
    private readonly IRepository<Category> _categories;
    private readonly IRepository<Operation> _operations;

    public CoreEntitiesAggregator(IRepository<BankAccount> accounts, IRepository<Category> categories,
        IRepository<Operation> operations)
    {
        _accounts = accounts;
        _categories = categories;
        _operations = operations;
    }

    public IEnumerable<ICoreEntityVisitable> GetAll()
    {
        return _accounts.GetAll().Cast<ICoreEntityVisitable>()
            .Concat(_categories.GetAll().Cast<ICoreEntityVisitable>())
            .Concat(_operations.GetAll().Cast<ICoreEntityVisitable>());
    }
}