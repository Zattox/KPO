using System.Xml.Schema;
using MiniHW_1.Zoo.Domain.Entities.Creatures;
using MiniHW_1.Zoo.Domain.Helpers;

namespace MiniHW_1.Zoo.Domain.Managers;

/// <summary>
/// Manages operations related to employees in the zoo, such as adding employees and generating reports.
/// </summary>
public class EmployeeManager
{
    private readonly List<Employee> _staff;

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    public EmployeeManager()
    {
        _staff = new List<Employee>();
    }

    /// <summary>
    /// Adds an employee to the staff list.
    /// </summary>
    /// <param name="employee">The employee to be added.</param>
    public void AddEmployee(Employee employee)
    {
        _staff.Add(employee);
        Methods.PrintTextWithColor($"Added to the staff {employee.Position} {employee.Name}.\n",
            ConsoleColor.DarkGreen);
    }

    public List<Employee> GetStaff() => _staff;
    
    /// <summary>
    /// Prints a list of all employees in the staff.
    /// </summary>
    public void PrintStaff()
    {
        Console.WriteLine("Staff:");
        foreach (var employee in _staff)
        {
            Console.WriteLine($"- {employee.Name}, Position: {employee.Position}, Food: {employee.Food} kgs");
        }
    }

    public int CalculateStaffFoodReport()
    {
        return _staff.Sum(e => e.Food);
    }
    
    /// <summary>
    /// Prints the total amount of food required for all staff members per day.
    /// </summary>
    public void PrintStaffFoodReport()
    {
        var totalFood = CalculateStaffFoodReport();
        Console.WriteLine($"Total amount of food for staff per day: {totalFood} kg.");
    }
}