﻿using System;
using ZooManagement.Domain.Enums;
using ZooManagement.Domain.ValueObjects;
using ZooManagement.Domain.Events;

namespace ZooManagement.Domain.Entities
{
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
            FeedingTime = feedingTime;
            FoodType = foodType;
            IsCompleted = false;
        }

        public FeedingTimeEvent MarkCompleted()
        {
            if (IsCompleted)
                throw new InvalidOperationException("Feeding schedule is already completed.");
            IsCompleted = true;
            return new FeedingTimeEvent(Id, AnimalId, FeedingTime.Value);
        }
    }
}