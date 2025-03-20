namespace BankHSE.Domain.Abstractions;

public interface IRepository<T> where T : IIdentifiable
{
    void Add(T entity);

    T GetById(Guid id);
    IEnumerable<T> GetAll();

    void Update(T entity);

    void Delete(Guid id);
}