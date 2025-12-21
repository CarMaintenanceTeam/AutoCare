using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Interfaces;
using AutoCare.Application.Features.ServiceCenters.Models;

namespace AutoCare.Application.Features.ServiceCenters.Queries.GetServiceCenterById
{

    /// <summary>
    /// Query to get a single service center by ID
    /// Includes all details and available services
    /// 
    /// Design Principles:
    /// - Single Responsibility: Only retrieves one service center
    /// - Immutability: Record type
    /// - Query Pattern: Read-only operation
    /// </summary>
    /// <param name="ServiceCenterId">Service center ID</param>
    public sealed record GetServiceCenterByIdQuery(
        int ServiceCenterId
    ) : IQuery<ServiceCenterDto>;
}