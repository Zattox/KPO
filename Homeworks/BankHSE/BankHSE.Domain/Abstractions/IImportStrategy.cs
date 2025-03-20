namespace BankHSE.Domain.Abstractions;

public interface IImportStrategy
{
    void Import(string path);
}