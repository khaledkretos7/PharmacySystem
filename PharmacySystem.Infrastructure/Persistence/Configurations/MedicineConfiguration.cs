using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacySystem.Domain.Entities;

namespace PharmacySystem.Infrastructure.Persistence.Configurations;

public class MedicineConfiguration : IEntityTypeConfiguration<Medicine>
{
    public void Configure(EntityTypeBuilder<Medicine> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Name).IsRequired().HasMaxLength(200);
        builder.Property(m => m.GenericName).IsRequired().HasMaxLength(200);
        builder.Property(m => m.Barcode).HasMaxLength(100);
        builder.Property(m => m.Description).HasMaxLength(1000);
        builder.Property(m => m.Manufacturer).HasMaxLength(200);
        builder.Property(m => m.Strength).HasMaxLength(50);
        builder.Property(m => m.Unit).IsRequired().HasMaxLength(50);
        builder.Property(m => m.StorageConditions).HasMaxLength(500);

        builder.Property(m => m.CostPrice).HasColumnType("decimal(18,4)");
        builder.Property(m => m.SellingPrice).HasColumnType("decimal(18,4)");

        // Unique filtered index — allows multiple medicines without barcode
        builder.HasIndex(m => m.Barcode)
               .IsUnique()
               .HasFilter("[Barcode] IS NOT NULL AND [IsDeleted] = 0");
        builder.HasIndex(m => m.Name);
        builder.HasIndex(m => m.CategoryId);

        builder.HasOne(m => m.Category)
               .WithMany(c => c.Medicines)
               .HasForeignKey(m => m.CategoryId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Supplier)
               .WithMany(s => s.Medicines)
               .HasForeignKey(m => m.SupplierId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
