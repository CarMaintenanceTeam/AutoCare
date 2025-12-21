using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Domain.Entities;
using AutoCare.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoCare.Infrastructure.Data.Configurations
{
    public class ServiceConfiguration : IEntityTypeConfiguration<Service>
    {
        /// <summary>
        /// EF Core configuration for Service entity
        /// </summary>
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            // Table name
            builder.ToTable("Services");

            // Primary Key
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id)
                .HasColumnName("ServiceId")
                .ValueGeneratedOnAdd();

            // Properties Configuration
            builder.Property(s => s.NameEn)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(s => s.NameAr)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(s => s.DescriptionEn)
                .HasMaxLength(1000);

            builder.Property(s => s.DescriptionAr)
                .HasMaxLength(1000);

            builder.Property(s => s.BasePrice)
                .IsRequired()
                .HasPrecision(10, 2);

            builder.Property(s => s.EstimatedDurationMinutes)
                .IsRequired();

            builder.Property(s => s.ServiceType)
    .IsRequired()
    .HasConversion<string>()
    .HasDefaultValue(ServiceType.Maintenance); // Store enum as string

            builder.Property(s => s.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Auditable properties
            builder.Property(s => s.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(s => s.UpdatedAt);

            builder.Property(s => s.CreatedBy);

            builder.Property(s => s.UpdatedBy);

            // Performance Indexes
            builder.HasIndex(s => s.IsActive)
                .HasDatabaseName("IDX_Services_IsActive");

            builder.HasIndex(s => s.ServiceType)
                .HasFilter("[IsActive] = 1")
                .HasDatabaseName("IDX_Services_ServiceType");

            // Check Constraints
            builder.ToTable(t => t.HasCheckConstraint(
                "CHK_Services_BasePrice",
                "[BasePrice] >= 0"));

            builder.ToTable(t => t.HasCheckConstraint(
                "CHK_Services_EstimatedDuration",
                "[EstimatedDurationMinutes] > 0"));

            builder.ToTable(t => t.HasCheckConstraint(
                "CHK_Services_ServiceType",
                "[ServiceType] IN ('Maintenance', 'SpareParts')"));

            // Relationships
            builder.HasMany(s => s.Bookings)
                .WithOne(b => b.Service)
                .HasForeignKey(b => b.ServiceId)
                .OnDelete(DeleteBehavior.Restrict); // Preserve booking history

            builder.HasMany(s => s.ServiceCenterServices)
                .WithOne(scs => scs.Service)
                .HasForeignKey(scs => scs.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ignore DomainEvents
            builder.Ignore(s => s.DomainEvents);
        }
    }
}