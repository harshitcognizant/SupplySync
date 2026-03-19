using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplySync.Constants.Enums;
using SupplySync.Models;

namespace SupplySync.Config.Configurations
{
    public class VendorDocumentConfiguration : IEntityTypeConfiguration<VendorDocument>
    {
        public void Configure(EntityTypeBuilder<VendorDocument> builder)
        {
			builder.HasKey(x => x.DocumentID);
			builder.Property(x => x.DocumentID)
				   .ValueGeneratedOnAdd();
			builder.Property(x => x.DocType).HasConversion<string>().HasMaxLength(30);
            builder.Property(x => x.VerificationStatus).HasConversion<string>().HasMaxLength(20).HasDefaultValue(VendorDocumentVerificationStatus.Pending);
			builder.Property(x => x.IsDeleted).HasDefaultValue(false);
			builder.Property(x => x.FileURI).IsRequired();

            builder.Property(x => x.UploadedDate).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(x => x.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(x => x.Vendor)
                   .WithMany()
                   .HasForeignKey(x => x.VendorID)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();
        }
    }

    public class ContractTermConfiguration : IEntityTypeConfiguration<ContractTerm>
    {
        public void Configure(EntityTypeBuilder<ContractTerm> builder)
        {
			builder.HasKey(x => x.TermID);
			builder.Property(x => x.TermID)
				   .ValueGeneratedOnAdd();
			builder.Property(x => x.Description).IsRequired();
			builder.Property(x => x.IsDeleted).HasDefaultValue(false);
			builder.Property(x => x.ComplianceFlag).HasDefaultValue(false);
            builder.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(x => x.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(x => x.Contract)
                   .WithMany()
                   .HasForeignKey(x => x.ContractID)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();
        }
    }
}