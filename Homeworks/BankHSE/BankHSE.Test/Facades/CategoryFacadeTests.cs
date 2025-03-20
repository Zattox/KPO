namespace BankHSE.Tests.Facades;

public class CategoryFacadeTests
{
    private readonly CategoryFacade _facade;
    private readonly IRepository<Category> _catRepo;
    private readonly CoreEntitiesFactory _factory;

    public CategoryFacadeTests()
    {
        _catRepo = new InMemoryRepository<Category>();
        _factory = new CoreEntitiesFactory();
        _facade = new CategoryFacade(_catRepo, _factory);
    }

    [Fact]
    public void CreateCategory_AddsCategory()
    {
        // Arrange
        var type = TransactionType.Income;
        string name = "Salary";

        // Act
        var category = _facade.CreateCategory(type, name);

        // Assert
        Assert.Equal(type, category.Type);
        Assert.Equal(name, category.Name);
        Assert.Contains(category, _catRepo.GetAll());
    }

    [Fact]
    public void UpdateCategoryById_UpdatesName()
    {
        // Arrange
        var category = _facade.CreateCategory(TransactionType.Expense, "Old");
        string newName = "New";

        // Act
        _facade.UpdateCategoryById(category.Id, null, newName);
        var updated = _facade.GetCategoryById(category.Id);

        // Assert
        Assert.Equal(newName, updated.Name);
    }

    [Fact]
    public void DeleteCategoryById_RemovesCategory()
    {
        // Arrange
        var category = _facade.CreateCategory(TransactionType.Income, "To Delete");

        // Act
        _facade.DeleteCategoryById(category.Id);

        // Assert
        Assert.Null(_catRepo.GetById(category.Id));
    }
}