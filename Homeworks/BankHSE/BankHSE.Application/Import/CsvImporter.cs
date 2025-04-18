using System.Globalization;
using System.Text;
using BankHSE.Domain.Abstractions;
using BankHSE.Domain.Entities;
using BankHSE.Domain.Enums;
using BankHSE.Application.Factories;

namespace BankHSE.Application.Import;

public class CsvImporter : Importer
{
    private readonly CoreEntitiesFactory _factory;
    private readonly IRepository<BankAccount> _accounts;
    private readonly IRepository<Category> _categories;
    private readonly IRepository<Operation> _operations;

    public CsvImporter(CoreEntitiesFactory factory, IRepository<BankAccount> accounts,
        IRepository<Category> categories, IRepository<Operation> operations)
    {
        _factory = factory;
        _accounts = accounts;
        _categories = categories;
        _operations = operations;
    }

    public void ImportAll(string path)
    {
        var lines = File.ReadAllLines(path, Encoding.UTF8);
        var section = "";
        var lineIndex = 0;

        while (lineIndex < lines.Length)
        {
            var line = lines[lineIndex].Trim();
            if (string.IsNullOrEmpty(line))
            {
                lineIndex++;
                continue;
            }

            if (line == "Accounts" || line == "Categories" || line == "Operations")
            {
                section = line;
                lineIndex += 2; // Пропускаем заголовок
                continue;
            }

            var parts = ParseCsvLine(line);
            switch (section)
            {
                case "Accounts":
                    var account = _factory.CreateBankAccount(
                        Guid.Parse(parts[0]),
                        UnescapeCsv(parts[1]),
                        decimal.Parse(parts[2], CultureInfo.InvariantCulture)
                    );
                    _accounts.Add(account);
                    break;

                case "Categories":
                    var category = _factory.CreateCategory(
                        Guid.Parse(parts[0]),
                        Enum.Parse<TransactionType>(parts[1]),
                        UnescapeCsv(parts[2])
                    );
                    _categories.Add(category);
                    break;

                case "Operations":
                    var operationAccount = _accounts.GetById(Guid.Parse(parts[2])) ??
                                           throw new InvalidOperationException("Account not found.");
                    var operationCategory = _categories.GetById(Guid.Parse(parts[6])) ??
                                            throw new InvalidOperationException("Category not found.");
                    var operation = _factory.CreateOperation(
                        Guid.Parse(parts[0]),
                        Enum.Parse<TransactionType>(parts[1]),
                        operationAccount,
                        decimal.Parse(parts[3], CultureInfo.InvariantCulture),
                        DateTime.Parse(parts[4], CultureInfo.InvariantCulture),
                        UnescapeCsv(parts[5]),
                        operationCategory
                    );
                    _operations.Add(operation);
                    break;
            }

            lineIndex++;
        }
    }

    protected override IEnumerable<Operation> Parse(string content)
    {
        ImportAll(content); // Для совместимости с базовым методом
        return _operations.GetAll();
    }

    private static string[] ParseCsvLine(string line)
    {
        var parts = new List<string>();
        var currentPart = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ';' && !inQuotes)
            {
                parts.Add(currentPart.ToString());
                currentPart.Clear();
            }
            else
            {
                currentPart.Append(c);
            }
        }

        parts.Add(currentPart.ToString()); // Последний элемент
        return parts.ToArray();
    }

    private static string UnescapeCsv(string value)
    {
        if (value.StartsWith("\"") && value.EndsWith("\""))
            return value.Substring(1, value.Length - 2).Replace("\"\"", "\"");
        return value;
    }
}