using Microsoft.EntityFrameworkCore;
using PharmacySystem.Domain.Entities;
using PharmacySystem.Domain.Interfaces;
using PharmacySystem.Infrastructure.Persistence;

namespace PharmacySystem.Infrastructure.Repositories;

public class PurchaseRepository : GenericRepository<Purchase, int>, IPurchaseRepository
{
    public PurchaseRepository(AppDbContext context) : base(context) { }

    public async Task<Purchase?> GetWithItemsAsync(int purchaseId, CancellationToken ct = default)
        => await _dbSet
            .Include(p => p.Supplier)
            .Include(p => p.Items).ThenInclude(item => item.Medicine)
            .FirstOrDefaultAsync(p => p.Id == purchaseId, ct);

    public async Task<string> GeneratePurchaseNumberAsync(CancellationToken ct = default)
    {
        var prefix = $"PO-{DateTime.UtcNow:yyyyMM}";
        var count  = await _dbSet.IgnoreQueryFilters()
                                 .CountAsync(p => p.PurchaseNumber.StartsWith(prefix), ct);
        return $"{prefix}{count + 1:D4}";
    }

    public async Task<IReadOnlyList<Purchase>> GetBySupplierAsync(int supplierId, CancellationToken ct = default)
        => await _dbSet
            .AsNoTracking()
            .Include(p => p.Supplier)
            .Include(p => p.Items)
            .Where(p => p.SupplierId == supplierId && !p.IsDeleted)
            .OrderByDescending(p => p.PurchaseDate)
            .ToListAsync(ct);
}

public class ExpenseRepository : GenericRepository<Expense, int>, IExpenseRepository
{
    public ExpenseRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Expense>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken ct = default)
        => await _dbSet
            .AsNoTracking()
            .Include(e => e.ExpenseCategory)
            .Where(e => !e.IsDeleted && e.ExpenseDate >= from && e.ExpenseDate < to)
            .OrderByDescending(e => e.ExpenseDate)
            .ToListAsync(ct);

    public async Task<decimal> GetTotalExpensesAsync(DateTime from, DateTime to, CancellationToken ct = default)
        => await _dbSet
            .Where(e => !e.IsDeleted && e.ExpenseDate >= from && e.ExpenseDate < to)
            .SumAsync(e => e.Amount, ct);

    public async Task<IReadOnlyList<Expense>> GetByCategoryAsync(int categoryId, CancellationToken ct = default)
        => await _dbSet
            .AsNoTracking()
            .Include(e => e.ExpenseCategory)
            .Where(e => e.ExpenseCategoryId == categoryId && !e.IsDeleted)
            .ToListAsync(ct);
}

public class InventoryAuditRepository : GenericRepository<InventoryAudit, long>, IInventoryAuditRepository
{
    public InventoryAuditRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<InventoryAudit>> GetByMedicineAsync(int medicineId, CancellationToken ct = default)
        => await _dbSet
            .AsNoTracking()
            .Include(a => a.Medicine)
            .Where(a => a.MedicineId == medicineId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<InventoryAudit>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken ct = default)
        => await _dbSet
            .AsNoTracking()
            .Include(a => a.Medicine)
            .Where(a => a.CreatedAt >= from && a.CreatedAt < to)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(ct);
}
