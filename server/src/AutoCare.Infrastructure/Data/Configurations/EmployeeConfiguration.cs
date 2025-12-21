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
    /// <summary>
    /// EF Configuration for Employee Entity
    /// </summary>
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            // Table Name
            builder.ToTable("Employees");

            #region  Primary Key
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasColumnName("EmployeeId").ValueGeneratedOnAdd();

            #endregion

            #region Properties Configuration

            // 1- UserId
            builder.Property(e => e.UserId).IsRequired();
            // 2- ServiceCenterId
            builder.Property(e => e.ServiceCenterId).IsRequired();
            // 3- Role
            builder.Property(e => e.Role)
    .IsRequired()
    .HasConversion<string>()
    .HasDefaultValue(EmployeeRole.Technician); // store enum as string value
            // 4- HireDate
            builder.Property(e => e.HireDate).IsRequired().HasColumnType("date");
            #endregion

            #region Auditable Properties
            // 5- createdAt 
            builder.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
            // 6- updatedAt
            builder.Property(e => e.UpdatedAt);
            // 7- createdBy
            builder.Property(e => e.CreatedBy);
            // 8- updatedBy
            builder.Property(e => e.UpdatedBy);
            #endregion

            #region Indexes
            // 1- userId index
            builder.HasIndex(e => e.UserId)
            .IsUnique()
            .HasDatabaseName("UK_Employees_UserId");

            // 2- serviceCenterId index
            builder.HasIndex(e => e.ServiceCenterId).HasDatabaseName("IDX_Employees_ServiceCenterId");

            // 3- Role Index
            builder.HasIndex(e => e.Role).HasDatabaseName("IDX_Employees_Role");
            #endregion

            #region check Constraints

            builder.ToTable(t => t.HasCheckConstraint("CHK_Employees_Role", "[Role] IN ('Technician', 'Manager', 'Owner')"));
            builder.ToTable(t => t.HasCheckConstraint("CHK_Employees_HireDate", "[HireDate] <= CAST(GETUTCDATE() AS DATE)"));
            #endregion

            #region Relationships

            // 1- Employee to User (one-to-one) 
            builder.HasOne(e => e.User)
            .WithOne(u => u.Employee)
            .HasForeignKey<Employee>(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Employees_Users");

            // 2- Employee to ServiceCenter (many-to-one)
            builder.HasOne(e => e.ServiceCenter)
            .WithMany(sc => sc.Employees)
            .HasForeignKey(e => e.ServiceCenterId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Employees_ServiceCenters");
            #endregion

            // Ignore Domain Events
            builder.Ignore(e => e.DomainEvents);

        }
    }
}

