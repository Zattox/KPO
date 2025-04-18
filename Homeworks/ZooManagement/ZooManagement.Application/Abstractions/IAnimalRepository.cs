using System;
using System.Collections.Generic;
using ZooManagement.Domain.Entities;

namespace ZooManagement.Application.Abstractions
{
    // Defines the contract for animal repository
    public interface IAnimalRepository
    {
        void Add(Animal animal);
        void Update(Animal animal);
        void Remove(Guid animalId);
        Animal GetById(Guid animalId);
        IEnumerable<Animal> GetAll();
    }
}