using System;
using Xunit;
using ZooManagement.Application.Abstractions;
using ZooManagement.Application.Services;
using ZooManagement.Domain.Entities;
using ZooManagement.Domain.Enums;
using ZooManagement.Domain.ValueObjects;
using ZooManagement.Domain.Events;
using ZooManagement.Infrastructure.Repositories;

namespace ZooManagement.Tests.Application
{
    public class FeedingOrganizationServiceTests
    {
        private readonly FeedingOrganizationService _service;
        private readonly IFeedingScheduleRepository _scheduleRepository;
        private readonly IAnimalRepository _animalRepository;
        private readonly FeedingSchedule _schedule;
        private readonly Animal _animal;

        public FeedingOrganizationServiceTests()
        {
            _scheduleRepository = new InMemoryFeedingScheduleRepository();
            _animalRepository = new InMemoryAnimalRepository();
            _service = new FeedingOrganizationService(_scheduleRepository, _animalRepository);

            _animal = new Animal(
                SpeciesType.Mammal,
                new AnimalName("Leo"),
                new BirthDate(DateTime.UtcNow.AddYears(-5)),
                Gender.Male,
                FoodType.Meat
            );
            _schedule = new FeedingSchedule(
                _animal.Id,
                new FeedingTime(DateTime.UtcNow.AddMinutes(1)), // Время в будущем
                FoodType.Meat
            );
            _animalRepository.Add(_animal);
            _scheduleRepository.Add(_schedule);
        }

        [Fact]
        public void CompleteFeeding_WhenValid_ShouldReturnEvent()
        {
            // Act
            var result = _service.CompleteFeeding(_schedule.Id);

            // Assert
            Assert.IsType<FeedingTimeEvent>(result);
            var updatedSchedule = _scheduleRepository.GetById(_schedule.Id);
            Assert.True(updatedSchedule.IsCompleted);
        }

        [Fact]
        public void CompleteFeeding_WhenScheduleNotFound_ShouldThrow()
        {
            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _service.CompleteFeeding(Guid.NewGuid()));
            Assert.Equal("Feeding schedule not found.", exception.Message);
        }

        [Fact]
        public void CompleteFeeding_WhenAnimalNotFound_ShouldThrow()
        {
            // Arrange
            var schedule = new FeedingSchedule(
                Guid.NewGuid(),
                new FeedingTime(DateTime.UtcNow.AddMinutes(1)), // Время в будущем
                FoodType.Meat
            );
            _scheduleRepository.Add(schedule);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _service.CompleteFeeding(schedule.Id));
            Assert.Equal("Animal not found.", exception.Message);
        }

        [Fact]
        public void CompleteFeeding_WhenFoodTypeDoesNotMatch_ShouldThrow()
        {
            // Arrange
            var schedule = new FeedingSchedule(
                _animal.Id,
                new FeedingTime(DateTime.UtcNow.AddMinutes(1)), // Время в будущем
                FoodType.Vegetables // Does not match animal's favorite food (Meat)
            );
            _scheduleRepository.Add(schedule);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _service.CompleteFeeding(schedule.Id));
            Assert.Equal("Food type does not match animal's favorite food.", exception.Message);
        }
    }
}