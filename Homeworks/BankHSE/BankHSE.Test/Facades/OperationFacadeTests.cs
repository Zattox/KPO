namespace BankHSE.Tests.Facades;

public class OperationFacadeTests
{
    private readonly OperationFacade _facade;
    private readonly IRepository<Operation> _opRepo;
    private readonly IRepository<BankAccount> _accRepo;
    private readonly IRepository<Category> _catRepo;
    private readonly CoreEntitiesFactory _factory;
    private readonly BankAccountFacade _accFacade;

    public OperationFacadeTests()
    {
        _opRepo = new InMemoryRepository<Operation>();
        _accRepo = new InMemoryRepository<BankAccount>();
        _catRepo = new InMemoryRepository<Category>();
        _factory = new CoreEntitiesFactory();
        _accFacade = new BankAccountFacade(_accRepo, _opRepo, _factory);
        _facade = new OperationFacade(_opRepo, _accRepo, _catRepo, _factory, _accFacade);
    }

    [Fact]
    public void CreateOperation_AddsOperationAndUpdatesBalance()
    {
        // Arrange
        var acc = _accFacade.CreateAccount("Test", 0m);
        var cat = new Category(TransactionType.Income, "Income");
        _catRepo.Add(cat);

        // Act
        var op = _facade.CreateOperation(TransactionType.Income, acc.Id, 50m, DateTime.Now, "Test Income", cat.Id);

        // Assert
        Assert.Equal(50m, op.Amount);
        Assert.Equal(acc.Id, op.BankAccountId);
        Assert.Equal(cat.Id, op.CategoryId);
        Assert.Equal(50m, _accFacade.GetAccountById(acc.Id).Balance);
    }

    [Fact]
    public void GetByAccountId_ReturnsCorrectOperations()
    {
        // Arrange
        var acc = _accFacade.CreateAccount("Test", 0m);
        var cat = new Category(TransactionType.Expense, "Expense");
        _catRepo.Add(cat);
        var op = _facade.CreateOperation(TransactionType.Expense, acc.Id, 20m, DateTime.Now, "Test", cat.Id);

        // Act
        var ops = _facade.GetByAccountId(acc.Id).ToList();

        // Assert
        Assert.Single(ops);
        Assert.Equal(op.Id, ops[0].Id);
    }

    [Fact]
    public void UpdateOperationById_UpdatesDescription()
    {
        // Arrange
        var acc = _accFacade.CreateAccount("Test", 0m);
        var cat = new Category(TransactionType.Income, "Income");
        _catRepo.Add(cat);
        var op = _facade.CreateOperation(TransactionType.Income, acc.Id, 10m, DateTime.Now, "Old", cat.Id);

        // Act
        _facade.UpdateOperationById(op.Id, "New");
        var updated = _facade.GetById(op.Id);

        // Assert
        Assert.Equal("New", updated.Description);
    }
}