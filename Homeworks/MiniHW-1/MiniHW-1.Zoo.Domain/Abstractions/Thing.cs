namespace MiniHW_1.Zoo.Domain.Abstractions;

public abstract class Thing : IInventory
{
    public int Number { get; set; }
    public string Name { get; }
    public string Description { get; }

    private static int _nextId = 1;
    public Thing(string name)
    {
        Name = name;
        Number = _nextId++;
        Description = GenerateDescription();
    }
    
    private string GenerateDescription()
    {
        string className = GetType().Name;
        return $"This is a {className} used in the zoo.";
    }
}