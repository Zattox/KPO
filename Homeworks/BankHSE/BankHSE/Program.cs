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
    static ServiceProvider ConfigureServices()
    {
        return new ServiceCollection()
            // Регистрируем CachedRepository как прокси над InMemoryRepository
            .AddSingleton<IRepository<BankAccount>>(sp =>
                new CachedRepository<BankAccount>(new InMemoryRepository<BankAccount>()))
            .AddSingleton<IRepository<Category>>(sp =>
                new CachedRepository<Category>(new InMemoryRepository<Category>()))
            .AddSingleton<IRepository<Operation>>(sp =>
                new CachedRepository<Operation>(new InMemoryRepository<Operation>()))
            .AddSingleton<CoreEntitiesFactory>()
            .AddSingleton<BankAccountFacade>()
            .AddSingleton<CategoryFacade>()
            .AddSingleton<OperationFacade>()
            .AddSingleton<AnalyticsService>()
            .AddSingleton<CommandFactory>()
            .AddSingleton<ICoreEntitiesAggregator, CoreEntitiesAggregator>()
            .AddSingleton<JsonExporter>()
            .AddSingleton<CsvExporter>()
            .AddSingleton<YamlExporter>()
            .AddSingleton<JsonImporter>()
            .AddSingleton<CsvImporter>()
            .AddSingleton<YamlImporter>()
            .BuildServiceProvider();
    }

    static void Main()
    {
        var services = ConfigureServices();
        var bankFacade = services.GetRequiredService<BankAccountFacade>();
        var catFacade = services.GetRequiredService<CategoryFacade>();
        var opFacade = services.GetRequiredService<OperationFacade>();
        var analytics = services.GetRequiredService<AnalyticsService>();
        var commandFactory = services.GetRequiredService<CommandFactory>();
        var jsonExporter = services.GetRequiredService<JsonExporter>();
        var csvExporter = services.GetRequiredService<CsvExporter>();
        var yamlExporter = services.GetRequiredService<YamlExporter>();
        var jsonImporter = services.GetRequiredService<JsonImporter>();
        var csvImporter = services.GetRequiredService<CsvImporter>();
        var yamlImporter = services.GetRequiredService<YamlImporter>();

        // Создание данных
        var account = bankFacade.CreateAccount("Основной счет", 0m);
        var incomeCat = catFacade.CreateCategory(TransactionType.Income, "Зарплата");
        var expenseCat = catFacade.CreateCategory(TransactionType.Expense, "Кафе");

        var opCommand = commandFactory.CreateOperationCommand(TransactionType.Income, account.Id, 1000m, DateTime.Now,
            "Зарплата за март", incomeCat.Id);
        var timedCommand = commandFactory.CreateTimedCommand(opCommand);
        timedCommand.Execute();

        opCommand = commandFactory.CreateOperationCommand(TransactionType.Expense, account.Id, 300m, DateTime.Now,
            "Обед в кафе", expenseCat.Id);
        timedCommand = commandFactory.CreateTimedCommand(opCommand);
        timedCommand.Execute();

        Console.WriteLine($"Баланс счета до пересчета: {account.Balance}");

        // После создания операций
        Console.WriteLine("Демонстрация кэширования:");
        var startTime = DateTime.Now;
        var accountFromCache = bankFacade.GetAccountById(account.Id); // Первый вызов, попадет в кэш
        var firstCallDuration = DateTime.Now - startTime;
        Console.WriteLine($"Первый вызов GetById: {firstCallDuration.TotalMilliseconds} ms");

        startTime = DateTime.Now;
        accountFromCache = bankFacade.GetAccountById(account.Id); // Второй вызов, из кэша
        var secondCallDuration = DateTime.Now - startTime;
        Console.WriteLine($"Второй вызов GetById (из кэша): {secondCallDuration.TotalMilliseconds} ms");

        // Демонстрация пересчета баланса
        bankFacade.IncreaseBalanceById(account.Id, 500m);
        Console.WriteLine($"Баланс после искусственного изменения: {account.Balance}");
        bankFacade.RecalculateBalance(account.Id);
        Console.WriteLine($"Баланс после пересчета: {account.Balance}");

        // Аналитика
        var difference = analytics.GetDifferenceByAccountId(account.Id, DateTime.Now.AddDays(-1), DateTime.Now);
        Console.WriteLine($"Разница доходов и расходов: {difference}");

        var grouped = analytics.GroupOperationsByCategory(account.Id, DateTime.Now.AddDays(-1), DateTime.Now);
        foreach (var kvp in grouped)
            Console.WriteLine($"Категория {catFacade.GetCategoryById(kvp.Key).Name}: {kvp.Value}");

        var topCategories = analytics.GetTopCategoriesByAmount(account.Id, DateTime.Now.AddDays(-1), DateTime.Now, 2);
        Console.WriteLine("Топ-2 категории по сумме операций:");
        foreach (var (category, totalAmount) in topCategories)
            Console.WriteLine($"Категория {category.Name}: {totalAmount}");

        // Путь к папке Data
        string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", ".."));
        string jsonExportPath = Path.Combine(projectRoot, "Data", "data.json");
        string csvExportPath = Path.Combine(projectRoot, "Data", "data.csv");
        string yamlExportPath = Path.Combine(projectRoot, "Data", "data.yaml");

        // Экспорт
        jsonExporter.Export(jsonExportPath);
        Console.WriteLine($"Данные экспортированы в JSON: {jsonExportPath}");
        csvExporter.Export(csvExportPath);
        Console.WriteLine($"Данные экспортированы в CSV: {csvExportPath}");
        yamlExporter.Export(yamlExportPath);
        Console.WriteLine($"Данные экспортированы в YAML: {yamlExportPath}");

        // Очистка перед импортом
        foreach (var op in opFacade.GetAll().ToList()) opFacade.Delete(op.Id);
        foreach (var cat in catFacade.GetAllCategories().ToList()) catFacade.DeleteCategoryById(cat.Id);
        foreach (var acc in bankFacade.GetAllAccounts().ToList()) bankFacade.DeleteAccountById(acc.Id);

        // Импорт из YAML
        yamlImporter.ImportAll(yamlExportPath);
        Console.WriteLine($"Импортировано счетов из YAML: {bankFacade.GetAllAccounts().Count()}");
        Console.WriteLine($"Импортировано категорий из YAML: {catFacade.GetAllCategories().Count()}");
        Console.WriteLine($"Импортировано операций из YAML: {opFacade.GetAll().Count()}");
        Console.WriteLine($"Баланс после импорта из YAML: {bankFacade.GetAccountById(account.Id).Balance}");
    }
}