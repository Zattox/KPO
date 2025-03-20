using BankHSE.Application.Facades;
using BankHSE.Domain.Abstractions;
using BankHSE.Domain.Entities;
using BankHSE.Domain.Enums;

namespace BankHSE.Application.Analytics;

public class AnalyticsService
{
    private readonly OperationFacade _operationFacade;
    private readonly IRepository<BankAccount> _accountsRepository;
    private readonly CategoryFacade _categoryFacade;

    public AnalyticsService(OperationFacade operationFacade, IRepository<BankAccount> accountsRepository,
        CategoryFacade categoryFacade)
    {
        _operationFacade = operationFacade;
        _accountsRepository = accountsRepository;
        _categoryFacade = categoryFacade;
    }

    public decimal GetDifferenceByAccountId(Guid accountId, DateTime startDate, DateTime endDate)
    {
        var operations = _operationFacade.GetByAccountId(accountId).ToList();
        var income = operations.Where(o => o.Type == TransactionType.Income && o.Date >= startDate && o.Date <= endDate)
            .Sum(o => o.Amount);
        var expense = operations
            .Where(o => o.Type == TransactionType.Expense && o.Date >= startDate && o.Date <= endDate)
            .Sum(o => o.Amount);
        return income - expense;
    }

    public Dictionary<Guid, decimal> GroupOperationsByCategory(Guid accountId, DateTime startDate, DateTime endDate)
    {
        var operations = _operationFacade.GetByAccountId(accountId).ToList();
        var result = new Dictionary<Guid, decimal>();
        foreach (var operation in operations)
        {
            if (operation.Date >= startDate && operation.Date <= endDate)
            {
                if (result.ContainsKey(operation.CategoryId))
                    result[operation.CategoryId] += operation.Amount;
                else
                    result.Add(operation.CategoryId, operation.Amount);
            }
        }
        return result;
    }

    public List<(Category Category, decimal TotalAmount)> GetTopCategoriesByAmount(
        Guid accountId, DateTime startDate, DateTime endDate, int topN)
    {
        var operations = _operationFacade.GetByAccountId(accountId)
            .Where(o => o.Date >= startDate && o.Date <= endDate)
            .GroupBy(o => o.CategoryId)
            .Select(g => new { CategoryId = g.Key, TotalAmount = g.Sum(o => o.Amount) })
            .OrderByDescending(x => x.TotalAmount)
            .Take(topN)
            .ToList();
        return operations.Select(x => (_categoryFacade.GetCategoryById(x.CategoryId), x.TotalAmount)).ToList();
    }

    public decimal GetAverageOperationAmountByCategory(Guid accountId, Guid categoryId, DateTime startDate, DateTime endDate)
    {
        var operations = _operationFacade.GetByAccountId(accountId)
            .Where(o => o.CategoryId == categoryId && o.Date >= startDate && o.Date <= endDate)
            .ToList();
        return operations.Any() ? operations.Average(o => o.Amount) : 0m;
    }
}