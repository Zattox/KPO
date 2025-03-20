namespace BankHSE.Tests.Strategy;

public class ImportStrategyTests
{
    private readonly CoreEntitiesFactory _factory = new();
    private readonly InMemoryRepository<BankAccount> _accRepo = new();
    private readonly InMemoryRepository<Category> _catRepo = new();
    private readonly InMemoryRepository<Operation> _opRepo = new();

    [Fact]
    public void JsonImportStrategy_ImportsCorrectly()
    {
        // Arrange
        string json = @"{
            ""Accounts"": [{""Id"": ""550e8400-e29b-41d4-a716-446655440000"", ""Name"": ""Test"", ""Balance"": 100}],
            ""Categories"": [],
            ""Operations"": []
        }";
        string path = $"test_json_{Guid.NewGuid()}.json"; // Уникальное имя файла
        File.WriteAllText(path, json);
        var strategy = new JsonImportStrategy(_factory, _accRepo, _catRepo, _opRepo);

        // Act
        strategy.Import(path);

        // Assert
        var acc = _accRepo.GetAll().First();
        Assert.Equal("Test", acc.Name);
        Assert.Equal(100m, acc.Balance);
        File.Delete(path);
    }

    [Fact]
    public void CsvImportStrategy_ImportsCorrectly()
    {
        // Arrange
        string csv = "Accounts\nId;Name;Balance\n" + $"{Guid.NewGuid()};Test;50";
        string path = $"test_csv_{Guid.NewGuid()}.csv"; // Уникальное имя файла
        File.WriteAllText(path, csv);
        var strategy = new CsvImportStrategy(_factory, _accRepo, _catRepo, _opRepo);

        // Act
        strategy.Import(path);

        // Assert
        var acc = _accRepo.GetAll().First();
        Assert.Equal("Test", acc.Name);
        Assert.Equal(50m, acc.Balance);
        File.Delete(path);
    }
}