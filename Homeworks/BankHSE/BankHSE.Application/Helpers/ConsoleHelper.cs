namespace BankHSE.Application.Helpers;

public static class ConsoleHelper
{
    public static int ReadInt(string prompt, int minValue = int.MinValue, int maxValue = int.MaxValue)
    {
        int result;
        while (true)
        {
            Console.WriteLine(prompt);
            if (int.TryParse(Console.ReadLine(), out result) && result >= minValue && result <= maxValue)
                return result;
            PrintTextWithColor($"Invalid input. Please enter a number between {minValue} and {maxValue}.",
                ConsoleColor.Red);
        }
    }

    public static decimal ReadDecimal(string prompt, decimal minValue = decimal.MinValue)
    {
        decimal result;
        while (true)
        {
            Console.WriteLine(prompt);
            if (decimal.TryParse(Console.ReadLine(), out result) && result >= minValue)
                return result;
            PrintTextWithColor($"Invalid input. Please enter a number >= {minValue}.", ConsoleColor.Red);
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
                return input;
            PrintTextWithColor("Input cannot be empty. Please try again.", ConsoleColor.Red);
        }
    }

    public static void PrintTextWithColor(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ForegroundColor = ConsoleColor.White;
    }

    public static T SelectItemFromList<T>(List<T> items, string prompt, Func<T, string> displayFunc)
    {
        if (!items.Any())
        {
            PrintTextWithColor("No items available.", ConsoleColor.Yellow);
            return default;
        }

        Console.WriteLine(prompt);
        for (int i = 0; i < items.Count; i++)
            Console.WriteLine($"{i + 1}. {displayFunc(items[i])}");

        int choice = ReadInt($"Enter number (1-{items.Count}):", 1, items.Count);
        return items[choice - 1];
    }
}