using System;
using Xunit;
using ZooManagement.Domain.Entities;
using ZooManagement.Domain.Enums;
using ZooManagement.Domain.ValueObjects;
using ZooManagement.Domain.Events;

namespace ZooManagement.Tests.Domain.Entities
{
    public class AnimalTests
    {
        private readonly Animal _animal;
        private readonly Enclosure _compatibleEnclosure;
        private readonly Enclosure _incompatibleEnclosure;

        public AnimalTests()
        {
            _animal = new Animal(
                SpeciesType.Mammal,
                new AnimalName("Leo"),
                new BirthDate(DateTime.UtcNow.AddYears(-5)),
                Gender.Male,
                FoodType.Meat
            );
            _compatibleEnclosure = new Enclosure(
                EnclosureType.Cage,
                new EnclosureSize(100),
                new EnclosureCapacity(5)
            );
            _incompatibleEnclosure = new Enclosure(
                EnclosureType.Aquarium,
                new EnclosureSize(100),
                new EnclosureCapacity(5)
            );
        }

        [Fact]
        public void Feed_WhenHealthyAndCorrectFood_ShouldNotThrow()
        {
            // Act & Assert
            _animal.Feed(FoodType.Meat);
        }

        [Fact]
        public void Feed_WhenSick_ShouldThrowInvalidOperationException()
        {
            // Arrange
            _animal.MakeSick();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _animal.Feed(FoodType.Meat));
            Assert.Equal("Sick animals cannot be fed without treatment.", exception.Message);
        }

        [Fact]
        public void Feed_WhenWrongFood_ShouldThrowInvalidOperationException()
        {
            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _animal.Feed(FoodType.Vegetables));
            Assert.Equal("Animal can only eat its favorite food.", exception.Message);
        }

        [Fact]
        public void Treat_ShouldSetHealthStatusToHealthy()
        {
            // Act
            _animal.Treat();

            // Assert
            Assert.Equal(HealthStatus.Healthy, _animal.HealthStatus);
        }

        [Fact]
        public void MoveToEnclosure_WhenCompatibleEnclosure_ShouldReturnEvent()
        {
            // Act
            var result = _animal.MoveToEnclosure(_compatibleEnclosure);

            // Assert
            Assert.IsType<AnimalMovedEvent>(result);
            Assert.Equal(_animal.Id, result.AnimalId);
            Assert.Equal(_compatibleEnclosure.Id, result.EnclosureId);
            Assert.Equal(_compatibleEnclosure.Id, _animal.EnclosureId);
        }

        [Fact]
        public void MoveToEnclosure_WhenIncompatibleEnclosure_ShouldThrow()
        {
            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _animal.MoveToEnclosure(_incompatibleEnclosure));
            Assert.Equal("Animal species is not compatible with enclosure type.", exception.Message);
        }

        [Fact]
        public void MoveToEnclosure_WhenNullEnclosure_ShouldThrow()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _animal.MoveToEnclosure(null));
        }
    }
}