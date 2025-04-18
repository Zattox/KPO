using System;
using ZooManagement.Domain.Enums;
using ZooManagement.Domain.Events;

namespace ZooManagement.Domain.Entities
{
    // Represents an animal in the zoo
    public class Animal
    {
        public Guid Id { get; private set; }
        public AnimalType Species { get; private set; }
        public string Name { get; private set; }
        public DateTime DateOfBirth { get; private set; }
        public Gender Gender { get; private set; }
        public FoodType FavoriteFood { get; private set; }
        public HealthStatus HealthStatus { get; private set; }
        public Guid? EnclosureId { get; private set; }
        

        public Animal(AnimalType species, string name, DateTime dateOfBirth, Gender gender, FoodType favoriteFood)
        {
            Id = Guid.NewGuid();
            Species = species;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DateOfBirth = dateOfBirth;
            Gender = gender;
            FavoriteFood = favoriteFood;
            HealthStatus = HealthStatus.Healthy;
        }

        // Feeds the animal with the specified food
        public void Feed(FoodType food)
        {
            if (food != FavoriteFood)
                throw new InvalidOperationException("Animal can only eat its favorite food.");
        }

        // Treats the animal, updating its health status
        public void Treat()
        {
            HealthStatus = HealthStatus.Healthy;
        }

        // Moves the animal to a new enclosure
        public AnimalMovedEvent MoveToEnclosure(Guid enclosureId)
        {
            EnclosureId = enclosureId;
            return new AnimalMovedEvent(Id, enclosureId);
        }
    }
}