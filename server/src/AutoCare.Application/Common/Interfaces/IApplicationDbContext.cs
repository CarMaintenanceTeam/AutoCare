using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace AutoCare.Application.Common.Interfaces
{
    /// <summary>
    /// Interface for database context
    /// Allows Application layer to access database without depending on Infrastructure
    /// </summary>
    public interface IApplicationDbContext
    {

        DbSet<User> Users { get; }
        DbSet<Customer> Customers { get; }
        DbSet<Employee> Employees { get; }
        DbSet<ServiceCenter> ServiceCenters { get; }
        DbSet<Service> Services { get; }
        DbSet<ServiceCenterService> ServiceCenterServices { get; }
        DbSet<Vehicle> Vehicles { get; }
        DbSet<Booking> Bookings { get; }
        DbSet<BookingStatusHistory> BookingStatusHistory { get; }
        DbSet<RefreshToken> RefreshTokens { get; }

        /// <summary>
        /// Saves all changes made in this context to the database
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}