using System;
using System.Collections.Generic;
using ZooManagement.Domain.Enums;
using ZooManagement.Domain.ValueObjects;

namespace ZooManagement.Domain.Entities
{
    public class Enclosure
    {
        public Guid Id { get; private set; }
        public EnclosureType Type { get; private set; }
        public EnclosureSize Size { get; private set; }
        public EnclosureCapacity MaxCapacity { get; private set; }
        public int CurrentAnimalCount { get; set; }
        private readonly List<Animal> _animals = new List<Animal>(); // Track specific animals

        public Enclosure(EnclosureType type, EnclosureSize size, EnclosureCapacity maxCapacity)
        {
            Id = Guid.NewGuid();
            Type = type;
            Size = size;
            MaxCapacity = maxCapacity;
            CurrentAnimalCount = 0;
        }

        public void AddAnimal(Animal animal)
        {
            if (animal == null)
                throw new ArgumentNullException(nameof(animal));
            if (!IsCompatibleWithAnimal(animal.Species))
                throw new InvalidOperationException("Animal species is not compatible with enclosure type.");
            if (CurrentAnimalCount >= MaxCapacity.Value)
                throw new InvalidOperationException("Enclosure is at maximum capacity.");

            _animals.Add(animal);
            CurrentAnimalCount++;
        }

        public void RemoveAnimal(Animal animal)
        {
            if (animal == null)
                throw new ArgumentNullException(nameof(animal));
            if (!_animals.Contains(animal))
                throw new InvalidOperationException("Animal is not in this enclosure.");

            _animals.Remove(animal);
            CurrentAnimalCount--;
        }

        public void Clean()
        {
            if (CurrentAnimalCount > 0)
                throw new InvalidOperationException("Cannot clean enclosure with animals inside.");
        }

        private bool IsCompatibleWithAnimal(SpeciesType species)
        {
            return (species, Type) switch
            {
                (SpeciesType.Mammal, EnclosureType.Cage) => true,
                (SpeciesType.Bird, EnclosureType.Aviary) => true,
                (SpeciesType.Fish, EnclosureType.Aquarium) => true,
                _ => false
            };
        }
    }
}