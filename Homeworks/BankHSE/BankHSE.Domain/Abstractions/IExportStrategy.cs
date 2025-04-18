namespace BankHSE.Domain.Abstractions;

public interface IExportStrategy
{
    void Export(string path, ICoreEntitiesAggregator agg);
}