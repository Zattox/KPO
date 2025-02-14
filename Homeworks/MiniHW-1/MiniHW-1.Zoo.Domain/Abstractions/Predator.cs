namespace MiniHW_1.Zoo.Domain.Abstractions;

public abstract class Predator : Animal
{
    public Predator(int food, string name)
        : base(food, name)
    {
    }
}