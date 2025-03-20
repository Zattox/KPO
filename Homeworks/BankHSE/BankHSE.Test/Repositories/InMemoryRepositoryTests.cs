namespace BankHSE.Tests.Repositories;

public class InMemoryRepositoryTests
{
    private readonly InMemoryRepository<BankAccount> _repo = new();

    [Fact]
    public void AddAndGetById_ReturnsAddedItem()
    {
        // Arrange
        var acc = new BankAccount("Test", 0m);

        // Act
        _repo.Add(acc);
        var result = _repo.GetById(acc.Id);

        // Assert
        Assert.Equal(acc.Id, result.Id);
    }

    [Fact]
    public void Update_ChangesItem()
    {
        // Arrange
        var acc = new BankAccount("Old", 0m);
        _repo.Add(acc);

        // Act
        acc.UpdateAccountName("New");
        _repo.Update(acc);
        var updated = _repo.GetById(acc.Id);

        // Assert
        Assert.Equal("New", updated.Name);
    }

    [Fact]
    public void Delete_RemovesItem()
    {
        // Arrange
        var acc = new BankAccount("To Delete", 0m);
        _repo.Add(acc);

        // Act
        _repo.Delete(acc.Id);

        // Assert
        Assert.Null(_repo.GetById(acc.Id));
    }
}