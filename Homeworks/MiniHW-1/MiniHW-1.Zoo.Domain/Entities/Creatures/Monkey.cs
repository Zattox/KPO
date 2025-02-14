using MiniHW_1.Zoo.Domain.Abstractions;
namespace MiniHW_1.Zoo.Domain.Entities.Creatures;

public class Monkey : Herbo
{
    public Monkey(int food, string name, int kindnessLevel)
        : base(food, name,  kindnessLevel)
    {
    }
}