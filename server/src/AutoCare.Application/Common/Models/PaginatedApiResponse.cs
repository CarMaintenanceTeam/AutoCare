using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoCare.Application.Common.Models
{
    /// <summary>
    /// Response wrapper for paginated API responses
    /// Extends ApiResponse with pagination metadata
    /// </summary>
    /// <typeparam name="T">Type of items in response</typeparam>
    public sealed class PaginatedApiResponse<T> : ApiResponse<List<T>>
    {
        /// <summary>
        /// Gets or sets the pagination metadata
        /// </summary>
        public PaginationMetadata? Pagination { get; set; }

        /// <summary>
        /// Creates a successful paginated API response
        /// </summary>
        /// <param name="paginatedList">Paginated list</param>
        /// <returns>Successful paginated API response</returns>
        public static PaginatedApiResponse<T> SuccessResponse(PaginatedList<T> paginatedList)
        {
            return new PaginatedApiResponse<T>
            {
                Success = true,
                Data = paginatedList.Items.ToList(),
                Pagination = new PaginationMetadata
                {
                    PageNumber = paginatedList.PageNumber,
                    PageSize = paginatedList.PageSize,
                    TotalCount = paginatedList.TotalCount,
                    TotalPages = paginatedList.TotalPages,
                    HasPreviousPage = paginatedList.HasPreviousPage,
                    HasNextPage = paginatedList.HasNextPage
                }
            };
        }
    }
}