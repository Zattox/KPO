using System;
using ZooManagement.Domain.Enums;

namespace ZooManagement.Application.DTOs
{
    // Data transfer object for Animal
    public class AnimalDto
    {
        public Guid Id { get; set; }
        public AnimalType Species { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public FoodType FavoriteFood { get; set; }
        public HealthStatus HealthStatus { get; set; }
        public Guid? EnclosureId { get; set; }
    }
}