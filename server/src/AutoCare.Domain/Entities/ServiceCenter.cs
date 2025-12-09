using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Domain.Common;
using AutoCare.Domain.ValueObjects;

namespace AutoCare.Domain.Entities
{
    public sealed class ServiceCenter : AuditableEntity<int>
    {
        #region Properties

        /// <summary>
        /// Gets the service center name in English
        /// </summary>
        public string NameEn { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the service center name in Arabic
        /// </summary>
        public string NameAr { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the address in English
        /// </summary>
        public string AddressEn { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the address in Arabic
        /// </summary>
        public string AddressAr { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the city name
        /// </summary>
        public string City { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the GPS latitude coordinate
        /// </summary>
        public decimal Latitude { get; private set; }

        /// <summary>
        /// Gets the GPS longitude coordinate
        /// </summary>
        public decimal Longitude { get; private set; }

        /// <summary>
        /// Gets the contact phone number
        /// </summary>
        public string PhoneNumber { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the contact email address (optional)
        /// </summary>
        public string? Email { get; private set; }

        /// <summary>
        /// Gets the working hours (e.g., "08:00-17:00" or "08:00-13:00,14:00-17:00")
        /// </summary>
        public string? WorkingHours { get; private set; }

        /// <summary>
        /// Gets whether the service center is active
        /// </summary>
        public bool IsActive { get; private set; } = true;

        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the employees working at this service center
        /// </summary>
        public ICollection<Employee> Employees { get; private set; } = new List<Employee>();

        /// <summary>
        /// Gets the bookings at this service center
        /// </summary>
        public ICollection<Booking> Bookings { get; private set; } = new List<Booking>();

        /// <summary>
        /// Gets the services offered at this center (many-to-many)
        /// </summary>
        public ICollection<ServiceCenterService> ServiceCenterServices { get; private set; } = new List<ServiceCenterService>();

        #endregion

        #region Constructors

        /// <summary>
        /// Private parameterless constructor for EF Core
        /// </summary>
        private ServiceCenter()
        {
        }

        #endregion

        #region Factory Methods

        /// <summary>
        /// Creates a new service center
        /// </summary>
        /// <param name="nameEn">Name in English</param>
        /// <param name="nameAr">Name in Arabic</param>
        /// <param name="addressEn">Address in English</param>
        /// <param name="addressAr">Address in Arabic</param>
        /// <param name="city">City name</param>
        /// <param name="latitude">GPS latitude</param>
        /// <param name="longitude">GPS longitude</param>
        /// <param name="phoneNumber">Contact phone</param>
        /// <param name="email">Contact email (optional)</param>
        /// <param name="workingHours">Working hours (optional)</param>
        /// <returns>A new ServiceCenter entity</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static ServiceCenter Create(
            string nameEn,
            string nameAr,
            string addressEn,
            string addressAr,
            string city,
            decimal latitude,
            decimal longitude,
            string phoneNumber,
            string? email = null,
            string? workingHours = null)
        {
            // Validate names
            ValidateName(nameEn, "English");
            ValidateName(nameAr, "Arabic");

            // Validate addresses
            ValidateAddress(addressEn, "English");
            ValidateAddress(addressAr, "Arabic");

            // Validate city
            ValidateCity(city);

            // Validate coordinates using Value Object
            var coordinate = Coordinate.Create(latitude, longitude);

            // Validate phone
            ValidatePhoneNumber(phoneNumber);

            var serviceCenter = new ServiceCenter
            {
                NameEn = nameEn.Trim(),
                NameAr = nameAr.Trim(),
                AddressEn = addressEn.Trim(),
                AddressAr = addressAr.Trim(),
                City = city.Trim(),
                Latitude = coordinate.Latitude,
                Longitude = coordinate.Longitude,
                PhoneNumber = phoneNumber.Trim(),
                Email = email?.Trim().ToLowerInvariant(),
                WorkingHours = workingHours?.Trim(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            return serviceCenter;
        }

        #endregion

        #region Business Methods

        /// <summary>
        /// Updates service center names
        /// </summary>
        /// <param name="nameEn">New name in English</param>
        /// <param name="nameAr">New name in Arabic</param>
        public void UpdateNames(string nameEn, string nameAr)
        {
            ValidateName(nameEn, "English");
            ValidateName(nameAr, "Arabic");

            NameEn = nameEn.Trim();
            NameAr = nameAr.Trim();
            MarkAsUpdated();
        }

        /// <summary>
        /// Updates service center addresses
        /// </summary>
        /// <param name="addressEn">New address in English</param>
        /// <param name="addressAr">New address in Arabic</param>
        /// <param name="city">New city</param>
        public void UpdateAddress(string addressEn, string addressAr, string city)
        {
            ValidateAddress(addressEn, "English");
            ValidateAddress(addressAr, "Arabic");
            ValidateCity(city);

            AddressEn = addressEn.Trim();
            AddressAr = addressAr.Trim();
            City = city.Trim();
            MarkAsUpdated();
        }

        /// <summary>
        /// Updates GPS location
        /// </summary>
        /// <param name="latitude">New latitude</param>
        /// <param name="longitude">New longitude</param>
        public void UpdateLocation(decimal latitude, decimal longitude)
        {
            var coordinate = Coordinate.Create(latitude, longitude);

            Latitude = coordinate.Latitude;
            Longitude = coordinate.Longitude;
            MarkAsUpdated();
        }

        /// <summary>
        /// Updates contact information
        /// </summary>
        /// <param name="phoneNumber">New phone number</param>
        /// <param name="email">New email (optional)</param>
        public void UpdateContactInfo(string phoneNumber, string? email)
        {
            ValidatePhoneNumber(phoneNumber);

            PhoneNumber = phoneNumber.Trim();
            Email = email?.Trim().ToLowerInvariant();
            MarkAsUpdated();
        }

        /// <summary>
        /// Updates working hours
        /// </summary>
        /// <param name="workingHours">New working hours</param>
        public void UpdateWorkingHours(string? workingHours)
        {
            WorkingHours = workingHours?.Trim();
            MarkAsUpdated();
        }

        /// <summary>
        /// Activates the service center
        /// </summary>
        public void Activate()
        {
            if (IsActive)
                return; // Already active

            IsActive = true;
            MarkAsUpdated();
        }

        /// <summary>
        /// Deactivates the service center
        /// </summary>
        public void Deactivate()
        {
            if (!IsActive)
                return; // Already inactive

            IsActive = false;
            MarkAsUpdated();
        }

        /// <summary>
        /// Calculates distance to another service center
        /// </summary>
        /// <param name="other">Other service center</param>
        /// <returns>Distance in kilometers</returns>
        public double DistanceTo(ServiceCenter other)
        {
            var thisCoord = Coordinate.Create(Latitude, Longitude);
            var otherCoord = Coordinate.Create(other.Latitude, other.Longitude);

            return thisCoord.DistanceTo(otherCoord);
        }

        #endregion

        #region Validation Methods

        /// <summary>
        /// Validates name in specified language
        /// </summary>
        private static void ValidateName(string name, string language)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"{language} name cannot be empty");

            if (name.Length < 3)
                throw new ArgumentException($"{language} name must be at least 3 characters");

            if (name.Length > 200)
                throw new ArgumentException($"{language} name cannot exceed 200 characters");
        }

        /// <summary>
        /// Validates address in specified language
        /// </summary>
        private static void ValidateAddress(string address, string language)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException($"{language} address cannot be empty");

            if (address.Length < 10)
                throw new ArgumentException($"{language} address must be at least 10 characters");

            if (address.Length > 500)
                throw new ArgumentException($"{language} address cannot exceed 500 characters");
        }

        /// <summary>
        /// Validates city name
        /// </summary>
        private static void ValidateCity(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City cannot be empty");

            if (city.Length < 2)
                throw new ArgumentException("City must be at least 2 characters");

            if (city.Length > 100)
                throw new ArgumentException("City cannot exceed 100 characters");
        }

        /// <summary>
        /// Validates phone number
        /// </summary>
        private static void ValidatePhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Phone number cannot be empty");

            var digits = new string(phoneNumber.Where(char.IsDigit).ToArray());

            if (digits.Length < 10)
                throw new ArgumentException("Phone number must be at least 10 digits");
        }

        #endregion
    }
}