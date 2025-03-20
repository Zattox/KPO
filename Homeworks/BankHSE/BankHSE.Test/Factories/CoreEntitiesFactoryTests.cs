namespace BankHSE.Tests.Factories;

public class CoreEntitiesFactoryTests
{
    private readonly CoreEntitiesFactory _factory = new();

    [Fact]
    public void CreateBankAccount_CreatesWithCorrectValues()
    {
        // Act
        var acc = _factory.CreateBankAccount("Test", 100m);

        // Assert
        Assert.Equal("Test", acc.Name);
        Assert.Equal(100m, acc.Balance);
    }

    [Fact]
    public void CreateCategory_CreatesWithCorrectValues()
    {
        // Act
        var cat = _factory.CreateCategory(TransactionType.Expense, "Food");

        // Assert
        Assert.Equal(TransactionType.Expense, cat.Type);
        Assert.Equal("Food", cat.Name);
    }

    [Fact]
    public void CreateOperation_CreatesWithCorrectValues()
    {
        // Arrange
        var acc = _factory.CreateBankAccount("Test", 0m);
        var cat = _factory.CreateCategory(TransactionType.Income, "Salary");

        // Act
        var op = _factory.CreateOperation(TransactionType.Income, acc, 50m, DateTime.Now, "Test", cat);

        // Assert
        Assert.Equal(TransactionType.Income, op.Type);
        Assert.Equal(50m, op.Amount);
        Assert.Equal(acc.Id, op.BankAccountId);
        Assert.Equal(cat.Id, op.CategoryId);
    }

    [Fact]
    public void CreateBankAccount_ThrowsOnNegativeBalance()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _factory.CreateBankAccount("Test", -1m));
    }
}