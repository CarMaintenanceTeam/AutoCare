using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Domain.Entities;
using Mapster;

namespace AutoCare.Application.Common.Mappings
{
    /// <summary>
    /// Global Mapster configuration
    /// Implements Convention over Configuration principle
    /// 
    /// Design Principles:
    /// - Single Responsibility: Configures all object mappings
    /// - Convention over Configuration: Uses sensible defaults
    /// - Explicit Mappings: Custom mappings for complex scenarios
    /// 
    /// Mapster automatically maps:
    /// - Properties with same name (case-insensitive)
    /// - Nested objects
    /// - Collections
    /// - Enums
    /// 
    /// Only configure custom mappings when needed
    /// </summary>
    public static class MappingConfig
    {
        /// <summary>
        /// Registers all mapping configurations
        /// Called automatically by DependencyInjection
        /// </summary>
        /// <param name="config">Mapster type adapter configuration</param>
        public static void Configure(TypeAdapterConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            // Global settings
            ConfigureGlobalSettings(config);

            // Entity to DTO mappings
            ConfigureUserMappings(config);
            ConfigureServiceCenterMappings(config);
            ConfigureServiceMappings(config);
            ConfigureBookingMappings(config);
            ConfigureVehicleMappings(config);
        }

        /// <summary>
        /// Configures global Mapster settings
        /// </summary>
        private static void ConfigureGlobalSettings(TypeAdapterConfig config)
        {
            // Ignore null values in source
            config.Default.IgnoreNullValues(true);

            // Map enums by name (not value)
            config.Default.NameMatchingStrategy(NameMatchingStrategy.Flexible);

            // Preserve references to avoid circular reference issues
            config.Default.PreserveReference(true);

            // Max depth to prevent stack overflow with circular references
            config.Default.MaxDepth(5);
        }

        /// <summary>
        /// Configures User entity mappings
        /// </summary>
        private static void ConfigureUserMappings(TypeAdapterConfig config)
        {
            // User -> UserDto (exclude sensitive data)
            config.NewConfig<User, object>()
                .Ignore(dest => dest.GetType().GetProperty("PasswordHash")!);

            // Example: Custom mapping for full name formatting
            // config.NewConfig<User, UserDto>()
            //     .Map(dest => dest.DisplayName, src => $"{src.FullName} ({src.Email})");
        }

        /// <summary>
        /// Configures ServiceCenter entity mappings
        /// </summary>
        private static void ConfigureServiceCenterMappings(TypeAdapterConfig config)
        {
            // ServiceCenter -> ServiceCenterDto
            // Mapster will automatically map:
            // - All properties with matching names
            // - Nested objects (Employees, Services)
            // - Collections

            // Example: Custom mapping for distance calculation
            // config.NewConfig<ServiceCenter, ServiceCenterDto>()
            //     .Map(dest => dest.Distance, src => CalculateDistance(src, userLocation));
        }

        /// <summary>
        /// Configures Service entity mappings
        /// </summary>
        private static void ConfigureServiceMappings(TypeAdapterConfig config)
        {
            // Service -> ServiceDto
            // Automatic mapping works for most cases

            // Example: Custom mapping for localized names
            // config.NewConfig<Service, ServiceDto>()
            //     .Map(dest => dest.Name, src => GetLocalizedName(src, currentCulture));
        }

        /// <summary>
        /// Configures Booking entity mappings
        /// </summary>
        private static void ConfigureBookingMappings(TypeAdapterConfig config)
        {
            // Booking -> BookingDto
            // Include related entities (Customer, Vehicle, Service, ServiceCenter)

            // Example: Flatten nested properties
            // config.NewConfig<Booking, BookingDto>()
            //     .Map(dest => dest.CustomerName, src => src.Customer.User.FullName)
            //     .Map(dest => dest.ServiceName, src => src.Service.NameEn)
            //     .Map(dest => dest.ServiceCenterName, src => src.ServiceCenter.NameEn);
        }

        /// <summary>
        /// Configures Vehicle entity mappings
        /// </summary>
        private static void ConfigureVehicleMappings(TypeAdapterConfig config)
        {
            // Vehicle -> VehicleDto
            // Automatic mapping handles most cases

            // Example: Custom computed property
            // config.NewConfig<Vehicle, VehicleDto>()
            //     .Map(dest => dest.DisplayName, src => $"{src.Brand} {src.Model} ({src.Year})");
        }
    }
}