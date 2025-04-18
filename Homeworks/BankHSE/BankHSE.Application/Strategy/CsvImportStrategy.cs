using System.Globalization;
using System.Text;
using BankHSE.Application.Factories;
using BankHSE.Domain.Abstractions;
using BankHSE.Domain.Entities;
using BankHSE.Domain.Enums;

namespace BankHSE.Application.Strategy;

public class CsvImportStrategy : IImportStrategy
{
    private readonly CoreEntitiesFactory _factory;
    private readonly IRepository<BankAccount> _accRepo;
    private readonly IRepository<Category> _catRepo;
    private readonly IRepository<Operation> _opRepo;

    public CsvImportStrategy(
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
        var lines = File.ReadAllLines(path, Encoding.UTF8);
        var section = "";
        var lineIdx = 0;

        while (lineIdx < lines.Length)
        {
            var line = lines[lineIdx].Trim();
            if (string.IsNullOrEmpty(line))
            {
                lineIdx++;
                continue;
            }

            if (line == "Accounts" || line == "Categories" || line == "Operations")
            {
                section = line;
                lineIdx += 2; // Skip header
                continue;
            }

            var parts = ParseCsvLine(line);
            switch (section)
            {
                case "Accounts":
                    _accRepo.Add(_factory.CreateBankAccount(
                        Guid.Parse(parts[0]),
                        UnescapeCsv(parts[1]),
                        decimal.Parse(parts[2], CultureInfo.InvariantCulture)));
                    break;

                case "Categories":
                    _catRepo.Add(_factory.CreateCategory(
                        Guid.Parse(parts[0]),
                        Enum.Parse<TransactionType>(parts[1]),
                        UnescapeCsv(parts[2])));
                    break;

                case "Operations":
                    var acc = _accRepo.GetById(Guid.Parse(parts[2])) ?? throw new InvalidOperationException("Account not found.");
                    var cat = _catRepo.GetById(Guid.Parse(parts[6])) ?? throw new InvalidOperationException("Category not found.");
                    _opRepo.Add(_factory.CreateOperation(
                        Guid.Parse(parts[0]),
                        Enum.Parse<TransactionType>(parts[1]),
                        acc,
                        decimal.Parse(parts[3], CultureInfo.InvariantCulture),
                        DateTime.Parse(parts[4], CultureInfo.InvariantCulture),
                        UnescapeCsv(parts[5]),
                        cat));
                    break;
            }
            lineIdx++;
        }
    }

    // Parse a CSV line into parts, handling quoted fields
    private static string[] ParseCsvLine(string line)
    {
        var parts = new List<string>();
        var current = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            if (c == '"')
                inQuotes = !inQuotes;
            else if (c == ';' && !inQuotes)
            {
                parts.Add(current.ToString());
                current.Clear();
            }
            else
                current.Append(c);
        }
        parts.Add(current.ToString());
        return parts.ToArray();
    }

    // Unescape CSV values by removing quotes and handling escaped quotes
    private static string UnescapeCsv(string value)
    {
        if (value.StartsWith("\"") && value.EndsWith("\""))
            return value.Substring(1, value.Length - 2).Replace("\"\"", "\"");
        return value;
    }
}