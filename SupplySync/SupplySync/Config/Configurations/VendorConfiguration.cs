using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplySync.Constants.Enums;
using SupplySync.Models;

namespace SupplySync.Config.Configurations
{
    public class VendorConfiguration : IEntityTypeConfiguration<Vendor>
    {
        public void Configure(EntityTypeBuilder<Vendor> builder)
        {
			builder.HasKey(x => x.VendorID);
			builder.Property(x => x.VendorID)
				   .ValueGeneratedOnAdd();
			builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
            builder.Property(x => x.ContactInfo).IsRequired().HasMaxLength(255);
			builder.Property(x => x.IsDeleted).HasDefaultValue(false);
			builder.Property(x => x.Category).HasConversion<string>().HasMaxLength(30);
            builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20).HasDefaultValue(VendorStatus.Pending);
            builder.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(x => x.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
        }
    }

    public class ContractConfiguration : IEntityTypeConfiguration<Contract>
    {
        public void Configure(EntityTypeBuilder<Contract> builder)
        {
			builder.HasKey(x => x.ContractID);
			builder.Property(x => x.ContractID)
				   .ValueGeneratedOnAdd();
			builder.Property(x => x.Value).HasPrecision(18, 2);
            builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20).HasDefaultValue(ContractStatus.Draft);
            builder.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(x => x.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
			builder.Property(x => x.IsDeleted).HasDefaultValue(false);

			builder.HasOne(x => x.Vendor)
                   .WithMany()
                   .HasForeignKey(x => x.VendorID)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();
        }
    }
}