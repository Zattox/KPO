using MiniHW_1.Zoo.Domain.Helpers;

while (true)
{
    while (true)
    {
        bool flag = Menu.ShowMenu();
        if (flag)
            break;
    }

    Methods.PrintTextWithColor("Press 'Q' to exit the program\n", ConsoleColor.DarkYellow);
    if (Console.ReadKey(true).Key == ConsoleKey.Q)
    {
        Methods.PrintTextWithColor("Program terminated. Thank you!\n", ConsoleColor.Green);
        break;
    }

}