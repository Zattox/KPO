using MiniHW_1.Zoo.Domain.Abstractions;

namespace MiniHW_1.Zoo.Domain.Entities.Creatures;

public enum HealthStatus
{
    Sick, // Животное болеет
    NeedsCheckup, // Животное нуждается в проверке
    Healthy // Животное здорово
}

public abstract class Animal : IAlive, IInventory
{
    public int Food { get; set; }
    public int Number { get; }
    public string Name { get; }
    public string Description { get; }
    public HealthStatus HealthStatus { get; set; }

    private static int _nextId = 1;

    protected Animal(int food, string name)
    {
        Name = name;
        Food = food;
        Number = _nextId++;
        Description = GenerateDescription();
        HealthStatus = HealthStatus.NeedsCheckup;
    }

    private string GenerateDescription()
    {
        string className = GetType().Name;
        return $"This is a {className}. It is a {(this is Herbo ? "Herbo" : "Predator")}.";
    }
}