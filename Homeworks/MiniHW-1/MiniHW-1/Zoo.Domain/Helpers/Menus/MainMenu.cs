namespace MiniHW_1.Zoo.Domain.Helpers.Menus;

/// <summary>
/// Represents the main menu of the zoo management application.
/// This class provides options to manage animals, items, and employees.
/// </summary>
public class MainMenu
{
    private readonly AnimalMenu _animalMenu;
    private readonly ThingMenu _thingMenu;
    private readonly EmployeeMenu _employeeMenu;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainMenu"/> class.
    /// </summary>
    /// <param name="animalMenu">The menu for managing animals.</param>
    /// <param name="thingMenu">The menu for managing items.</param>
    /// <param name="employeeMenu">The menu for managing employees.</param>
    public MainMenu(AnimalMenu animalMenu, ThingMenu thingMenu, EmployeeMenu employeeMenu)
    {
        _animalMenu = animalMenu;
        _thingMenu = thingMenu;
        _employeeMenu = employeeMenu;
    }

    /// <summary>
    /// Displays the main menu and handles user input.
    /// </summary>
    public void ShowMenu()
    {
        while (true)
        {
            Console.Clear();
            Methods.PrintTextWithColor("=============Zoo manager=============\n", ConsoleColor.DarkCyan);
            Console.WriteLine("1. Manage animals");
            Console.WriteLine("2. Manage items");
            Console.WriteLine("3. Manage employees");
            Console.WriteLine("4. Exit the program");
            Methods.PrintTextWithColor("=====================================\n", ConsoleColor.DarkCyan);
            
            var key = Console.ReadKey().Key;

            switch (key)
            {
                case ConsoleKey.D1:
                    _animalMenu.ShowMenu();
                    break;
                case ConsoleKey.D2:
                    _thingMenu.ShowMenu();
                    break;
                case ConsoleKey.D3:
                    _employeeMenu.ShowMenu();
                    break;
                case ConsoleKey.D4:
                    Methods.PrintTextWithColor("Program terminated. Thank you!\n", ConsoleColor.Green);
                    return;
                default:
                    Methods.PrintTextWithColor("Invalid option (#>_<)\n", ConsoleColor.Red);
                    break;
            }

            Methods.PrintTextWithColor("Press any key to continue...\n", ConsoleColor.DarkYellow);
            Console.ReadKey();
        }
    }
}