using MiniHW_1.Zoo.Domain.Entities.Firms;
using MiniHW_1.Zoo.Domain.Entities.Creatures;
using MiniHW_1.Zoo.Domain.Helpers;

namespace MiniHW_1.Zoo.Domain.Managers;

/// <summary>
/// Manages operations related to animals in the zoo, such as adding animals, checking their health, and generating reports.
/// </summary>
public class AnimalManager
{
    // List to store all animals managed by this class
    private readonly List<Animal> _animals;

    // Reference to the veterinary clinic for health checks
    private readonly VeterinaryClinic _clinic;

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="clinic">The veterinary clinic used for health checks.</param>
    public AnimalManager(VeterinaryClinic clinic)
    {
        _clinic = clinic;
        _animals = new List<Animal>();
    }

    /// <summary>
    /// Adds an animal to the zoo after checking its health status.
    /// </summary>
    /// <param name="animal">The animal to be added.</param>
    public void AddAnimal(Animal animal)
    {
        var healthStatus = _clinic.CheckHealth(animal);

        switch (healthStatus)
        {
            case HealthStatus.Healthy:
                _animals.Add(animal);
                Methods.PrintTextWithColor($"{animal.Name} accepted in zoo. Description: \"{animal.Description}\"\n",
                    ConsoleColor.DarkGreen);
                break;
            case HealthStatus.Sick:
                Methods.PrintTextWithColor($"{animal.Name} not accepted in zoo. Animal is sick!\n",
                    ConsoleColor.DarkRed);
                break;
            default:
                Methods.PrintTextWithColor($"A re-check is required for the animal {animal.Name}\n",
                    ConsoleColor.DarkYellow);
                break;
        }
    }

    /// <summary>
    /// Prints the total amount of food required for all animals per day.
    /// </summary>
    public void PrintAnimalFoodReport()
    {
        // Calculate the total food required by summing up the food requirement of each animal
        var totalFood = _animals.Sum(a => a.Food);
        Console.WriteLine($"Total amount of food for animals per day: {totalFood} kg.");
    }

    /// <summary>
    /// Displays a list of animals that can be part of the contact zoo (herbivores with a high kindness level).
    /// </summary>
    public void PrintContactZooAnimals()
    {
        Console.Clear();
        Methods.PrintTextWithColor($"Display the list of contact animals\n", ConsoleColor.DarkCyan);
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

    /// <summary>
    /// Prints a list of all animals in the zoo along with their details.
    /// </summary>
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