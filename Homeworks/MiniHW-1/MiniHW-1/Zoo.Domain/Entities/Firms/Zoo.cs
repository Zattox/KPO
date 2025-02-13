using MiniHW_1.Zoo.Domain.Entities.Creatures;
using MiniHW_1.Zoo.Domain.Entities.Firms;
using MiniHW_1.Zoo.Domain.Entities.Objects;
using MiniHW_1.Zoo.Domain.Helpers;
using MiniHW_1.Zoo.Domain.Managers;

namespace MiniHW_1.Zoo.Domain.Entities.Firms;

public class Zoo
{
    private readonly AnimalManager _animalManager;
    private readonly ThingManager _thingManager;
    private readonly EmployeeManager _employeeManager;

    public Zoo(VeterinaryClinic clinic)
    {
        _animalManager = new AnimalManager(clinic);
        _thingManager = new ThingManager();
        _employeeManager = new EmployeeManager();
    }

    public void AddAnimal(Animal animal) => _animalManager.AddAnimal(animal);
    public void AddThing(Thing thing) => _thingManager.AddThing(thing);
    public void AddEmployee(Employee employee) => _employeeManager.AddEmployee(employee);

    public void PrintAnimalFoodReport() => _animalManager.PrintAnimalFoodReport();
    public void PrintContactZooAnimals() => _animalManager.PrintContactZooAnimals();

    public void PrintInventory()
    {
        Console.WriteLine("==============The zoo's inventory==============");
        _animalManager.PrintAnimals();
        _thingManager.PrintThings();
        _animalManager.PrintAnimalFoodReport();
        Console.WriteLine("===============================================");
    }

    public void PrintStaff()
    {
        Console.WriteLine("===================Zoo staff===================");
        _employeeManager.PrintStaff();
        _employeeManager.PrintStaffFoodReport();
        Console.WriteLine("===============================================");
    }
}