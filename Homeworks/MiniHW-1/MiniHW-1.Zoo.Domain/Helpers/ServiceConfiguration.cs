using Microsoft.Extensions.DependencyInjection;
using MiniHW_1.Zoo.Domain.Entities.Firms;
using MiniHW_1.Zoo.Domain.Entities.Creatures;
using MiniHW_1.Zoo.Domain.Entities.Objects;
using MiniHW_1.Zoo.Domain.Helpers.Menus;

namespace MiniHW_1.Zoo.Domain.Helpers;

public static class ServiceConfiguration
{
    public static IServiceProvider ConfigureServices()
    {
        // Create a new service collection to register dependencies
        var services = new ServiceCollection();

        // Register VeterinaryClinic and Zoo as singletons (one instance shared across the application)
        services.AddSingleton<VeterinaryClinic>();
        services.AddSingleton<Entities.Firms.Zoo>();

        // Register factory methods for creating creatures with transient lifetime (new instance each time)
        services.AddTransient<Func<int, string, int, Monkey>>(provider =>
            (food, name, kindnessLevel) => new Monkey(food, name, kindnessLevel));

        services.AddTransient<Func<int, string, int, Rabbit>>(provider =>
            (food, name, kindnessLevel) => new Rabbit(food, name, kindnessLevel));

        services.AddTransient<Func<int, string, Tiger>>(provider =>
            (food, name) => new Tiger(food, name));

        services.AddTransient<Func<int, string, Wolf>>(provider =>
            (food, name) => new Wolf(food, name));

        // Register factory methods for creating objects with transient lifetime
        services.AddTransient<Func<string, Table>>(provider =>
            (name) => new Table(name));

        services.AddTransient<Func<string, Computer>>(provider =>
            (name) => new Computer(name));

        // Register factory method for creating employees with transient lifetime
        services.AddTransient<Func<int, string, string, Employee>>(provider =>
            (food, name, position) => new Employee(food, name, position));

        // Register menus as transient services (new instance each time they are requested)
        services.AddTransient<AnimalMenu>();
        services.AddTransient<ThingMenu>();
        services.AddTransient<EmployeeMenu>();
        services.AddTransient<MainMenu>();

        // Build and return the service provider
        return services.BuildServiceProvider();
    }
}