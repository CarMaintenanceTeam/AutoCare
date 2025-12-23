using System;
using System.Threading;
using System.Threading.Tasks;
using AutoCare.Application.Common.Models;
using AutoCare.Application.Features.Bookings.Commands.CancelBooking;
using AutoCare.Application.Features.Bookings.Commands.CompleteBooking;
using AutoCare.Application.Features.Bookings.Commands.ConfirmBooking;
using AutoCare.Application.Features.Bookings.Commands.StartBooking;
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
    [Authorize]
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
    }
}