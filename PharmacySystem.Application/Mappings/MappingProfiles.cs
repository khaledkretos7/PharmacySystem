using AutoMapper;
using PharmacySystem.Application.DTOs.Category;
using PharmacySystem.Application.DTOs.Customer;
using PharmacySystem.Application.DTOs.Expense;
using PharmacySystem.Application.DTOs.Inventory;
using PharmacySystem.Application.DTOs.Invoice;
using PharmacySystem.Application.DTOs.Medicine;
using PharmacySystem.Application.DTOs.Purchase;
using PharmacySystem.Application.DTOs.Supplier;
using PharmacySystem.Domain.Entities;
using PharmacySystem.Domain.Enums;

namespace PharmacySystem.Application.Mappings;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        // ── Category ────────────────────────────────────────────────────────
        CreateMap<Category, CategoryResponseDto>()
            .ForMember(d => d.MedicineCount, o => o.MapFrom(s => s.Medicines.Count(m => !m.IsDeleted)));

        CreateMap<CreateCategoryDto, Category>();
        CreateMap<UpdateCategoryDto, Category>();

        // ── Supplier ─────────────────────────────────────────────────────────
        CreateMap<Supplier, SupplierResponseDto>()
            .ForMember(d => d.MedicineCount, o => o.MapFrom(s => s.Medicines.Count(m => !m.IsDeleted)))
            .ForMember(d => d.PurchaseCount, o => o.MapFrom(s => s.Purchases.Count(p => !p.IsDeleted)));
        CreateMap<CreateSupplierDto, Supplier>();
        CreateMap<UpdateSupplierDto, Supplier>();

        // ── Customer ─────────────────────────────────────────────────────────
        CreateMap<Customer, CustomerResponseDto>()
            .ForMember(d => d.TotalInvoices, o => o.MapFrom(s => s.Invoices.Count(i => !i.IsDeleted)))
            .ForMember(d => d.TotalSpent,    o => o.MapFrom(s => s.Invoices.Where(i => !i.IsDeleted).Sum(i => i.TotalAmount)));

        CreateMap<Customer, CustomerListDto>()
            .ForMember(d => d.TotalInvoices, o => o.MapFrom(s => s.Invoices.Count(i => !i.IsDeleted)))
            .ForMember(d => d.TotalSpent,    o => o.MapFrom(s => s.Invoices.Where(i => !i.IsDeleted).Sum(i => i.TotalAmount)));

        CreateMap<CreateCustomerDto, Customer>();
        CreateMap<UpdateCustomerDto, Customer>();

        // ── Medicine ─────────────────────────────────────────────────────────
        CreateMap<Medicine, MedicineResponseDto>()
            .ForMember(d => d.DosageForm,    o => o.MapFrom(s => s.DosageForm.ToString()))
            .ForMember(d => d.Status,        o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.CategoryName,  o => o.MapFrom(s => s.Category != null ? s.Category.Name : string.Empty))
            .ForMember(d => d.SupplierName,  o => o.MapFrom(s => s.Supplier != null ? s.Supplier.Name : null))
            .ForMember(d => d.IsLowStock,    o => o.MapFrom(s => s.QuantityInStock <= s.ReorderLevel))
            .ForMember(d => d.IsNearExpiry,  o => o.MapFrom(s => s.ExpiryDate.HasValue && s.ExpiryDate.Value <= DateTime.UtcNow.AddDays(30)))
            .ForMember(d => d.IsExpired,     o => o.MapFrom(s => s.ExpiryDate.HasValue && s.ExpiryDate.Value < DateTime.UtcNow));

        CreateMap<Medicine, MedicineListDto>()
            .ForMember(d => d.Status,        o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.CategoryName,  o => o.MapFrom(s => s.Category != null ? s.Category.Name : string.Empty))
            .ForMember(d => d.IsLowStock,    o => o.MapFrom(s => s.QuantityInStock <= s.ReorderLevel))
            .ForMember(d => d.IsExpired,     o => o.MapFrom(s => s.ExpiryDate.HasValue && s.ExpiryDate.Value < DateTime.UtcNow));

        CreateMap<CreateMedicineDto, Medicine>();
        CreateMap<UpdateMedicineDto, Medicine>();

        // ── Invoice ──────────────────────────────────────────────────────────
        CreateMap<Invoice, InvoiceResponseDto>()
            .ForMember(d => d.PaymentMethod, o => o.MapFrom(s => s.PaymentMethod.ToString()))
            .ForMember(d => d.Status,        o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.CustomerName,  o => o.MapFrom(s => s.Customer != null ? s.Customer.Name : null))
            .ForMember(d => d.Items,         o => o.MapFrom(s => s.Items))
            .ForMember(d => d.Returns,       o => o.MapFrom(s => s.Returns))
            .ForMember(d => d.CashierName,   o => o.Ignore());   // resolved in service

        CreateMap<Invoice, InvoiceListDto>()
            .ForMember(d => d.PaymentMethod, o => o.MapFrom(s => s.PaymentMethod.ToString()))
            .ForMember(d => d.Status,        o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.CustomerName,  o => o.MapFrom(s => s.Customer != null ? s.Customer.Name : null))
            .ForMember(d => d.ItemCount,     o => o.MapFrom(s => s.Items.Count))
            .ForMember(d => d.CashierName,   o => o.Ignore());

        CreateMap<InvoiceItem, InvoiceItemResponseDto>()
            .ForMember(d => d.MedicineName,  o => o.MapFrom(s => s.Medicine != null ? s.Medicine.Name : string.Empty))
            .ForMember(d => d.GenericName,   o => o.MapFrom(s => s.Medicine != null ? s.Medicine.GenericName : string.Empty))
            .ForMember(d => d.Unit,          o => o.MapFrom(s => s.Medicine != null ? s.Medicine.Unit : string.Empty));

        CreateMap<InvoiceReturn, InvoiceReturnResponseDto>()
            .ForMember(d => d.MedicineName,  o => o.MapFrom(s => s.Medicine != null ? s.Medicine.Name : string.Empty))
            .ForMember(d => d.Reason,        o => o.MapFrom(s => s.Reason.ToString()));

        // ── Purchase ─────────────────────────────────────────────────────────
        CreateMap<Purchase, PurchaseResponseDto>()
            .ForMember(d => d.SupplierName,  o => o.MapFrom(s => s.Supplier != null ? s.Supplier.Name : string.Empty))
            .ForMember(d => d.Status,        o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.Items,         o => o.MapFrom(s => s.Items));

        CreateMap<Purchase, PurchaseListDto>()
            .ForMember(d => d.SupplierName,  o => o.MapFrom(s => s.Supplier != null ? s.Supplier.Name : string.Empty))
            .ForMember(d => d.Status,        o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.ItemCount,     o => o.MapFrom(s => s.Items.Count));

        CreateMap<PurchaseItem, PurchaseItemResponseDto>()
            .ForMember(d => d.MedicineName,  o => o.MapFrom(s => s.Medicine != null ? s.Medicine.Name : string.Empty))
            .ForMember(d => d.Unit,          o => o.MapFrom(s => s.Medicine != null ? s.Medicine.Unit : string.Empty));

        // ── Expense ──────────────────────────────────────────────────────────
        CreateMap<Expense, ExpenseResponseDto>()
            .ForMember(d => d.CategoryName,     o => o.MapFrom(s => s.ExpenseCategory != null ? s.ExpenseCategory.Name : string.Empty))
            .ForMember(d => d.RecordedByUser,   o => o.Ignore());  // resolved in service

        CreateMap<Expense, ExpenseListDto>()
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.ExpenseCategory != null ? s.ExpenseCategory.Name : string.Empty));

        CreateMap<CreateExpenseDto, Expense>();
        CreateMap<UpdateExpenseDto, Expense>();

        CreateMap<ExpenseCategory, ExpenseCategoryResponseDto>()
            .ForMember(d => d.ExpenseCount, o => o.MapFrom(s => s.Expenses.Count(e => !e.IsDeleted)))
            .ForMember(d => d.TotalAmount,  o => o.MapFrom(s => s.Expenses.Where(e => !e.IsDeleted).Sum(e => e.Amount)));

        CreateMap<CreateExpenseCategoryDto, ExpenseCategory>();
        CreateMap<UpdateExpenseCategoryDto, ExpenseCategory>();

        // ── Inventory Audit ──────────────────────────────────────────────────
        CreateMap<InventoryAudit, InventoryAuditResponseDto>()
            .ForMember(d => d.MedicineName,      o => o.MapFrom(s => s.Medicine != null ? s.Medicine.Name : string.Empty))
            .ForMember(d => d.ActionType,        o => o.MapFrom(s => s.ActionType.ToString()))
            .ForMember(d => d.PerformedByUser,   o => o.Ignore());  // resolved in service
    }
}
