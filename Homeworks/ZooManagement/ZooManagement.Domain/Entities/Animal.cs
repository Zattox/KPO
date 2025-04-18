using System;
using ZooManagement.Domain.Enums;
using ZooManagement.Domain.Events;
using ZooManagement.Domain.ValueObjects;

namespace ZooManagement.Domain.Entities
{
    // Represents an animal in the zoo
    public class Animal
    {
        public Guid Id { get; private set; }
        public SpeciesType Species { get; private set; }
        public AnimalName Name { get; private set; }
        public BirthDate DateOfBirth { get; private set; }
        public Gender Gender { get; private set; }
        public FoodType FavoriteFood { get; private set; }
        public HealthStatus HealthStatus { get; private set; }
        public Guid? EnclosureId { get; private set; }

        public Animal(SpeciesType species, AnimalName name, BirthDate dateOfBirth, Gender gender, FoodType favoriteFood)
        {
            Id = Guid.NewGuid();
            Species = species;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DateOfBirth = dateOfBirth ?? throw new ArgumentNullException(nameof(dateOfBirth));
            Gender = gender;
            FavoriteFood = favoriteFood;
            HealthStatus = HealthStatus.Healthy;
        }

        // Feeds the animal with the specified food
        public void Feed(FoodType food)
        {
            if (HealthStatus == HealthStatus.Sick)
                throw new InvalidOperationException("Sick animals cannot be fed without treatment.");
            if (food != FavoriteFood)
                throw new InvalidOperationException("Animal can only eat its favorite food.");
        }

        // Treats the animal, updating its health status
        public void Treat()
        {
            HealthStatus = HealthStatus.Healthy;
        }
        
        public void MakeSick()
        {
            HealthStatus = HealthStatus.Sick;
        }

        // Moves the animal to a new enclosure
        public AnimalMovedEvent MoveToEnclosure(Enclosure enclosure)
        {
            if (enclosure == null)
                throw new ArgumentNullException(nameof(enclosure));
            if (!IsCompatibleEnclosure(enclosure.Type))
                throw new InvalidOperationException("Animal species is not compatible with enclosure type.");
            EnclosureId = enclosure.Id;
            return new AnimalMovedEvent(Id, enclosure.Id);
        }

        // Checks if the enclosure type is compatible with the animal's species
        private bool IsCompatibleEnclosure(EnclosureType enclosureType)
        {
            return (Species, enclosureType) switch
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