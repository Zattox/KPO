using YamlDotNet.Serialization;
using BankHSE.Domain.Abstractions;
using BankHSE.Domain.Entities;
using BankHSE.Domain.Enums;
using BankHSE.Application.Factories;

namespace BankHSE.Application.Import;

public class YamlImporter : Importer
{
    private readonly CoreEntitiesFactory _factory;
    private readonly IRepository<BankAccount> _accounts;
    private readonly IRepository<Category> _categories;
    private readonly IRepository<Operation> _operations;

    public YamlImporter(CoreEntitiesFactory factory, IRepository<BankAccount> accounts,
        IRepository<Category> categories, IRepository<Operation> operations)
    {
        _factory = factory;
        _accounts = accounts;
        _categories = categories;
        _operations = operations;
    }

    public void ImportAll(string path)
    {
        var content = File.ReadAllText(path, System.Text.Encoding.UTF8);
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(YamlDotNet.Serialization.NamingConventions.CamelCaseNamingConvention.Instance)
            .Build();

        var yamlData = deserializer.Deserialize<YamlData>(content);

        // Импорт счетов
        foreach (var acc in yamlData.Accounts)
        {
            var account = _factory.CreateBankAccount(acc.Id, acc.Name, acc.Balance);
            _accounts.Add(account);
        }

        // Импорт категорий
        foreach (var cat in yamlData.Categories)
        {
            var category = _factory.CreateCategory(cat.Id, cat.Type, cat.Name);
            _categories.Add(category);
        }

        // Импорт операций
        foreach (var op in yamlData.Operations)
        {
            var account = _accounts.GetById(op.BankAccountId) ??
                          throw new InvalidOperationException("Account not found.");
            var category = _categories.GetById(op.CategoryId) ??
                           throw new InvalidOperationException("Category not found.");
            var operation =
                _factory.CreateOperation(op.Id, op.Type, account, op.Amount, op.Date, op.Description, category);
            _operations.Add(operation);
        }
    }

    protected override IEnumerable<Operation> Parse(string content)
    {
        ImportAll(content);
        return _operations.GetAll();
    }

    private class YamlData
    {
        public List<AccountData> Accounts { get; set; }
        public List<CategoryData> Categories { get; set; }
        public List<OperationData> Operations { get; set; }
    }

    private class AccountData
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
    }

    private class CategoryData
    {
        public Guid Id { get; set; }
        public TransactionType Type { get; set; }
        public string Name { get; set; }
    }

    private class OperationData
    {
        public Guid Id { get; set; }
        public TransactionType Type { get; set; }
        public Guid BankAccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public Guid CategoryId { get; set; }
    }
}