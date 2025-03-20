using BankHSE.Domain.Abstractions;
using BankHSE.Application.Export;

namespace BankHSE.Application.Strategy;

public class CsvExportStrategy : IExportStrategy
{
    public void Export(string path, ICoreEntitiesAggregator aggregator)
    {
        new CsvExporter(aggregator).Export(path);
    }
}