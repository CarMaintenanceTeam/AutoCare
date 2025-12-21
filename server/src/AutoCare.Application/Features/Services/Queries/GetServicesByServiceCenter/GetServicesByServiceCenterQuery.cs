using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Interfaces;
using AutoCare.Application.Features.ServiceCenters.Models;

namespace AutoCare.Application.Features.Services.Queries.GetServicesByServiceCenter
{
    /// <summary>
    /// Query to get all services offered at a specific service center
    /// Returns services with custom pricing for that center
    /// 
    /// Design Principles:
    /// - Single Responsibility: Only retrieves services for one center
    /// - Immutability: Record type
    /// - Query Pattern: Read-only operation
    /// 
    /// Use Case:
    /// Customer selects a service center and wants to see all available services
    /// </summary>
    /// <param name="ServiceCenterId">Service center ID</param>
    /// <param name="IncludeUnavailable">Include unavailable services (default: false)</param>
    /// <param name="ServiceType">Filter by service type (optional)</param>
    public sealed record GetServicesByServiceCenterQuery(
        int ServiceCenterId,
        bool IncludeUnavailable = false,
        string? ServiceType = null
    ) : IQuery<List<ServiceCenterServiceDto>>;
}