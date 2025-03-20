using BankHSE.Domain.Abstractions;
using BankHSE.Domain.Entities;
using BankHSE.Application.Factories;
using BankHSE.Domain.Enums;

namespace BankHSE.Application.Facades;

public class OperationFacade
{
    private readonly IRepository<Operation> _operationsRepository;
    private readonly IRepository<BankAccount> _accountRepository;
    private readonly IRepository<Category> _categoriesRepository;
    private readonly CoreEntitiesFactory _factory;

    public OperationFacade(IRepository<Operation> operationsRepository, IRepository<BankAccount> accountRepository,
        IRepository<Category> categoriesRepository, CoreEntitiesFactory factory)
    {
        _operationsRepository = operationsRepository ?? throw new ArgumentNullException(nameof(operationsRepository));
        _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
        _categoriesRepository = categoriesRepository ?? throw new ArgumentNullException(nameof(categoriesRepository));
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    public Operation CreateOperation(TransactionType type, Guid accountId, decimal amount, DateTime date,
        string description, Guid categoryId)
    {
        var account = _accountRepository.GetById(accountId) ??
                      throw new InvalidOperationException($"Account with id {accountId} was not found");

        var category = _categoriesRepository.GetById(categoryId) ??
                       throw new InvalidOperationException($"Category with id {categoryId} was not found");

        var operation = _factory.CreateOperation(type, account, amount, date, description, category);
        _operationsRepository.Add(operation);
        return operation;
    }

    public Operation GetById(Guid operationId)
    {
        return _operationsRepository.GetById(operationId);
    }

    public IEnumerable<Operation> GetByAccountId(Guid accountId)
    {
        var account = _accountRepository.GetById(accountId) ??
                      throw new InvalidOperationException($"Account with id {accountId} was not found");

        var allOperations = _operationsRepository.GetAll().ToList();
        return allOperations.Where(o => o.BankAccountId == accountId);
    }

    public IEnumerable<Operation> GetAll()
    {
        return _operationsRepository.GetAll();
    }

    public void UpdateOperationById(Guid operationId, string description)
    {
        var operation = _operationsRepository.GetById(operationId) ??
                        throw new InvalidOperationException($"Operation with id {operationId} was not found");

        operation.UpdateOpeationDescription(description);

        _operationsRepository.Update(operation);
    }

    public void Delete(Guid id)
    {
        _operationsRepository.Delete(id);
    }
}