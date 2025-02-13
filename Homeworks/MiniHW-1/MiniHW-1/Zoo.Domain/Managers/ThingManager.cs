using MiniHW_1.Zoo.Domain.Entities.Objects;
using MiniHW_1.Zoo.Domain.Helpers;

namespace MiniHW_1.Zoo.Domain.Managers;

public class ThingManager
{
    private readonly List<Thing> _things;

    public ThingManager()
    {
        _things = new List<Thing>();
    }

    public void AddThing(Thing thing)
    {
        _things.Add(thing);
        Methods.PrintTextWithColor($"The item {thing.Name} was added to the zoo's inventory.\n", ConsoleColor.DarkGreen);
    }

    public void PrintThings()
    {
        Console.WriteLine("Things:");
        foreach (var thing in _things)
        {
            Console.WriteLine($"- {thing.Name} (№{thing.Number})");
        }
    }
}