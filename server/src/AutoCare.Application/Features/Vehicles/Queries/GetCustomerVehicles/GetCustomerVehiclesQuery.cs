using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Interfaces;
using AutoCare.Application.Features.Vehicles.Models;

namespace AutoCare.Application.Features.Vehicles.Queries.GetCustomerVehicles
{

    /// <summary>
    /// Query to get all vehicles for the current customer
    /// Implements CQRS Query pattern
    /// 
    /// Design Principles:
    /// - Single Responsibility: Only retrieves customer's vehicles
    /// - Immutability: Record type
    /// - Query Pattern: Read-only operation
    /// - Authorization: Only returns vehicles owned by authenticated user
    /// 
    /// Use Case:
    /// Customer wants to see their registered vehicles
    /// Used in booking flow to select vehicle
    /// </summary>
    public sealed record GetCustomerVehiclesQuery() : IQuery<List<VehicleListDto>>;
}