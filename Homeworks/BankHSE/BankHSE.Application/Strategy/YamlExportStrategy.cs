using System.Text;
using YamlDotNet.Serialization;
using BankHSE.Domain.Entities;
using BankHSE.Domain.Abstractions;

namespace BankHSE.Application.Strategy;

public class YamlExportStrategy : IExportStrategy
{
    public void Export(string path, ICoreEntitiesAggregator agg)
    {
        var visitor = new YamlExportVisitor();
        foreach (var entity in agg.GetAll())
            entity.Accept(visitor);

        var serializer = new SerializerBuilder()
            .WithNamingConvention(YamlDotNet.Serialization.NamingConventions.CamelCaseNamingConvention.Instance)
            .Build();

        var yamlData = serializer.Serialize(new
        {
            Accounts = visitor.Accounts,
            Categories = visitor.Categories,
            Operations = visitor.Operations
        });

        using var stream = new FileStream(path, FileMode.Create);
        using var writer = new StreamWriter(stream, Encoding.UTF8);
        writer.Write(yamlData);
    }
}

// Visitor for collecting data in YAML-compatible format
public class YamlExportVisitor : ICoreEntityVisitor
{
    public List<object> Accounts { get; } = new();
    public List<object> Categories { get; } = new();
    public List<object> Operations { get; } = new();

    public void Visit(BankAccount acc) => Accounts.Add(new { acc.Id, acc.Name, acc.Balance });
    public void Visit(Category cat) => Categories.Add(new { cat.Id, cat.Type, cat.Name });
    public void Visit(Operation op) => Operations.Add(new { op.Id, op.Type, op.BankAccountId, op.Amount, op.Date, op.Description, op.CategoryId });
}