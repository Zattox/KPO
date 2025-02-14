using MiniHW_1.Zoo.Domain.Entities.Creatures;
using MiniHW_1.Zoo.Domain.Abstractions;
using MiniHW_1.Zoo.Domain.Helpers;
using MiniHW_1.Zoo.Domain.Managers;

namespace MiniHW_1.Zoo.Domain.Entities.Firms;

/// <summary>
/// Represents a zoo that manages animals, items, and employees.
/// </summary>
public class Zoo
{
    private readonly AnimalManager _animalManager;
    private readonly ThingManager _thingManager;
    private readonly EmployeeManager _employeeManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="Zoo"/> class.
    /// </summary>
    /// <param name="clinic">The veterinary clinic used to check animal health.</param>
    public Zoo(VeterinaryClinic clinic)
    {
        _animalManager = new AnimalManager(clinic);
        _thingManager = new ThingManager();
        _employeeManager = new EmployeeManager();
    }

    /// <summary>
    /// Adds an animal to the zoo.
    /// </summary>
    /// <param name="animal">The animal to add.</param>
    public void AddAnimal(Animal animal) => _animalManager.AddAnimal(animal);

    /// <summary>
    /// Adds an item to the zoo's inventory.
    /// </summary>
    /// <param name="thing">The item to add.</param>
    public void AddThing(Thing thing) => _thingManager.AddThing(thing);

    /// <summary>
    /// Adds an employee to the zoo.
    /// </summary>
    /// <param name="employee">The employee to add.</param>
    public void AddEmployee(Employee employee) => _employeeManager.AddEmployee(employee);

    /// <summary>
    /// Prints the total amount of food required for all animals per day.
    /// </summary>
    public void PrintAnimalFoodReport() => _animalManager.PrintAnimalFoodReport();

    /// <summary>
    /// Prints the list of animals that can be placed in the contact zoo.
    /// </summary>
    public void PrintContactZooAnimals() => _animalManager.PrintContactZooAnimals();

    /// <summary>
    /// Prints the zoo's inventory, including animals and items.
    /// </summary>
    public void PrintInventory()
    {
        Console.Clear();
        Methods.PrintTextWithColor("Display the zoo's inventory\n", ConsoleColor.DarkCyan);
        Console.WriteLine("==============The zoo's inventory==============");
        _animalManager.PrintAnimals();
        _thingManager.PrintThings();
        _animalManager.PrintAnimalFoodReport();
        Console.WriteLine("===============================================");
    }

    /// <summary>
    /// Prints the list of zoo employees and their total food consumption.
    /// </summary>
    public void PrintStaff()
    {
        Console.Clear();
        Methods.PrintTextWithColor("Display zoo employees\n", ConsoleColor.DarkCyan);
        Console.WriteLine("===================Zoo staff===================");
        _employeeManager.PrintStaff();
        _employeeManager.PrintStaffFoodReport();
        Console.WriteLine("===============================================");
    }
}