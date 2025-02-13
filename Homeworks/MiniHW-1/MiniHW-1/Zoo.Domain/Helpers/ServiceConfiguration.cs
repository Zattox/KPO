using Microsoft.Extensions.DependencyInjection;
using MiniHW_1.Zoo.Domain.Entities.Firms;
using MiniHW_1.Zoo.Domain.Entities.Creatures;
using MiniHW_1.Zoo.Domain.Entities.Objects;

namespace MiniHW_1.Zoo.Domain.Helpers;

public static class ServiceConfiguration
{
    public static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<VeterinaryClinic>();
        services.AddSingleton<Entities.Firms.Zoo>();

        services.AddTransient<Func<int, string, int, Monkey>>(provider =>
            (food, name, kindnessLevel) => new Monkey(food, name, kindnessLevel));

        services.AddTransient<Func<int, string, int, Rabbit>>(provider =>
            (food, name, kindnessLevel) => new Rabbit(food, name, kindnessLevel));

        services.AddTransient<Func<int, string, Tiger>>(provider =>
            (food, name) => new Tiger(food, name));

        services.AddTransient<Func<int, string, Wolf>>(provider =>
            (food, name) => new Wolf(food, name));

        services.AddTransient<Func<string, Table>>(provider =>
            (name) => new Table(name));

        services.AddTransient<Func<string, Computer>>(provider =>
            (name) => new Computer(name));

        services.AddTransient<Func<int, string, string, Employee>>(provider =>
            (food, name, position) => new Employee(food, name, position));

        return services.BuildServiceProvider();
    }
}