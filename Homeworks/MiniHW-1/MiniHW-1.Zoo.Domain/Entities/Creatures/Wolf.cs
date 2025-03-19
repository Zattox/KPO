using MiniHW_1.Zoo.Domain.Abstractions;

namespace MiniHW_1.Zoo.Domain.Entities.Creatures;

public class Wolf : Predator
{
    public Wolf(int food, string name)
        : base(food, name)
    {
    }
}