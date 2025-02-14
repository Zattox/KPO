using MiniHW_1.Zoo.Domain.Entities.Creatures;
using MiniHW_1.Zoo.Domain.Managers;
using Xunit;

namespace MiniHW_1.Zoo.Tests
{
    public class EmployeeManagerTests
    {
        [Fact]
        public void AddEmployee_ShouldAddToStaff()
        {
            // Arrange
            var manager = new EmployeeManager();
            var employee = new Employee(100, "John Doe", "Zookeeper");

            // Act
            manager.AddEmployee(employee);

            // Assert
            Assert.Single(manager.GetStaff());
        }

        [Fact]
        public void PrintStaffFoodReport_ShouldReturnCorrectTotalFood()
        {
            // Arrange
            var manager = new EmployeeManager();
            var employee1 = new Employee(100, "John Doe", "Zookeeper");
            var employee2 = new Employee(200, "Jane Doe", "Veterinarian");

            manager.AddEmployee(employee1);
            manager.AddEmployee(employee2);

            // Act
            var totalFood = manager.CalculateStaffFoodReport();

            // Assert
            Assert.Equal(300, totalFood);
        }
    }
}