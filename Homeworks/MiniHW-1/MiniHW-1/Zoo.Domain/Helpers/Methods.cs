namespace MiniHW_1.Zoo.Domain.Helpers;

public class Methods
{
    private const int Inf = Int32.MaxValue;

    public static int ReadInt(string prompt, int minValue = -Inf, int maxValue = Inf)
    {
        int result;
        while (true)
        {
            Console.WriteLine(prompt);
            if (int.TryParse(Console.ReadLine(), out result) && result >= minValue && result <= maxValue)
                return result;
            Methods.PrintTextWithColor($"Invalid input. Please enter a {minValue} <= number <= {maxValue}.\n",
                ConsoleColor.Red);
        }
    }

    public static string ReadNonEmptyString(string prompt)
    {
        string? input;
        while (true)
        {
            Console.WriteLine(prompt);
            input = Console.ReadLine()?.Trim();

            if (!string.IsNullOrEmpty(input))
            {
                return input;
            }

            Methods.PrintTextWithColor($"Input cannot be empty. Please try again.\n", ConsoleColor.Red);
        }
    }

    public static void PrintTextWithColor(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write(text);
        Console.ForegroundColor = ConsoleColor.White;
    }
}