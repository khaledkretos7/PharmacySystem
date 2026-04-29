namespace PharmacySystem.Domain.Interfaces;

/// <summary>
/// Unit of Work — groups all repositories under one transaction scope.
/// Application layer calls SaveChangesAsync() to commit everything atomically.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IMedicineRepository       Medicines        { get; }
    ICategoryRepository       Categories       { get; }
    ISupplierRepository       Suppliers        { get; }
    ICustomerRepository       Customers        { get; }
    IInvoiceRepository        Invoices         { get; }
    IInvoiceReturnRepository  InvoiceReturns   { get; }
    IPurchaseRepository       Purchases        { get; }
    IExpenseRepository        Expenses         { get; }
    IExpenseCategoryRepository ExpenseCategories{ get; }
    IInventoryAuditRepository  InventoryAudits  { get; }

    Task<int>  SaveChangesAsync(CancellationToken ct = default);
    Task       BeginTransactionAsync(CancellationToken ct = default);
    Task       CommitTransactionAsync(CancellationToken ct = default);
    Task       RollbackTransactionAsync(CancellationToken ct = default);
}
