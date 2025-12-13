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
    /// EF Core configuration for BookingStatusHistory entity
    /// Audit trail - immutable records
    /// </summary>
    public class BookingStatusHistoryConfiguration : IEntityTypeConfiguration<BookingStatusHistory>
    {
        public void Configure(EntityTypeBuilder<BookingStatusHistory> builder)
        {
            // Table name
            builder.ToTable("BookingStatusHistory");

            // Primary Key
            builder.HasKey(h => h.Id);
            builder.Property(h => h.Id)
                .HasColumnName("HistoryId")
                .ValueGeneratedOnAdd();

            // Properties Configuration
            builder.Property(h => h.BookingId)
                .IsRequired();

            builder.Property(h => h.OldStatus)
                .HasMaxLength(50);

            builder.Property(h => h.NewStatus)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(h => h.ChangedBy)
                .IsRequired();

            builder.Property(h => h.ChangedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(h => h.Notes)
                .HasMaxLength(500);

            // Performance Indexes
            builder.HasIndex(h => h.BookingId)
                .HasDatabaseName("IDX_BookingStatusHistory_BookingId");

            builder.HasIndex(h => h.ChangedBy)
                .HasDatabaseName("IDX_BookingStatusHistory_ChangedBy");

            builder.HasIndex(h => h.ChangedAt)
                .HasDatabaseName("IDX_BookingStatusHistory_ChangedAt");

            // Check Constraints
            builder.ToTable(t => t.HasCheckConstraint(
                "CHK_BookingStatusHistory_Status",
                "[NewStatus] IN ('Pending', 'Confirmed', 'InProgress', 'Completed', 'Cancelled') " +
                "AND ([OldStatus] IS NULL OR [OldStatus] IN ('Pending', 'Confirmed', 'InProgress', 'Completed', 'Cancelled'))"));

            // Relationships
            builder.HasOne(h => h.Booking)
                .WithMany(b => b.StatusHistory)
                .HasForeignKey(h => h.BookingId)
                .OnDelete(DeleteBehavior.Cascade) // Delete history if booking deleted
                .HasConstraintName("FK_BookingStatusHistory_Bookings");

            builder.HasOne(h => h.ChangedByUser)
                .WithMany()
                .HasForeignKey(h => h.ChangedBy)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_BookingStatusHistory_Users");

            // Ignore DomainEvents
            builder.Ignore(h => h.DomainEvents);
        }
    }
}