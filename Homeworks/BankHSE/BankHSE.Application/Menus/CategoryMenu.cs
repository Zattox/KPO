using BankHSE.Application.Facades;
using BankHSE.Application.Helpers;
using BankHSE.Domain.Enums;

namespace BankHSE.Application.Menus;

public class CategoryMenu
{
    private readonly CategoryFacade _categoryFacade;

    public CategoryMenu(CategoryFacade categoryFacade)
    {
        _categoryFacade = categoryFacade;
    }

    public bool ShowMenu()
    {
        while (true)
        {
            Console.Clear();
            ConsoleHelper.PrintTextWithColor("=========== Manage Categories ===========", ConsoleColor.DarkCyan);
            Console.WriteLine("1. Add a new category");
            Console.WriteLine("2. View all categories");
            Console.WriteLine("3. Update category");
            Console.WriteLine("4. Delete category");
            Console.WriteLine("5. Back to main menu");
            ConsoleHelper.PrintTextWithColor("=========================================", ConsoleColor.DarkCyan);

            var key = Console.ReadKey().Key;
            switch (key)
            {
                case ConsoleKey.D1:
                    AddCategory();
                    break;
                case ConsoleKey.D2:
                    ViewAllCategories();
                    break;
                case ConsoleKey.D3:
                    UpdateCategory();
                    break;
                case ConsoleKey.D4:
                    DeleteCategory();
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

    private void AddCategory()
    {
        Console.Clear();
        ConsoleHelper.PrintTextWithColor("Add a new category", ConsoleColor.DarkCyan);
        Console.WriteLine("Select category type: 1 - Income, 2 - Expense");
        var typeChoice = ConsoleHelper.ReadInt("Enter choice (1 or 2):", 1, 2);
        var type = typeChoice == 1 ? TransactionType.Income : TransactionType.Expense;
        var name = ConsoleHelper.ReadNonEmptyString("Enter category name:");
        var category = _categoryFacade.CreateCategory(type, name);
        ConsoleHelper.PrintTextWithColor($"Category {category.Name} added with ID: {category.Id}", ConsoleColor.Green);
    }

    private void ViewAllCategories()
    {
        Console.Clear();
        ConsoleHelper.PrintTextWithColor("All Categories", ConsoleColor.DarkCyan);
        var categories = _categoryFacade.GetAllCategories().ToList();
        if (!categories.Any())
            ConsoleHelper.PrintTextWithColor("No categories available.", ConsoleColor.Yellow);
        else
            foreach (var cat in categories)
                Console.WriteLine($"ID: {cat.Id}, Type: {cat.Type}, Name: {cat.Name}");
    }

    private void UpdateCategory()
    {
        Console.Clear();
        ConsoleHelper.PrintTextWithColor("Update Category", ConsoleColor.DarkCyan);
        var categories = _categoryFacade.GetAllCategories().ToList();
        var category = ConsoleHelper.SelectItemFromList(categories, "Available categories:", cat => $"ID: {cat.Id}, Type: {cat.Type}, Name: {cat.Name}");
        if (category == null) return;

        var name = ConsoleHelper.ReadNonEmptyString("Enter new category name:");
        try
        {
            _categoryFacade.UpdateCategoryById(category.Id, null, name);
            ConsoleHelper.PrintTextWithColor("Category updated successfully.", ConsoleColor.Green);
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintTextWithColor($"Error: {ex.Message}", ConsoleColor.Red);
        }
    }

    private void DeleteCategory()
    {
        Console.Clear();
        ConsoleHelper.PrintTextWithColor("Delete Category", ConsoleColor.DarkCyan);
        var categories = _categoryFacade.GetAllCategories().ToList();
        var category = ConsoleHelper.SelectItemFromList(categories, "Available categories:", cat => $"ID: {cat.Id}, Type: {cat.Type}, Name: {cat.Name}");
        if (category == null) return;

        try
        {
            _categoryFacade.DeleteCategoryById(category.Id);
            ConsoleHelper.PrintTextWithColor("Category deleted successfully.", ConsoleColor.Green);
        }
        catch (Exception ex)
        {
            ConsoleHelper.PrintTextWithColor($"Error: {ex.Message}", ConsoleColor.Red);
        }
    }
}