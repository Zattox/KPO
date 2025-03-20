using BankHSE.Domain.Abstractions;

namespace BankHSE.Application.Repositories;

public class CachedRepository<T> : IRepository<T> where T : IIdentifiable
{
    private readonly IRepository<T> _realRepository;
    private readonly Dictionary<Guid, T> _cache;

    public CachedRepository(IRepository<T> realRepository)
    {
        _realRepository = realRepository;
        _cache = new Dictionary<Guid, T>();
        foreach (var entity in _realRepository.GetAll())
            _cache[entity.Id] = entity;
    }

    public void Add(T entity)
    {
        _realRepository.Add(entity);
        _cache[entity.Id] = entity;
    }

    public T GetById(Guid id)
    {
        if (!_cache.ContainsKey(id))
        {
            var entity = _realRepository.GetById(id);
            if (entity != null) _cache[id] = entity;
        }

        return _cache.TryGetValue(id, out var result) ? result : default;
    }

    public IEnumerable<T> GetAll()
    {
        return _cache.Values;
    }

    public void Update(T entity)
    {
        _realRepository.Update(entity);
        _cache[entity.Id] = entity;
    }

    public void Delete(Guid id)
    {
        _realRepository.Delete(id);
        _cache.Remove(id);
    }
}