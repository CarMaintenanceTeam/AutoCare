using System;
using System.Threading;
using System.Threading.Tasks;
using AutoCare.Application.Common.Models;
using AutoCare.Application.Features.Customers.Models;
using AutoCare.Application.Features.Customers.Queries.GetAllCustomers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoCare.Api.Controllers
{
    /// <summary>
    /// Admin controller for managing customers.
    /// Read-only overview for now (list + basic filters).
    /// </summary>
    [ApiController]
    [Route("api/admin/customers")]
    [Authorize(Roles = "Admin,Employee")]
    [Produces("application/json")]
    public sealed class AdminCustomersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminCustomersController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Gets customers for admin overview with pagination and optional filters.
        /// Only Admin and manager/owner employees are allowed.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedApiResponse<CustomerListDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? city = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] string sortBy = "Name",
            [FromQuery] string sortOrder = "Asc",
            CancellationToken cancellationToken = default)
        {
            var query = new GetAllCustomersQuery(
                pageNumber,
                pageSize,
                search,
                city,
                isActive,
                sortBy,
                sortOrder);

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(PaginatedApiResponse<CustomerListDto>.SuccessResponse(result));
        }
    }
}