using BankHSE.Domain.Entities;

namespace BankHSE.Application.Import;

public abstract class Importer
{
    public IEnumerable<Operation> Import(string path)
    {
        var content = File.ReadAllText(path);
        var operations = Parse(content);
        Validate(operations);
        return operations;
    }

    protected abstract IEnumerable<Operation> Parse(string content);
    private void Validate(IEnumerable<Operation> operations)
    {
        if (operations == null || !operations.Any())
            throw new InvalidOperationException("No valid operations found.");
    }
}