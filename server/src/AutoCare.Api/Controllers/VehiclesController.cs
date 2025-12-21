using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Models;
using AutoCare.Application.Features.Vehicles.Commands.AddVehicle;
using AutoCare.Application.Features.Vehicles.Commands.DeleteVehicle;
using AutoCare.Application.Features.Vehicles.Commands.UpdateVehicle;
using AutoCare.Application.Features.Vehicles.Models;
using AutoCare.Application.Features.Vehicles.Queries.GetCustomerVehicles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoCare.Api.Controllers
{
    /// <summary>
    /// Vehicles controller
    /// Handles vehicle management for customers
    /// 
    /// Design Principles:
    /// - Single Responsibility: Only handles HTTP concerns
    /// - Thin Controller: Delegates all logic to handlers
    /// - RESTful API: Follows REST conventions
    /// - Authorization: Requires authentication (Customer only)
    /// 
    /// Endpoints:
    /// - GET /api/vehicles - Get customer's vehicles
    /// - POST /api/vehicles - Add new vehicle
    /// - PUT /api/vehicles/{id} - Update vehicle
    /// - DELETE /api/vehicles/{id} - Delete vehicle
    /// </summary>
    [ApiController]
    [Route("api/vehicles")]
    [Authorize] // All endpoints require authentication
    [Produces("application/json")]
    public sealed class VehiclesController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of VehiclesController
        /// </summary>
        /// <param name="mediator">MediatR mediator</param>
        public VehiclesController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Gets all vehicles for the authenticated customer
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of customer's vehicles</returns>
        /// <response code="200">Vehicles retrieved successfully</response>
        /// <response code="401">User not authenticated</response>
        /// <response code="404">Customer profile not found</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<VehicleListDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMyVehicles(CancellationToken cancellationToken)
        {
            var query = new GetCustomerVehiclesQuery();
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(ApiResponse<List<VehicleListDto>>.SuccessResponse(result));
        }

        /// <summary>
        /// Adds a new vehicle for the authenticated customer
        /// </summary>
        /// <param name="command">Vehicle details</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Created vehicle</returns>
        /// <response code="200">Vehicle added successfully</response>
        /// <response code="400">Validation failed</response>
        /// <response code="401">User not authenticated</response>
        /// <response code="409">Plate number or VIN already exists</response>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/vehicles
        ///     {
        ///         "brand": "Toyota",
        ///         "model": "Corolla",
        ///         "year": 2023,
        ///         "plateNumber": "ABC123",
        ///         "vin": "1HGBH41JXMN109186",
        ///         "color": "White"
        ///     }
        ///     
        /// Notes:
        /// - Plate number must be unique
        /// - VIN must be exactly 17 characters (if provided)
        /// - VIN cannot contain letters I, O, or Q
        /// - Year must be between 1900 and next year
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<VehicleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AddVehicle(
            [FromBody] AddVehicleCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(ApiResponse<VehicleDto>.SuccessResponse(result));
        }

        /// <summary>
        /// Updates an existing vehicle
        /// Only the vehicle owner can update it
        /// </summary>
        /// <param name="id">Vehicle ID</param>
        /// <param name="command">Updated vehicle details</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Updated vehicle</returns>
        /// <response code="200">Vehicle updated successfully</response>
        /// <response code="400">Validation failed or vehicle ID mismatch</response>
        /// <response code="401">User not authenticated</response>
        /// <response code="403">User doesn't own this vehicle</response>
        /// <response code="404">Vehicle not found</response>
        /// <remarks>
        /// Note: Plate number and VIN cannot be changed (immutable identifiers)
        /// </remarks>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<VehicleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateVehicle(
            int id,
            [FromBody] UpdateVehicleCommand command,
            CancellationToken cancellationToken)
        {
            // Validate ID match
            if (id != command.VehicleId)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    "Vehicle ID in URL does not match vehicle ID in body",
                    "INVALID_REQUEST"));
            }

            var result = await _mediator.Send(command, cancellationToken);
            return Ok(ApiResponse<VehicleDto>.SuccessResponse(result));
        }

        /// <summary>
        /// Deletes a vehicle
        /// Only the vehicle owner can delete it
        /// Cannot delete vehicle with active bookings
        /// </summary>
        /// <param name="id">Vehicle ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Success response</returns>
        /// <response code="200">Vehicle deleted successfully</response>
        /// <response code="400">Vehicle has active bookings</response>
        /// <response code="401">User not authenticated</response>
        /// <response code="403">User doesn't own this vehicle</response>
        /// <response code="404">Vehicle not found</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteVehicle(
            int id,
            CancellationToken cancellationToken)
        {
            var command = new DeleteVehicleCommand(id);
            await _mediator.Send(command, cancellationToken);

            return Ok(ApiResponse<object>.SuccessResponse(
                new { message = "Vehicle deleted successfully" }));
        }
    }
}