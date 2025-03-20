using System.Text.Json;
using System.Text.Json.Serialization;
using BankHSE.Application.Factories;
using BankHSE.Domain.Abstractions;
using BankHSE.Domain.Entities;
using BankHSE.Domain.Enums;

namespace BankHSE.Application.Strategy;

public class JsonImportStrategy : IImportStrategy
{
    private readonly CoreEntitiesFactory _factory;
    private readonly IRepository<BankAccount> _accRepo;
    private readonly IRepository<Category> _catRepo;
    private readonly IRepository<Operation> _opRepo;

    public JsonImportStrategy(
        CoreEntitiesFactory factory,
        IRepository<BankAccount> accRepo,
        IRepository<Category> catRepo,
        IRepository<Operation> opRepo)
    {
        _factory = factory;
        _accRepo = accRepo;
        _catRepo = catRepo;
        _opRepo = opRepo;
    }

    public void Import(string path)
    {
        var content = File.ReadAllText(path);
        var options = new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } };
        var data = JsonSerializer.Deserialize<JsonData>(content, options);

        foreach (var acc in data.Accounts)
            _accRepo.Add(_factory.CreateBankAccount(acc.Id, acc.Name, acc.Balance));

        foreach (var cat in data.Categories)
            _catRepo.Add(_factory.CreateCategory(cat.Id, cat.Type, cat.Name));

        foreach (var op in data.Operations)
        {
            var acc = _accRepo.GetById(op.BankAccountId) ?? throw new InvalidOperationException("Account not found.");
            var cat = _catRepo.GetById(op.CategoryId) ?? throw new InvalidOperationException("Category not found.");
            _opRepo.Add(_factory.CreateOperation(op.Id, op.Type, acc, op.Amount, op.Date, op.Description, cat));
        }
    }

    // Data structure for JSON deserialization
    private class JsonData
    {
        public List<AccountData> Accounts { get; set; } = new();
        public List<CategoryData> Categories { get; set; } = new();
        public List<OperationData> Operations { get; set; } = new();
    }

    private class AccountData
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Balance { get; set; }
    }

    private class CategoryData
    {
        public Guid Id { get; set; }
        public TransactionType Type { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    private class OperationData
    {
        public Guid Id { get; set; }
        public TransactionType Type { get; set; }
        public Guid BankAccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
    }
}