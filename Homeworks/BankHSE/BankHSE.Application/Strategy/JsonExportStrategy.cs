using BankHSE.Domain.Abstractions;
using BankHSE.Application.Export;

namespace BankHSE.Application.Strategy;

public class JsonExportStrategy : IExportStrategy
{
    public void Export(string path, ICoreEntitiesAggregator aggregator)
    {
        new JsonExporter(aggregator).Export(path);
    }
}