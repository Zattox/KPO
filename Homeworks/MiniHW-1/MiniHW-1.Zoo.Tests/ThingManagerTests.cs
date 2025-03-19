using Microsoft.Extensions.DependencyInjection;
using MiniHW_1.Zoo.Domain.Helpers;
using MiniHW_1.Zoo.Domain.Entities.Objects;
using MiniHW_1.Zoo.Domain.Managers;
using Xunit;

namespace MiniHW_1.Zoo.Tests
{
    public class ThingManagerTests
    {
        private readonly IServiceProvider _serviceProvider;

        public ThingManagerTests()
        {
            _serviceProvider = ServiceConfiguration.ConfigureServices();
        }
        
        [Fact]
        public void TestAddThing()
        {
            // Arrange
            var manager = new ThingManager();
            var table = new Table("Table 1");

            // Act
            manager.AddThing(table);

            // Assert
            Assert.Single(manager.GetThings());
        }

        [Fact]
        public void TestGetThings()
        {
            var manager = new ThingManager();
            var table = new Table("Table 1");
            var computer = new Computer("Computer 1");

            manager.AddThing(table);
            manager.AddThing(computer);
            
            var things = manager.GetThings();
            
            Assert.Equal(2, things.Count);
        }
        
        [Fact]
        public void TestRightNumbersOfThings()
        {
            var manager = new ThingManager();
            var table = new Table("Table 1");
            var computer = new Computer("Computer 1");

            manager.AddThing(table);
            manager.AddThing(computer);
            
            var things = manager.GetThings();
            
            Assert.Equal(table.Number, things[0].Number);
            Assert.Equal(computer.Number, things[1].Number);
        }
    }
}