using BankHSE.Domain.Abstractions;
using BankHSE.Domain.Entities;
using BankHSE.Application.Factories;
using BankHSE.Domain.Enums;

namespace BankHSE.Application.Facades;

public class CategoryFacade
{
    private readonly IRepository<Category> _categoriesRepository;
    private readonly CoreEntitiesFactory _factory;

    public CategoryFacade(IRepository<Category> categoriesRepository, CoreEntitiesFactory factory)
    {
        _categoriesRepository = categoriesRepository ?? throw new ArgumentNullException(nameof(categoriesRepository));
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    public Category CreateCategory(TransactionType type, string name)
    {
        var category = _factory.CreateCategory(type, name);
        _categoriesRepository.Add(category);
        return category;
    }

    public Category GetCategoryById(Guid categoryId)
    {
        return _categoriesRepository.GetById(categoryId) ??
               throw new InvalidOperationException($"Category with id {categoryId} was not found");
    }

    public IEnumerable<Category> GetAllCategories()
    {
        return _categoriesRepository.GetAll();
    }

    public void UpdateCategoryById(Guid categoryId, TransactionType? type, string? name)
    {
        var category = GetCategoryById(categoryId);

        if (type != null) category.UpdateCategoryType(type.GetValueOrDefault());
        if (name != null) category.UpdateCategoryName(name);

        _categoriesRepository.Update(category);
    }

    public void DeleteCategoryById(Guid categoryId)
    {
        _categoriesRepository.Delete(categoryId);
    }
}