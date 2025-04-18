using BankHSE.Application.Analytics;
using BankHSE.Application.Export;
using BankHSE.Application.Facades;
using BankHSE.Application.Factories;
using BankHSE.Application.Menus;
using BankHSE.Application.Repositories;
using BankHSE.Application.Strategy;
using BankHSE.Domain.Abstractions;
using BankHSE.Domain.Entities;
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
        var services = new ServiceCollection()
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
            .AddSingleton<ExportContext>()
            .AddSingleton<ImportContext>()
            // Register all export strategies
            .AddSingleton<IExportStrategy, JsonExportStrategy>()
            .AddSingleton<IExportStrategy, CsvExportStrategy>()
            .AddSingleton<IExportStrategy, YamlExportStrategy>()
            // Register all import strategies
            .AddSingleton<IImportStrategy, JsonImportStrategy>()
            .AddSingleton<IImportStrategy, CsvImportStrategy>()
            .AddSingleton<IImportStrategy, YamlImportStrategy>()
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
            .AddSingleton<MainMenu>();

        return services.BuildServiceProvider();
    }
}