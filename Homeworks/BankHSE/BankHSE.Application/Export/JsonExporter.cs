using System.Text.Json;
using System.Text.Json.Serialization;
using BankHSE.Domain.Abstractions;
using BankHSE.Domain.Entities;

namespace BankHSE.Application.Export;

public class JsonExporter : Exporter
{
    private string _jsonData;

    public JsonExporter(ICoreEntitiesAggregator aggregator) : base(aggregator)
    {
    }

    protected override void FormatData()
    {
        var visitor = new JsonExportVisitor();
        foreach (var entity in _aggregator.GetAll())
            entity.Accept(visitor);
        var options = new JsonSerializerOptions
        {
            WriteIndented = true, // Красивый отступ
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder
                .UnsafeRelaxedJsonEscaping, // Не экранировать русские символы
            Converters = { new JsonStringEnumConverter() } // Сериализовать перечисления как строки
        };
        _jsonData = JsonSerializer.Serialize(new
        {
            Accounts = visitor.Accounts,
            Categories = visitor.Categories,
            Operations = visitor.Operations
        }, options);
    }

    protected override void Write(Stream stream)
    {
        using var writer = new StreamWriter(stream);
        writer.Write(_jsonData);
    }
}

public class JsonExportVisitor : ICoreEntityVisitor
{
    public List<object> Accounts { get; } = new();
    public List<object> Categories { get; } = new();
    public List<object> Operations { get; } = new();

    public void Visit(BankAccount bankAccount) =>
        Accounts.Add(new { bankAccount.Id, bankAccount.Name, bankAccount.Balance });

    public void Visit(Category category) => Categories.Add(new { category.Id, category.Type, category.Name });

    public void Visit(Operation operation) => Operations.Add(new
    {
        operation.Id, operation.Type, operation.BankAccountId, operation.Amount, operation.Date, operation.Description,
        operation.CategoryId
    });
}