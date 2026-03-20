using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplySync.Constants.Enums;
using SupplySync.Models;

namespace SupplySync.Config.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.UserID);
            builder.Property(x => x.UserID)
                   .ValueGeneratedOnAdd();

			builder.HasIndex(u => u.Email).IsUnique();

			builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
            builder.Property(x => x.Password).IsRequired().HasMaxLength(255);

            // Map Email as a shadow property because the model's Email is private
            builder.Property<string>("Email").IsRequired().HasMaxLength(100);
			builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
            builder.Property(x => x.IsDeleted).HasDefaultValue(false);
            builder.HasQueryFilter(ur => !ur.IsDeleted);
			builder.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(x => x.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
        }
    }

    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(x => x.RoleID);
            builder.Property(x => x.RoleID)
                   .ValueGeneratedOnAdd();


			builder.HasData(
					Enum.GetValues(typeof(RoleType))
						.Cast<RoleType>()
						.Select(rt => new Role
						{
							RoleID = (int)rt,           // stable keys aligned to enum numeric value
							RoleType = rt,
							IsDeleted = false
						})
				);

			builder.HasQueryFilter(ur => !ur.IsDeleted);
			builder.Property(x => x.RoleType).HasConversion<string>().HasMaxLength(30);
			builder.Property(x => x.IsDeleted).HasDefaultValue(false);
			builder.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(x => x.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
        }
    }

    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
			// Use the ID property as PK (to match your model)
			builder.HasKey(x => x.UserRoleID);
			builder.Property(x => x.UserRoleID)
				   .ValueGeneratedOnAdd();
            builder.
                HasIndex(ur => new { ur.UserID, ur.RoleID })
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");


			// Ensure uniqueness of (UserID, RoleID)
			builder.HasIndex(ur => new { ur.UserID, ur.RoleID }).IsUnique();
            builder.Property(x => x.IsDeleted).HasDefaultValue(false);
            builder.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.Property(x => x.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

			builder.HasOne(x => x.User)
                   .WithMany(x => x.UserRoles)
                   .HasForeignKey(x => x.UserID)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();

            builder.HasOne(x => x.Role)
                   .WithMany(x => x.UserRoles)
                   .HasForeignKey(x => x.RoleID)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();

            // Removed invalid filter on IsDeleted (property doesn't exist)
        }
    }
}