using BankHSE.Domain.Abstractions;

namespace BankHSE.Application.Export;

public abstract class Exporter
{
    protected readonly ICoreEntitiesAggregator _aggregator;

    protected Exporter(ICoreEntitiesAggregator aggregator)
    {
        _aggregator = aggregator;
    }

    public void Export(string path)
    {
        FormatData();

        var stream = OpenWriter(path);
        try
        {
            Write(stream);
        }
        finally
        {
            stream.Close();
        }
    }

    private Stream OpenWriter(string path)
    {
        try
        {
            return new FileStream(path, FileMode.Create);
        }
        catch (Exception e)
        {
            // Wraps all exceptions into IOException
            throw new IOException("Failed to open file for writing", e);
        }
    }

    protected abstract void FormatData();
    protected abstract void Write(Stream stream);
}