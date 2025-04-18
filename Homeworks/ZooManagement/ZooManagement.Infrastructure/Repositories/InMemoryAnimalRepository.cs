using System;
using System.Collections.Generic;
using System.Linq;
using ZooManagement.Application.Abstractions;
using ZooManagement.Domain.Entities;

namespace ZooManagement.Infrastructure.Repositories
{
    // In-memory implementation of animal repository
    public class InMemoryAnimalRepository : IAnimalRepository
    {
        private readonly Dictionary<Guid, Animal> _animals = new Dictionary<Guid, Animal>();

        // Adds a new animal to the repository
        public void Add(Animal animal)
        {
            if (animal == null)
                throw new ArgumentNullException(nameof(animal));
            _animals[animal.Id] = animal;
        }

        // Updates an existing animal in the repository
        public void Update(Animal animal)
        {
            if (animal == null)
                throw new ArgumentNullException(nameof(animal));
            if (!_animals.ContainsKey(animal.Id))
                throw new InvalidOperationException("Animal not found.");
            _animals[animal.Id] = animal;
        }

        // Removes an animal from the repository
        public void Remove(Guid animalId)
        {
            _animals.Remove(animalId);
        }

        // Retrieves an animal by its ID
        public Animal GetById(Guid animalId)
        {
            _animals.TryGetValue(animalId, out var animal);
            return animal;
        }

        // Retrieves all animals
        public IEnumerable<Animal> GetAll()
        {
            return _animals.Values.ToList();
        }
    }
}