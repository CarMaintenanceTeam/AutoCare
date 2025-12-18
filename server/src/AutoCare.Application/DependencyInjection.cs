using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoCare.Application.Common.Behaviors;
using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace AutoCare.Application
{
    /// <summary>
    /// Application layer dependency injection configuration
    /// Implements Single Responsibility Principle - handles only DI configuration
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Adds application layer services to the DI container
        /// Configures:
        /// - MediatR for CQRS pattern
        /// - FluentValidation for request validation
        /// - Mapster for object mapping
        /// - Pipeline behaviors for cross-cutting concerns
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <returns>Modified service collection for method chaining</returns>
        /// <exception cref="ArgumentNullException">Thrown when services is null</exception>
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Validate input
            ArgumentNullException.ThrowIfNull(services);

            // Get the current assembly
            var assembly = Assembly.GetExecutingAssembly();

            // =========== MediatR Registration ===========
            AddMediatRServices(services, assembly);

            // =========== FluentValidation Registration ===========
            AddValidationServices(services, assembly);

            // =========== Mapster Configuration ===========
            AddMappingServices(services, assembly);

            return services;

        }

        /// <summary>
        /// Configures MediatR services and pipeline behaviors
        /// Pipeline order (execution sequence):
        /// 1. LoggingBehavior - Request/Response logging
        /// 2. ValidationBehavior - Input validation
        /// 3. PerformanceBehavior - Performance monitoring
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="assembly">Assembly containing handlers</param>
        private static void AddMediatRServices(this IServiceCollection services, Assembly assembly)
        {
            services.AddMediatR(config =>
            {
                // Register all IRequestHandler implementations from assembly
                config.RegisterServicesFromAssembly(assembly);

                // Add pipeline behaviors (order matters!)
                // Logging first to capture all requests
                config.AddOpenBehavior(typeof(LoggingBehavior<,>));

                // Validation before execution
                config.AddOpenBehavior(typeof(ValidationBehavior<,>));

                // Performance monitoring
                config.AddOpenBehavior(typeof(PerformanceBehavior<,>));
            });
        }

        /// <summary>
        /// Configures FluentValidation services
        /// Automatically discovers and registers all IValidator implementations
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="assembly">Assembly containing validators</param>
        private static void AddValidationServices(this IServiceCollection services, Assembly assembly)
        {
            // Scan assembly and register all validators
            services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);
        }

        /// <summary>
        /// Configures Mapster mapping services
        /// Uses global configuration with assembly scanning for mapping rules
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="assembly">Assembly containing mapping configurations</param>
        private static void AddMappingServices(this IServiceCollection services, Assembly assembly)
        {
            // Get global Mapster configuration
            var config = TypeAdapterConfig.GlobalSettings;

            // Scan assembly for mapping configurations
            config.Scan(assembly);

            // Register configuration as singleton (thread-safe)
            services.AddSingleton(config);

            // Register IMapper as scoped (per request)
            services.AddScoped<IMapper, ServiceMapper>();
        }




    }
}