using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplySync.Constants.Enums;
using SupplySync.Models;

namespace SupplySync.Config.Configurations
{
    public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
    {
        public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
        {
            builder.HasKey(x => x.POID);
            builder.Property(x => x.POID)
                   .ValueGeneratedOnAdd();

            builder.Property(x => x.Item).IsRequired();

            builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20).HasDefaultValue(POStatus.Draft);
            builder.Property(x => x.IsDeleted).HasDefaultValue(false);
            builder.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(x => x.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(x => x.Contract)
                   .WithMany()
                   .HasForeignKey(x => x.ContractID)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();

            // Inverse side handled by Delivery.WithMany(x => x.Deliveries)
        }
    }

    public class DeliveryConfiguration : IEntityTypeConfiguration<Delivery>
    {
        public void Configure(EntityTypeBuilder<Delivery> builder)
        {
            builder.HasKey(x => x.DeliveryID);
            builder.Property(x => x.DeliveryID)
                   .ValueGeneratedOnAdd();
            builder.Property(x => x.Item).IsRequired();

            builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20).HasDefaultValue(DeliveryStatus.Shipped);
            builder.Property(x => x.IsDeleted).HasDefaultValue(false);
            builder.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(x => x.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(x => x.PurchaseOrder)
                   .WithMany(x => x.Deliveries)
                   .HasForeignKey(x => x.POID)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();

            builder.HasOne(x => x.Vendor)
                   .WithMany()
                   .HasForeignKey(x => x.VendorID)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();
        }
    }

    public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
    {
        public void Configure(EntityTypeBuilder<Inventory> builder)
        {
			builder.HasKey(x => x.InventoryID);
			builder.Property(x => x.InventoryID)
				   .ValueGeneratedOnAdd();
			builder.Property(x => x.Item).IsRequired();
			builder.Property(x => x.IsDeleted).HasDefaultValue(false);
			builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
            builder.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

            // DateOnly -> date
            builder.Property(x => x.DateAdded)
                   .HasConversion(
                        v => v.ToDateTime(TimeOnly.MinValue),
                        v => DateOnly.FromDateTime(v))
                   .HasColumnType("date");

            builder.HasOne(x => x.Warehouse)
                   .WithMany(x => x.Inventories)
                   .HasForeignKey(x => x.WarehouseID)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();
        }
    }
}