﻿namespace MiniHW_1.Zoo.Domain.Helpers.Menus;

using Entities.Objects;
using Entities.Firms;

/// <summary>
/// Represents a menu for managing things in the zoo.
/// </summary>
public class ThingMenu
{
    private readonly Func<string, Table> _tableFactory;
    private readonly Func<string, Computer> _computerFactory;
    private readonly Zoo _zoo;

    /// <summary>
    /// Initializes a new instance of the <see cref="AnimalMenu"/> class.
    /// </summary>
    /// <param name="tableFactory">Factory for creating tables.</param>
    /// <param name="computerFactory">Factory for creating computers.</param>
    /// <param name="zoo">The zoo instance to manage things.</param>
    public ThingMenu(
        Func<string, Table> tableFactory,
        Func<string, Computer> computerFactory,
        Zoo zoo)
    {
        _tableFactory = tableFactory;
        _computerFactory = computerFactory;
        _zoo = zoo;
    }

    public void ShowMenu()
    {
        while (true)
        {
            Console.Clear();
            Methods.PrintTextWithColor("===========Manage items===========\n", ConsoleColor.DarkCyan);
            Console.WriteLine("1. Add a new item to the zoo's inventory");
            Console.WriteLine("2. Display the zoo's inventory");
            Console.WriteLine("3. Back to main menu");
            Methods.PrintTextWithColor("==================================\n", ConsoleColor.DarkCyan);

            var key = Console.ReadKey().Key;

            switch (key)
            {
                case ConsoleKey.D1:
                    AddThing();
                    break;
                case ConsoleKey.D2:
                    _zoo.PrintInventory();
                    break;
                case ConsoleKey.D3:
                    return;
                default:
                    Methods.PrintTextWithColor("Invalid option (#>_<)\n", ConsoleColor.Red);
                    break;
            }

            Methods.PrintTextWithColor("Press any key to continue...\n", ConsoleColor.DarkYellow);
            Console.ReadKey();
        }
    }

    /// <summary>
    /// Displays the animal management menu and handles user input.
    /// </summary>
    private void AddThing()
    {
        Console.Clear();
        Methods.PrintTextWithColor("Add a new item to the zoo's inventory\n", ConsoleColor.DarkCyan);
        Console.WriteLine("Choose the item you want to add:");
        Console.WriteLine("1. Table");
        Console.WriteLine("2. Computer");
        var key = Console.ReadKey().Key;

        string name = Methods.ReadNonEmptyString("Enter the item name:");

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
}