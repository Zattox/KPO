using System;
using ZooManagement.Domain.Enums;
using ZooManagement.Domain.ValueObjects;

namespace ZooManagement.Domain.Entities
{
    // Represents an enclosure in the zoo
    public class Enclosure
    {
        public Guid Id { get; private set; }
        public EnclosureType Type { get; private set; }
        public EnclosureSize Size { get; private set; }
        public int CurrentAnimalCount { get; private set; }
        public EnclosureCapacity MaxCapacity { get; private set; }

        public Enclosure(EnclosureType type, EnclosureSize size, EnclosureCapacity maxCapacity)
        {
            Id = Guid.NewGuid();
            Type = type;
            Size = size ?? throw new ArgumentNullException(nameof(size));
            MaxCapacity = maxCapacity ?? throw new ArgumentNullException(nameof(maxCapacity));
            CurrentAnimalCount = 0;
        }

        // Adds an animal to the enclosure
        public void AddAnimal(Animal animal)
        {
            if (animal == null)
                throw new ArgumentNullException(nameof(animal));
            if (!IsCompatibleAnimal(animal.Species))
                throw new InvalidOperationException("Animal species is not compatible with enclosure type.");
            if (CurrentAnimalCount >= MaxCapacity.Value)
                throw new InvalidOperationException("Enclosure is at maximum capacity.");
            CurrentAnimalCount++;
        }

        // Removes a specific animal from the enclosure
        public void RemoveAnimal(Animal animal)
        {
            if (animal == null)
                throw new ArgumentNullException(nameof(animal));
            if (CurrentAnimalCount <= 0)
                throw new InvalidOperationException("No animals to remove.");
            if (animal.EnclosureId != Id)
                throw new InvalidOperationException("Animal is not in this enclosure.");
            CurrentAnimalCount--;
        }

        // Cleans the enclosure
        public void Clean()
        {
            if (CurrentAnimalCount > 0)
                throw new InvalidOperationException("Cannot clean enclosure with animals inside.");
        }

        // Checks if the animal's species is compatible with the enclosure type
        private bool IsCompatibleAnimal(SpeciesType species)
        {
            return (species, Type) switch
            {
                (SpeciesType.Mammal, EnclosureType.Cage) => true,
                (SpeciesType.Mammal, EnclosureType.OpenEnclosure) => true,
                (SpeciesType.Reptile, EnclosureType.Terrarium) => true,
                (SpeciesType.Amphibian, EnclosureType.Terrarium) => true,
                (SpeciesType.Bird, EnclosureType.Aviary) => true,
                (SpeciesType.Fish, EnclosureType.Aquarium) => true,
                _ => false
            };
        }
    }
}