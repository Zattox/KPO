using BankHSE.Domain.Abstractions;
using BankHSE.Application.Export;

namespace BankHSE.Application.Strategy;

public class YamlExportStrategy : IExportStrategy
{
    public void Export(string path, ICoreEntitiesAggregator aggregator)
    {
        new YamlExporter(aggregator).Export(path);
    }
}