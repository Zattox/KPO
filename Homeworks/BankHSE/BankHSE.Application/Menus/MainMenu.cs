using BankHSE.Application.Export;
using BankHSE.Application.Helpers;
using BankHSE.Application.Import;

namespace BankHSE.Application.Menus;

public class MainMenu
{
    private readonly AccountMenu _accountMenu;
    private readonly CategoryMenu _categoryMenu;
    private readonly OperationMenu _operationMenu;
    private readonly AnalyticsMenu _analyticsMenu;
    private readonly JsonExporter _jsonExporter;
    private readonly CsvExporter _csvExporter;
    private readonly YamlExporter _yamlExporter;
    private readonly JsonImporter _jsonImporter;
    private readonly CsvImporter _csvImporter;
    private readonly YamlImporter _yamlImporter;

    public MainMenu(AccountMenu accountMenu, CategoryMenu categoryMenu, OperationMenu operationMenu,
        AnalyticsMenu analyticsMenu, JsonExporter jsonExporter, CsvExporter csvExporter, YamlExporter yamlExporter,
        JsonImporter jsonImporter, CsvImporter csvImporter, YamlImporter yamlImporter)
    {
        _accountMenu = accountMenu;
        _categoryMenu = categoryMenu;
        _operationMenu = operationMenu;
        _analyticsMenu = analyticsMenu;
        _jsonExporter = jsonExporter;
        _csvExporter = csvExporter;
        _yamlExporter = yamlExporter;
        _jsonImporter = jsonImporter;
        _csvImporter = csvImporter;
        _yamlImporter = yamlImporter;
    }

    public void ShowMenu()
    {
        while (true)
        {
            Console.Clear();
            ConsoleHelper.PrintTextWithColor("=========== Bank Manager ===========", ConsoleColor.DarkCyan);
            Console.WriteLine("1. Manage accounts");
            Console.WriteLine("2. Manage categories");
            Console.WriteLine("3. Manage operations");
            Console.WriteLine("4. Analytics");
            Console.WriteLine("5. Export data");
            Console.WriteLine("6. Import data");
            Console.WriteLine("7. Exit");
            ConsoleHelper.PrintTextWithColor("====================================", ConsoleColor.DarkCyan);

            var key = Console.ReadKey().Key;
            switch (key)
            {
                case ConsoleKey.D1:
                    _accountMenu.ShowMenu();
                    break;
                case ConsoleKey.D2:
                    _categoryMenu.ShowMenu();
                    break;
                case ConsoleKey.D3:
                    _operationMenu.ShowMenu();
                    break;
                case ConsoleKey.D4:
                    _analyticsMenu.ShowMenu();
                    break;
                case ConsoleKey.D5:
                    ExportData();
                    break;
                case ConsoleKey.D6:
                    ImportData();
                    break;
                case ConsoleKey.D7:
                    ConsoleHelper.PrintTextWithColor("Program terminated. Thank you!", ConsoleColor.Green);
                    return;
                default:
                    ConsoleHelper.PrintTextWithColor("Invalid option.", ConsoleColor.Red);
                    break;
            }
            ConsoleHelper.PrintTextWithColor("Press any key to continue...", ConsoleColor.DarkYellow);
            Console.ReadKey();
        }
    }

    private void ExportData()
    {
        while (true)
        {
            Console.Clear();
            ConsoleHelper.PrintTextWithColor("=========== Export Data ===========", ConsoleColor.DarkCyan);
            Console.WriteLine("1. Export to JSON");
            Console.WriteLine("2. Export to CSV");
            Console.WriteLine("3. Export to YAML");
            Console.WriteLine("4. Back to main menu");
            ConsoleHelper.PrintTextWithColor("===================================", ConsoleColor.DarkCyan);

            var key = Console.ReadKey().Key;
            if (key == ConsoleKey.D4) return;

            string? extension = key switch
            {
                ConsoleKey.D1 => ".json",
                ConsoleKey.D2 => ".csv",
                ConsoleKey.D3 => ".yaml",
                _ => null
            };

            if (extension == null)
            {
                ConsoleHelper.PrintTextWithColor("Invalid option.", ConsoleColor.Red);
                ConsoleHelper.PrintTextWithColor("Press any key to continue...", ConsoleColor.DarkYellow);
                Console.ReadKey();
                continue;
            }

            var fileName = ConsoleHelper.ReadNonEmptyString($"Enter file name (will be saved with {extension} extension):");
            string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", ".."));
            string path = Path.Combine(projectRoot, "Data", $"{fileName}{extension}");

            try
            {
                switch (key)
                {
                    case ConsoleKey.D1:
                        _jsonExporter.Export(path);
                        break;
                    case ConsoleKey.D2:
                        _csvExporter.Export(path);
                        break;
                    case ConsoleKey.D3:
                        _yamlExporter.Export(path);
                        break;
                }
                ConsoleHelper.PrintTextWithColor($"Data exported to {path}", ConsoleColor.Green);
            }
            catch (Exception ex)
            {
                ConsoleHelper.PrintTextWithColor($"Error during export: {ex.Message}", ConsoleColor.Red);
            }

            ConsoleHelper.PrintTextWithColor("Press any key to continue...", ConsoleColor.DarkYellow);
            Console.ReadKey();
        }
    }

    private void ImportData()
    {
        while (true)
        {
            Console.Clear();
            ConsoleHelper.PrintTextWithColor("=========== Import Data ===========", ConsoleColor.DarkCyan);
            Console.WriteLine("1. Import from JSON");
            Console.WriteLine("2. Import from CSV");
            Console.WriteLine("3. Import from YAML");
            Console.WriteLine("4. Back to main menu");
            ConsoleHelper.PrintTextWithColor("===================================", ConsoleColor.DarkCyan);

            var key = Console.ReadKey().Key;
            if (key == ConsoleKey.D4) return;

            string? extension = key switch
            {
                ConsoleKey.D1 => ".json",
                ConsoleKey.D2 => ".csv",
                ConsoleKey.D3 => ".yaml",
                _ => null
            };

            if (extension == null)
            {
                ConsoleHelper.PrintTextWithColor("Invalid option.", ConsoleColor.Red);
                ConsoleHelper.PrintTextWithColor("Press any key to continue...", ConsoleColor.DarkYellow);
                Console.ReadKey();
                continue;
            }

            var fileName = ConsoleHelper.ReadNonEmptyString($"Enter file name (should have {extension} extension):");
            string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", ".."));
            string path = Path.Combine(projectRoot, "Data", $"{fileName}{extension}");

            if (!File.Exists(path))
            {
                ConsoleHelper.PrintTextWithColor($"File {path} does not exist. Please check the name and try again.", ConsoleColor.Red);
                ConsoleHelper.PrintTextWithColor("Press any key to continue...", ConsoleColor.DarkYellow);
                Console.ReadKey();
                continue;
            }

            try
            {
                switch (key)
                {
                    case ConsoleKey.D1:
                        _jsonImporter.ImportAll(path);
                        break;
                    case ConsoleKey.D2:
                        _csvImporter.ImportAll(path);
                        break;
                    case ConsoleKey.D3:
                        _yamlImporter.ImportAll(path);
                        break;
                }
                ConsoleHelper.PrintTextWithColor($"Data imported from {path}", ConsoleColor.Green);
            }
            catch (Exception ex)
            {
                ConsoleHelper.PrintTextWithColor($"Error during import: {ex.Message}", ConsoleColor.Red);
            }

            ConsoleHelper.PrintTextWithColor("Press any key to continue...", ConsoleColor.DarkYellow);
            Console.ReadKey();
        }
    }
}