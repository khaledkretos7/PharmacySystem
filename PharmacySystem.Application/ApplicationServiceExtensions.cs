using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PharmacySystem.Application.Interfaces;
using PharmacySystem.Application.Mappings;
using PharmacySystem.Application.Services;

namespace PharmacySystem.Application;

/// <summary>
/// Extension method that registers all Application-layer services.
/// Called once from Program.cs — keeps DI setup out of the API project.
/// </summary>
public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // ── AutoMapper ───────────────────────────────────────────────────────
        services.AddAutoMapper(typeof(MappingProfiles).Assembly);

        // ── FluentValidation (auto-register all validators in this assembly) ─
        services.AddValidatorsFromAssembly(typeof(ApplicationServiceExtensions).Assembly);

        // ── Application Services ─────────────────────────────────────────────
        services.AddScoped<ICategoryService,  CategoryService>();
        services.AddScoped<ISupplierService,  SupplierService>();
        services.AddScoped<ICustomerService,  CustomerService>();
        services.AddScoped<IMedicineService,  MedicineService>();
        services.AddScoped<IInvoiceService,   InvoiceService>();
        services.AddScoped<IPurchaseService,  PurchaseService>();
        services.AddScoped<IExpenseService,   ExpenseService>();
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<IReportService,    ReportService>();

        return services;
    }
}
