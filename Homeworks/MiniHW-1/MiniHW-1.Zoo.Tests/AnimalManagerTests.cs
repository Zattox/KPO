using Microsoft.Extensions.DependencyInjection;
using MiniHW_1.Zoo.Domain.Entities.Creatures;
using MiniHW_1.Zoo.Domain.Helpers;
using MiniHW_1.Zoo.Domain.Entities.Firms;
using MiniHW_1.Zoo.Domain.Managers;
using Xunit;

namespace MiniHW_1.Zoo.Tests
{
    public class AnimalManagerTests
    {
        private readonly IServiceProvider _serviceProvider;

        public AnimalManagerTests()
        {
            _serviceProvider = ServiceConfiguration.ConfigureServices();
        }

        [Fact]
        public void TestAddAnimalHealthyAnimal()
        {
            // Arrange
            var clinic = _serviceProvider.GetRequiredService<VeterinaryClinic>();
            var manager = new AnimalManager(clinic);
            var monkey = new Monkey(10, "Monkey", 8);

            // Act
            manager.AddAnimal(monkey);

            // Assert
            Assert.Single(manager.GetAnimals());
        }

        [Fact]
        public void TestAddAnimalSickAnimal()
        {
            var clinic = _serviceProvider.GetRequiredService<VeterinaryClinic>();
            var manager = new AnimalManager(clinic);
            var monkey = new Monkey(0, "Sick Monkey", 8); // Food = 0 makes it sick
            
            manager.AddAnimal(monkey);
            
            Assert.Empty(manager.GetAnimals());
        }

        [Fact]
        public void TestFoodReportAfterCheck()
        {
            var clinic = _serviceProvider.GetRequiredService<VeterinaryClinic>();
            var manager = new AnimalManager(clinic);
            var monkey = new Monkey(1000, "Monkey", 8);
            var tiger = new Tiger(200, "Tiger");
            var rabbit = new Rabbit(30, "Krol", 5);
            var wolf = new Wolf(1, "Wolfyaka");
            
            manager.AddAnimal(monkey);
            manager.AddAnimal(tiger);
            manager.AddAnimal(rabbit);
            manager.AddAnimal(wolf);
            
            var totalFood = manager.CalculateAnimalFoodReport();
            
            Assert.Equal(31, totalFood);
        }

        [Fact] public void TestRightNumbersOfAnimals()
        {
            var clinic = _serviceProvider.GetRequiredService<VeterinaryClinic>();
            var manager = new AnimalManager(clinic);
            var monkey = new Monkey(1000, "Monkey", 8);
            var tiger = new Tiger(200, "Tiger");
            var rabbit = new Rabbit(30, "Krol", 5);
            var wolf = new Wolf(1, "Wolfyaka");
            
            manager.AddAnimal(monkey);
            manager.AddAnimal(tiger);
            manager.AddAnimal(rabbit);
            manager.AddAnimal(wolf);
            
            var animals = manager.GetAnimals();
        
            Assert.Equal(rabbit.Number, animals[0].Number);
            Assert.Equal(wolf.Number, animals[1].Number);
        }
    }
}