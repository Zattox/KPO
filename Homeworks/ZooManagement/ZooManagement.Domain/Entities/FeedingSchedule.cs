using System;
using ZooManagement.Domain.Events;
using ZooManagement.Domain.Enums;

namespace ZooManagement.Domain.Entities
{
    // Represents a feeding schedule for an animal
    public class FeedingSchedule
    {
        public Guid Id { get; private set; }
        public Guid AnimalId { get; private set; }
        public DateTime FeedingTime { get; private set; }
        public FoodType FoodType { get; private set; }
        public bool IsCompleted { get; private set; }

        public FeedingSchedule(Guid animalId, DateTime feedingTime, FoodType foodType)
        {
            Id = Guid.NewGuid();
            AnimalId = animalId;
            FeedingTime = feedingTime;
            FoodType = foodType;
            IsCompleted = false;
        }

        // Updates the feeding schedule
        public void Update(DateTime newFeedingTime, FoodType newFoodType)
        {
            FeedingTime = newFeedingTime;
            FoodType = newFoodType;
        }

        // Marks the feeding as completed
        public FeedingTimeEvent MarkCompleted()
        {
            IsCompleted = true;
            return new FeedingTimeEvent(Id, AnimalId, FeedingTime);
        }
    }
}