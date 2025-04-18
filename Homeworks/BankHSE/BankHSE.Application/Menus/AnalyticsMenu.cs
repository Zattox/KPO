using BankHSE.Application.Analytics;
using BankHSE.Application.Facades;
using BankHSE.Application.Helpers;

namespace BankHSE.Application.Menus;

public class AnalyticsMenu
{
    private readonly AnalyticsService _analyticsService;
    private readonly CategoryFacade _categoryFacade;
    private readonly BankAccountFacade _bankAccountFacade;

    public AnalyticsMenu(AnalyticsService analyticsService, CategoryFacade categoryFacade, BankAccountFacade bankAccountFacade)
    {
        _analyticsService = analyticsService;
        _categoryFacade = categoryFacade;
        _bankAccountFacade = bankAccountFacade;
    }

    public bool ShowMenu()
    {
        while (true)
        {
            Console.Clear();
            ConsoleHelper.PrintTextWithColor("=========== Analytics ===========", ConsoleColor.DarkCyan);
            Console.WriteLine("1. View income vs expense difference");
            Console.WriteLine("2. Group operations by category");
            Console.WriteLine("3. View top categories by amount");
            Console.WriteLine("4. Back to main menu");
            ConsoleHelper.PrintTextWithColor("=================================", ConsoleColor.DarkCyan);

            var key = Console.ReadKey().Key;
            switch (key)
            {
                case ConsoleKey.D1:
                    ViewDifference();
                    break;
                case ConsoleKey.D2:
                    GroupByCategory();
                    break;
                case ConsoleKey.D3:
                    ViewTopCategories();
                    break;
                case ConsoleKey.D4:
                    return true;
                default:
                    ConsoleHelper.PrintTextWithColor("Invalid option.", ConsoleColor.Red);
                    break;
            }
            ConsoleHelper.PrintTextWithColor("Press any key to continue...", ConsoleColor.DarkYellow);
            Console.ReadKey();
        }
    }

    private void ViewDifference()
    {
        Console.Clear();
        ConsoleHelper.PrintTextWithColor("Income vs Expense Difference", ConsoleColor.DarkCyan);
        var accounts = _bankAccountFacade.GetAllAccounts().ToList();
        var account = ConsoleHelper.SelectItemFromList(accounts, "Available accounts:", acc => $"ID: {acc.Id}, Name: {acc.Name}, Balance: {acc.Balance}");
        if (account == null) return;

        var days = ConsoleHelper.ReadInt("Enter days back for analysis:", 1);
        var difference = _analyticsService.GetDifferenceByAccountId(account.Id, DateTime.Now.AddDays(-days), DateTime.Now);
        ConsoleHelper.PrintTextWithColor($"Difference: {difference}", ConsoleColor.Green);
    }

    private void GroupByCategory()
    {
        Console.Clear();
        ConsoleHelper.PrintTextWithColor("Group Operations by Category", ConsoleColor.DarkCyan);
        var accounts = _bankAccountFacade.GetAllAccounts().ToList();
        var account = ConsoleHelper.SelectItemFromList(accounts, "Available accounts:", acc => $"ID: {acc.Id}, Name: {acc.Name}, Balance: {acc.Balance}");
        if (account == null) return;

        var days = ConsoleHelper.ReadInt("Enter days back for analysis:", 1);
        var grouped = _analyticsService.GroupOperationsByCategory(account.Id, DateTime.Now.AddDays(-days), DateTime.Now);
        foreach (var kvp in grouped)
            Console.WriteLine($"Category {_categoryFacade.GetCategoryById(kvp.Key).Name}: {kvp.Value}");
    }

    private void ViewTopCategories()
    {
        Console.Clear();
        ConsoleHelper.PrintTextWithColor("Top Categories by Amount", ConsoleColor.DarkCyan);
        var accounts = _bankAccountFacade.GetAllAccounts().ToList();
        var account = ConsoleHelper.SelectItemFromList(accounts, "Available accounts:", acc => $"ID: {acc.Id}, Name: {acc.Name}, Balance: {acc.Balance}");
        if (account == null) return;

        var days = ConsoleHelper.ReadInt("Enter days back for analysis:", 1);
        var topN = ConsoleHelper.ReadInt("Enter number of top categories:", 1);
        var topCategories = _analyticsService.GetTopCategoriesByAmount(account.Id, DateTime.Now.AddDays(-days), DateTime.Now, topN);
        foreach (var (category, totalAmount) in topCategories)
            Console.WriteLine($"Category {category.Name}: {totalAmount}");
    }
}