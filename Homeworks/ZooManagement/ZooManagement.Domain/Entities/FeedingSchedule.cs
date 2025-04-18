using System;
using ZooManagement.Domain.Enums;
using ZooManagement.Domain.Events;
using ZooManagement.Domain.ValueObjects;

namespace ZooManagement.Domain.Entities
{
    // Represents a feeding schedule for an animal
    public class FeedingSchedule
    {
        public Guid Id { get; private set; }
        public Guid AnimalId { get; private set; }
        public FeedingTime FeedingTime { get; private set; }
        public FoodType FoodType { get; private set; }
        public bool IsCompleted { get; private set; }

        public FeedingSchedule(Guid animalId, FeedingTime feedingTime, FoodType foodType)
        {
            Id = Guid.NewGuid();
            AnimalId = animalId;
            FeedingTime = feedingTime ?? throw new ArgumentNullException(nameof(feedingTime));
            FoodType = foodType;
            IsCompleted = false;
        }

        // Updates the feeding schedule
        public void Update(Animal animal, FeedingTime newFeedingTime, FoodType newFoodType)
        {
            if (animal == null)
                throw new ArgumentNullException(nameof(animal));
            if (newFeedingTime == null)
                throw new ArgumentNullException(nameof(newFeedingTime));
            if (newFoodType != animal.FavoriteFood)
                throw new InvalidOperationException("Food type must match animal's favorite food.");
            FeedingTime = newFeedingTime;
            FoodType = newFoodType;
        }

        // Marks the feeding as completed
        public FeedingTimeEvent MarkCompleted()
        {
            if (IsCompleted)
                throw new InvalidOperationException("Feeding is already completed.");
            IsCompleted = true;
            return new FeedingTimeEvent(Id, AnimalId, FeedingTime.Value);
        }
    }
}