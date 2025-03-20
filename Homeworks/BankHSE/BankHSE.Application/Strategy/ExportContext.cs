using BankHSE.Domain.Abstractions;

namespace BankHSE.Application.Strategy;

public class ExportContext
{
    private IExportStrategy _strategy;

    public void SetStrategy(IExportStrategy strategy)
    {
        _strategy = strategy;
    }

    public void ExecuteExport(string path, ICoreEntitiesAggregator aggregator)
    {
        if (_strategy == null)
            throw new InvalidOperationException("Export strategy is not set.");
        _strategy.Export(path, aggregator);
    }
}