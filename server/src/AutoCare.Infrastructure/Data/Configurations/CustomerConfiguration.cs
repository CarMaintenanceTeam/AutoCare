using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoCare.Infrastructure.Data.Configurations
{
    /// <summary>
    /// EF Core configuration for Customer entity
    /// </summary>
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {

            // Table name
            builder.ToTable("Customers");

            // Primary Key
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id)
                .HasColumnName("CustomerId")
                .ValueGeneratedOnAdd();

            // Properties Configuration
            builder.Property(c => c.UserId)
                .IsRequired();

            builder.Property(c => c.Address)
                .HasMaxLength(500);

            builder.Property(c => c.City)
                .HasMaxLength(100);

            builder.Property(c => c.NewsletterSubscribed)
                .IsRequired()
                .HasDefaultValue(false);

            // Auditable properties
            builder.Property(c => c.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(c => c.UpdatedAt);

            builder.Property(c => c.CreatedBy);

            builder.Property(c => c.UpdatedBy);

            // Indexes
            builder.HasIndex(c => c.UserId)
                .IsUnique()
                .HasDatabaseName("UK_Customers_UserId");

            builder.HasIndex(c => c.City)
                .HasDatabaseName("IDX_Customers_City");

            // Relationships
            builder.HasMany(c => c.Vehicles)
                .WithOne(v => v.Customer)
                .HasForeignKey(v => v.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Bookings)
                .WithOne(b => b.Customer)
                .HasForeignKey(b => b.CustomerId)
                .OnDelete(DeleteBehavior.Restrict); // Preserve booking history

            // Ignore DomainEvents
            builder.Ignore(c => c.DomainEvents);

        }
    }
}