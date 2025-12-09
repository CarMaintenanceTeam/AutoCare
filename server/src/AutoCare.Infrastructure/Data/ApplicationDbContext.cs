using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoCare.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoCare.Infrastructure.Data
{
    /// <summary>
    /// Main database context for the AutoCare Platform
    /// Implements Unit of Work pattern
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Constructor with options
        /// </summary>
        /// <param name="options">DbContext options</param>
        /// <exception cref="ArgumentNullException">Thrown when options are null</exception>
        /// <exception cref="InvalidOperationException">Thrown when options are invalid</exception>
        /// <returns>Instance of ApplicationDbContext</returns>
        /// <remarks>
        /// This constructor initializes the database context with the provided options.
        /// It ensures that the options are not null and valid.
        /// </remarks>
        /// <example>
        /// var options = new DbContextOptionsBuilder&lt;ApplicationDbContext&gt;()
        ///    .UseSqlServer("connection_string")
        ///    .Options;
        /// var context = new ApplicationDbContext(options);
        /// </example>
        /// <seealso cref="DbContext"/>
        /// <seealso cref="DbContextOptions{TContext}"/>
        /// <seealso cref="ArgumentNullException"/>
        /// <seealso cref="InvalidOperationException"/>
        /// <seealso cref="Microsoft.EntityFrameworkCore"/>
        /// <seealso cref="AutoCare.Infrastructure.Data"/>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options), "DbContext options cannot be null.");
            }
        }

        #region DbSets

        /// <summary>
        /// Users table - all system users
        /// </summary>
        public DbSet<User> Users => Set<User>();

        /// <summary>
        /// Customers table - customer profiles
        /// </summary>
        public DbSet<Customer> Customers => Set<Customer>();

        /// <summary>
        /// Employees table - service center staff
        /// </summary>
        public DbSet<Employee> Employees => Set<Employee>();

        /// <summary>
        /// Service Centers table - physical locations
        /// </summary>
        public DbSet<ServiceCenter> ServiceCenters => Set<ServiceCenter>();

        /// <summary>
        /// Services table - maintenance services offered
        /// </summary>
        public DbSet<Service> Services => Set<Service>();

        /// <summary>
        /// Service Center Services table - many-to-many relationship
        /// </summary>
        public DbSet<ServiceCenterService> ServiceCenterServices => Set<ServiceCenterService>();

        /// <summary>
        /// Vehicles table - customer vehicles
        /// </summary>
        public DbSet<Vehicle> Vehicles => Set<Vehicle>();

        /// <summary>
        /// Bookings table - service appointments
        /// </summary>
        public DbSet<Booking> Bookings => Set<Booking>();

        /// <summary>
        /// Booking Status History table - audit trail
        /// </summary>
        public DbSet<BookingStatusHistory> BookingStatusHistory => Set<BookingStatusHistory>();

        /// <summary>
        /// Refresh Tokens table - JWT token management
        /// </summary>
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        #endregion

        #region Model Configuration

        /// <summary>
        /// Configures the database model using Fluent API
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apply all configurations from current assembly
            // This will find all classes implementing IEntityTypeConfiguration<T>
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }

        #endregion

        #region SaveChanges Overrides

        /// <summary>
        /// Saves all changes made in this context to the database
        /// Automatically handles:
        /// 1. Auditable entity timestamps
        /// 2. Domain event dispatching
        /// </summary>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Note: Actual auditing and event dispatching will be handled by Interceptors
            // This is just the base SaveChanges
            return await base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Synchronous version of SaveChangesAsync
        /// </summary>
        public override int SaveChanges()
        {
            return SaveChangesAsync().GetAwaiter().GetResult();
        }

        #endregion

    }
}