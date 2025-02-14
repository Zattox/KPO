using Microsoft.Extensions.DependencyInjection;
using MiniHW_1.Zoo.Domain.Entities.Creatures;
using MiniHW_1.Zoo.Domain.Abstractions;
using MiniHW_1.Zoo.Domain.Helpers;
using MiniHW_1.Zoo.Domain.Entities.Firms;
using Xunit;

namespace MiniHW_1.Zoo.Tests
{
    public class VeterinaryClinicTests
    {
        private readonly IServiceProvider _serviceProvider;

        public VeterinaryClinicTests()
        {
            _serviceProvider = ServiceConfiguration.ConfigureServices();
        }

        [Fact]
        public void TestCheckHealthyAnimal()
        {
            var clinic = _serviceProvider.GetRequiredService<VeterinaryClinic>();
            var monkey = new Monkey(10, "Monkey", 8);

            var healthStatus = clinic.CheckHealth(monkey);

            Assert.Equal(HealthStatus.Healthy, healthStatus);
        }

        [Fact]
        public void TestCheckSickAnimal()
        {
            var clinic = _serviceProvider.GetRequiredService<VeterinaryClinic>();
            var monkey = new Monkey(0, "Sick Monkey", 8); // Food = 0 makes it sick

            var healthStatus = clinic.CheckHealth(monkey);

            Assert.Equal(HealthStatus.Sick, healthStatus);
        }
    }
}