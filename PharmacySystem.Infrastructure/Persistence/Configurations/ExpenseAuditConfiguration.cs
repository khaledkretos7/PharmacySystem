using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacySystem.Domain.Entities;

namespace PharmacySystem.Infrastructure.Persistence.Configurations;

public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Title).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Description).HasMaxLength(1000);
        builder.Property(e => e.Reference).HasMaxLength(100);
        builder.Property(e => e.Notes).HasMaxLength(1000);
        builder.Property(e => e.RecordedByUserId).IsRequired().HasMaxLength(450);
        builder.Property(e => e.Amount).HasColumnType("decimal(18,4)");

        builder.HasIndex(e => e.ExpenseDate);

        builder.HasOne(e => e.ExpenseCategory)
               .WithMany(c => c.Expenses)
               .HasForeignKey(e => e.ExpenseCategoryId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

public class InventoryAuditConfiguration : IEntityTypeConfiguration<InventoryAudit>
{
    public void Configure(EntityTypeBuilder<InventoryAudit> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.BatchNumber).HasMaxLength(100);
        builder.Property(a => a.ReferenceDocument).HasMaxLength(100);
        builder.Property(a => a.Notes).HasMaxLength(1000);
        builder.Property(a => a.PerformedByUserId).IsRequired().HasMaxLength(450);

        // Append-only — never update this table
        builder.HasIndex(a => a.MedicineId);
        builder.HasIndex(a => a.CreatedAt);

        builder.HasOne(a => a.Medicine)
               .WithMany(m => m.InventoryAudits)
               .HasForeignKey(a => a.MedicineId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
