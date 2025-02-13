namespace MiniHW_1.Zoo.Domain.Helpers.Menus;

using Entities.Creatures;
using Entities.Firms;

public class EmployeeMenu
{
    private readonly Func<int, string, string, Employee> _employeeFactory;
    private readonly Zoo _zoo;

    public EmployeeMenu(
        Func<int, string, string, Employee> employeeFactory,
        Zoo zoo)
    {
        _employeeFactory = employeeFactory;
        _zoo = zoo;
    }

    public void ShowMenu()
    {
        while (true)
        {
            Console.Clear();
            Methods.PrintTextWithColor("===========Manage employees===========\n", ConsoleColor.DarkCyan);
            Console.WriteLine("1. Add a zoo employee");
            Console.WriteLine("2. Display zoo employees");
            Console.WriteLine("3. Back to main menu");
            Methods.PrintTextWithColor("======================================\n", ConsoleColor.DarkCyan);

            var key = Console.ReadKey().Key;

            switch (key)
            {
                case ConsoleKey.D1:
                    AddEmployee();
                    break;
                case ConsoleKey.D2:
                    _zoo.PrintStaff();
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

    private void AddEmployee()
    {
        Console.Clear();
        Methods.PrintTextWithColor("Add a zoo employee\n", ConsoleColor.DarkCyan);
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