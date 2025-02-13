namespace MiniHW_1.Zoo.Domain.Entities.Creatures;

public class Herbo : Animal
{
    public int KindnessLevel { get; } // Уровень доброты для травоядных

    protected Herbo(int food, string name, int kindnessLevel)
        : base(food, name)
    {
        KindnessLevel = kindnessLevel;
    }
}