using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplySync.Constants.Enums;
using SupplySync.Models;

namespace SupplySync.Config.Configurations
{
    public class AuditConfiguration : IEntityTypeConfiguration<Audit>
    {
        public void Configure(EntityTypeBuilder<Audit> builder)
        {
            builder.HasKey(x => x.AuditID);
            builder.Property(x => x.AuditID)
                   .ValueGeneratedOnAdd();
            builder.Property(x => x.IsDeleted).HasDefaultValue(false);
            builder.Property(x => x.Status)
                    .HasConversion<string>()
                    .HasMaxLength(20)
                    .HasDefaultValue(AuditStatus.Planned);


            builder.Property(x => x.Scope)
                            .IsRequired()
                            .HasMaxLength(200);


            builder.Property(x => x.Date)
                            .HasColumnType("date");


            builder.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(x => x.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(x => x.ComplianceOfficer)
                   .WithMany()
                   .HasForeignKey(x => x.ComplianceOfficerID)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();
        }
    }

    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
			builder.HasKey(x => x.AuditID);
			builder.Property(x => x.AuditID)
				   .ValueGeneratedOnAdd();

			builder.Property(x => x.Action).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Resource).IsRequired().HasMaxLength(200);
			builder.Property(x => x.IsDeleted).HasDefaultValue(false);
			builder.Property(x => x.Timestamp).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(x => x.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            // If you intend AuditLog to relate to Audit, add this:
            // builder.HasOne<Audit>()
            //        .WithMany()
            //        .HasForeignKey(x => x.AuditID)
            //        .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class ComplianceRecordConfiguration : IEntityTypeConfiguration<ComplianceRecord>
    {
        public void Configure(EntityTypeBuilder<ComplianceRecord> builder)
        {
            builder.HasKey(x => x.ComplianceID);
            builder.Property(x => x.ComplianceID)
                   .ValueGeneratedOnAdd();
            builder.Property(x => x.IsDeleted).HasDefaultValue(false);

            builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(30);
            builder.Property(x => x.Result).HasConversion<string>().HasMaxLength(20);

            builder.Property(x => x.Date)
                   .HasConversion(
                        v => v.ToDateTime(TimeOnly.MinValue),
                        v => DateOnly.FromDateTime(v))
                   .HasColumnType("date");

            builder.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(x => x.Contract)
                   .WithMany()
                   .HasForeignKey(x => x.ContractID)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();
        }
    }
}