using Microsoft.EntityFrameworkCore.Storage;
using PharmacySystem.Domain.Interfaces;
using PharmacySystem.Infrastructure.Persistence;
using PharmacySystem.Infrastructure.Repositories;

namespace PharmacySystem.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;

    // ── Lazy-initialised repositories ────────────────────────────────────────
    private IMedicineRepository?        _medicines;
    private ICategoryRepository?        _categories;
    private ISupplierRepository?        _suppliers;
    private ICustomerRepository?        _customers;
    private IInvoiceRepository?         _invoices;
    private IInvoiceReturnRepository?   _invoiceReturns;
    private IPurchaseRepository?        _purchases;
    private IExpenseRepository?         _expenses;
    private IExpenseCategoryRepository? _expenseCategories;
    private IInventoryAuditRepository?  _inventoryAudits;

    public UnitOfWork(AppDbContext context) => _context = context;

    public IMedicineRepository       Medicines         => _medicines        ??= new MedicineRepository(_context);
    public ICategoryRepository       Categories        => _categories       ??= new CategoryRepository(_context);
    public ISupplierRepository       Suppliers         => _suppliers        ??= new SupplierRepository(_context);
    public ICustomerRepository       Customers         => _customers        ??= new CustomerRepository(_context);
    public IInvoiceRepository        Invoices          => _invoices         ??= new InvoiceRepository(_context);
    public IInvoiceReturnRepository  InvoiceReturns    => _invoiceReturns   ??= new InvoiceReturnRepository(_context);
    public IPurchaseRepository       Purchases         => _purchases        ??= new PurchaseRepository(_context);
    public IExpenseRepository        Expenses          => _expenses         ??= new ExpenseRepository(_context);
    public IExpenseCategoryRepository ExpenseCategories=> _expenseCategories??= new ExpenseCategoryRepository(_context);
    public IInventoryAuditRepository  InventoryAudits  => _inventoryAudits  ??= new InventoryAuditRepository(_context);

    // ── Persistence ──────────────────────────────────────────────────────────
    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);

    // ── Transaction management ────────────────────────────────────────────────
    public async Task BeginTransactionAsync(CancellationToken ct = default)
        => _transaction = await _context.Database.BeginTransactionAsync(ct);

    public async Task CommitTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction is null) return;
        await _transaction.CommitAsync(ct);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task RollbackTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction is null) return;
        await _transaction.RollbackAsync(ct);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
