using Microsoft.EntityFrameworkCore;
using PharmacySystem.Domain.Entities;
using PharmacySystem.Domain.Interfaces;
using PharmacySystem.Infrastructure.Persistence;

namespace PharmacySystem.Infrastructure.Repositories;

public class CategoryRepository : GenericRepository<Category, int>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context) { }

    public async Task<bool> HasMedicinesAsync(int categoryId, CancellationToken ct = default)
        => await _context.Medicines.AnyAsync(m => m.CategoryId == categoryId && !m.IsDeleted, ct);
}

public class SupplierRepository : GenericRepository<Supplier, int>, ISupplierRepository
{
    public SupplierRepository(AppDbContext context) : base(context) { }

    public async Task<Supplier?> GetWithPurchasesAsync(int supplierId, CancellationToken ct = default)
        => await _dbSet
            .Include(s => s.Medicines.Where(m => !m.IsDeleted))
            .Include(s => s.Purchases.Where(p => !p.IsDeleted))
            .FirstOrDefaultAsync(s => s.Id == supplierId, ct);
}

public class CustomerRepository : GenericRepository<Customer, int>, ICustomerRepository
{
    public CustomerRepository(AppDbContext context) : base(context) { }

    public async Task<Customer?> GetByPhoneAsync(string phone, CancellationToken ct = default)
        => await _dbSet.FirstOrDefaultAsync(c => c.Phone == phone && !c.IsDeleted, ct);

    public async Task<IReadOnlyList<Customer>> GetTopCustomersAsync(int count, CancellationToken ct = default)
        => await _dbSet
            .AsNoTracking()
            .Include(c => c.Invoices.Where(i => !i.IsDeleted))
            .OrderByDescending(c => c.Invoices.Where(i => !i.IsDeleted).Sum(i => i.TotalAmount))
            .Take(count)
            .ToListAsync(ct);
}

public class ExpenseCategoryRepository : GenericRepository<ExpenseCategory, int>, IExpenseCategoryRepository
{
    public ExpenseCategoryRepository(AppDbContext context) : base(context) { }

    public async Task<bool> HasExpensesAsync(int categoryId, CancellationToken ct = default)
        => await _context.Expenses.AnyAsync(e => e.ExpenseCategoryId == categoryId && !e.IsDeleted, ct);
}
