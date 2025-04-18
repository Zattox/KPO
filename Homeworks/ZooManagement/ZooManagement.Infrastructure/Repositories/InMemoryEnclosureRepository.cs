using System;
using System.Collections.Generic;
using System.Linq;
using ZooManagement.Application.Abstractions;
using ZooManagement.Domain.Entities;

namespace ZooManagement.Infrastructure.Repositories
{
    // In-memory implementation of enclosure repository
    public class InMemoryEnclosureRepository : IEnclosureRepository
    {
        private readonly Dictionary<Guid, Enclosure> _enclosures = new Dictionary<Guid, Enclosure>();

        public void Add(Enclosure enclosure)
        {
            _enclosures[enclosure.Id] = enclosure;
        }

        public void Remove(Guid enclosureId)
        {
            _enclosures.Remove(enclosureId);
        }

        public Enclosure GetById(Guid enclosureId)
        {
            _enclosures.TryGetValue(enclosureId, out var enclosure);
            return enclosure;
        }

        public IEnumerable<Enclosure> GetAll()
        {
            return _enclosures.Values.ToList();
        }
    }
}