namespace MiniHW_1.Zoo.Domain.Helpers;

using Microsoft.Extensions.DependencyInjection;
using Entities.Creatures;
using Entities.Firms;
using Entities.Objects;

public class Menu
{
    private static readonly Func<int, string, int, Monkey> _monkeyFactory;
    private static readonly Func<int, string, int, Rabbit> _rabbitFactory;
    private static readonly Func<int, string, Tiger> _tigerFactory;
    private static readonly Func<int, string, Wolf> _wolfFactory;
    private static readonly Func<int, string, string, Employee> _employeeFactory;
    private static readonly Func<string, Table> _tableFactory;
    private static readonly Func<string, Computer> _computerFactory;
    private static readonly Zoo _zoo;

    static Menu()
    {
        var serviceProvider = ServiceConfiguration.ConfigureServices();
        _zoo = serviceProvider.GetRequiredService<Zoo>();
        _monkeyFactory = serviceProvider.GetRequiredService<Func<int, string, int, Monkey>>();
        _rabbitFactory = serviceProvider.GetRequiredService<Func<int, string, int, Rabbit>>();
        _tigerFactory = serviceProvider.GetRequiredService<Func<int, string, Tiger>>();
        _wolfFactory = serviceProvider.GetRequiredService<Func<int, string, Wolf>>();
        _employeeFactory = serviceProvider.GetRequiredService<Func<int, string, string, Employee>>();
        _tableFactory = serviceProvider.GetRequiredService<Func<string, Table>>();
        _computerFactory = serviceProvider.GetRequiredService<Func<string, Computer>>();
    }

    public static bool ShowMenu()
    {
        bool exit = false;
        while (!exit)
        {
            Console.Clear();
            Console.WriteLine("1. Add a new animal to the zoo");
            Console.WriteLine("2. Add a new item to the zoo's inventory");
            Console.WriteLine("3. Add a zoo employee");
            Console.WriteLine("4. Display the required daily food amount for animals");
            Console.WriteLine("5. Display the list of contact animals");
            Console.WriteLine("6. Display the zoo's inventory");
            Console.WriteLine("7. Display zoo employees");
            Console.WriteLine("8. Exit the program");

            exit = Option(Console.ReadKey().Key);

            if (!exit) // Show pause only if the user did not choose to exit
            {
                Methods.PrintTextWithColor("Press any key to continue\n", ConsoleColor.DarkYellow);
                Console.ReadKey();
            }
        }

        return exit;
    }

    private static bool Option(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.D1:
                Console.Clear();
                Methods.PrintTextWithColor("1. Add a new animal to the zoo\n", ConsoleColor.Yellow);
                AddAnimal();
                return false;

            case ConsoleKey.D2:
                Console.Clear();
                Methods.PrintTextWithColor("2. Add a new item to the zoo's inventory\n", ConsoleColor.Yellow);
                AddThing();
                return false;

            case ConsoleKey.D3:
                Console.Clear();
                Methods.PrintTextWithColor("3. Add a zoo employee\n", ConsoleColor.Yellow);
                AddEmployee();
                return false;

            case ConsoleKey.D4:
                Console.Clear();
                Methods.PrintTextWithColor("4. Display the required daily food amount for animals\n",
                    ConsoleColor.Yellow);
                _zoo.PrintAnimalFoodReport();
                return false;

            case ConsoleKey.D5:
                Console.Clear();
                Methods.PrintTextWithColor("5. Display the list of contact animals\n", ConsoleColor.Yellow);
                _zoo.PrintContactZooAnimals();
                return false;

            case ConsoleKey.D6:
                Console.Clear();
                Methods.PrintTextWithColor("6. Display the zoo's inventory\n", ConsoleColor.Yellow);
                _zoo.PrintInventory();
                return false;

            case ConsoleKey.D7:
                Console.Clear();
                Methods.PrintTextWithColor("7. Display zoo employees\n", ConsoleColor.Yellow);
                _zoo.PrintStaff();
                return false;

            case ConsoleKey.D8:
                Console.Clear();
                Methods.PrintTextWithColor("8. Exit the program\n", ConsoleColor.Yellow);
                return true;

            default:
                Console.Clear();
                Methods.PrintTextWithColor("Invalid option (#>_<)\n", ConsoleColor.Red);
                return false;
        }
    }

    private static void AddAnimal()
    {
        Console.WriteLine("Select the animal you want to add:");
        Console.WriteLine("1. Monkey");
        Console.WriteLine("2. Rabbit");
        Console.WriteLine("3. Wolf");
        Console.WriteLine("4. Tiger");
        var key = Console.ReadKey().Key;

        Console.WriteLine("\nEnter the amount of food (kg/day):");
        int food = int.Parse(Console.ReadLine());

        Console.WriteLine("Enter the animal's name:");
        string name = Console.ReadLine();

        switch (key)
        {
            case ConsoleKey.D1:
                Console.WriteLine("Enter kindness level (1-10):");
                int kindnessLevel = int.Parse(Console.ReadLine());
                var monkey = _monkeyFactory(food, name, kindnessLevel);
                _zoo.AddAnimal(monkey);
                break;

            case ConsoleKey.D2:
                Console.WriteLine("Enter kindness level (1-10):");
                kindnessLevel = int.Parse(Console.ReadLine());
                var rabbit = _rabbitFactory(food, name, kindnessLevel);
                _zoo.AddAnimal(rabbit);
                break;

            case ConsoleKey.D3:
                var wolf = _wolfFactory(food, name);
                _zoo.AddAnimal(wolf);
                break;

            case ConsoleKey.D4:
                var tiger = _tigerFactory(food, name);
                _zoo.AddAnimal(tiger);
                break;

            default:
                Methods.PrintTextWithColor("Invalid animal type.", ConsoleColor.Red);
                break;
        }
    }

    private static void AddThing()
    {
        Console.WriteLine("Choose the item you want to add:");
        Console.WriteLine("1. Table");
        Console.WriteLine("2. Computer");
        var key = Console.ReadKey().Key;

        Console.WriteLine("\nEnter the item name:");
        string name = Console.ReadLine();

        switch (key)
        {
            case ConsoleKey.D1:
                var table = _tableFactory(name);
                _zoo.AddThing(table);
                break;

            case ConsoleKey.D2:
                var computer = _computerFactory(name);
                _zoo.AddThing(computer);
                break;

            default:
                Methods.PrintTextWithColor("Invalid item type.", ConsoleColor.Red);
                break;
        }
    }

    private static void AddEmployee()
    {
        Console.WriteLine("Enter the amount of food (kg/day):");
        int food = int.Parse(Console.ReadLine());

        Console.WriteLine("Enter the employee's name:");
        string name = Console.ReadLine();

        Console.WriteLine("Enter the employee's position:");
        string position = Console.ReadLine();

        var employee = _employeeFactory(food, name, position);
        _zoo.AddEmployee(employee);
    }
}