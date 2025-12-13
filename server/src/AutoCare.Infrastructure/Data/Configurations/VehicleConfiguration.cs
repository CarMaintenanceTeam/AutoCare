using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoCare.Infrastructure.Data.Configurations
{
    /// <summary>
    /// EF Core configuration for Vehicle entity
    /// </summary>
    public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Vehicle> builder)
        {
            // Table name
            builder.ToTable("Vehicles");

            // Primary Key
            builder.HasKey(v => v.Id);
            builder.Property(v => v.Id)
                .HasColumnName("VehicleId")
                .ValueGeneratedOnAdd();

            // Properties Configuration
            builder.Property(v => v.CustomerId)
                .IsRequired();

            builder.Property(v => v.Brand)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(v => v.Model)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(v => v.Year)
                .IsRequired();

            builder.Property(v => v.PlateNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(v => v.VIN)
                .HasMaxLength(17);

            builder.Property(v => v.Color)
                .HasMaxLength(50);

            // Auditable properties
            builder.Property(v => v.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(v => v.UpdatedAt);

            builder.Property(v => v.CreatedBy);

            builder.Property(v => v.UpdatedBy);

            // Unique Constraints
            builder.HasIndex(v => v.PlateNumber)
                .IsUnique()
                .HasDatabaseName("UK_Vehicles_PlateNumber");

            // Performance Indexes
            builder.HasIndex(v => v.CustomerId)
                .HasDatabaseName("IDX_Vehicles_CustomerId");

            builder.HasIndex(v => v.VIN)
                .HasFilter("[VIN] IS NOT NULL")
                .HasDatabaseName("IDX_Vehicles_VIN");

            // Check Constraints
            builder.ToTable(t => t.HasCheckConstraint(
                "CHK_Vehicles_Year",
                "[Year] BETWEEN 1900 AND YEAR(GETUTCDATE()) + 1"));

            builder.ToTable(t => t.HasCheckConstraint(
                "CHK_Vehicles_VIN",
                "[VIN] IS NULL OR LEN([VIN]) = 17"));

            // Relationships
            builder.HasOne(v => v.Customer)
                .WithMany(c => c.Vehicles)
                .HasForeignKey(v => v.CustomerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Vehicles_Customers");

            builder.HasMany(v => v.Bookings)
                .WithOne(b => b.Vehicle)
                .HasForeignKey(b => b.VehicleId)
                .OnDelete(DeleteBehavior.Restrict); // Preserve booking history

            // Ignore DomainEvents
            builder.Ignore(v => v.DomainEvents);
        }
    }
}