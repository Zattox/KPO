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
        Console.WriteLine("1. Добавить нового животного в зоопарк");
        Console.WriteLine("2. Добавить новый предмет в инвентарь зоопарка");
        Console.WriteLine("3. Добавить сотрудника зоопарка");
        Console.WriteLine("4. Вывести необходимое количество еды в день для животных");
        Console.WriteLine("5. Вывести список контактных животных");
        Console.WriteLine("6. Вывести инветарь зоопарка");
        Console.WriteLine("7. Вывести сотрудников зоопарка");
        Console.WriteLine("8. Выйти из программы");
        return Option(Console.ReadKey().Key);
    }

    private static bool Option(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.D1:
                Console.Clear();
                Methods.PrintTextWithColor("1. Добавить нового животного в зоопарк\n", ConsoleColor.Yellow);
                AddAnimal();
                return false;

            case ConsoleKey.D2:
                Console.Clear();
                Methods.PrintTextWithColor("2. Добавить новый предмет в инвентарь зоопарка\n", ConsoleColor.Yellow);
                AddThing();
                return false;

            case ConsoleKey.D3:
                Console.Clear();
                Methods.PrintTextWithColor("3. Добавить сотрудника зоопарка\n", ConsoleColor.Yellow);
                AddEmployee();
                return false;

            case ConsoleKey.D4:
                Console.Clear();
                Methods.PrintTextWithColor("4. Вывести необходимое количество еды в день для животных\n",
                    ConsoleColor.Yellow);
                _zoo.PrintAnimalFoodReport();
                return false;

            case ConsoleKey.D5:
                Console.Clear();
                Methods.PrintTextWithColor("5. Вывести список контактных животных\n", ConsoleColor.Yellow);
                _zoo.PrintContactZooAnimals();
                return false;

            case ConsoleKey.D6:
                Console.Clear();
                Methods.PrintTextWithColor("6. Вывести инветарь зоопарка\n", ConsoleColor.Yellow);
                _zoo.PrintInventory();
                return false;

            case ConsoleKey.D7:
                Console.Clear();
                Methods.PrintTextWithColor("7. Вывести сотрудников зоопарка\n", ConsoleColor.Yellow);
                _zoo.PrintStaff();
                return false;

            case ConsoleKey.D8:
                Console.Clear();
                Methods.PrintTextWithColor("8. Выйти из программы\n", ConsoleColor.Yellow);
                return true;

            default:
                Console.Clear();
                Methods.PrintTextWithColor("Такой опции нет (#>_<)\n", ConsoleColor.Red);
                return false;
        }
    }

    private static void AddAnimal()
    {
        Console.WriteLine("Выберите животного, которого хотите добавить:");
        Console.WriteLine("1. Обезьяна");
        Console.WriteLine("2. Кролик");
        Console.WriteLine("3. Волк");
        Console.WriteLine("4. Тигр");
        var key = Console.ReadKey().Key;

        Console.WriteLine("\nВведите количество еды (кг/день):");
        int food = int.Parse(Console.ReadLine());

        Console.WriteLine("Введите имя животного:");
        string name = Console.ReadLine();

        switch (key)
        {
            case ConsoleKey.D1:
                Console.WriteLine("Введите уровень доброты (1-10):");
                int kindnessLevel = int.Parse(Console.ReadLine());
                var monkey = _monkeyFactory(food, name, kindnessLevel);
                _zoo.AddAnimal(monkey);
                break;

            case ConsoleKey.D2:
                Console.WriteLine("Введите уровень доброты (1-10):");
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
                Methods.PrintTextWithColor("Неверный тип животного.", ConsoleColor.Red);
                break;
        }
    }

    private static void AddThing()
    {
        Console.WriteLine("Выберите предмет, который хотите добавить:");
        Console.WriteLine("1. Стол");
        Console.WriteLine("2. Компьютер");
        var key = Console.ReadKey().Key;

        Console.WriteLine("\nВведите название предмета:");
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
                Methods.PrintTextWithColor("Неверный тип предмета.", ConsoleColor.Red);
                break;
        }
    }

    private static void AddEmployee()
    {
        Console.WriteLine("Введите количество еды (кг/день):");
        int food = int.Parse(Console.ReadLine());

        Console.WriteLine("Введите имя сотрудника:");
        string name = Console.ReadLine();

        Console.WriteLine("Введите должность сотрудника:");
        string position = Console.ReadLine();

        var employee = _employeeFactory(food, name, position);
        _zoo.AddEmployee(employee);
    }
}