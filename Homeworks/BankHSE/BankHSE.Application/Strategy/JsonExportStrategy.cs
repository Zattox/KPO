using System.Text.Json;
using System.Text.Json.Serialization;
using BankHSE.Domain.Entities;
using BankHSE.Domain.Abstractions;

namespace BankHSE.Application.Strategy;

public class JsonExportStrategy : IExportStrategy
{
    public void Export(string path, ICoreEntitiesAggregator agg)
    {
        var visitor = new JsonExportVisitor();
        foreach (var entity in agg.GetAll())
            entity.Accept(visitor);

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters = { new JsonStringEnumConverter() }
        };

        var jsonData = JsonSerializer.Serialize(new
        {
            Accounts = visitor.Accounts,
            Categories = visitor.Categories,
            Operations = visitor.Operations
        }, options);

        using var stream = new FileStream(path, FileMode.Create);
        using var writer = new StreamWriter(stream);
        writer.Write(jsonData);
    }
}

// Visitor for collecting data in JSON-compatible format
public class JsonExportVisitor : ICoreEntityVisitor
{
    public List<object> Accounts { get; } = new();
    public List<object> Categories { get; } = new();
    public List<object> Operations { get; } = new();

    public void Visit(BankAccount acc) => Accounts.Add(new { acc.Id, acc.Name, acc.Balance });
    public void Visit(Category cat) => Categories.Add(new { cat.Id, cat.Type, cat.Name });
    public void Visit(Operation op) => Operations.Add(new { op.Id, op.Type, op.BankAccountId, op.Amount, op.Date, op.Description, op.CategoryId });
}