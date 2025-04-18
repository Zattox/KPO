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

        public void Add(Animal animal)
        {
            _animals[animal.Id] = animal;
        }

        public void Remove(Guid animalId)
        {
            _animals.Remove(animalId);
        }

        public Animal GetById(Guid animalId)
        {
            _animals.TryGetValue(animalId, out var animal);
            return animal;
        }

        public IEnumerable<Animal> GetAll()
        {
            return _animals.Values.ToList();
        }
    }
}