using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Models;
using AutoCare.Application.Features.Bookings.Commands.CancelBooking;
using AutoCare.Application.Features.Bookings.Commands.CreateBooking;
using AutoCare.Application.Features.Bookings.Models;
using AutoCare.Application.Features.Bookings.Queries.GetBookingById;
using AutoCare.Application.Features.Bookings.Queries.GetCustomerBookings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoCare.Api.Controllers
{
    /// <summary>
    /// Bookings controller
    /// Handles booking management
    /// 
    /// Endpoints:
    /// - GET /api/bookings - Get customer's bookings
    /// - GET /api/bookings/{id} - Get booking by ID
    /// - POST /api/bookings - Create new booking
    /// - POST /api/bookings/{id}/cancel - Cancel booking
    /// </summary>
    [ApiController]
    [Route("api/bookings")]
    [Authorize]
    [Produces("application/json")]
    public sealed class BookingsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BookingsController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Gets bookings for the authenticated customer
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedApiResponse<BookingListDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMyBookings(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? status = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] string sortBy = "Date",
            [FromQuery] string sortOrder = "Desc",
            CancellationToken cancellationToken = default)
        {
            var query = new GetCustomerBookingsQuery(
                pageNumber, pageSize, status, fromDate, toDate, sortBy, sortOrder);

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(PaginatedApiResponse<BookingListDto>.SuccessResponse(result));
        }

        /// <summary>
        /// Gets booking details by ID
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<BookingDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBookingById(
            int id,
            CancellationToken cancellationToken)
        {
            var query = new GetBookingByIdQuery(id);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(ApiResponse<BookingDto>.SuccessResponse(result));
        }

        /// <summary>
        /// Creates a new booking
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/bookings
        ///     {
        ///         "vehicleId": 1,
        ///         "serviceCenterId": 2,
        ///         "serviceId": 3,
        ///         "bookingDate": "2024-12-25",
        ///         "bookingTime": "09:00:00",
        ///         "customerNotes": "Please check brakes carefully"
        ///     }
        ///     
        /// Business Rules:
        /// - Vehicle must be owned by customer
        /// - Service must be offered at service center
        /// - Date must be today or future (max 3 months)
        /// - Time must be on 30-minute intervals
        /// - No double booking allowed
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<BookingDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateBooking(
            [FromBody] CreateBookingCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(ApiResponse<BookingDto>.SuccessResponse(result));
        }

        /// <summary>
        /// Cancels a booking
        /// Customer can cancel Pending/Confirmed bookings
        /// Staff can cancel any booking except Completed
        /// </summary>
        [HttpPost("{id:int}/cancel")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CancelBooking(
            int id,
            [FromBody] CancelBookingRequest request,
            CancellationToken cancellationToken)
        {
            var command = new CancelBookingCommand(id, request.Reason);
            await _mediator.Send(command, cancellationToken);

            return Ok(ApiResponse<object>.SuccessResponse(
                new { message = "Booking cancelled successfully" }));
        }
    }

    /// <summary>
    /// Request model for cancelling a booking
    /// </summary>
    public sealed class CancelBookingRequest
    {
        /// <summary>
        /// Cancellation reason
        /// </summary>
        public string Reason { get; set; } = string.Empty;
    }
}