using System;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using ZooManagement.Application.Abstractions;
using ZooManagement.Application.DTOs;
using ZooManagement.Domain.Entities;
using ZooManagement.Domain.Enums;
using ZooManagement.Domain.ValueObjects;
using ZooManagement.Infrastructure.Repositories;
using ZooManagement.Presentation.Controllers;

namespace ZooManagement.Tests.Controllers
{
    public class EnclosuresControllerTests
    {
        private readonly EnclosuresController _controller;
        private readonly IEnclosureRepository _enclosureRepository;
        private readonly Enclosure _enclosure;
        private readonly EnclosureDto _enclosureDto;

        public EnclosuresControllerTests()
        {
            _enclosureRepository = new InMemoryEnclosureRepository();
            _controller = new EnclosuresController(_enclosureRepository);

            _enclosure = new Enclosure(
                EnclosureType.Cage,
                new EnclosureSize(100),
                new EnclosureCapacity(5)
            );
            _enclosureDto = new EnclosureDto
            {
                Id = _enclosure.Id,
                Type = EnclosureType.Cage,
                Size = 100,
                CurrentAnimalCount = 0,
                MaxCapacity = 5
            };
            _enclosureRepository.Add(_enclosure);
        }

        [Fact]
        public void GetAll_ShouldReturnOkWithEnclosures()
        {
            // Act
            var result = _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<EnclosureDto>>(okResult.Value);
            Assert.Single(dtos);
            Assert.Equal(100, dtos.First().Size);
        }

        [Fact]
        public void Add_WhenValidDto_ShouldReturnCreated()
        {
            // Arrange
            var newDto = new EnclosureDto
            {
                Type = EnclosureType.Aquarium,
                Size = 200,
                MaxCapacity = 10
            };

            // Act
            var result = _controller.Add(newDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, createdResult.StatusCode);
            var returnedDto = Assert.IsType<EnclosureDto>(createdResult.Value);
            var addedEnclosure = _enclosureRepository.GetById(returnedDto.Id);
            Assert.NotNull(addedEnclosure); // Добавляем проверку на null
            Assert.Equal(200, addedEnclosure.Size.Value);
        }

        [Fact]
        public void Add_WhenInvalidDto_ShouldReturnBadRequest()
        {
            // Arrange
            var invalidDto = new EnclosureDto
            {
                Type = EnclosureType.Cage,
                Size = 0, // Invalid size
                MaxCapacity = 5
            };

            // Act
            var result = _controller.Add(invalidDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public void Delete_WhenEnclosureExists_ShouldReturnNoContent()
        {
            // Act
            var result = _controller.Delete(_enclosure.Id);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Null(_enclosureRepository.GetById(_enclosure.Id));
        }

        [Fact]
        public void Delete_WhenEnclosureNotFound_ShouldReturnNotFound()
        {
            // Act
            var result = _controller.Delete(Guid.NewGuid());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}