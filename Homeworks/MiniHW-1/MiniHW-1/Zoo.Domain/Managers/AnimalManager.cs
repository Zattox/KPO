using MiniHW_1.Zoo.Domain.Entities.Firms;
using MiniHW_1.Zoo.Domain.Entities.Creatures;
using MiniHW_1.Zoo.Domain.Helpers;

namespace MiniHW_1.Zoo.Domain.Managers;

public class AnimalManager
{
    private readonly List<Animal> _animals;
    private readonly VeterinaryClinic _clinic;

    public AnimalManager(VeterinaryClinic clinic)
    {
        _clinic = clinic;
        _animals = new List<Animal>();
    }

    public void AddAnimal(Animal animal)
    {
        var healthStatus = _clinic.CheckHealth(animal);

        switch (healthStatus)
        {
            case HealthStatus.Healthy:
                _animals.Add(animal);
                Methods.PrintTextWithColor($"{animal.Name} accepted in zoo. Description: \"{animal.Description}\"\n", ConsoleColor.DarkGreen);
                break;
            case HealthStatus.Sick:
                Methods.PrintTextWithColor($"{animal.Name} not accepted in zoo. Animal is sick!\n", ConsoleColor.DarkRed);
                break;
            default:
                Methods.PrintTextWithColor($"A re-check is required for the animal {animal.Name}\n", ConsoleColor.DarkYellow);
                break;
        }
    }

    public void PrintAnimalFoodReport()
    {
        var totalFood = _animals.Sum(a => a.Food);
        Methods.PrintTextWithColor($"Total amount of food for animals per day: {totalFood} kg.\n", ConsoleColor.DarkGreen);
    }

    public void PrintContactZooAnimals()
    {
        Console.WriteLine("================Contact animals================");
        var contactAnimals = _animals.OfType<Herbo>().Where(a => a.KindnessLevel > 5);

        if (!contactAnimals.Any())
        {
            Methods.PrintTextWithColor("None of the animals can be in the contact zoo.\n", ConsoleColor.DarkGray);
        }
        else
        {
            foreach (var animal in contactAnimals)
            {
                Console.WriteLine($"- {animal.Name}. {animal.Description}");
            }
        }

        Console.WriteLine("===============================================");
    }

    public void PrintAnimals()
    {
        Console.WriteLine("Animals:");
        foreach (var animal in _animals)
        {
            Console.Write($"- {animal.Name} (№{animal.Number}, {animal.Food} kg, ");
            var textColor = animal.HealthStatus switch
            {
                HealthStatus.Sick => ConsoleColor.DarkRed,
                HealthStatus.Healthy => ConsoleColor.DarkGreen,
                _ => ConsoleColor.DarkYellow,
            };
            Methods.PrintTextWithColor($"{animal.HealthStatus}", textColor);
            Console.WriteLine(")");
        }
    }
}