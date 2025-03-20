﻿using BankHSE.Domain.Entities;
using BankHSE.Domain.Enums;

namespace BankHSE.Application.Factories;

public class CoreEntitiesFactory
{
    public BankAccount CreateBankAccount(string name, decimal balance = decimal.Zero) => new(name, balance);
    public BankAccount CreateBankAccount(Guid id, string name, decimal balance) => new(id, name, balance);

    public Category CreateCategory(TransactionType type, string name) => new(type, name);
    public Category CreateCategory(Guid id, TransactionType type, string name) => new(id, type, name);

    public Operation CreateOperation(TransactionType type, BankAccount bankAccount, decimal amount, DateTime date,
        string description, Category category) => new(type, bankAccount.Id, amount, date, description, category.Id);

    public Operation CreateOperation(Guid id, TransactionType type, BankAccount bankAccount, decimal amount,
        DateTime date,
        string description, Category category) => new(id, type, bankAccount.Id, amount, date, description, category.Id);
}