using MiniHW_1.Zoo.Domain.Entities.Objects;
using MiniHW_1.Zoo.Domain.Helpers;

namespace MiniHW_1.Zoo.Domain.Managers;

/// <summary>
/// Manages operations related to things (objects) in the zoo, such as adding items to the inventory and listing them.
/// </summary>
public class ThingManager
{
    private readonly List<Thing> _things;

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    public ThingManager()
    {
        _things = new List<Thing>();
    }

    /// <summary>
    /// Adds a thing to the zoo's inventory.
    /// </summary>
    /// <param name="thing">The thing to be added.</param>
    public void AddThing(Thing thing)
    {
        _things.Add(thing);
        Methods.PrintTextWithColor($"The item {thing.Name} was added to the zoo's inventory.\n", ConsoleColor.DarkGreen);
    }

    /// <summary>
    /// Prints a list of all things in the zoo's inventory.
    /// </summary>
    public void PrintThings()
    {
        Console.WriteLine("Things:");
        foreach (var thing in _things)
        {
            Console.WriteLine($"- {thing.Name} (№{thing.Number})");
        }
    }
}