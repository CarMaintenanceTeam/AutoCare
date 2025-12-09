using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Domain.Common;

namespace AutoCare.Domain.Entities
{
    /// <summary>
    /// Employee entity - represents service center employees
    /// One-to-One relationship with User
    /// </summary>
    public sealed class Employee : AuditableEntity<int>
    {
        #region Properties

        /// <summary>
        /// Gets the associated user ID (foreign key)
        /// </summary>
        public int UserId { get; private set; }

        /// <summary>
        /// Gets the service center ID where the employee works
        /// </summary>
        public int ServiceCenterId { get; private set; }

        /// <summary>
        /// Gets the employee's role (Technician, Manager, Owner)
        /// </summary>
        public EmployeeRole Role { get; private set; }

        /// <summary>
        /// Gets the date the employee was hired
        /// </summary>
        public DateTime HireDate { get; private set; }

        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the associated user account
        /// </summary>
        public User User { get; private set; } = null!;

        /// <summary>
        /// Gets the service center where the employee works
        /// </summary>
        public ServiceCenter ServiceCenter { get; private set; } = null!;

        #endregion

        #region Constructors

        /// <summary>
        /// Private parameterless constructor for EF Core
        /// </summary>
        private Employee()
        {
        }

        #endregion

        #region Factory Methods

        /// <summary>
        /// Creates a new employee
        /// </summary>
        /// <param name="userId">Associated user ID</param>
        /// <param name="serviceCenterId">Service center ID</param>
        /// <param name="role">Employee role</param>
        /// <param name="hireDate">Hire date (defaults to today)</param>
        /// <returns>A new Employee entity</returns>
        public static Employee Create(
            int userId,
            int serviceCenterId,
            EmployeeRole role,
            DateTime? hireDate = null)
        {
            if (userId <= 0)
                throw new ArgumentException("User ID must be positive", nameof(userId));

            if (serviceCenterId <= 0)
                throw new ArgumentException("Service center ID must be positive", nameof(serviceCenterId));

            var actualHireDate = hireDate ?? DateTime.UtcNow.Date;

            if (actualHireDate > DateTime.UtcNow.Date)
                throw new ArgumentException("Hire date cannot be in the future", nameof(hireDate));

            var employee = new Employee
            {
                UserId = userId,
                ServiceCenterId = serviceCenterId,
                Role = role,
                HireDate = actualHireDate,
                CreatedAt = DateTime.UtcNow
            };

            return employee;
        }

        #endregion

        #region Business Methods

        /// <summary>
        /// Promotes technician to manager
        /// </summary>
        public void PromoteToManager()
        {
            if (Role == EmployeeRole.Technician)
            {
                Role = EmployeeRole.Manager;
                MarkAsUpdated();
            }
            else
            {
                throw new InvalidOperationException("Only technicians can be promoted to manager");
            }
        }

        /// <summary>
        /// Promotes manager to owner
        /// </summary>
        public void PromoteToOwner()
        {
            if (Role == EmployeeRole.Manager)
            {
                Role = EmployeeRole.Owner;
                MarkAsUpdated();

                Continue


}
            else
            {
                throw new InvalidOperationException("Only managers can be promoted to owner");
            }
        }

        /// <summary>
        /// Transfers employee to another service center
        /// </summary>
        /// <param name="newServiceCenterId">New service center ID</param>
        public void TransferToServiceCenter(int newServiceCenterId)
        {
            if (newServiceCenterId <= 0)
                throw new ArgumentException("Service center ID must be positive", nameof(newServiceCenterId));

            if (ServiceCenterId == newServiceCenterId)
                return; // Already at this center

            ServiceCenterId = newServiceCenterId;
            MarkAsUpdated();
        }

        /// <summary>
        /// Changes the employee's role
        /// </summary>
        /// <param name="newRole">New role</param>
        public void ChangeRole(EmployeeRole newRole)
        {
            if (Role == newRole)
                return; // Same role

            Role = newRole;
            MarkAsUpdated();
        }

        #endregion
    }
}