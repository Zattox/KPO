using System;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using ZooManagement.Application.Abstractions;
using ZooManagement.Application.DTOs;
using ZooManagement.Application.Services;
using ZooManagement.Domain.Entities;
using ZooManagement.Domain.Enums;
using ZooManagement.Domain.ValueObjects;
using ZooManagement.Infrastructure.Repositories;
using ZooManagement.Presentation.Controllers;

namespace ZooManagement.Tests.Controllers
{
    public class FeedingSchedulesControllerTests
    {
        private readonly FeedingSchedulesController _controller;
        private readonly IFeedingScheduleRepository _feedingScheduleRepository;
        private readonly IAnimalRepository _animalRepository;
        private readonly FeedingOrganizationService _feedingOrganizationService;
        private readonly FeedingSchedule _schedule;
        private readonly FeedingScheduleDto _scheduleDto;
        private readonly Animal _animal;

        public FeedingSchedulesControllerTests()
        {
            _feedingScheduleRepository = new InMemoryFeedingScheduleRepository();
            _animalRepository = new InMemoryAnimalRepository();
            _feedingOrganizationService = new FeedingOrganizationService(_feedingScheduleRepository, _animalRepository);
            _controller = new FeedingSchedulesController(_feedingScheduleRepository, _animalRepository, _feedingOrganizationService);

            _animal = new Animal(
                SpeciesType.Mammal,
                new AnimalName("Leo"),
                new BirthDate(DateTime.UtcNow.AddYears(-5)),
                Gender.Male,
                FoodType.Meat
            );
            _animalRepository.Add(_animal);

            _schedule = new FeedingSchedule(
                _animal.Id,
                new FeedingTime(DateTime.UtcNow.AddMinutes(1)), // Добавляем 1 минуту, чтобы время было в будущем
                FoodType.Meat
            );
            _scheduleDto = new FeedingScheduleDto
            {
                AnimalId = _animal.Id,
                FeedingTime = DateTime.UtcNow.AddMinutes(1), // То же самое для DTO
                FoodType = FoodType.Meat
            };
        }

        [Fact]
        public void GetAll_ShouldReturnOkWithSchedules()
        {
            // Arrange
            _feedingScheduleRepository.Add(_schedule);

            // Act
            var result = _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<FeedingScheduleDto>>(okResult.Value);
            Assert.Single(dtos);
            Assert.Equal(_schedule.Id, dtos.First().Id);
        }

        [Fact]
        public void Add_WhenValidDto_ShouldReturnCreated()
        {
            // Act
            var result = _controller.Add(_scheduleDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, createdResult.StatusCode);
            var returnedDto = Assert.IsType<FeedingScheduleDto>(createdResult.Value);
            var addedSchedule = _feedingScheduleRepository.GetById(returnedDto.Id);
            Assert.NotNull(addedSchedule);
            Assert.Equal(_scheduleDto.AnimalId, addedSchedule.AnimalId);
        }

        [Fact]
        public void Add_WhenInvalidDto_ShouldReturnBadRequest()
        {
            // Arrange
            var invalidDto = new FeedingScheduleDto(); // Missing required fields
            _controller.ModelState.AddModelError("AnimalId", "The AnimalId field is required.");

            // Act
            var result = _controller.Add(invalidDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public void Add_WhenAnimalNotFound_ShouldReturnBadRequest()
        {
            // Arrange
            var invalidDto = new FeedingScheduleDto
            {
                AnimalId = Guid.NewGuid(),
                FeedingTime = DateTime.UtcNow.AddMinutes(1),
                FoodType = FoodType.Meat
            };

            // Act
            var result = _controller.Add(invalidDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Animal not found.", badRequestResult.Value);
        }

        [Fact]
        public void Add_WhenFoodTypeDoesNotMatch_ShouldReturnBadRequest()
        {
            // Arrange
            var invalidDto = new FeedingScheduleDto
            {
                AnimalId = _animal.Id,
                FeedingTime = DateTime.UtcNow.AddMinutes(1),
                FoodType = FoodType.Vegetables // Does not match animal's favorite food
            };

            // Act
            var result = _controller.Add(invalidDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Food type does not match animal's favorite food.", badRequestResult.Value);
        }

        [Fact]
        public void CompleteFeeding_WhenScheduleExists_ShouldReturnOk()
        {
            // Arrange
            _feedingScheduleRepository.Add(_schedule);

            // Act
            var result = _controller.CompleteFeeding(_schedule.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            var updatedSchedule = _feedingScheduleRepository.GetById(_schedule.Id);
            Assert.True(updatedSchedule.IsCompleted);
        }

        [Fact]
        public void CompleteFeeding_WhenScheduleNotFound_ShouldReturnBadRequest()
        {
            // Act
            var result = _controller.CompleteFeeding(Guid.NewGuid());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}