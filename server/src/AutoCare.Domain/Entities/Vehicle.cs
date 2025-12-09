using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Domain.Common;
using AutoCare.Domain.Events.VehicleEvents;

namespace AutoCare.Domain.Entities
{
    /// <summary>
    /// Vehicle entity - represents customer vehicles
    /// </summary>
    public sealed class Vehicle : AuditableEntity<int>
    {
        #region Properties

        /// <summary>
        /// Gets the customer ID who owns this vehicle
        /// </summary>
        public int CustomerId { get; private set; }

        /// <summary>
        /// Gets the vehicle brand (e.g., Toyota, BMW)
        /// </summary>
        public string Brand { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the vehicle model (e.g., Corolla, X5)
        /// </summary>
        public string Model { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the manufacturing year
        /// </summary>
        public int Year { get; private set; }

        /// <summary>
        /// Gets the license plate number (unique identifier)
        /// </summary>
        public string PlateNumber { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the Vehicle Identification Number (VIN) - optional
        /// </summary>
        public string? VIN { get; private set; }

        /// <summary>
        /// Gets the vehicle color - optional
        /// </summary>
        public string? Color { get; private set; }

        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the customer who owns this vehicle
        /// </summary>
        public Customer Customer { get; private set; } = null!;

        /// <summary>
        /// Gets the service bookings for this vehicle
        /// </summary>
        public ICollection<Booking> Bookings { get; private set; } = new List<Booking>();

        #endregion

        #region Constructors

        /// <summary>
        /// Private parameterless constructor for EF Core
        /// </summary>
        private Vehicle()
        {
        }

        #endregion

        #region Factory Methods

        /// <summary>
        /// Creates a new vehicle
        /// </summary>
        /// <param name="customerId">Customer ID who owns the vehicle</param>
        /// <param name="brand">Vehicle brand</param>
        /// <param name="model">Vehicle model</param>
        /// <param name="year">Manufacturing year</param>
        /// <param name="plateNumber">License plate number</param>
        /// <param name="vin">Optional VIN</param>
        /// <param name="color">Optional color</param>
        /// <returns>A new Vehicle entity</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static Vehicle Create(
            int customerId,
            string brand,
            string model,
            int year,
            string plateNumber,
            string? vin = null,
            string? color = null)
        {
            // Validate inputs
            if (customerId <= 0)
                throw new ArgumentException("Customer ID must be positive", nameof(customerId));

            ValidateBrand(brand);
            ValidateModel(model);
            ValidateYear(year);
            ValidatePlateNumber(plateNumber);

            if (!string.IsNullOrWhiteSpace(vin))
                ValidateVIN(vin);

            var vehicle = new Vehicle
            {
                CustomerId = customerId,
                Brand = brand.Trim(),
                Model = model.Trim(),
                Year = year,
                PlateNumber = plateNumber.Trim().ToUpperInvariant(),
                VIN = vin?.Trim().ToUpperInvariant(),
                Color = color?.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            // Raise domain event
            vehicle.AddDomainEvent(new VehicleCreatedEvent(
                vehicle.Id,
                vehicle.CustomerId,
                vehicle.PlateNumber,
                $"{vehicle.Brand} {vehicle.Model} ({vehicle.Year})"));

            return vehicle;
        }

        #endregion

        #region Business Methods

        /// <summary>
        /// Updates vehicle details
        /// </summary>
        /// <param name="brand">New brand</param>
        /// <param name="model">New model</param>
        /// <param name="year">New year</param>
        /// <param name="color">New color</param>
        public void UpdateDetails(string brand, string model, int year, string? color)
        {
            ValidateBrand(brand);
            ValidateModel(model);
            ValidateYear(year);

            Brand = brand.Trim();
            Model = model.Trim();
            Year = year;
            Color = color?.Trim();

            MarkAsUpdated();

            // Raise domain event
            AddDomainEvent(new VehicleUpdatedEvent(Id, CustomerId));
        }

        /// <summary>
        /// Updates the VIN
        /// </summary>
        /// <param name="vin">New VIN</param>
        public void UpdateVIN(string vin)
        {
            ValidateVIN(vin);
            VIN = vin.Trim().ToUpperInvariant();
            MarkAsUpdated();
        }

        /// <summary>
        /// Updates the plate number
        /// </summary>
        /// <param name="plateNumber">New plate number</param>
        public void UpdatePlateNumber(string plateNumber)
        {
            ValidatePlateNumber(plateNumber);
            PlateNumber = plateNumber.Trim().ToUpperInvariant();
            MarkAsUpdated();
        }

        #endregion

        #region Validation Methods

        /// <summary>
        /// Validates vehicle brand
        /// </summary>
        private static void ValidateBrand(string brand)
        {
            if (string.IsNullOrWhiteSpace(brand))
                throw new ArgumentException("Brand cannot be empty", nameof(brand));

            if (brand.Length < 2)
                throw new ArgumentException("Brand must be at least 2 characters", nameof(brand));

            if (brand.Length > 100)
                throw new ArgumentException("Brand cannot exceed 100 characters", nameof(brand));
        }

        /// <summary>
        /// Validates vehicle model
        /// </summary>
        private static void ValidateModel(string model)
        {
            if (string.IsNullOrWhiteSpace(model))
                throw new ArgumentException("Model cannot be empty", nameof(model));

            if (model.Length < 1)
                throw new ArgumentException("Model must be at least 1 character", nameof(model));

            if (model.Length > 100)
                throw new ArgumentException("Model cannot exceed 100 characters", nameof(model));
        }

        /// <summary>
        /// Validates manufacturing year
        /// </summary>
        private static void ValidateYear(int year)
        {
            int currentYear = DateTime.UtcNow.Year;
            int minYear = 1900;
            int maxYear = currentYear + 1; // Allow next year for pre-orders

            if (year < minYear || year > maxYear)
                throw new ArgumentException(
                    $"Year must be between {minYear} and {maxYear}",
                    nameof(year));
        }

        /// <summary>
        /// Validates plate number
        /// </summary>
        private static void ValidatePlateNumber(string plateNumber)
        {
            if (string.IsNullOrWhiteSpace(plateNumber))
                throw new ArgumentException("Plate number cannot be empty", nameof(plateNumber));

            if (plateNumber.Length < 3)
                throw new ArgumentException("Plate number must be at least 3 characters", nameof(plateNumber));

            if (plateNumber.Length > 50)
                throw new ArgumentException("Plate number cannot exceed 50 characters", nameof(plateNumber));
        }

        /// <summary>
        /// Validates VIN (17 characters)
        /// </summary>
        private static void ValidateVIN(string vin)
        {
            if (string.IsNullOrWhiteSpace(vin))
                throw new ArgumentException("VIN cannot be empty", nameof(vin));

            var cleanedVIN = vin.Trim();

            if (cleanedVIN.Length != 17)
                throw new ArgumentException("VIN must be exactly 17 characters", nameof(vin));

            // VIN should not contain I, O, Q
            if (cleanedVIN.Any(c => c == 'I' || c == 'O' || c == 'Q'))
                throw new ArgumentException("VIN cannot contain letters I, O, or Q", nameof(vin));
        }

        #endregion

    }
}