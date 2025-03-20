using BankHSE.Domain.Abstractions;

namespace BankHSE.Application.Strategy;

public class ImportContext
{
    private IImportStrategy? _strategy;

    // Set the import strategy to be used
    public void SetStrategy(IImportStrategy strategy)
    {
        _strategy = strategy;
    }

    // Execute the import using the current strategy
    public void ExecuteImport(string path)
    {
        if (_strategy == null)
            throw new InvalidOperationException("Import strategy not set.");
        _strategy.Import(path);
    }
}