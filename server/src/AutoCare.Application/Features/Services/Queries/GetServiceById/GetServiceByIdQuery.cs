using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Interfaces;
using AutoCare.Application.Features.Services.Models;

namespace AutoCare.Application.Features.Services.Queries.GetServiceById
{
    /// <summary>
    /// Query to get a single service by ID
    /// Includes complete details and service centers offering this service
    /// 
    /// Design Principles:
    /// - Single Responsibility: Only retrieves one service
    /// - Immutability: Record type
    /// - Query Pattern: Read-only operation
    /// </summary>
    /// <param name="ServiceId">Service ID</param>
    /// <param name="IncludeServiceCenters">Whether to include service centers (default: false)</param>
    public sealed record GetServiceByIdQuery(
        int ServiceId,
        bool IncludeServiceCenters = false
    ) : IQuery<ServiceDto>;
}