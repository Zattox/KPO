using System;

namespace ZooManagement.Domain.Events
{
    // Represents an event when a feeding is completed
    public class FeedingTimeEvent
    {
        public Guid FeedingScheduleId { get; }
        public Guid AnimalId { get; }
        public DateTime FeedingTime { get; }
        public DateTime OccurredAt { get; }

        public FeedingTimeEvent(Guid feedingScheduleId, Guid animalId, DateTime feedingTime)
        {
            FeedingScheduleId = feedingScheduleId;
            AnimalId = animalId;
            FeedingTime = feedingTime;
            OccurredAt = DateTime.UtcNow;
        }
    }
}