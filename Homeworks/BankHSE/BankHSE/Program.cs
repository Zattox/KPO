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
            .AddSingleton<JsonImporter>()
            .BuildServiceProvider();

        var bankFacade = services.GetRequiredService<BankAccountFacade>();
        var catFacade = services.GetRequiredService<CategoryFacade>();
        var opFacade = services.GetRequiredService<OperationFacade>();
        var analytics = services.GetRequiredService<AnalyticsService>();
        var exporter = services.GetRequiredService<JsonExporter>();
        var importer = services.GetRequiredService<JsonImporter>();

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

        // Путь в корень проекта
        string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", ".."));
        string exportPath = Path.Combine(Path.Combine(projectRoot, "Data"), "data.json");
        exporter.Export(exportPath);
        Console.WriteLine($"Данные экспортированы в {exportPath}");

        foreach (var op in opFacade.GetAll().ToList()) opFacade.Delete(op.Id);
        foreach (var cat in catFacade.GetAllCategories().ToList()) catFacade.DeleteCategoryById(cat.Id);
        foreach (var acc in bankFacade.GetAllAccounts().ToList()) bankFacade.DeleteAccountById(acc.Id);

        importer.ImportAll(exportPath);
        Console.WriteLine($"Импортировано счетов: {bankFacade.GetAllAccounts().Count()}");
        Console.WriteLine($"Импортировано категорий: {catFacade.GetAllCategories().Count()}");
        Console.WriteLine($"Импортировано операций: {opFacade.GetAll().Count()}");
        Console.WriteLine($"Баланс после импорта: {bankFacade.GetAccountById(account.Id).Balance}");
    }
}