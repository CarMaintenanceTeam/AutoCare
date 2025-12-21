using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Models;
using AutoCare.Application.Features.ServiceCenters.Models;
using AutoCare.Application.Features.ServiceCenters.Queries.GetAllServiceCenters;
using AutoCare.Application.Features.ServiceCenters.Queries.GetNearbyServiceCenters;
using AutoCare.Application.Features.ServiceCenters.Queries.GetServiceCenterById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoCare.Api.Controllers
{
    /// <summary>
    /// Service Centers controller
    /// Handles service center queries
    /// 
    /// Design Principles:
    /// - Single Responsibility: Only handles HTTP concerns
    /// - Thin Controller: Delegates all logic to handlers
    /// - RESTful API: Follows REST conventions
    /// 
    /// Endpoints:
    /// - GET /api/service-centers - Get all service centers with filters
    /// - GET /api/service-centers/{id} - Get service center by ID
    /// - GET /api/service-centers/nearby - Get nearby service centers
    /// </summary>
    [ApiController]
    [Route("api/service-centers")]
    [Produces("application/json")]
    public class ServiceCentersController : ControllerBase
    {


        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of ServiceCentersController
        /// </summary>
        /// <param name="mediator">MediatR mediator</param>
        public ServiceCentersController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Gets all service centers with optional filtering and pagination
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10, max: 100)</param>
        /// <param name="city">Filter by city</param>
        /// <param name="isActive">Filter by active status (default: true)</param>
        /// <param name="serviceId">Filter by service offered</param>
        /// <param name="sortBy">Sort field (Name, City)</param>
        /// <param name="sortOrder">Sort order (Asc, Desc)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated list of service centers</returns>
        /// <response code="200">Service centers retrieved successfully</response>
        /// <response code="400">Invalid query parameters</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(PaginatedApiResponse<ServiceCenterListDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllServiceCenters(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? city = null,
            [FromQuery] bool? isActive = true,
            [FromQuery] int? serviceId = null,
            [FromQuery] string sortBy = "Name",
            [FromQuery] string sortOrder = "Asc",
            CancellationToken cancellationToken = default)
        {
            var query = new GetAllServiceCentersQuery(
                pageNumber,
                pageSize,
                city,
                isActive,
                serviceId,
                sortBy,
                sortOrder);

            var result = await _mediator.Send(query, cancellationToken);

            return Ok(PaginatedApiResponse<ServiceCenterListDto>.SuccessResponse(result));
        }

        /// <summary>
        /// Gets a service center by ID with all details
        /// </summary>
        /// <param name="id">Service center ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Service center details</returns>
        /// <response code="200">Service center retrieved successfully</response>
        /// <response code="404">Service center not found</response>
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<ServiceCenterDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetServiceCenterById(
            int id,
            CancellationToken cancellationToken)
        {
            var query = new GetServiceCenterByIdQuery(id);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(ApiResponse<ServiceCenterDto>.SuccessResponse(result));
        }

        /// <summary>
        /// Gets nearby service centers based on GPS location
        /// Uses Haversine formula for accurate distance calculation
        /// Results are sorted by distance (nearest first)
        /// </summary>
        /// <param name="latitude">User's latitude (-90 to 90)</param>
        /// <param name="longitude">User's longitude (-180 to 180)</param>
        /// <param name="radiusKm">Search radius in kilometers (default: 50, max: 200)</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10, max: 100)</param>
        /// <param name="serviceId">Filter by service offered</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated list of nearby service centers with distances</returns>
        /// <response code="200">Nearby service centers retrieved successfully</response>
        /// <response code="400">Invalid coordinates or parameters</response>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/service-centers/nearby?latitude=30.0444&amp;longitude=31.2357&amp;radiusKm=50
        ///     
        /// Example coordinates:
        /// - Cairo, Egypt: latitude=30.0444, longitude=31.2357
        /// - Alexandria, Egypt: latitude=31.2001, longitude=29.9187
        /// </remarks>
        [HttpGet("nearby")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(PaginatedApiResponse<ServiceCenterListDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetNearbyServiceCenters(
            [FromQuery] decimal latitude,
            [FromQuery] decimal longitude,
            [FromQuery] int radiusKm = 50,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? serviceId = null,
            CancellationToken cancellationToken = default)
        {
            var query = new GetNearbyServiceCentersQuery(
                latitude,
                longitude,
                radiusKm,
                pageNumber,
                pageSize,
                serviceId);

            var result = await _mediator.Send(query, cancellationToken);

            return Ok(PaginatedApiResponse<ServiceCenterListDto>.SuccessResponse(result));
        }
    }
}