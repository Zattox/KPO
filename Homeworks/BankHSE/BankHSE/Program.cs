using BankHSE.Application.Analytics;
using BankHSE.Application.Export;
using BankHSE.Application.Facades;
using BankHSE.Application.Factories;
using BankHSE.Application.Import;
using BankHSE.Application.Repositories;
using BankHSE.Domain.Abstractions;
using BankHSE.Domain.Entities;
using BankHSE.Application.Menus;
using Microsoft.Extensions.DependencyInjection;

class Program
{
    static void Main()
    {
        var services = ConfigureServices();
        var mainMenu = services.GetRequiredService<MainMenu>();
        mainMenu.ShowMenu();
    }

    // Configure dependency injection container
    static ServiceProvider ConfigureServices()
    {
        return new ServiceCollection()
            .AddSingleton<IRepository<BankAccount>>(sp => new CachedRepository<BankAccount>(new InMemoryRepository<BankAccount>()))
            .AddSingleton<IRepository<Category>>(sp => new CachedRepository<Category>(new InMemoryRepository<Category>()))
            .AddSingleton<IRepository<Operation>>(sp => new CachedRepository<Operation>(new InMemoryRepository<Operation>()))
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
            .AddSingleton<AccountMenu>()
            .AddSingleton<CategoryMenu>()
            .AddSingleton<OperationMenu>(sp => new OperationMenu(
                sp.GetRequiredService<OperationFacade>(),
                sp.GetRequiredService<CommandFactory>(),
                sp.GetRequiredService<BankAccountFacade>(),
                sp.GetRequiredService<CategoryFacade>()))
            .AddSingleton<AnalyticsMenu>(sp => new AnalyticsMenu(
                sp.GetRequiredService<AnalyticsService>(),
                sp.GetRequiredService<CategoryFacade>(),
                sp.GetRequiredService<BankAccountFacade>()))
            .AddSingleton<MainMenu>()
            .BuildServiceProvider();
    }
}