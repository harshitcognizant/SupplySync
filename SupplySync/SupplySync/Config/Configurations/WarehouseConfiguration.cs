using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplySync.Models;

namespace SupplySync.Config.Configurations
{
    public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
    {

        public void Configure(EntityTypeBuilder<Warehouse> builder)
        {
			builder.HasKey(x => x.WarehouseID);
			builder.Property(x => x.WarehouseID)
				   .ValueGeneratedOnAdd();

			builder.Property(x => x.IsDeleted).HasDefaultValue(false);
			builder.Property(x => x.Location).IsRequired();
            builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
            builder.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        }
    }

    public class ReceiptConfiguration : IEntityTypeConfiguration<Receipt>
    {
        public void Configure(EntityTypeBuilder<Receipt> builder)
        {
			builder.HasKey(x => x.ReceiptID);
			builder.Property(x => x.ReceiptID)
				   .ValueGeneratedOnAdd();
			builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
            builder.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
			builder.Property(x => x.IsDeleted).HasDefaultValue(false);

			// DateOnly -> date
			builder.Property(x => x.Date)
                   .HasConversion(
                        v => v.ToDateTime(TimeOnly.MinValue),
                        v => DateOnly.FromDateTime(v))
                   .HasColumnType("date");

            builder.HasOne(x => x.Warehouse)
                   .WithMany(x => x.Receipts)
                   .HasForeignKey(x => x.WarehouseID)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();

            builder.HasOne(x => x.Delivery)
                   .WithMany()
                   .HasForeignKey(x => x.DeliveryID)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();
        }
    }
}