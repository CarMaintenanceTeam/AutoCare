using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Models;
using AutoCare.Application.Features.ServiceCenters.Models;
using AutoCare.Application.Features.Services.Models;
using AutoCare.Application.Features.Services.Queries.GetAllServices;
using AutoCare.Application.Features.Services.Queries.GetServiceById;
using AutoCare.Application.Features.Services.Queries.GetServicesByServiceCenter;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoCare.Api.Controllers
{
    /// <summary>
    /// Services controller
    /// Handles service queries
    /// 
    /// Design Principles:
    /// - Single Responsibility: Only handles HTTP concerns
    /// - Thin Controller: Delegates all logic to handlers
    /// - RESTful API: Follows REST conventions
    /// 
    /// Endpoints:
    /// - GET /api/services - Get all services with filters
    /// - GET /api/services/{id} - Get service by ID
    /// - GET /api/services/by-service-center/{serviceCenterId} - Get services by center
    /// </summary>
    [ApiController]
    [Route("api/services")]
    [Produces("application/json")]
    public sealed class ServicesController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of ServicesController
        /// </summary>
        /// <param name="mediator">MediatR mediator</param>
        public ServicesController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Gets all services with optional filtering and pagination
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10, max: 100)</param>
        /// <param name="serviceType">Filter by service type (Maintenance, SpareParts)</param>
        /// <param name="isActive">Filter by active status (default: true)</param>
        /// <param name="minPrice">Minimum price filter</param>
        /// <param name="maxPrice">Maximum price filter</param>
        /// <param name="sortBy">Sort field (Name, Price, Duration)</param>
        /// <param name="sortOrder">Sort order (Asc, Desc)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated list of services</returns>
        /// <response code="200">Services retrieved successfully</response>
        /// <response code="400">Invalid query parameters</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(PaginatedApiResponse<ServiceListDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllServices(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? serviceType = null,
            [FromQuery] bool? isActive = true,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] string sortBy = "Name",
            [FromQuery] string sortOrder = "Asc",
            CancellationToken cancellationToken = default)
        {
            var query = new GetAllServicesQuery(
                pageNumber,
                pageSize,
                serviceType,
                isActive,
                minPrice,
                maxPrice,
                sortBy,
                sortOrder);

            var result = await _mediator.Send(query, cancellationToken);

            return Ok(PaginatedApiResponse<ServiceListDto>.SuccessResponse(result));
        }

        /// <summary>
        /// Gets a service by ID with optional service centers
        /// </summary>
        /// <param name="id">Service ID</param>
        /// <param name="includeServiceCenters">Include service centers offering this service</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Service details</returns>
        /// <response code="200">Service retrieved successfully</response>
        /// <response code="404">Service not found</response>
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<ServiceDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetServiceById(
            int id,
            [FromQuery] bool includeServiceCenters = false,
            CancellationToken cancellationToken = default)
        {
            var query = new GetServiceByIdQuery(id, includeServiceCenters);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(ApiResponse<ServiceDto>.SuccessResponse(result));
        }

        /// <summary>
        /// Gets all services offered at a specific service center
        /// Returns services with custom pricing for that center
        /// </summary>
        /// <param name="serviceCenterId">Service center ID</param>
        /// <param name="includeUnavailable">Include unavailable services (default: false)</param>
        /// <param name="serviceType">Filter by service type (Maintenance, SpareParts)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of services with pricing</returns>
        /// <response code="200">Services retrieved successfully</response>
        /// <response code="404">Service center not found</response>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/services/by-service-center/1?serviceType=Maintenance
        ///     
        /// Use this endpoint when customer selects a service center 
        /// and wants to see what services are available there.
        /// </remarks>
        [HttpGet("by-service-center/{serviceCenterId:int}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<List<ServiceCenterServiceDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetServicesByServiceCenter(
            int serviceCenterId,
            [FromQuery] bool includeUnavailable = false,
            [FromQuery] string? serviceType = null,
            CancellationToken cancellationToken = default)
        {
            var query = new GetServicesByServiceCenterQuery(
                serviceCenterId,
                includeUnavailable,
                serviceType);

            var result = await _mediator.Send(query, cancellationToken);

            return Ok(ApiResponse<List<ServiceCenterServiceDto>>.SuccessResponse(result));
        }
    }
}