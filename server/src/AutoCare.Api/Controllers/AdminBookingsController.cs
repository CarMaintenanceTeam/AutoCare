using System;
using System.Threading;
using System.Threading.Tasks;
using AutoCare.Application.Common.Models;
using AutoCare.Application.Features.Bookings.Commands.CancelBooking;
using AutoCare.Application.Features.Bookings.Commands.CompleteBooking;
using AutoCare.Application.Features.Bookings.Commands.ConfirmBooking;
using AutoCare.Application.Features.Bookings.Commands.StartBooking;
using AutoCare.Application.Features.Bookings.Models;
using AutoCare.Application.Features.Bookings.Queries.GetAllBookings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoCare.Api.Controllers
{
    /// <summary>
    /// Admin/Staff bookings controller
    /// Provides endpoints for managing booking lifecycle.
    /// </summary>
    [ApiController]
    [Route("api/admin/bookings")]
    [Authorize(Roles = "Admin,Employee")]
    [Produces("application/json")]
    public sealed class AdminBookingsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminBookingsController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public sealed class BookingStatusRequest
        {
            public string? Notes { get; set; }
        }

        /// <summary>
        /// Gets all bookings for admin/staff overview with pagination and optional filters.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedApiResponse<BookingListDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? status = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] string sortBy = "Date",
            [FromQuery] string sortOrder = "Desc",
            CancellationToken cancellationToken = default)
        {
            var query = new GetAllBookingsQuery(
                pageNumber,
                pageSize,
                status,
                fromDate,
                toDate,
                sortBy,
                sortOrder);

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(PaginatedApiResponse<BookingListDto>.SuccessResponse(result));
        }

        /// <summary>
        /// Confirms a booking (Pending -> Confirmed)
        /// </summary>
        [HttpPost("{id:int}/confirm")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Confirm(
            int id,
            [FromBody] BookingStatusRequest request,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(new ConfirmBookingCommand(id, request.Notes), cancellationToken);
            return Ok(ApiResponse<object>.SuccessResponse(new { message = "Booking confirmed successfully" }));
        }

        /// <summary>
        /// Starts work on a booking (Confirmed -> InProgress)
        /// </summary>
        [HttpPost("{id:int}/start")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Start(
            int id,
            [FromBody] BookingStatusRequest request,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(new StartBookingCommand(id, request.Notes), cancellationToken);
            return Ok(ApiResponse<object>.SuccessResponse(new { message = "Booking started successfully" }));
        }

        /// <summary>
        /// Completes a booking (InProgress -> Completed)
        /// </summary>
        [HttpPost("{id:int}/complete")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Complete(
            int id,
            [FromBody] BookingStatusRequest request,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(new CompleteBookingCommand(id, request.Notes), cancellationToken);
            return Ok(ApiResponse<object>.SuccessResponse(new { message = "Booking completed successfully" }));
        }

        /// <summary>
        /// Gets booking details by id for admin/staff.
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<BookingDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(
            int id,
            CancellationToken cancellationToken)
        {
            // Re-use existing GetBookingByIdQuery, but admin handler already checks role
            var query = new AutoCare.Application.Features.Bookings.Queries.GetBookingById.GetBookingByIdQuery(id);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(ApiResponse<BookingDto>.SuccessResponse(result));
        }

    }
}