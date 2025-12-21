using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Interfaces;
using AutoCare.Application.Features.Vehicles.Models;

namespace AutoCare.Application.Features.Vehicles.Commands.AddVehicle
{
    /// <summary>
    /// Command to add a new vehicle for a customer
    /// Implements CQRS Command pattern
    /// 
    /// Design Principles:
    /// - Single Responsibility: Only handles vehicle creation
    /// - Immutability: Record type
    /// - Command Pattern: Encapsulates vehicle addition request
    /// 
    /// Business Rules:
    /// - Customer must exist and be active
    /// - Plate number must be unique
    /// - VIN must be unique (if provided)
    /// - Year must be valid (1900 to current year + 1)
    /// 
    /// Flow:
    /// 1. Validate input (AddVehicleCommandValidator)
    /// 2. Verify customer exists and is active
    /// 3. Check plate number uniqueness
    /// 4. Check VIN uniqueness (if provided)
    /// 5. Create vehicle entity
    /// 6. Save to database
    /// 7. Return vehicle DTO
    /// </summary>
    /// <param name="Brand">Vehicle brand (e.g., Toyota, BMW)</param>
    /// <param name="Model">Vehicle model (e.g., Corolla, X5)</param>
    /// <param name="Year">Manufacturing year</param>
    /// <param name="PlateNumber">License plate number (unique)</param>
    /// <param name="VIN">Vehicle Identification Number (optional, unique)</param>
    /// <param name="Color">Vehicle color (optional)</param>
    public sealed record AddVehicleCommand(
        string Brand,
        string Model,
        int Year,
        string PlateNumber,
        string? VIN,
        string? Color
    ) : ICommand<VehicleDto>;
}