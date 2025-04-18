namespace BankHSE.Test.Factories;

public class CommandFactoryTests
{
    private readonly CommandFactory _factory;
    private readonly BankAccountFacade _accFacade;
    private readonly CategoryFacade _catFacade;
    private readonly OperationFacade _opFacade;
    private readonly IRepository<BankAccount> _accRepo;
    private readonly IRepository<Category> _catRepo;
    private readonly IRepository<Operation> _opRepo;
    private readonly CoreEntitiesFactory _entityFactory;

    public CommandFactoryTests()
    {
        _accRepo = new InMemoryRepository<BankAccount>();
        _catRepo = new InMemoryRepository<Category>();
        _opRepo = new InMemoryRepository<Operation>();
        _entityFactory = new CoreEntitiesFactory();
        _accFacade = new BankAccountFacade(_accRepo, _opRepo, _entityFactory);
        _catFacade = new CategoryFacade(_catRepo, _entityFactory);
        _opFacade = new OperationFacade(_opRepo, _accRepo, _catRepo, _entityFactory, _accFacade);
        _factory = new CommandFactory(_accFacade, _catFacade, _opFacade);
    }

    [Fact]
    public void CreateBankAccountCommand_CreatesAndExecutesCorrectly()
    {
        // Arrange
        string name = "Test Account";
        decimal balance = 100m;

        // Act
        var command = _factory.CreateBankAccountCommand(name, balance);
        command.Execute();

        // Assert
        var account = _accRepo.GetAll().First();
        Assert.Equal(name, account.Name);
        Assert.Equal(balance, account.Balance);
        Assert.IsType<TimingCommand>(command); // Проверяем, что команда обёрнута в TimingCommand
    }

    [Fact]
    public void CreateCategoryCommand_CreatesAndExecutesCorrectly()
    {
        // Arrange
        var type = TransactionType.Income;
        string name = "Salary";

        // Act
        var command = _factory.CreateCategoryCommand(type, name);
        command.Execute();

        // Assert
        var category = _catRepo.GetAll().First();
        Assert.Equal(type, category.Type);
        Assert.Equal(name, category.Name);
        Assert.IsType<TimingCommand>(command);
    }

    [Fact]
    public void CreateOperationCommand_CreatesAndExecutesCorrectly()
    {
        // Arrange
        var acc = _accFacade.CreateAccount("Test", 0m);
        var cat = _catFacade.CreateCategory(TransactionType.Expense, "Food");
        var type = TransactionType.Expense;
        decimal amount = 50m;
        var date = DateTime.Now;
        string desc = "Test Expense";

        // Act
        var command = _factory.CreateOperationCommand(type, acc.Id, amount, date, desc, cat.Id);
        command.Execute();

        // Assert
        var operation = _opRepo.GetAll().First();
        Assert.Equal(type, operation.Type);
        Assert.Equal(amount, operation.Amount);
        Assert.Equal(acc.Id, operation.BankAccountId);
        Assert.Equal(cat.Id, operation.CategoryId);
        Assert.Equal(desc, operation.Description);
        Assert.Equal(-amount, _accFacade.GetAccountById(acc.Id).Balance); // Balance updated
        Assert.IsType<TimingCommand>(command);
    }

    [Fact]
    public void CreateBankAccountCommand_ThrowsOnInvalidBalance()
    {
        // Arrange
        string name = "Invalid Account";
        decimal invalidBalance = -10m;

        // Act & Assert
        var command = _factory.CreateBankAccountCommand(name, invalidBalance);
        Assert.Throws<ArgumentException>(() => command.Execute());
    }

    [Fact]
    public void CreateOperationCommand_ThrowsOnMismatchedCategoryType()
    {
        // Arrange
        var acc = _accFacade.CreateAccount("Test", 0m);
        var cat = _catFacade.CreateCategory(TransactionType.Income, "Salary"); // Income category
        var type = TransactionType.Expense; // Expense operation

        // Act & Assert
        var command = _factory.CreateOperationCommand(type, acc.Id, 50m, DateTime.Now, "Test", cat.Id);
        Assert.Throws<ArgumentException>(() => command.Execute());
    }
}