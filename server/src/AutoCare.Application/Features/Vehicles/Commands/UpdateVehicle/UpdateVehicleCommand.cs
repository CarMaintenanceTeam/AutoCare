using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Interfaces;
using AutoCare.Application.Features.Vehicles.Models;

namespace AutoCare.Application.Features.Vehicles.Commands.UpdateVehicle
{
    /// <summary>
    /// Command to update an existing vehicle
    /// Implements CQRS Command pattern
    /// 
    /// Design Principles:
    /// - Single Responsibility: Only handles vehicle updates
    /// - Immutability: Record type
    /// - Command Pattern: Encapsulates update request
    /// 
    /// Business Rules:
    /// - Only owner can update their vehicle
    /// - Cannot update plate number or VIN (immutable identifiers)
    /// - Year must be valid
    /// 
    /// Note: Plate number and VIN are intentionally excluded
    /// because they are unique identifiers and shouldn't change
    /// </summary>
    /// <param name="VehicleId">Vehicle ID to update</param>
    /// <param name="Brand">Updated vehicle brand</param>
    /// <param name="Model">Updated vehicle model</param>
    /// <param name="Year">Updated manufacturing year</param>
    /// <param name="Color">Updated vehicle color (optional)</param>
    public sealed record UpdateVehicleCommand(
        int VehicleId,
        string Brand,
        string Model,
        int Year,
        string? Color
    ) : ICommand<VehicleDto>;
}