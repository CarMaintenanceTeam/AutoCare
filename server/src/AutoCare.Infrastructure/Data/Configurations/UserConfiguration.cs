using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Domain.Common;
using AutoCare.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoCare.Infrastructure.Data.Configurations
{
    /// <summary>
    /// EF Core configuration for User entity
    /// </summary>
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Table name
            builder.ToTable("Users");

            // Primary Key
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id)
                .HasColumnName("UserId")
                .ValueGeneratedOnAdd();

            // Properties Configuration
            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(u => u.FullName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(u => u.PhoneNumber)
                .HasMaxLength(20);

            builder.Property(u => u.UserType)
                .IsRequired()
                .HasMaxLength(50)
                .HasConversion<string>(); // Store enum as string

            builder.Property(u => u.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Auditable properties
            builder.Property(u => u.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(u => u.UpdatedAt);

            builder.Property(u => u.CreatedBy);

            builder.Property(u => u.UpdatedBy);

            // Indexes
            builder.HasIndex(u => u.Email)
                .IsUnique()
                .HasDatabaseName("UK_Users_Email");

            builder.HasIndex(u => u.UserType)
                .HasDatabaseName("IDX_Users_UserType");

            builder.HasIndex(u => u.IsActive)
                .HasDatabaseName("IDX_Users_IsActive");

            // Check Constraints
            builder.ToTable(t => t.HasCheckConstraint(
                "CHK_Users_UserType",
                "[UserType] IN ('Customer', 'Employee', 'Admin')"));

            builder.ToTable(t => t.HasCheckConstraint(
                "CHK_Users_Email_Format",
                "[Email] LIKE '%_@__%.__%'"));

            // Relationships
            builder.HasOne(u => u.Customer)
                .WithOne(c => c.User)
                .HasForeignKey<Customer>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(u => u.Employee)
                .WithOne(e => e.User)
                .HasForeignKey<Employee>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.RefreshTokens)
                .WithOne(rt => rt.User)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ignore DomainEvents (not mapped to database)
            builder.Ignore(u => u.DomainEvents);
        }
    }
}