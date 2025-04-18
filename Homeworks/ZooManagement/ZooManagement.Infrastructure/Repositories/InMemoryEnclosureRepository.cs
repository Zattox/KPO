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

        // Adds a new enclosure to the repository
        public void Add(Enclosure enclosure)
        {
            if (enclosure == null)
                throw new ArgumentNullException(nameof(enclosure));
            _enclosures[enclosure.Id] = enclosure;
        }

        // Updates an existing enclosure in the repository
        public void Update(Enclosure enclosure)
        {
            if (enclosure == null)
                throw new ArgumentNullException(nameof(enclosure));
            if (!_enclosures.ContainsKey(enclosure.Id))
                throw new InvalidOperationException("Enclosure not found.");
            _enclosures[enclosure.Id] = enclosure;
        }

        // Removes an enclosure from the repository
        public void Remove(Guid enclosureId)
        {
            _enclosures.Remove(enclosureId);
        }

        // Retrieves an enclosure by its ID
        public Enclosure GetById(Guid enclosureId)
        {
            _enclosures.TryGetValue(enclosureId, out var enclosure);
            return enclosure;
        }

        // Retrieves all enclosures
        public IEnumerable<Enclosure> GetAll()
        {
            return _enclosures.Values.ToList();
        }
    }
}