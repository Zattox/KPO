namespace BankHSE.Domain.Abstractions;

public interface ICoreEntitiesAggregator
{
    IEnumerable<ICoreEntityVisitable> GetAll();
}