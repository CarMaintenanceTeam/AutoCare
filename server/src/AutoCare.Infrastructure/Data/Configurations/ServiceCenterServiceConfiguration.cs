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
    /// EF Core configuration for ServiceCenterService entity (Many-to-Many junction)
    /// </summary>
    public class ServiceCenterServiceConfiguration : IEntityTypeConfiguration<ServiceCenterService>
    {
        public void Configure(EntityTypeBuilder<ServiceCenterService> builder)
        {
            // Table name
            builder.ToTable("ServiceCenterServices");

            // Primary Key
            builder.HasKey(scs => scs.Id);
            builder.Property(scs => scs.Id)
                .HasColumnName("ServiceCenterServiceId")
                .ValueGeneratedOnAdd();

            // Properties Configuration
            builder.Property(scs => scs.ServiceCenterId)
                .IsRequired();

            builder.Property(scs => scs.ServiceId)
                .IsRequired();

            builder.Property(scs => scs.CustomPrice)
                .HasPrecision(10, 2);

            builder.Property(scs => scs.IsAvailable)
                .IsRequired()
                .HasDefaultValue(true);

            // Auditable properties
            builder.Property(scs => scs.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(scs => scs.UpdatedAt);

            builder.Property(scs => scs.CreatedBy);

            builder.Property(scs => scs.UpdatedBy);

            // Unique Constraints - Prevent duplicate service assignments
            builder.HasIndex(scs => new { scs.ServiceCenterId, scs.ServiceId })
                .IsUnique()
                .HasDatabaseName("UK_ServiceCenterServices_Center_Service");

            // Performance Indexes
            builder.HasIndex(scs => scs.ServiceCenterId)
                .HasDatabaseName("IDX_ServiceCenterServices_ServiceCenterId");

            builder.HasIndex(scs => scs.ServiceId)
                .HasDatabaseName("IDX_ServiceCenterServices_ServiceId");

            builder.HasIndex(scs => new { scs.ServiceCenterId, scs.ServiceId })
                .HasFilter("[IsAvailable] = 1")
                .HasDatabaseName("IDX_ServiceCenterServices_Available");

            // Check Constraints
            builder.ToTable(t => t.HasCheckConstraint(
                "CHK_ServiceCenterServices_CustomPrice",
                "[CustomPrice] IS NULL OR [CustomPrice] >= 0"));

            // Relationships already defined in ServiceCenter and Service configurations

            // Ignore DomainEvents
            builder.Ignore(scs => scs.DomainEvents);
        }
    }
}