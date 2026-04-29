using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacySystem.Domain.Entities;

namespace PharmacySystem.Infrastructure.Persistence.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.InvoiceNumber).IsRequired().HasMaxLength(30);
        builder.Property(i => i.PrescriptionReference).HasMaxLength(100);
        builder.Property(i => i.Notes).HasMaxLength(1000);
        builder.Property(i => i.UserId).IsRequired().HasMaxLength(450);

        builder.Property(i => i.SubTotal).HasColumnType("decimal(18,4)");
        builder.Property(i => i.DiscountAmount).HasColumnType("decimal(18,4)");
        builder.Property(i => i.TaxAmount).HasColumnType("decimal(18,4)");
        builder.Property(i => i.TotalAmount).HasColumnType("decimal(18,4)");
        builder.Property(i => i.AmountPaid).HasColumnType("decimal(18,4)");
        builder.Property(i => i.ChangeAmount).HasColumnType("decimal(18,4)");

        builder.HasIndex(i => i.InvoiceNumber).IsUnique();
        builder.HasIndex(i => i.InvoiceDate);
        builder.HasIndex(i => i.CustomerId);

        builder.HasOne(i => i.Customer)
               .WithMany(c => c.Invoices)
               .HasForeignKey(i => i.CustomerId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}

public class InvoiceItemConfiguration : IEntityTypeConfiguration<InvoiceItem>
{
    public void Configure(EntityTypeBuilder<InvoiceItem> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.BatchNumber).HasMaxLength(100);

        builder.Property(i => i.UnitPrice).HasColumnType("decimal(18,4)");
        builder.Property(i => i.DiscountRate).HasColumnType("decimal(5,4)");
        builder.Property(i => i.DiscountAmount).HasColumnType("decimal(18,4)");
        builder.Property(i => i.TaxRate).HasColumnType("decimal(5,4)");
        builder.Property(i => i.TaxAmount).HasColumnType("decimal(18,4)");
        builder.Property(i => i.TotalAmount).HasColumnType("decimal(18,4)");

        builder.HasOne(i => i.Invoice)
               .WithMany(inv => inv.Items)
               .HasForeignKey(i => i.InvoiceId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.Medicine)
               .WithMany(m => m.InvoiceItems)
               .HasForeignKey(i => i.MedicineId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

public class InvoiceReturnConfiguration : IEntityTypeConfiguration<InvoiceReturn>
{
    public void Configure(EntityTypeBuilder<InvoiceReturn> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.ReturnNumber).IsRequired().HasMaxLength(30);
        builder.Property(r => r.Notes).HasMaxLength(1000);
        builder.Property(r => r.ProcessedByUserId).IsRequired().HasMaxLength(450);

        builder.Property(r => r.UnitPrice).HasColumnType("decimal(18,4)");
        builder.Property(r => r.RefundAmount).HasColumnType("decimal(18,4)");

        builder.HasIndex(r => r.ReturnNumber).IsUnique();

        builder.HasOne(r => r.Invoice)
               .WithMany(i => i.Returns)
               .HasForeignKey(r => r.InvoiceId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Medicine)
               .WithMany(m => m.InvoiceReturns)
               .HasForeignKey(r => r.MedicineId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
