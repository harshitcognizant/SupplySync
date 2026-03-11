using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplySync.Models;

namespace SupplySync.Config.Configurations
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.HasKey(x => x.InvoiceId);
            builder.Property(x => x.InvoiceId)
                   .ValueGeneratedOnAdd();

            builder.Property(x => x.IsDeleted).HasDefaultValue(false);
            builder.Property(x => x.Amount).HasPrecision(18, 2);
            builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
            builder.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(x => x.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(x => x.PurchaseOrder)
                   .WithMany()
                   .HasForeignKey(x => x.POID)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();

            builder.HasOne(x => x.Vendor)
                   .WithMany()
                   .HasForeignKey(x => x.VendorId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();
        }
    }

    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(x => x.PaymentId);
            builder.Property(x => x.PaymentId)
                   .ValueGeneratedOnAdd();

            builder.Property(x => x.IsDeleted).HasDefaultValue(false);
            builder.Property(x => x.Amount).HasPrecision(18, 2);
            builder.Property(x => x.Method).HasConversion<string>().HasMaxLength(20);
            builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
            builder.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(x => x.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(x => x.Invoice)
                   .WithMany()
                   .HasForeignKey(x => x.InvoiceId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();
        }
    }
}