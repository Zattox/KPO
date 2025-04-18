using BankHSE.Application.Factories;
using BankHSE.Domain.Abstractions;
using BankHSE.Domain.Entities;

namespace BankHSE.Application.Facades;

public class BankAccountFacade
{
    private readonly IRepository<BankAccount> _accountsRepository;
    private readonly IRepository<Operation> _operationsRepository;
    private readonly CoreEntitiesFactory _factory;

    public BankAccountFacade(
        IRepository<BankAccount> accountsRepository,
        IRepository<Operation> operationsRepository,
        CoreEntitiesFactory factory)
    {
        _accountsRepository = accountsRepository ?? throw new ArgumentNullException(nameof(accountsRepository));
        _operationsRepository = operationsRepository ?? throw new ArgumentNullException(nameof(operationsRepository));
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    public BankAccount CreateAccount(string name, decimal balance)
    {
        var account = _factory.CreateBankAccount(name, balance);
        _accountsRepository.Add(account);
        return account;
    }

    public BankAccount GetAccountById(Guid accountId)
    {
        return _accountsRepository.GetById(accountId) ??
               throw new InvalidOperationException($"Account with id {accountId} was not found");
    }

    public IEnumerable<BankAccount> GetAllAccounts()
    {
        return _accountsRepository.GetAll();
    }

    public void UpdateAccountById(Guid accountId, string name)
    {
        var account = GetAccountById(accountId);
        account.UpdateAccountName(name);
        _accountsRepository.Update(account);
    }

    public void IncreaseBalanceById(Guid accountId, decimal amount)
    {
        var account = _accountsRepository.GetById(accountId);
        account.IncreaseBalance(amount);
        _accountsRepository.Update(account);
    }

    public void DecreaseBalanceById(Guid accountId, decimal amount)
    {
        var account = _accountsRepository.GetById(accountId);
        account.DecreaseBalance(amount);
        _accountsRepository.Update(account);
    }

    public void DeleteAccountById(Guid accountId)
    {
        _accountsRepository.Delete(accountId);
    }

    public void RecalculateBalance(Guid accountId)
    {
        var account = GetAccountById(accountId);
        var operations = _operationsRepository.GetAll().Where(o => o.BankAccountId == accountId).ToList();

        decimal recalculatedBalance = 0m;
        foreach (var operation in operations)
        {
            if (operation.Type == Domain.Enums.TransactionType.Income)
                recalculatedBalance += operation.Amount;
            else
                recalculatedBalance -= operation.Amount;
        }

        account.IncreaseBalance(recalculatedBalance - account.Balance); // Корректируем баланс
        _accountsRepository.Update(account);
    }
}