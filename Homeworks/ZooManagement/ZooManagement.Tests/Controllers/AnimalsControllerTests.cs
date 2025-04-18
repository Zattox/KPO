using Microsoft.AspNetCore.Mvc;
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
    public class AnimalsControllerTests
    {
        private readonly AnimalsController _controller;
        private readonly IAnimalRepository _animalRepository;
        private readonly IEnclosureRepository _enclosureRepository;
        private readonly AnimalTransferService _animalTransferService;
        private readonly Animal _animal;
        private readonly Enclosure _enclosure;
        private readonly AnimalDto _animalDto;

        public AnimalsControllerTests()
        {
            _animalRepository = new InMemoryAnimalRepository();
            _enclosureRepository = new InMemoryEnclosureRepository();
            _animalTransferService = new AnimalTransferService(_animalRepository, _enclosureRepository);
            _controller = new AnimalsController(_animalRepository, _animalTransferService);

            _animal = new Animal(
                SpeciesType.Mammal,
                new AnimalName("Leo"),
                new BirthDate(DateTime.UtcNow.AddYears(-5)),
                Gender.Male,
                FoodType.Meat
            );
            _enclosure = new Enclosure(
                EnclosureType.Cage,
                new EnclosureSize(100),
                new EnclosureCapacity(5)
            );
            _animalDto = new AnimalDto
            {
                Species = SpeciesType.Mammal,
                Name = "Leo",
                DateOfBirth = DateTime.UtcNow.AddYears(-5),
                Gender = Gender.Male,
                FavoriteFood = FoodType.Meat
            };
        }

        [Fact]
        public void GetAll_ShouldReturnOkWithAnimals()
        {
            // Arrange
            _animalRepository.Add(_animal);

            // Act
            var result = _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<AnimalDto>>(okResult.Value);
            Assert.Single(dtos);
            Assert.Equal(_animal.Id, dtos.First().Id);
        }

        [Fact]
        public void Add_WhenValidDto_ShouldReturnCreated()
        {
            // Act
            var result = _controller.Add(_animalDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, createdResult.StatusCode);
            var returnedDto = Assert.IsType<AnimalDto>(createdResult.Value);
            var addedAnimal = _animalRepository.GetById(returnedDto.Id);
            Assert.NotNull(addedAnimal);
            Assert.Equal(_animalDto.Name, addedAnimal.Name.Value);
        }

        [Fact]
        public void Delete_WhenAnimalExists_ShouldReturnNoContent()
        {
            // Arrange
            _animalRepository.Add(_animal);

            // Act
            var result = _controller.Delete(_animal.Id);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Null(_animalRepository.GetById(_animal.Id));
        }

        [Fact]
        public void Delete_WhenAnimalNotFound_ShouldReturnNotFound()
        {
            // Act
            var result = _controller.Delete(Guid.NewGuid());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Transfer_WhenValid_ShouldReturnOk()
        {
            // Arrange
            _animalRepository.Add(_animal);
            _enclosureRepository.Add(_enclosure);
            _animal.MoveToEnclosure(_enclosure);
            _enclosure.AddAnimal(_animal);
            var newEnclosure = new Enclosure(EnclosureType.Cage, new EnclosureSize(100), new EnclosureCapacity(5));
            _enclosureRepository.Add(newEnclosure);

            // Act
            var result = _controller.Transfer(_animal.Id, newEnclosure.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public void Transfer_WhenAnimalNotFound_ShouldReturnBadRequest()
        {
            // Act
            var result = _controller.Transfer(Guid.NewGuid(), Guid.NewGuid());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}