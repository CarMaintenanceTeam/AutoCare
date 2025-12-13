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
    /// EF Core configuration for RefreshToken entity
    /// JWT token management
    /// </summary>
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            // Table name
            builder.ToTable("RefreshTokens");

            // Primary Key
            builder.HasKey(rt => rt.Id);
            builder.Property(rt => rt.Id)
                .HasColumnName("RefreshTokenId")
                .ValueGeneratedOnAdd();

            // Properties Configuration
            builder.Property(rt => rt.UserId)
                .IsRequired();

            builder.Property(rt => rt.Token)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(rt => rt.ExpiresAt)
                .IsRequired();

            builder.Property(rt => rt.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(rt => rt.RevokedAt);

            builder.Property(rt => rt.IsUsed)
                .IsRequired()
                .HasDefaultValue(false);

            // Unique Constraints
            builder.HasIndex(rt => rt.Token)
                .IsUnique()
                .HasDatabaseName("UK_RefreshTokens_Token");

            // Performance Indexes (covering index for token validation)
            builder.HasIndex(rt => rt.Token)
                .HasDatabaseName("IDX_RefreshTokens_Token");

            builder.HasIndex(rt => rt.UserId)
                .HasDatabaseName("IDX_RefreshTokens_UserId");

            builder.HasIndex(rt => rt.ExpiresAt)
                .HasFilter("[IsUsed] = 0 AND [RevokedAt] IS NULL")
                .HasDatabaseName("IDX_RefreshTokens_ExpiresAt");

            // Check Constraints
            builder.ToTable(t => t.HasCheckConstraint(
                "CHK_RefreshTokens_ExpiresAt",
                "[ExpiresAt] > [CreatedAt]"));

            builder.ToTable(t => t.HasCheckConstraint(
                "CHK_RefreshTokens_Revoked",
                "([RevokedAt] IS NOT NULL AND [IsUsed] = 1) OR ([RevokedAt] IS NULL)"));

            // Relationships already defined in User configuration

            // Ignore DomainEvents
            builder.Ignore(rt => rt.DomainEvents);
        }
    }
}