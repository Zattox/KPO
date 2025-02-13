using MiniHW_1.Zoo.Domain.Abstractions;

namespace MiniHW_1.Zoo.Domain.Entities.Creatures;

public class Employee : IAlive
{
    public int Food { get; set; }
    public string Name { get; set; }
    public string Position { get; set; }

    public Employee(int food, string name, string position)
    {
        Food = food;
        Name = name;
        Position = position;
    }
}