namespace BankHSE.Tests.Strategy;

public class ExportStrategyTests
{
    private readonly CoreEntitiesFactory _factory = new();
    private readonly InMemoryRepository<BankAccount> _accRepo = new();
    private readonly InMemoryRepository<Category> _catRepo = new();
    private readonly InMemoryRepository<Operation> _opRepo = new();

    [Fact]
    public void JsonExportStrategy_ExportsCorrectly()
    {
        // Arrange
        var agg = new CoreEntitiesAggregator(_accRepo, _catRepo, _opRepo);
        var acc = _factory.CreateBankAccount("Test", 100m);
        _accRepo.Add(acc);
        var strategy = new JsonExportStrategy();
        string path = $"test_json_{Guid.NewGuid()}.json"; // Уникальное имя файла

        // Act
        strategy.Export(path, agg);
        string content = File.ReadAllText(path);

        // Assert
        Assert.Contains("Test", content);
        Assert.Contains("100", content);
        File.Delete(path);
    }

    [Fact]
    public void CsvExportStrategy_ExportsCorrectly()
    {
        // Arrange
        var agg = new CoreEntitiesAggregator(_accRepo, _catRepo, _opRepo);
        var acc = _factory.CreateBankAccount("Test", 50m);
        _accRepo.Add(acc);
        var strategy = new CsvExportStrategy();
        string path = $"test_csv_{Guid.NewGuid()}.csv"; // Уникальное имя файла

        // Act
        strategy.Export(path, agg);
        string content = File.ReadAllText(path);

        // Assert
        Assert.Contains("Test;50", content);
        File.Delete(path);
    }
}