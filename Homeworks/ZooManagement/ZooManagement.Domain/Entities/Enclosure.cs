using System;
using ZooManagement.Domain.Enums;

namespace ZooManagement.Domain.Entities
{
    // Represents an enclosure in the zoo
    public class Enclosure
    {
        public Guid Id { get; private set; }
        public AnimalType AllowedAnimalType { get; private set; }
        public int Size { get; private set; }
        public int CurrentAnimalCount { get; private set; }
        public int MaxCapacity { get; private set; }

        public Enclosure(AnimalType allowedAnimalType, int size, int maxCapacity)
        {
            Id = Guid.NewGuid();
            AllowedAnimalType = allowedAnimalType;
            Size = size > 0 ? size : throw new ArgumentException("Size must be positive.");
            MaxCapacity = maxCapacity > 0 ? maxCapacity : throw new ArgumentException("Max capacity must be positive.");
            CurrentAnimalCount = 0;
        }

        // Adds an animal to the enclosure
        public void AddAnimal(Animal animal)
        {
            if (animal.Species != AllowedAnimalType)
                throw new InvalidOperationException("Animal species does not match enclosure species.");
            if (CurrentAnimalCount >= MaxCapacity)
                throw new InvalidOperationException("Enclosure is at maximum capacity.");
            CurrentAnimalCount++;
        }

        // Removes an animal from the enclosure
        public void RemoveAnimal()
        {
            if (CurrentAnimalCount <= 0)
                throw new InvalidOperationException("No animals to remove.");
            CurrentAnimalCount--;
        }
        
    }
}