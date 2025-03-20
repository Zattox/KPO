using BankHSE.Domain.Abstractions;

namespace BankHSE.Application.Strategy;
public class ExportContext
{
    private IExportStrategy? _strategy;

    // Set the export strategy to be used
    public void SetStrategy(IExportStrategy strategy)
    {
        _strategy = strategy;
    }

    // Execute the export using the current strategy
    public void ExecuteExport(string path, ICoreEntitiesAggregator agg)
    {
        if (_strategy == null)
            throw new InvalidOperationException("Export strategy not set.");
        _strategy.Export(path, agg);
    }
}