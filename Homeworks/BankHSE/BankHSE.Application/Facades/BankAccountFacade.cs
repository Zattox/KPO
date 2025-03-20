using BankHSE.Application.Factories;
using BankHSE.Domain.Abstractions;
using BankHSE.Domain.Entities;

namespace BankHSE.Application.Facades;

public class BankAccountFacade
{
    private readonly IRepository<BankAccount> _accountsRepository;
    private readonly CoreEntitiesFactory _factory;

    public BankAccountFacade(IRepository<BankAccount> accountsRepository, CoreEntitiesFactory factory)
    {
        _accountsRepository = accountsRepository ?? throw new ArgumentNullException(nameof(accountsRepository));
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
        account.UpdateNameAccount(name);
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

    public void Delete(Guid accountId)
    {
        _accountsRepository.Delete(accountId);
    }
}