using BankHSE.Application.Factories;
using BankHSE.Application.Facades;
using BankHSE.Application.Helpers;
using BankHSE.Domain.Enums;

namespace BankHSE.Application.Menus;

public class OperationMenu
{
    private readonly OperationFacade _operationFacade;
    private readonly CommandFactory _commandFactory;
    private readonly BankAccountFacade _bankAccountFacade;
    private readonly CategoryFacade _categoryFacade;

    public OperationMenu(OperationFacade operationFacade, CommandFactory commandFactory, 
        BankAccountFacade bankAccountFacade, CategoryFacade categoryFacade)
    {
        _operationFacade = operationFacade;
        _commandFactory = commandFactory;
        _bankAccountFacade = bankAccountFacade;
        _categoryFacade = categoryFacade;
    }

    public bool ShowMenu()
    {
        while (true)
        {
            Console.Clear();
            ConsoleHelper.PrintTextWithColor("=========== Manage Operations ===========", ConsoleColor.DarkCyan);
            Console.WriteLine("1. Add a new operation");
            Console.WriteLine("2. View all operations");
            Console.WriteLine("3. Update operation description");
            Console.WriteLine("4. Delete operation");
            Console.WriteLine("5. Back to main menu");
            ConsoleHelper.PrintTextWithColor("=========================================", ConsoleColor.DarkCyan);

            var key = Console.ReadKey().Key;
            switch (key)
            {
                case ConsoleKey.D1:
                    AddOperation();
                    break;
                case ConsoleKey.D2:
                    ViewAllOperations();
                    break;
                case ConsoleKey.D3:
                    UpdateOperation();
                    break;
                case ConsoleKey.D4:
                    DeleteOperation();
                    break;
                case ConsoleKey.D5:
                    return true;
                default:
                    ConsoleHelper.PrintTextWithColor("Invalid option.", ConsoleColor.Red);
                    break;
            }
            ConsoleHelper.PrintTextWithColor("Press any key to continue...", ConsoleColor.DarkYellow);
            Console.ReadKey();
        }
    }

    private void AddOperation()
    {
        Console.Clear();
        ConsoleHelper.PrintTextWithColor("Add a new operation", ConsoleColor.DarkCyan);
        Console.WriteLine("Select operation type: 1 - Income, 2 - Expense");
        var typeChoice = ConsoleHelper.ReadInt("Enter choice (1 or 2):", 1, 2);
        var type = typeChoice == 1 ? TransactionType.Income : TransactionType.Expense;

        var accounts = _bankAccountFacade.GetAllAccounts().ToList();
        var account = ConsoleHelper.SelectItemFromList(accounts, "Available accounts:", 
            acc => $"ID: {acc.Id}, Name: {acc.Name}, Balance: {acc.Balance}");
        if (account == null) return;

        var amount = ConsoleHelper.ReadDecimal("Enter amount (greater than 0):", 0.01m);
        var description = ConsoleHelper.ReadNonEmptyString("Enter description:");

        // Фильтруем категории по типу операции
        var categories = _categoryFacade.GetAllCategories()
            .Where(cat => cat.Type == type)
            .ToList();
        var category = ConsoleHelper.SelectItemFromList(categories, $"Available {type} categories:", 
            cat => $"ID: {cat.Id}, Type: {cat.Type}, Name: {cat.Name}");
        if (category == null)
        {
            ConsoleHelper.PrintTextWithColor($"No {type} categories available. Please add one first.", ConsoleColor.Yellow);
            return;
        }

        var opCommand = _commandFactory.CreateOperationCommand(type, account.Id, amount, DateTime.Now, description, category.Id);
        var timedCommand = _commandFactory.CreateTimedCommand(opCommand);
        try
        {
            timedCommand.Execute();
            ConsoleHelper.PrintTextWithColor("Operation added successfully.", ConsoleColor.Green);
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintTextWithColor($"Error: {ex.Message}", ConsoleColor.Red);
        }
    }

    private void ViewAllOperations()
    {
        Console.Clear();
        ConsoleHelper.PrintTextWithColor("All Operations", ConsoleColor.DarkCyan);
        var operations = _operationFacade.GetAll().ToList();
        if (!operations.Any())
            ConsoleHelper.PrintTextWithColor("No operations available.", ConsoleColor.Yellow);
        else
            foreach (var op in operations)
                Console.WriteLine($"ID: {op.Id}, Type: {op.Type}, Account: {op.BankAccountId}, Amount: {op.Amount}, Date: {op.Date}, Desc: {op.Description}, Cat: {op.CategoryId}");
    }

    private void UpdateOperation()
    {
        Console.Clear();
        ConsoleHelper.PrintTextWithColor("Update Operation", ConsoleColor.DarkCyan);
        var operations = _operationFacade.GetAll().ToList();
        var operation = ConsoleHelper.SelectItemFromList(operations, "Available operations:", 
            op => $"ID: {op.Id}, Type: {op.Type}, Amount: {op.Amount}, Desc: {op.Description}");
        if (operation == null) return;

        var description = ConsoleHelper.ReadNonEmptyString("Enter new description:");
        try
        {
            _operationFacade.UpdateOperationById(operation.Id, description);
            ConsoleHelper.PrintTextWithColor("Operation updated successfully.", ConsoleColor.Green);
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintTextWithColor($"Error: {ex.Message}", ConsoleColor.Red);
        }
    }

    private void DeleteOperation()
    {
        Console.Clear();
        ConsoleHelper.PrintTextWithColor("Delete Operation", ConsoleColor.DarkCyan);
        var operations = _operationFacade.GetAll().ToList();
        var operation = ConsoleHelper.SelectItemFromList(operations, "Available operations:", 
            op => $"ID: {op.Id}, Type: {op.Type}, Amount: {op.Amount}, Desc: {op.Description}");
        if (operation == null) return;

        try
        {
            _operationFacade.Delete(operation.Id);
            ConsoleHelper.PrintTextWithColor("Operation deleted successfully.", ConsoleColor.Green);
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintTextWithColor($"Error: {ex.Message}", ConsoleColor.Red);
        }
    }
}