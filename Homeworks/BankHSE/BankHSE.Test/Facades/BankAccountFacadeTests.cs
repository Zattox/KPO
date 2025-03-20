namespace BankHSE.Tests.Facades;

public class BankAccountFacadeTests
{
    private readonly BankAccountFacade _facade;
    private readonly IRepository<BankAccount> _accRepo;
    private readonly IRepository<Operation> _opRepo;
    private readonly CoreEntitiesFactory _factory;

    public BankAccountFacadeTests()
    {
        _accRepo = new InMemoryRepository<BankAccount>();
        _opRepo = new InMemoryRepository<Operation>();
        _factory = new CoreEntitiesFactory();
        _facade = new BankAccountFacade(_accRepo, _opRepo, _factory);
    }

    [Fact]
    public void CreateAccount_AddsAccountToRepository()
    {
        // Arrange
        string name = "Test Account";
        decimal balance = 100m;

        // Act
        var account = _facade.CreateAccount(name, balance);

        // Assert
        Assert.Equal(name, account.Name);
        Assert.Equal(balance, account.Balance);
        Assert.Contains(account, _accRepo.GetAll());
    }

    [Fact]
    public void GetAccountById_ReturnsCorrectAccount()
    {
        // Arrange
        var account = _facade.CreateAccount("Test", 50m);

        // Act
        var result = _facade.GetAccountById(account.Id);

        // Assert
        Assert.Equal(account.Id, result.Id);
    }

    [Fact]
    public void UpdateAccountById_UpdatesName()
    {
        // Arrange
        var account = _facade.CreateAccount("Old Name", 0m);
        string newName = "New Name";

        // Act
        _facade.UpdateAccountById(account.Id, newName);
        var updated = _facade.GetAccountById(account.Id);

        // Assert
        Assert.Equal(newName, updated.Name);
    }

    [Fact]
    public void DeleteAccountById_RemovesAccount()
    {
        // Arrange
        var account = _facade.CreateAccount("To Delete", 0m);

        // Act
        _facade.DeleteAccountById(account.Id);

        // Assert
        Assert.Null(_accRepo.GetById(account.Id));
    }

    [Fact]
    public void RecalculateBalance_AdjustsBalanceBasedOnOperations()
    {
        // Arrange
        var account = _facade.CreateAccount("Test", 0m);
        var op1 = new Operation(TransactionType.Income, account.Id, 100m, DateTime.Now, "Income", Guid.NewGuid());
        var op2 = new Operation(TransactionType.Expense, account.Id, 30m, DateTime.Now, "Expense", Guid.NewGuid());
        _opRepo.Add(op1);
        _opRepo.Add(op2);

        // Act
        _facade.RecalculateBalance(account.Id);
        var updated = _facade.GetAccountById(account.Id);

        // Assert
        Assert.Equal(70m, updated.Balance); // 100 - 30
    }
}