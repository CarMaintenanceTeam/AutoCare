using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Interfaces;

namespace AutoCare.Application.Features.Vehicles.Commands.DeleteVehicle
{
    /// <summary>
    /// Command to delete a vehicle
    /// Implements CQRS Command pattern
    /// 
    /// Design Principles:
    /// - Single Responsibility: Only handles vehicle deletion
    /// - Immutability: Record type
    /// - Command Pattern: Encapsulates delete request
    /// 
    /// Business Rules:
    /// - Only owner can delete their vehicle
    /// - Cannot delete vehicle with active bookings
    /// - Soft delete (preserves historical data)
    /// 
    /// Note: This is a soft delete - vehicle remains in database
    /// but is no longer available to the customer
    /// </summary>
    /// <param name="VehicleId">Vehicle ID to delete</param>
    public sealed record DeleteVehicleCommand(
        int VehicleId
    ) : ICommand;
}