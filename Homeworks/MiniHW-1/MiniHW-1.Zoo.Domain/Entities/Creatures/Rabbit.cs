using MiniHW_1.Zoo.Domain.Abstractions;
namespace MiniHW_1.Zoo.Domain.Entities.Creatures;

public class Rabbit : Herbo
{
    public Rabbit(int food, string name, int kindnessLevel)
        : base(food, name, kindnessLevel)
    {
    }
}