using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PharmacySystem.Domain.Common;
using PharmacySystem.Domain.Entities;
using PharmacySystem.Infrastructure.Identity;

namespace PharmacySystem.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // ── DbSets ───────────────────────────────────────────────────────────────
    public DbSet<Medicine>       Medicines        { get; set; }
    public DbSet<Category>       Categories       { get; set; }
    public DbSet<Supplier>       Suppliers        { get; set; }
    public DbSet<Customer>       Customers        { get; set; }
    public DbSet<Invoice>        Invoices         { get; set; }
    public DbSet<InvoiceItem>    InvoiceItems     { get; set; }
    public DbSet<InvoiceReturn>  InvoiceReturns   { get; set; }
    public DbSet<Purchase>       Purchases        { get; set; }
    public DbSet<PurchaseItem>   PurchaseItems    { get; set; }
    public DbSet<Expense>        Expenses         { get; set; }
    public DbSet<ExpenseCategory>ExpenseCategories{ get; set; }
    public DbSet<InventoryAudit> InventoryAudits  { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply all IEntityTypeConfiguration<T> in this assembly
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // ── Rename Identity tables ────────────────────────────────────────
        builder.Entity<ApplicationUser>().ToTable("Users");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityRole>().ToTable("Roles");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<string>>().ToTable("UserRoles");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserClaim<string>>().ToTable("UserClaims");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<string>>().ToTable("UserLogins");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>>().ToTable("RoleClaims");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<string>>().ToTable("UserTokens");

        // ── Global soft-delete query filters ──────────────────────────────
        builder.Entity<Medicine>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Category>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Supplier>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Customer>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Invoice>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<InvoiceItem>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<InvoiceReturn>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Purchase>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<PurchaseItem>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Expense>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<ExpenseCategory>().HasQueryFilter(e => !e.IsDeleted);
    }

    // ── Auto-set audit timestamps on every save ───────────────────────────
    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added)
            {
                if (entry.Entity is BaseEntity<int>  ei) { ei.CreatedAt = now; }
                if (entry.Entity is BaseEntity<long> el) { el.CreatedAt = now; }
            }
            else if (entry.State == EntityState.Modified)
            {
                if (entry.Entity is BaseEntity<int>  ei) { ei.UpdatedAt = now; }
                if (entry.Entity is BaseEntity<long> el) { el.UpdatedAt = now; }
            }
        }
        return await base.SaveChangesAsync(ct);
    }
}
