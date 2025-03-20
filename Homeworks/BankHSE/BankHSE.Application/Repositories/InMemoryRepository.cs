using BankHSE.Domain.Abstractions;

namespace BankHSE.Application.Repositories
{
    public class InMemoryRepository<T> : IRepository<T> where T : IIdentifiable
    {
        private readonly List<T> _entities = new();

        public void Add(T entity) => _entities.Add(entity);
        public T GetById(Guid id) => _entities.FirstOrDefault(e => e.Id == id);
        public IEnumerable<T> GetAll() => _entities.AsReadOnly();

        public void Update(T entity)
        {
            var index = _entities.FindIndex(e => e.Id == entity.Id);
            if (index != -1) _entities[index] = entity;
        }

        public void Delete(Guid id) => _entities.RemoveAll(e => e.Id == id);
    }
}