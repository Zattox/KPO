using System;
using Xunit;
using ZooManagement.Application.Services;
using ZooManagement.Domain.Entities;
using ZooManagement.Domain.Enums;
using ZooManagement.Domain.ValueObjects;
using ZooManagement.Domain.Events;
using ZooManagement.Infrastructure.Repositories;

namespace ZooManagement.Tests.Application.Services
{
    public class AnimalTransferServiceTests
    {
        private readonly AnimalTransferService _service;
        private readonly InMemoryAnimalRepository _animalRepository;
        private readonly InMemoryEnclosureRepository _enclosureRepository;
        private readonly Animal _animal;
        private readonly Enclosure _newEnclosure;
        private readonly Enclosure _oldEnclosure;

        public AnimalTransferServiceTests()
        {
            _animalRepository = new InMemoryAnimalRepository();
            _enclosureRepository = new InMemoryEnclosureRepository();
            _service = new AnimalTransferService(_animalRepository, _enclosureRepository);

            _animal = new Animal(
                SpeciesType.Mammal,
                new AnimalName("Leo"),
                new BirthDate(DateTime.UtcNow.AddYears(-5)),
                Gender.Male,
                FoodType.Meat
            );
            _newEnclosure = new Enclosure(
                EnclosureType.Cage,
                new EnclosureSize(100),
                new EnclosureCapacity(5)
            );
            _oldEnclosure = new Enclosure(
                EnclosureType.Cage,
                new EnclosureSize(100),
                new EnclosureCapacity(5)
            );
            _animal.MoveToEnclosure(_oldEnclosure); // Simulate animal in old enclosure
            _animalRepository.Add(_animal);
            _enclosureRepository.Add(_newEnclosure);
            _enclosureRepository.Add(_oldEnclosure);
        }

        [Fact]
        public void TransferAnimal_WhenValid_ShouldReturnEvent()
        {
            // Act
            var result = _service.TransferAnimal(_animal.Id, _newEnclosure.Id);

            // Assert
            Assert.IsType<AnimalMovedEvent>(result);
            var updatedAnimal = _animalRepository.GetById(_animal.Id);
            Assert.Equal(_newEnclosure.Id, updatedAnimal.EnclosureId);
            var updatedNewEnclosure = _enclosureRepository.GetById(_newEnclosure.Id);
            Assert.Equal(1, updatedNewEnclosure.CurrentAnimalCount);
            var updatedOldEnclosure = _enclosureRepository.GetById(_oldEnclosure.Id);
            Assert.Equal(0, updatedOldEnclosure.CurrentAnimalCount);
        }

        [Fact]
        public void TransferAnimal_WhenAnimalNotFound_ShouldThrow()
        {
            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _service.TransferAnimal(Guid.NewGuid(), _newEnclosure.Id));
            Assert.Equal("Animal not found.", exception.Message);
        }

        [Fact]
        public void TransferAnimal_WhenEnclosureNotFound_ShouldThrow()
        {
            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _service.TransferAnimal(_animal.Id, Guid.NewGuid()));
            Assert.Equal("Enclosure not found.", exception.Message);
        }
    }
}