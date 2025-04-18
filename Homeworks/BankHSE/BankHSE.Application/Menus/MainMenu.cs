using BankHSE.Application.Helpers;
using BankHSE.Application.Strategy;
using BankHSE.Domain.Abstractions;

namespace BankHSE.Application.Menus;

public class MainMenu
{
    private readonly AccountMenu _accMenu;
    private readonly CategoryMenu _catMenu;
    private readonly OperationMenu _opMenu;
    private readonly AnalyticsMenu _analyticsMenu;
    private readonly ExportContext _exportCtx;
    private readonly ImportContext _importCtx;
    private readonly IEnumerable<IExportStrategy> _exportStrategies;
    private readonly IEnumerable<IImportStrategy> _importStrategies;
    private readonly ICoreEntitiesAggregator _agg;

    public MainMenu(
        AccountMenu accMenu,
        CategoryMenu catMenu,
        OperationMenu opMenu,
        AnalyticsMenu analyticsMenu,
        ExportContext exportCtx,
        ImportContext importCtx,
        IEnumerable<IExportStrategy> exportStrategies,
        IEnumerable<IImportStrategy> importStrategies,
        ICoreEntitiesAggregator agg)
    {
        _accMenu = accMenu;
        _catMenu = catMenu;
        _opMenu = opMenu;
        _analyticsMenu = analyticsMenu;
        _exportCtx = exportCtx;
        _importCtx = importCtx;
        _exportStrategies = exportStrategies;
        _importStrategies = importStrategies;
        _agg = agg;
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
                case ConsoleKey.D1: _accMenu.ShowMenu(); break;
                case ConsoleKey.D2: _catMenu.ShowMenu(); break;
                case ConsoleKey.D3: _opMenu.ShowMenu(); break;
                case ConsoleKey.D4: _analyticsMenu.ShowMenu(); break;
                case ConsoleKey.D5: ExportData(); break;
                case ConsoleKey.D6: ImportData(); break;
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

            string? format = key switch
            {
                ConsoleKey.D1 => "json",
                ConsoleKey.D2 => "csv",
                ConsoleKey.D3 => "yaml",
                _ => null
            };

            if (format == null)
            {
                ConsoleHelper.PrintTextWithColor("Invalid option.", ConsoleColor.Red);
                ConsoleHelper.PrintTextWithColor("Press any key to continue...", ConsoleColor.DarkYellow);
                Console.ReadKey();
                continue;
            }

            var ext = $".{format}";
            var fileName = ConsoleHelper.ReadNonEmptyString($"Enter file name (will be saved with {ext} extension):");
            string root = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", ".."));
            string path = Path.Combine(root, "Data", $"{fileName}{ext}");

            try
            {
                var strategy = _exportStrategies.FirstOrDefault(s => s.GetType().Name.ToLower().Contains(format))
                               ?? throw new InvalidOperationException($"No export strategy found for {format}");
                _exportCtx.SetStrategy(strategy);
                _exportCtx.ExecuteExport(path, _agg);
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

            string? format = key switch
            {
                ConsoleKey.D1 => "json",
                ConsoleKey.D2 => "csv",
                ConsoleKey.D3 => "yaml",
                _ => null
            };

            if (format == null)
            {
                ConsoleHelper.PrintTextWithColor("Invalid option.", ConsoleColor.Red);
                ConsoleHelper.PrintTextWithColor("Press any key to continue...", ConsoleColor.DarkYellow);
                Console.ReadKey();
                continue;
            }

            var ext = $".{format}";
            var fileName = ConsoleHelper.ReadNonEmptyString($"Enter file name (should have {ext} extension):");
            string root = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", ".."));
            string path = Path.Combine(root, "Data", $"{fileName}{ext}");

            if (!File.Exists(path))
            {
                ConsoleHelper.PrintTextWithColor($"File {path} does not exist. Please check the name and try again.",
                    ConsoleColor.Red);
                ConsoleHelper.PrintTextWithColor("Press any key to continue...", ConsoleColor.DarkYellow);
                Console.ReadKey();
                continue;
            }

            try
            {
                var strategy = _importStrategies.FirstOrDefault(s => s.GetType().Name.ToLower().Contains(format))
                               ?? throw new InvalidOperationException($"No import strategy found for {format}");
                _importCtx.SetStrategy(strategy);
                _importCtx.ExecuteImport(path);
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