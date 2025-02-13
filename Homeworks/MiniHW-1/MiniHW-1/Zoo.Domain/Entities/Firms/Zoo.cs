using MiniHW_1.Zoo.Domain.Abstractions;
using MiniHW_1.Zoo.Domain.Entities.Creatures;
using MiniHW_1.Zoo.Domain.Entities.Objects;
using MiniHW_1.Zoo.Domain.Helpers;

namespace MiniHW_1.Zoo.Domain.Entities.Firms;

public class Zoo
{
    private readonly List<Animal> _animals;
    private readonly List<Employee> _staff;
    private readonly List<Thing> _things;
    private readonly VeterinaryClinic _clinic;

    public Zoo(VeterinaryClinic clinic)
    {
        _clinic = clinic;
        _animals = [];
        _things = [];
        _staff = [];
    }

    public void AddAnimal(Animal animal)
    {
        var healthStatus = _clinic.CheckHealth(animal);

        switch (healthStatus)
        {
            case HealthStatus.Healthy:
                _animals.Add(animal);
                string text = $"{animal.Name} accepted in zoo. Description: \"{animal.Description}\"\n";
                Methods.PrintTextWithColor(text, ConsoleColor.DarkGreen);
                break;
            case HealthStatus.Sick:
                text = $"{animal.Name} not accepted in zoo. Animal is sick!\n";
                Methods.PrintTextWithColor(text, ConsoleColor.DarkRed);
                break;
            default:
                text = $"A re-check is required for the animal {animal.Name}\n";
                Methods.PrintTextWithColor(text, ConsoleColor.DarkYellow);
                break;
        }
    }

    public void AddThing(Thing thing)
    {
        _things.Add(thing);
        string text = $"The item {thing.Name} was added to the zoo's inventory.\n";
        Methods.PrintTextWithColor(text, ConsoleColor.DarkGreen);
    }

    public void AddEmployee(Employee employee)
    {
        _staff.Add(employee);
        string text = $"Added to the staff {employee.Position} {employee.Name}.\n";
        Methods.PrintTextWithColor(text, ConsoleColor.DarkGreen);
    }

    private void PrintFoodReport(List<IAlive> arr, string entity)
    {
        var totalFood = arr.Sum(a => a.Food);
        string text = $"Total amount of food for {entity} per day: {totalFood} kg.\n";
        Methods.PrintTextWithColor(text, ConsoleColor.DarkYellow);
    }

    public void PrintAnimalFoodReport()
    {
        PrintFoodReport(_animals.Cast<IAlive>().ToList(), "animal");
    }

    public void PrintStaffFoodReport()
    {
        PrintFoodReport(_staff.Cast<IAlive>().ToList(), "staff");
    }

    public void PrintContactZooAnimals()
    {
        Console.WriteLine("\n================Contact animals================");
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

    public void PrintInventory()
    {
        Console.WriteLine("\n==============The zoo's inventory==============");
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

        Console.WriteLine("Things:");
        foreach (var thing in _things)
        {
            Console.WriteLine($"- {thing.Name} (№{thing.Number})");
        }

        PrintAnimalFoodReport();
        Console.WriteLine("===============================================");
    }

    public void PrintStaff()
    {
        Console.WriteLine("\n===================Zoo staff===================");
        foreach (var employee in _staff)
        {
            Console.WriteLine($"- {employee.Name}, Position: {employee.Position}, Food: {employee.Food} kgs");
        }

        PrintStaffFoodReport();
        Console.WriteLine("===============================================");
    }
}