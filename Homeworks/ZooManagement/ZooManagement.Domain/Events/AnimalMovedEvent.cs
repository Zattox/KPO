using System;

namespace ZooManagement.Domain.Events
{
    // Represents an event when an animal is moved to a new enclosure
    public class AnimalMovedEvent
    {
        public Guid AnimalId { get; }
        public Guid EnclosureId { get; }
        public DateTime OccurredAt { get; }

        public AnimalMovedEvent(Guid animalId, Guid enclosureId)
        {
            AnimalId = animalId;
            EnclosureId = enclosureId;
            OccurredAt = DateTime.UtcNow;
        }
    }
}