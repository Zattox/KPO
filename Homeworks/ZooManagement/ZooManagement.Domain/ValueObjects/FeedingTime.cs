using System;

namespace ZooManagement.Domain.ValueObjects
{
    // Represents the scheduled feeding time
    public class FeedingTime
    {
        public DateTime Value { get; }

        public FeedingTime(DateTime value)
        {
            if (value < DateTime.UtcNow)
                throw new ArgumentException("Feeding time cannot be in the past.");
            Value = value;
        }
    }
}