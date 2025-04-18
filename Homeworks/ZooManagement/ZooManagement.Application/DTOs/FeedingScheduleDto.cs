using System;
using ZooManagement.Domain.Enums;

namespace ZooManagement.Application.DTOs
{
    // Data transfer object for FeedingSchedule
    public class FeedingScheduleDto
    {
        public Guid Id { get; set; }
        public Guid AnimalId { get; set; }
        public DateTime FeedingTime { get; set; }
        public FoodType FoodType { get; set; }
        public bool IsCompleted { get; set; }
    }
}