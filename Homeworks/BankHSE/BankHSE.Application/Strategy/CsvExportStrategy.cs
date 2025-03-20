using System.Text;
using BankHSE.Domain.Entities;
using BankHSE.Domain.Abstractions;
using BankHSE.Domain.Enums;

namespace BankHSE.Application.Strategy;

public class CsvExportStrategy : IExportStrategy
{
    public void Export(string path, ICoreEntitiesAggregator agg)
    {
        var visitor = new CsvExportVisitor();
        foreach (var entity in agg.GetAll())
            entity.Accept(visitor);

        var sb = new StringBuilder();
        sb.AppendLine("Accounts");
        sb.AppendLine("Id;Name;Balance");
        foreach (var acc in visitor.Accounts)
            sb.AppendLine($"{acc.Id};{EscapeCsv(acc.Name)};{acc.Balance}");

        sb.AppendLine("\nCategories");
        sb.AppendLine("Id;Type;Name");
        foreach (var cat in visitor.Categories)
            sb.AppendLine($"{cat.Id};{cat.Type};{EscapeCsv(cat.Name)}");

        sb.AppendLine("\nOperations");
        sb.AppendLine("Id;Type;BankAccountId;Amount;Date;Description;CategoryId");
        foreach (var op in visitor.Operations)
            sb.AppendLine(
                $"{op.Id};{op.Type};{op.BankAccountId};{op.Amount};{op.Date};{EscapeCsv(op.Description)};{op.CategoryId}");

        using var stream = new FileStream(path, FileMode.Create);
        using var writer = new StreamWriter(stream, Encoding.UTF8);
        writer.Write(sb.ToString());
    }

    // Escape CSV values to handle special characters
    private static string EscapeCsv(string value)
    {
        if (string.IsNullOrEmpty(value)) return "";
        if (value.Contains(";") || value.Contains("\"") || value.Contains("\n"))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }
}

// Visitor for collecting data in CSV-compatible format
public class CsvExportVisitor : ICoreEntityVisitor
{
    public List<(Guid Id, string Name, decimal Balance)> Accounts { get; } = new();
    public List<(Guid Id, TransactionType Type, string Name)> Categories { get; } = new();

    public List<(Guid Id, TransactionType Type, Guid BankAccountId, decimal Amount, DateTime Date, string Description,
        Guid CategoryId)> Operations { get; } = new();

    public void Visit(BankAccount acc) => Accounts.Add((acc.Id, acc.Name, acc.Balance));
    public void Visit(Category cat) => Categories.Add((cat.Id, cat.Type, cat.Name));

    public void Visit(Operation op) => Operations.Add((op.Id, op.Type, op.BankAccountId, op.Amount, op.Date,
        op.Description, op.CategoryId));
}