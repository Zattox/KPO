using Microsoft.Extensions.DependencyInjection;
using MiniHW_1.Zoo.Domain.Entities.Creatures;
using MiniHW_1.Zoo.Domain.Helpers;
using MiniHW_1.Zoo.Domain.Managers;
using Xunit;

namespace MiniHW_1.Zoo.Tests
{
    public class EmployeeManagerTests
    {
        private readonly IServiceProvider _serviceProvider;

        public EmployeeManagerTests()
        {
            _serviceProvider = ServiceConfiguration.ConfigureServices();
        }

        [Fact]
        public void TestAddEmployees()
        {
            var manager = new EmployeeManager();
            var employee1 = new Employee(1, "John Doe", "Zookeeper");
            var employee2 = new Employee(2, "Kirok Sou", "Director");
            
            manager.AddEmployee(employee1);
            manager.AddEmployee(employee2);
            
            Assert.Equal(2, manager.GetStaff().Count);
        }

        [Fact]
        public void TestCountStaffFoodReport()
        {
            var manager = new EmployeeManager();
            var employee1 = new Employee(100, "John Doe", "Zookeeper");
            var employee2 = new Employee(200, "Jane Doe", "Veterinarian");

            manager.AddEmployee(employee1);
            manager.AddEmployee(employee2);
            
            var totalFood = manager.CalculateStaffFoodReport();
            
            Assert.Equal(300, totalFood);
        }
    }
}