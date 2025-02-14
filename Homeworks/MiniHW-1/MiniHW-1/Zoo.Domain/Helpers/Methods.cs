namespace MiniHW_1.Zoo.Domain.Helpers;

public class Methods
{
    private const int Inf = Int32.MaxValue;

    /// <summary>
    /// Reads an integer from the console within a specified range.
    /// </summary>
    /// <param name="prompt">The message to display to the user.</param>
    /// <param name="minValue">The minimum allowed value.</param>
    /// <param name="maxValue">The maximum allowed value.</param>
    /// <returns>A valid integer within the specified range.</returns>
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

    /// <summary>
    /// Reads a non-empty string from the console.
    /// </summary>
    /// <param name="prompt">The message to display to the user.</param>
    /// <returns>A non-empty string entered by the user.</returns>
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

    /// <summary>
    /// Prints text to the console with the specified color.
    /// </summary>
    /// <param name="text">The text to print.</param>
    /// <param name="color">The color to use for the text.</param>
    public static void PrintTextWithColor(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write(text);
        Console.ForegroundColor = ConsoleColor.White;
    }
}