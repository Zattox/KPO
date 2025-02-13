using MiniHW_1.Zoo.Domain.Entities.Creatures;
using MiniHW_1.Zoo.Domain.Helpers;

namespace MiniHW_1.Zoo.Domain.Managers;

public class EmployeeManager
{
    private readonly List<Employee> _staff;

    public EmployeeManager()
    {
        _staff = new List<Employee>();
    }

    public void AddEmployee(Employee employee)
    {
        _staff.Add(employee);
        Methods.PrintTextWithColor($"Added to the staff {employee.Position} {employee.Name}.\n",
            ConsoleColor.DarkGreen);
    }

    public void PrintStaff()
    {
        Console.WriteLine("Staff:");
        foreach (var employee in _staff)
        {
            Console.WriteLine($"- {employee.Name}, Position: {employee.Position}, Food: {employee.Food} kgs");
        }
    }

    public void PrintStaffFoodReport()
    {
        var totalFood = _staff.Sum(e => e.Food);
        Console.WriteLine($"Total amount of food for staff per day: {totalFood} kg.");
    }
}