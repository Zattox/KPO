using BankHSE.Application.Facades;
using BankHSE.Application.Helpers;

namespace BankHSE.Application.Menus;

public class AccountMenu
{
    private readonly BankAccountFacade _bankAccountFacade;

    public AccountMenu(BankAccountFacade bankAccountFacade)
    {
        _bankAccountFacade = bankAccountFacade;
    }

    public bool ShowMenu()
    {
        while (true)
        {
            Console.Clear();
            ConsoleHelper.PrintTextWithColor("=========== Manage Accounts ===========", ConsoleColor.DarkCyan);
            Console.WriteLine("1. Add a new account");
            Console.WriteLine("2. View all accounts");
            Console.WriteLine("3. Update account name");
            Console.WriteLine("4. Delete account");
            Console.WriteLine("5. Recalculate balance");
            Console.WriteLine("6. Back to main menu");
            ConsoleHelper.PrintTextWithColor("=======================================", ConsoleColor.DarkCyan);

            var key = Console.ReadKey().Key;
            switch (key)
            {
                case ConsoleKey.D1:
                    AddAccount();
                    break;
                case ConsoleKey.D2:
                    ViewAllAccounts();
                    break;
                case ConsoleKey.D3:
                    UpdateAccount();
                    break;
                case ConsoleKey.D4:
                    DeleteAccount();
                    break;
                case ConsoleKey.D5:
                    RecalculateBalance();
                    break;
                case ConsoleKey.D6:
                    return true;
                default:
                    ConsoleHelper.PrintTextWithColor("Invalid option.", ConsoleColor.Red);
                    break;
            }
            ConsoleHelper.PrintTextWithColor("Press any key to continue...", ConsoleColor.DarkYellow);
            Console.ReadKey();
        }
    }

    private void AddAccount()
    {
        Console.Clear();
        ConsoleHelper.PrintTextWithColor("Add a new account", ConsoleColor.DarkCyan);
        var name = ConsoleHelper.ReadNonEmptyString("Enter account name:");
        var balance = ConsoleHelper.ReadDecimal("Enter initial balance (0 or more):", 0m);
        var account = _bankAccountFacade.CreateAccount(name, balance);
        ConsoleHelper.PrintTextWithColor($"Account {account.Name} added with ID: {account.Id}", ConsoleColor.Green);
    }

    private void ViewAllAccounts()
    {
        Console.Clear();
        ConsoleHelper.PrintTextWithColor("All Accounts", ConsoleColor.DarkCyan);
        var accounts = _bankAccountFacade.GetAllAccounts().ToList();
        if (!accounts.Any())
            ConsoleHelper.PrintTextWithColor("No accounts available.", ConsoleColor.Yellow);
        else
            foreach (var acc in accounts)
                Console.WriteLine($"ID: {acc.Id}, Name: {acc.Name}, Balance: {acc.Balance}");
    }

    private void UpdateAccount()
    {
        Console.Clear();
        ConsoleHelper.PrintTextWithColor("Update Account", ConsoleColor.DarkCyan);
        var accounts = _bankAccountFacade.GetAllAccounts().ToList();
        var account = ConsoleHelper.SelectItemFromList(accounts, "Available accounts:", acc => $"ID: {acc.Id}, Name: {acc.Name}, Balance: {acc.Balance}");
        if (account == null) return;

        var name = ConsoleHelper.ReadNonEmptyString("Enter new account name:");
        try
        {
            _bankAccountFacade.UpdateAccountById(account.Id, name);
            ConsoleHelper.PrintTextWithColor("Account updated successfully.", ConsoleColor.Green);
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintTextWithColor($"Error: {ex.Message}", ConsoleColor.Red);
        }
    }

    private void DeleteAccount()
    {
        Console.Clear();
        ConsoleHelper.PrintTextWithColor("Delete Account", ConsoleColor.DarkCyan);
        var accounts = _bankAccountFacade.GetAllAccounts().ToList();
        var account = ConsoleHelper.SelectItemFromList(accounts, "Available accounts:", acc => $"ID: {acc.Id}, Name: {acc.Name}, Balance: {acc.Balance}");
        if (account == null) return;

        try
        {
            _bankAccountFacade.DeleteAccountById(account.Id);
            ConsoleHelper.PrintTextWithColor("Account deleted successfully.", ConsoleColor.Green);
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintTextWithColor($"Error: {ex.Message}", ConsoleColor.Red);
        }
    }

    private void RecalculateBalance()
    {
        Console.Clear();
        ConsoleHelper.PrintTextWithColor("Recalculate Balance", ConsoleColor.DarkCyan);
        var accounts = _bankAccountFacade.GetAllAccounts().ToList();
        var account = ConsoleHelper.SelectItemFromList(accounts, "Available accounts:", acc => $"ID: {acc.Id}, Name: {acc.Name}, Balance: {acc.Balance}");
        if (account == null) return;

        try
        {
            _bankAccountFacade.RecalculateBalance(account.Id);
            var updatedAccount = _bankAccountFacade.GetAccountById(account.Id);
            ConsoleHelper.PrintTextWithColor($"Balance recalculated. New balance: {updatedAccount.Balance}", ConsoleColor.Green);
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintTextWithColor($"Error: {ex.Message}", ConsoleColor.Red);
        }
    }
}