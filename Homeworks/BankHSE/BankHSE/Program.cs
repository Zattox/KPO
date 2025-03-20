using BankHSE.Application.Analytics;
using BankHSE.Application.Commands;
using BankHSE.Application.Export;
using BankHSE.Application.Facades;
using BankHSE.Application.Factories;
using BankHSE.Application.Import;
using BankHSE.Application.Repositories;
using BankHSE.Domain.Abstractions;
using BankHSE.Domain.Entities;
using BankHSE.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;

class Program
{
    static void Main()
    {
        var services = new ServiceCollection()
            .AddSingleton<IRepository<BankAccount>, InMemoryRepository<BankAccount>>()
            .AddSingleton<IRepository<Category>, InMemoryRepository<Category>>()
            .AddSingleton<IRepository<Operation>, InMemoryRepository<Operation>>()
            .AddSingleton<CoreEntitiesFactory>()
            .AddSingleton<BankAccountFacade>()
            .AddSingleton<CategoryFacade>()
            .AddSingleton<OperationFacade>()
            .AddSingleton<AnalyticsService>()
            .AddSingleton<ICoreEntitiesAggregator, CoreEntitiesAggregator>()
            .AddSingleton<JsonExporter>()
            .AddSingleton<CsvExporter>()
            .AddSingleton<YamlExporter>()
            .AddSingleton<JsonImporter>()
            .AddSingleton<CsvImporter>()
            .AddSingleton<YamlImporter>()
            .BuildServiceProvider();

        var bankFacade = services.GetRequiredService<BankAccountFacade>();
        var catFacade = services.GetRequiredService<CategoryFacade>();
        var opFacade = services.GetRequiredService<OperationFacade>();
        var analytics = services.GetRequiredService<AnalyticsService>();
        var jsonExporter = services.GetRequiredService<JsonExporter>();
        var csvExporter = services.GetRequiredService<CsvExporter>();
        var yamlExporter = services.GetRequiredService<YamlExporter>();
        var jsonImporter = services.GetRequiredService<JsonImporter>();
        var csvImporter = services.GetRequiredService<CsvImporter>();
        var yamlImporter = services.GetRequiredService<YamlImporter>();

        var account = bankFacade.CreateAccount("Основной счет", 0m);
        var incomeCat = catFacade.CreateCategory(TransactionType.Income, "Зарплата");
        var expenseCat = catFacade.CreateCategory(TransactionType.Expense, "Кафе");

        var opCommand = new CreateOperationCommand(opFacade, TransactionType.Income, account.Id, 1000m, DateTime.Now,
            "Зарплата за март", incomeCat.Id);
        var timedCommand = new TimingCommand(opCommand);
        timedCommand.Execute();

        opCommand = new CreateOperationCommand(opFacade, TransactionType.Expense, account.Id, 300m, DateTime.Now,
            "Обед в кафе", expenseCat.Id);
        timedCommand = new TimingCommand(opCommand);
        timedCommand.Execute();

        Console.WriteLine($"Баланс счета: {account.Balance}");

        var difference = analytics.GetDifferenceByAccountId(account.Id, DateTime.Now.AddDays(-1), DateTime.Now);
        Console.WriteLine($"Разница доходов и расходов: {difference}");

        var grouped = analytics.GroupOperationsByCategory(account.Id, DateTime.Now.AddDays(-1), DateTime.Now);
        foreach (var kvp in grouped)
            Console.WriteLine($"Категория {catFacade.GetCategoryById(kvp.Key).Name}: {kvp.Value}");

        // Путь к папке Data
        string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", ".."));
        string jsonExportPath = Path.Combine(projectRoot, "Data", "data.json");
        string csvExportPath = Path.Combine(projectRoot, "Data", "data.csv");
        string yamlExportPath = Path.Combine(projectRoot, "Data", "data.yaml");

        // Экспорт в JSON
        jsonExporter.Export(jsonExportPath);
        Console.WriteLine($"Данные экспортированы в JSON: {jsonExportPath}");

        // Экспорт в CSV
        csvExporter.Export(csvExportPath);
        Console.WriteLine($"Данные экспортированы в CSV: {csvExportPath}");

        // Экспорт в YAML
        yamlExporter.Export(yamlExportPath);
        Console.WriteLine($"Данные экспортированы в YAML: {yamlExportPath}");

        // Очистка перед импортом
        foreach (var op in opFacade.GetAll().ToList()) opFacade.Delete(op.Id);
        foreach (var cat in catFacade.GetAllCategories().ToList()) catFacade.DeleteCategoryById(cat.Id);
        foreach (var acc in bankFacade.GetAllAccounts().ToList()) bankFacade.DeleteAccountById(acc.Id);

        // Импорт из YAML (можно переключить на JSON или CSV)
        yamlImporter.ImportAll(yamlExportPath);
        Console.WriteLine($"Импортировано счетов из YAML: {bankFacade.GetAllAccounts().Count()}");
        Console.WriteLine($"Импортировано категорий из YAML: {catFacade.GetAllCategories().Count()}");
        Console.WriteLine($"Импортировано операций из YAML: {opFacade.GetAll().Count()}");
        Console.WriteLine($"Баланс после импорта из YAML: {bankFacade.GetAccountById(account.Id).Balance}");
    }
}