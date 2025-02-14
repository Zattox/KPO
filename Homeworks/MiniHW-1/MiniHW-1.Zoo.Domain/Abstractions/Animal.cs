namespace MiniHW_1.Zoo.Domain.Abstractions;

public enum HealthStatus
{
    Sick, // The animal is sick
    NeedsCheckup, // The animal needs to be checked
    Healthy // The animal is great
}

/// <summary>
/// Represents an abstract base class for all animals in the zoo.
/// Implements <see cref="IAlive"/> and <see cref="IInventory"/> interfaces.
/// </summary>
public abstract class Animal : IAlive, IInventory
{
    public int Food { get; set; }

    public int Number { get; }

    public string Name { get; }

    public string Description { get; }

    public HealthStatus HealthStatus { get; set; }

    private static int _nextId = 1;

    /// <summary>
    /// Initializes a new instance of the <see cref="Animal"/> class.
    /// </summary>
    /// <param name="food">The amount of food the animal consumes per day (in kg).</param>
    /// <param name="name">The name of the animal.</param>
    protected Animal(int food, string name)
    {
        Name = name;
        Food = food;
        Number = _nextId++;
        Description = GenerateDescription();
        HealthStatus = HealthStatus.NeedsCheckup;
    }

    /// <summary>
    /// Generates a description for the animal based on its type.
    /// </summary>
    /// <returns>A string describing the animal.</returns>
    private string GenerateDescription()
    {
        string className = GetType().Name;
        return $"This is a {className}. It is a {(this is Herbo ? "Herbo" : "Predator")}.";
    }
}