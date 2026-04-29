using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacySystem.Domain.Entities;

namespace PharmacySystem.Infrastructure.Persistence.Configurations;

public class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
{
    public void Configure(EntityTypeBuilder<Purchase> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.PurchaseNumber).IsRequired().HasMaxLength(30);
        builder.Property(p => p.SupplierInvoiceNumber).HasMaxLength(100);
        builder.Property(p => p.Notes).HasMaxLength(1000);
        builder.Property(p => p.CreatedByUserId).IsRequired().HasMaxLength(450);

        builder.Property(p => p.SubTotal).HasColumnType("decimal(18,4)");
        builder.Property(p => p.DiscountAmount).HasColumnType("decimal(18,4)");
        builder.Property(p => p.TaxAmount).HasColumnType("decimal(18,4)");
        builder.Property(p => p.TotalAmount).HasColumnType("decimal(18,4)");
        builder.Property(p => p.AmountPaid).HasColumnType("decimal(18,4)");
        builder.Property(p => p.RemainingBalance).HasColumnType("decimal(18,4)");

        builder.HasIndex(p => p.PurchaseNumber).IsUnique();
        builder.HasIndex(p => p.PurchaseDate);

        builder.HasOne(p => p.Supplier)
               .WithMany(s => s.Purchases)
               .HasForeignKey(p => p.SupplierId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

public class PurchaseItemConfiguration : IEntityTypeConfiguration<PurchaseItem>
{
    public void Configure(EntityTypeBuilder<PurchaseItem> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.BatchNumber).HasMaxLength(100);

        builder.Property(i => i.UnitCost).HasColumnType("decimal(18,4)");
        builder.Property(i => i.DiscountRate).HasColumnType("decimal(5,4)");
        builder.Property(i => i.DiscountAmount).HasColumnType("decimal(18,4)");
        builder.Property(i => i.TaxRate).HasColumnType("decimal(5,4)");
        builder.Property(i => i.TaxAmount).HasColumnType("decimal(18,4)");
        builder.Property(i => i.TotalCost).HasColumnType("decimal(18,4)");

        builder.HasOne(i => i.Purchase)
               .WithMany(p => p.Items)
               .HasForeignKey(i => i.PurchaseId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.Medicine)
               .WithMany(m => m.PurchaseItems)
               .HasForeignKey(i => i.MedicineId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
