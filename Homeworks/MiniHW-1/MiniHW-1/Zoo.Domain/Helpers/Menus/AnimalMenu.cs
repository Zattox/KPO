namespace MiniHW_1.Zoo.Domain.Helpers.Menus;

using Entities.Creatures;
using Entities.Firms;

public class AnimalMenu
{
    private readonly Func<int, string, int, Monkey> _monkeyFactory;
    private readonly Func<int, string, int, Rabbit> _rabbitFactory;
    private readonly Func<int, string, Tiger> _tigerFactory;
    private readonly Func<int, string, Wolf> _wolfFactory;
    private readonly Zoo _zoo;

    public AnimalMenu(
        Func<int, string, int, Monkey> monkeyFactory,
        Func<int, string, int, Rabbit> rabbitFactory,
        Func<int, string, Tiger> tigerFactory,
        Func<int, string, Wolf> wolfFactory,
        Zoo zoo)
    {
        _monkeyFactory = monkeyFactory;
        _rabbitFactory = rabbitFactory;
        _tigerFactory = tigerFactory;
        _wolfFactory = wolfFactory;
        _zoo = zoo;
    }

    public void ShowMenu()
    {
        while (true)
        {
            Console.Clear();
            Methods.PrintTextWithColor("=====================Manage animals=====================\n",
                ConsoleColor.DarkCyan);
            Console.WriteLine("1. Add a new animal to the zoo");
            Console.WriteLine("2. Display the required daily food amount for animals");
            Console.WriteLine("3. Display the list of contact animals");
            Console.WriteLine("4. Back to main menu");
            Methods.PrintTextWithColor("========================================================\n",
                ConsoleColor.DarkCyan);

            var key = Console.ReadKey().Key;

            switch (key)
            {
                case ConsoleKey.D1:
                    AddAnimal();
                    break;
                case ConsoleKey.D2:
                    Console.Clear();
                    Methods.PrintTextWithColor($"Display the required daily food amount for animals\n",
                        ConsoleColor.DarkCyan);
                    _zoo.PrintAnimalFoodReport();
                    break;
                case ConsoleKey.D3:
                    _zoo.PrintContactZooAnimals();
                    break;
                case ConsoleKey.D4:
                    return;
                default:
                    Methods.PrintTextWithColor("Invalid option (#>_<)\n", ConsoleColor.Red);
                    break;
            }

            Methods.PrintTextWithColor("Press any key to continue...\n", ConsoleColor.DarkYellow);
            Console.ReadKey();
        }
    }

    private void AddAnimal()
    {
        Console.Clear();
        Methods.PrintTextWithColor("Add a new animal to the zoo\n", ConsoleColor.DarkCyan);
        Console.WriteLine("Select the animal you want to add:");
        Console.WriteLine("1. Monkey");
        Console.WriteLine("2. Rabbit");
        Console.WriteLine("3. Wolf");
        Console.WriteLine("4. Tiger");
        var key = Console.ReadKey().Key;

        int food = Methods.ReadInt("Enter the amount of food (kg/day):", 0);
        string name = Methods.ReadNonEmptyString("Enter the animal name:");

        switch (key)
        {
            case ConsoleKey.D1:
                int kindnessLevel = Methods.ReadInt("Enter the animal's kindness level (1-10):", 1, 10);
                var monkey = _monkeyFactory(food, name, kindnessLevel);
                _zoo.AddAnimal(monkey);
                break;

            case ConsoleKey.D2:
                kindnessLevel = Methods.ReadInt("Enter the animal's kindness level (1-10):", 1, 10);
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
}