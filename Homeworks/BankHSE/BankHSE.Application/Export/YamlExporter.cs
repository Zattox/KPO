using YamlDotNet.Serialization;
using BankHSE.Domain.Abstractions;
using BankHSE.Domain.Entities;

namespace BankHSE.Application.Export;

public class YamlExporter : Exporter
{
    private string _yamlData;

    public YamlExporter(ICoreEntitiesAggregator aggregator) : base(aggregator)
    {
    }

    protected override void FormatData()
    {
        var visitor = new YamlExportVisitor();
        foreach (var entity in _aggregator.GetAll())
            entity.Accept(visitor);

        var serializer = new SerializerBuilder()
            .WithNamingConvention(YamlDotNet.Serialization.NamingConventions.CamelCaseNamingConvention.Instance)
            .Build();

        _yamlData = serializer.Serialize(new
        {
            Accounts = visitor.Accounts,
            Categories = visitor.Categories,
            Operations = visitor.Operations
        });
    }

    protected override void Write(Stream stream)
    {
        using var writer = new StreamWriter(stream, System.Text.Encoding.UTF8);
        writer.Write(_yamlData);
    }
}

public class YamlExportVisitor : ICoreEntityVisitor
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