using System.Collections.Generic;
using Xunit;
using ZooManagement.Application.Services;
using ZooManagement.Domain.Entities;
using ZooManagement.Domain.Enums;
using ZooManagement.Domain.ValueObjects;
using ZooManagement.Infrastructure.Repositories;

namespace ZooManagement.Tests.Application.Services
{
    public class ZooStatisticsServiceTests
    {
        private readonly ZooStatisticsService _service;
        private readonly InMemoryAnimalRepository _animalRepository;
        private readonly InMemoryEnclosureRepository _enclosureRepository;

        public ZooStatisticsServiceTests()
        {
            _animalRepository = new InMemoryAnimalRepository();
            _enclosureRepository = new InMemoryEnclosureRepository();
            _service = new ZooStatisticsService(_animalRepository, _enclosureRepository);
        }

        [Fact]
        public void GetTotalAnimals_ShouldReturnCorrectCount()
        {
            // Arrange
            var animals = new List<Animal>
            {
                new Animal(SpeciesType.Mammal, new AnimalName("Leo"), new BirthDate(System.DateTime.UtcNow.AddYears(-5)), Gender.Male, FoodType.Meat),
                new Animal(SpeciesType.Bird, new AnimalName("Tweety"), new BirthDate(System.DateTime.UtcNow.AddYears(-2)), Gender.Female, FoodType.Vegetables)
            };
            foreach (var animal in animals)
            {
                _animalRepository.Add(animal);
            }

            // Act
            var result = _service.GetTotalAnimals();

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public void GetFreeEnclosures_ShouldReturnCorrectCount()
        {
            // Arrange
            var enclosures = new List<Enclosure>
            {
                new Enclosure(EnclosureType.Cage, new EnclosureSize(100), new EnclosureCapacity(2)) { CurrentAnimalCount = 1 },
                new Enclosure(EnclosureType.Aquarium, new EnclosureSize(200), new EnclosureCapacity(5)) { CurrentAnimalCount = 5 },
                new Enclosure(EnclosureType.Aviary, new EnclosureSize(150), new EnclosureCapacity(3)) { CurrentAnimalCount = 0 }
            };
            foreach (var enclosure in enclosures)
            {
                _enclosureRepository.Add(enclosure);
            }

            // Act
            var result = _service.GetFreeEnclosures();

            // Assert
            Assert.Equal(2, result); // Cage and Aviary are free
        }
    }
}