using System;
using Xunit;
using ZooManagement.Domain.Entities;
using ZooManagement.Domain.Enums;
using ZooManagement.Domain.ValueObjects;
using ZooManagement.Domain.Events;

namespace ZooManagement.Tests.Domain.Entities
{
    public class FeedingScheduleTests
    {
        private readonly FeedingSchedule _schedule;
        private readonly Animal _animal;

        public FeedingScheduleTests()
        {
            _animal = new Animal(
                SpeciesType.Mammal,
                new AnimalName("Leo"),
                new BirthDate(DateTime.UtcNow.AddYears(-5)),
                Gender.Male,
                FoodType.Meat
            );
            _schedule = new FeedingSchedule(
                _animal.Id,
                new FeedingTime(DateTime.UtcNow.AddHours(1)),
                FoodType.Meat
            );
        }

        [Fact]
        public void Update_WhenValidData_ShouldUpdateProperties()
        {
            // Arrange
            var newTime = new FeedingTime(DateTime.UtcNow.AddHours(2));

            // Act
            _schedule.Update(_animal, newTime, FoodType.Meat);

            // Assert
            Assert.Equal(newTime, _schedule.FeedingTime);
            Assert.Equal(FoodType.Meat, _schedule.FoodType);
        }

        [Fact]
        public void Update_WhenWrongFoodType_ShouldThrow()
        {
            // Arrange
            var newTime = new FeedingTime(DateTime.UtcNow.AddHours(2));

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _schedule.Update(_animal, newTime, FoodType.Vegetables));
            Assert.Equal("Food type must match animal's favorite food.", exception.Message);
        }

        [Fact]
        public void MarkCompleted_WhenNotCompleted_ShouldReturnEvent()
        {
            // Act
            var result = _schedule.MarkCompleted();

            // Assert
            Assert.IsType<FeedingTimeEvent>(result);
            Assert.Equal(_schedule.Id, result.FeedingScheduleId);
            Assert.Equal(_animal.Id, result.AnimalId);
            Assert.True(_schedule.IsCompleted);
        }

        [Fact]
        public void MarkCompleted_WhenAlreadyCompleted_ShouldThrow()
        {
            // Arrange
            _schedule.MarkCompleted();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _schedule.MarkCompleted());
            Assert.Equal("Feeding is already completed.", exception.Message);
        }
    }
}