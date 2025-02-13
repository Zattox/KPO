namespace MiniHW_1.Zoo.Domain.Helpers;

public class Methods
{
    public static void PrintTextWithColor(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write(text);
        Console.ForegroundColor = ConsoleColor.White;
    }
}