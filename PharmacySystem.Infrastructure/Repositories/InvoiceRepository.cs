using Microsoft.EntityFrameworkCore;
using PharmacySystem.Domain.Entities;
using PharmacySystem.Domain.Interfaces;
using PharmacySystem.Infrastructure.Persistence;

namespace PharmacySystem.Infrastructure.Repositories;

public class InvoiceRepository : GenericRepository<Invoice, int>, IInvoiceRepository
{
    public InvoiceRepository(AppDbContext context) : base(context) { }

    public async Task<Invoice?> GetWithItemsAsync(int invoiceId, CancellationToken ct = default)
        => await _dbSet
            .Include(i => i.Customer)
            .Include(i => i.Items).ThenInclude(item => item.Medicine)
            .Include(i => i.Returns).ThenInclude(r => r.Medicine)
            .FirstOrDefaultAsync(i => i.Id == invoiceId, ct);

    public async Task<string> GenerateInvoiceNumberAsync(CancellationToken ct = default)
    {
        var prefix = $"INV-{DateTime.UtcNow:yyyyMM}";
        // IgnoreQueryFilters so we count all invoices including soft-deleted (number integrity)
        var count  = await _dbSet.IgnoreQueryFilters()
                                 .CountAsync(i => i.InvoiceNumber.StartsWith(prefix), ct);
        return $"{prefix}{count + 1:D4}";
    }

    public async Task<IReadOnlyList<Invoice>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken ct = default)
        => await _dbSet
            .AsNoTracking()
            .Include(i => i.Customer)
            .Include(i => i.Items).ThenInclude(item => item.Medicine)
            .Where(i => !i.IsDeleted && i.InvoiceDate >= from && i.InvoiceDate < to)
            .OrderByDescending(i => i.InvoiceDate)
            .ToListAsync(ct);

    public async Task<decimal> GetTotalRevenueAsync(DateTime from, DateTime to, CancellationToken ct = default)
        => await _dbSet
            .Where(i => !i.IsDeleted && i.InvoiceDate >= from && i.InvoiceDate < to)
            .SumAsync(i => i.TotalAmount, ct);
}

public class InvoiceReturnRepository : GenericRepository<InvoiceReturn, int>, IInvoiceReturnRepository
{
    public InvoiceReturnRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<InvoiceReturn>> GetByInvoiceAsync(int invoiceId, CancellationToken ct = default)
        => await _dbSet
            .AsNoTracking()
            .Include(r => r.Medicine)
            .Where(r => r.InvoiceId == invoiceId && !r.IsDeleted)
            .ToListAsync(ct);

    public async Task<string> GenerateReturnNumberAsync(CancellationToken ct = default)
    {
        var prefix = $"RET-{DateTime.UtcNow:yyyyMM}";
        var count  = await _dbSet.IgnoreQueryFilters()
                                 .CountAsync(r => r.ReturnNumber.StartsWith(prefix), ct);
        return $"{prefix}{count + 1:D4}";
    }
}
