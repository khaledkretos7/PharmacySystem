using Microsoft.EntityFrameworkCore;
using PharmacySystem.Domain.Entities;
using PharmacySystem.Domain.Interfaces;
using PharmacySystem.Infrastructure.Persistence;

namespace PharmacySystem.Infrastructure.Repositories;

public class MedicineRepository : GenericRepository<Medicine, int>, IMedicineRepository
{
    public MedicineRepository(AppDbContext context) : base(context) { }

    public async Task<Medicine?> GetByBarcodeAsync(string barcode, CancellationToken ct = default)
        => await _dbSet
            .Include(m => m.Category)
            .Include(m => m.Supplier)
            .FirstOrDefaultAsync(m => m.Barcode == barcode && !m.IsDeleted, ct);

    public async Task<IReadOnlyList<Medicine>> GetLowStockAsync(CancellationToken ct = default)
        => await _dbSet
            .AsNoTracking()
            .Include(m => m.Category)
            .Where(m => !m.IsDeleted && m.QuantityInStock <= m.ReorderLevel)
            .OrderBy(m => m.QuantityInStock)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Medicine>> GetNearExpiryAsync(int daysThreshold, CancellationToken ct = default)
    {
        var threshold = DateTime.UtcNow.AddDays(daysThreshold);
        return await _dbSet
            .AsNoTracking()
            .Include(m => m.Category)
            .Where(m => !m.IsDeleted && m.ExpiryDate.HasValue && m.ExpiryDate.Value <= threshold && m.ExpiryDate.Value >= DateTime.UtcNow)
            .OrderBy(m => m.ExpiryDate)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Medicine>> GetByCategoryAsync(int categoryId, CancellationToken ct = default)
        => await _dbSet
            .AsNoTracking()
            .Include(m => m.Category)
            .Where(m => m.CategoryId == categoryId && !m.IsDeleted)
            .ToListAsync(ct);

    // Override GetByIdAsync to include nav properties
    public override async Task<Medicine?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _dbSet
            .Include(m => m.Category)
            .Include(m => m.Supplier)
            .FirstOrDefaultAsync(m => m.Id == id, ct);
}
