using MiniHW_1.Zoo.Domain.Helpers;

while (true)
{
    while (true)
    {
        bool flag = Menu.ShowMenu();
        if (flag)
            break;
    }

    Methods.PrintTextWithColor("Если хотите закончить программу нажмите на ESC", ConsoleColor.DarkYellow);
    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
    {
        Methods.PrintTextWithColor("Программа завершена. Спасибо!", ConsoleColor.Green);
        break;
    }
}