namespace BankHSE.Tests.Repositories;

public class CachedRepositoryTests
{
    private readonly CachedRepository<BankAccount> _repo;
    private readonly InMemoryRepository<BankAccount> _innerRepo;

    public CachedRepositoryTests()
    {
        _innerRepo = new InMemoryRepository<BankAccount>();
        _repo = new CachedRepository<BankAccount>(_innerRepo);
    }

    [Fact]
    public void Add_CachesItem()
    {
        // Arrange
        var acc = new BankAccount("Test", 0m);

        // Act
        _repo.Add(acc);
        var cached = _repo.GetById(acc.Id);

        // Assert
        Assert.Equal(acc.Id, cached.Id);
    }

    [Fact]
    public void Update_UpdatesCache()
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
    public void Delete_RemovesFromCache()
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