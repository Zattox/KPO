using ZooManagement.Domain.Entities;
using ZooManagement.Domain.Enums;
using ZooManagement.Domain.ValueObjects;

namespace ZooManagement.Tests.Domain
{
    public class EnclosureTests
    {
        private readonly Enclosure _enclosure;
        private readonly Animal _compatibleAnimal;
        private readonly Animal _incompatibleAnimal;

        public EnclosureTests()
        {
            _enclosure = new Enclosure(
                EnclosureType.Cage,
                new EnclosureSize(100),
                new EnclosureCapacity(2)
            );
            _compatibleAnimal = new Animal(
                SpeciesType.Mammal,
                new AnimalName("Leo"),
                new BirthDate(DateTime.UtcNow.AddYears(-5)),
                Gender.Male,
                FoodType.Meat
            );
            _incompatibleAnimal = new Animal(
                SpeciesType.Fish,
                new AnimalName("Nemo"),
                new BirthDate(DateTime.UtcNow.AddYears(-1)),
                Gender.Male,
                FoodType.Vegetables
            );
        }

        [Fact]
        public void AddAnimal_WhenCompatibleAndHasCapacity_ShouldIncreaseCount()
        {
            // Act
            _enclosure.AddAnimal(_compatibleAnimal);

            // Assert
            Assert.Equal(1, _enclosure.CurrentAnimalCount);
        }

        [Fact]
        public void AddAnimal_WhenIncompatibleAnimal_ShouldThrow()
        {
            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _enclosure.AddAnimal(_incompatibleAnimal));
            Assert.Equal("Animal species is not compatible with enclosure type.", exception.Message);
        }

        [Fact]
        public void AddAnimal_WhenNoCapacity_ShouldThrow()
        {
            // Arrange
            _enclosure.AddAnimal(_compatibleAnimal);
            _enclosure.AddAnimal(new Animal(
                SpeciesType.Mammal,
                new AnimalName("Simba"),
                new BirthDate(DateTime.UtcNow.AddYears(-3)),
                Gender.Male,
                FoodType.Meat
            ));

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _enclosure.AddAnimal(_compatibleAnimal));
            Assert.Equal("Enclosure is at maximum capacity.", exception.Message);
        }

        [Fact]
        public void AddAnimal_WhenNullAnimal_ShouldThrow()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _enclosure.AddAnimal(null));
        }

        [Fact]
        public void RemoveAnimal_WhenAnimalInEnclosure_ShouldDecreaseCount()
        {
            // Arrange
            _enclosure.AddAnimal(_compatibleAnimal);
            _compatibleAnimal.MoveToEnclosure(_enclosure);

            // Act
            _enclosure.RemoveAnimal(_compatibleAnimal);

            // Assert
            Assert.Equal(0, _enclosure.CurrentAnimalCount);
        }

        [Fact]
        public void RemoveAnimal_WhenAnimalNotInEnclosure_ShouldThrow()
        {
            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _enclosure.RemoveAnimal(_compatibleAnimal));
            Assert.Equal("Animal is not in this enclosure.", exception.Message);
        }

        [Fact]
        public void Clean_WhenEmpty_ShouldNotThrow()
        {
            // Act
            _enclosure.Clean();
        }

        [Fact]
        public void Clean_WhenNotEmpty_ShouldThrow()
        {
            // Arrange
            _enclosure.AddAnimal(_compatibleAnimal);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _enclosure.Clean());
            Assert.Equal("Cannot clean enclosure with animals inside.", exception.Message);
        }
    }
}