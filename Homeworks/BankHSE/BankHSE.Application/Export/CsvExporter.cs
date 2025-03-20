using System.Text;
using BankHSE.Domain.Abstractions;
using BankHSE.Domain.Entities;
using BankHSE.Domain.Enums;

namespace BankHSE.Application.Export;

public class CsvExporter : Exporter
{
    private string _csvData;

    public CsvExporter(ICoreEntitiesAggregator aggregator) : base(aggregator)
    {
    }

    protected override void FormatData()
    {
        var visitor = new CsvExportVisitor();
        foreach (var entity in _aggregator.GetAll())
            entity.Accept(visitor);

        var sb = new StringBuilder();

        // Accounts
        sb.AppendLine("Accounts");
        sb.AppendLine("Id;Name;Balance");
        foreach (var account in visitor.Accounts)
            sb.AppendLine($"{account.Id};{EscapeCsv(account.Name)};{account.Balance}");

        // Categories
        sb.AppendLine("\nCategories");
        sb.AppendLine("Id;Type;Name");
        foreach (var category in visitor.Categories)
            sb.AppendLine($"{category.Id};{category.Type};{EscapeCsv(category.Name)}");

        // Operations
        sb.AppendLine("\nOperations");
        sb.AppendLine("Id;Type;BankAccountId;Amount;Date;Description;CategoryId");
        foreach (var operation in visitor.Operations)
            sb.AppendLine(
                $"{operation.Id};{operation.Type};{operation.BankAccountId};{operation.Amount};{operation.Date};{EscapeCsv(operation.Description)};{operation.CategoryId}");

        _csvData = sb.ToString();
    }

    protected override void Write(Stream stream)
    {
        using var writer = new StreamWriter(stream, Encoding.UTF8); // UTF-8 для поддержки русских символов
        writer.Write(_csvData);
    }

    private static string EscapeCsv(string value)
    {
        if (string.IsNullOrEmpty(value)) return "";
        if (value.Contains(";") || value.Contains("\"") || value.Contains("\n"))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }
}

public class CsvExportVisitor : ICoreEntityVisitor
{
    public List<(Guid Id, string Name, decimal Balance)> Accounts { get; } = new();
    public List<(Guid Id, TransactionType Type, string Name)> Categories { get; } = new();

    public List<(Guid Id, TransactionType Type, Guid BankAccountId, decimal Amount, DateTime Date, string Description,
        Guid CategoryId)> Operations { get; } = new();

    public void Visit(BankAccount bankAccount) => Accounts.Add((bankAccount.Id, bankAccount.Name, bankAccount.Balance));
    public void Visit(Category category) => Categories.Add((category.Id, category.Type, category.Name));

    public void Visit(Operation operation) => Operations.Add((operation.Id, operation.Type, operation.BankAccountId,
        operation.Amount, operation.Date, operation.Description, operation.CategoryId));
}