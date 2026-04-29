using PharmacySystem.Domain.Entities;

namespace PharmacySystem.Domain.Interfaces;

// ─────────────────────────────────────────────────────────────────────────────
//  Strongly-typed repository interfaces for each aggregate root.
//  These extend IRepository<T,TId> and can declare extra query methods.
// ─────────────────────────────────────────────────────────────────────────────

public interface IMedicineRepository : IRepository<Medicine, int>
{
    Task<Medicine?> GetByBarcodeAsync(string barcode, CancellationToken ct = default);
    Task<IReadOnlyList<Medicine>> GetLowStockAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Medicine>> GetNearExpiryAsync(int daysThreshold, CancellationToken ct = default);
    Task<IReadOnlyList<Medicine>> GetByCategoryAsync(int categoryId, CancellationToken ct = default);
}

public interface ICategoryRepository : IRepository<Category, int>
{
    Task<bool> HasMedicinesAsync(int categoryId, CancellationToken ct = default);
}

public interface ISupplierRepository : IRepository<Supplier, int>
{
    Task<Supplier?> GetWithPurchasesAsync(int supplierId, CancellationToken ct = default);
}

public interface ICustomerRepository : IRepository<Customer, int>
{
    Task<Customer?> GetByPhoneAsync(string phone, CancellationToken ct = default);
    Task<IReadOnlyList<Customer>> GetTopCustomersAsync(int count, CancellationToken ct = default);
}

public interface IInvoiceRepository : IRepository<Invoice, int>
{
    Task<Invoice?> GetWithItemsAsync(int invoiceId, CancellationToken ct = default);
    Task<string> GenerateInvoiceNumberAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Invoice>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken ct = default);
    Task<decimal> GetTotalRevenueAsync(DateTime from, DateTime to, CancellationToken ct = default);
}

public interface IInvoiceReturnRepository : IRepository<InvoiceReturn, int>
{
    Task<IReadOnlyList<InvoiceReturn>> GetByInvoiceAsync(int invoiceId, CancellationToken ct = default);
    Task<string> GenerateReturnNumberAsync(CancellationToken ct = default);
}

public interface IPurchaseRepository : IRepository<Purchase, int>
{
    Task<Purchase?> GetWithItemsAsync(int purchaseId, CancellationToken ct = default);
    Task<string> GeneratePurchaseNumberAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Purchase>> GetBySupplierAsync(int supplierId, CancellationToken ct = default);
}

public interface IExpenseRepository : IRepository<Expense, int>
{
    Task<IReadOnlyList<Expense>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken ct = default);
    Task<decimal> GetTotalExpensesAsync(DateTime from, DateTime to, CancellationToken ct = default);
    Task<IReadOnlyList<Expense>> GetByCategoryAsync(int categoryId, CancellationToken ct = default);
}

public interface IExpenseCategoryRepository : IRepository<ExpenseCategory, int>
{
    Task<bool> HasExpensesAsync(int categoryId, CancellationToken ct = default);
}

public interface IInventoryAuditRepository : IRepository<InventoryAudit, long>
{
    Task<IReadOnlyList<InventoryAudit>> GetByMedicineAsync(int medicineId, CancellationToken ct = default);
    Task<IReadOnlyList<InventoryAudit>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken ct = default);
}
