using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplySync.Models;

namespace SupplySync.Config.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.Property(x => x.Message).IsRequired();

            builder.Property(x => x.Category).HasConversion<string>().HasMaxLength(20);
            builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);

            builder.Property(x => x.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(x => x.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(x => x.User)
                   .WithMany()
                   .HasForeignKey(x => x.UserID)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();

            builder.HasOne(x => x.Contract)
                   .WithMany()
                   .HasForeignKey(x => x.ContractID)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class ReportConfiguration : IEntityTypeConfiguration<Report>
    {
        public void Configure(EntityTypeBuilder<Report> builder)
        {
            builder.HasKey(x => x.ReportID);
            builder.Property(x => x.ReportID)
                   .ValueGeneratedOnAdd();
            builder.Property(x => x.IsDeleted).HasDefaultValue(false);

            builder.Property(x => x.Scope).HasConversion<string>().HasMaxLength(20);
            builder.Property(x => x.Metrics).IsRequired();

            builder.Property(x => x.GeneratedDate).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        }
    }
}