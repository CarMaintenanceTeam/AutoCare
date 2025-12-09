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
    /// EF Core configuration for Booking entity
    /// Most complex configuration due to business rules
    /// </summary>
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            // Table name
            builder.ToTable("Bookings");

            // Primary Key
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id)
                .HasColumnName("BookingId")
                .ValueGeneratedOnAdd();

            // Properties Configuration
            builder.Property(b => b.BookingNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(b => b.CustomerId)
                .IsRequired();

            builder.Property(b => b.VehicleId)
                .IsRequired();

            builder.Property(b => b.ServiceCenterId)
                .IsRequired();

            builder.Property(b => b.ServiceId)
                .IsRequired();

            builder.Property(b => b.BookingDate)
                .IsRequired()
                .HasColumnType("date");

            builder.Property(b => b.BookingTime)
                .IsRequired()
                .HasColumnType("time");

            builder.Property(b => b.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasConversion<string>() // Store enum as string
                .HasDefaultValue("Pending");

            builder.Property(b => b.CustomerNotes)
                .HasMaxLength(1000);

            builder.Property(b => b.StaffNotes)
                .HasMaxLength(1000);

            builder.Property(b => b.ConfirmedAt)
                .HasColumnType("datetime");

            builder.Property(b => b.ConfirmedBy);

            builder.Property(b => b.CompletedAt)
                .HasColumnType("datetime");

            builder.Property(b => b.CancelledAt)
                .HasColumnType("datetime");

            builder.Property(b => b.CancellationReason)
                .HasMaxLength(500);

            // Auditable properties
            builder.Property(b => b.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(b => b.UpdatedAt);

            builder.Property(b => b.CreatedBy);

            builder.Property(b => b.UpdatedBy);

            // Unique Constraints
            builder.HasIndex(b => b.BookingNumber)
                .IsUnique()
                .HasDatabaseName("UK_Bookings_BookingNumber");

            // CRITICAL: Prevent double booking at same time/center
            builder.HasIndex(b => new { b.ServiceCenterId, b.BookingDate, b.BookingTime })
                .IsUnique()
                .HasFilter("[Status] NOT IN ('Cancelled', 'Completed')")
                .HasDatabaseName("IDX_Bookings_NoDoubleBooking");

            // Performance Indexes
            builder.HasIndex(b => b.CustomerId)
                .HasDatabaseName("IDX_Bookings_CustomerId");

            builder.HasIndex(b => b.ServiceCenterId)
                .HasDatabaseName("IDX_Bookings_ServiceCenterId");

            builder.HasIndex(b => b.Status)
                .HasDatabaseName("IDX_Bookings_Status");

            builder.HasIndex(b => b.BookingDate)
                .HasDatabaseName("IDX_Bookings_BookingDate");

            // Check Constraints
            builder.ToTable(t => t.HasCheckConstraint(
                "CHK_Bookings_Status",
                "[Status] IN ('Pending', 'Confirmed', 'InProgress', 'Completed', 'Cancelled')"));

            builder.ToTable(t => t.HasCheckConstraint(
                "CHK_Bookings_CancellationReason",
                "([Status] = 'Cancelled' AND [CancellationReason] IS NOT NULL) OR ([Status] != 'Cancelled')"));

            // Relationships
            builder.HasOne(b => b.Customer)
                .WithMany(c => c.Bookings)
                .HasForeignKey(b => b.CustomerId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Bookings_Customers");

            builder.HasOne(b => b.Vehicle)
                .WithMany(v => v.Bookings)
                .HasForeignKey(b => b.VehicleId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Bookings_Vehicles");

            builder.HasOne(b => b.ServiceCenter)
                .WithMany(sc => sc.Bookings)
                .HasForeignKey(b => b.ServiceCenterId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Bookings_ServiceCenters");

            builder.HasOne(b => b.Service)
                .WithMany(s => s.Bookings)
                .HasForeignKey(b => b.ServiceId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Bookings_Services");

            builder.HasOne(b => b.ConfirmedByUser)
                .WithMany()
                .HasForeignKey(b => b.ConfirmedBy)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Bookings_ConfirmedBy")
                .IsRequired(false);

            builder.HasMany(b => b.StatusHistory)
                .WithOne(h => h.Booking)
                .HasForeignKey(h => h.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ignore DomainEvents
            builder.Ignore(b => b.DomainEvents);
        }
    }
}