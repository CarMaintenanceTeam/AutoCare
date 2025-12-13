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
    /// EF Core configuration for ServiceCenter entity
    /// </summary>
    public class ServiceCenterConfiguration : IEntityTypeConfiguration<ServiceCenter>
    {
        public void Configure(EntityTypeBuilder<ServiceCenter> builder)
        {
            // Table name
            builder.ToTable("ServiceCenters");

            // Primary Key
            builder.HasKey(sc => sc.Id);
            builder.Property(sc => sc.Id)
                .HasColumnName("ServiceCenterId")
                .ValueGeneratedOnAdd();

            // Properties Configuration
            builder.Property(sc => sc.NameEn)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(sc => sc.NameAr)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(sc => sc.AddressEn)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(sc => sc.AddressAr)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(sc => sc.City)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(sc => sc.Latitude)
                .IsRequired()
                .HasPrecision(10, 8); // decimal(10,8) for GPS accuracy

            builder.Property(sc => sc.Longitude)
                .IsRequired()
                .HasPrecision(11, 8); // decimal(11,8) for GPS accuracy

            builder.Property(sc => sc.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(sc => sc.Email)
                .HasMaxLength(255);

            builder.Property(sc => sc.WorkingHours)
                .HasMaxLength(200);

            builder.Property(sc => sc.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Auditable properties
            builder.Property(sc => sc.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(sc => sc.UpdatedAt);

            builder.Property(sc => sc.CreatedBy);

            builder.Property(sc => sc.UpdatedBy);

            // Performance Indexes
            builder.HasIndex(sc => sc.IsActive)
                .HasDatabaseName("IDX_ServiceCenters_IsActive");

            builder.HasIndex(sc => sc.City)
                .HasFilter("[IsActive] = 1")
                .HasDatabaseName("IDX_ServiceCenters_City");

            builder.HasIndex(sc => new { sc.Latitude, sc.Longitude })
                .HasFilter("[IsActive] = 1")
                .HasDatabaseName("IDX_ServiceCenters_Location");

            // Check Constraints
            builder.ToTable(t => t.HasCheckConstraint(
                "CHK_ServiceCenters_Latitude",
                "[Latitude] BETWEEN -90 AND 90"));

            builder.ToTable(t => t.HasCheckConstraint(
                "CHK_ServiceCenters_Longitude",
                "[Longitude] BETWEEN -180 AND 180"));

            builder.ToTable(t => t.HasCheckConstraint(
                "CHK_ServiceCenters_PhoneNumber",
                "LEN([PhoneNumber]) >= 10"));

            builder.ToTable(t => t.HasCheckConstraint(
                "CHK_ServiceCenters_Email",
                "[Email] IS NULL OR [Email] LIKE '%_@__%.__%'"));

            // Relationships
            builder.HasMany(sc => sc.Employees)
                .WithOne(e => e.ServiceCenter)
                .HasForeignKey(e => e.ServiceCenterId)
                .OnDelete(DeleteBehavior.Restrict); // Cannot delete if has employees

            builder.HasMany(sc => sc.Bookings)
                .WithOne(b => b.ServiceCenter)
                .HasForeignKey(b => b.ServiceCenterId)
                .OnDelete(DeleteBehavior.Restrict); // Preserve booking history

            builder.HasMany(sc => sc.ServiceCenterServices)
                .WithOne(scs => scs.ServiceCenter)
                .HasForeignKey(scs => scs.ServiceCenterId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ignore DomainEvents
            builder.Ignore(sc => sc.DomainEvents);
        }
    }
}