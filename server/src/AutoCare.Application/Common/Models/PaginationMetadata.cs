using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoCare.Application.Common.Models
{
    /// <summary>
    /// Pagination metadata for API responses
    /// </summary>
    public sealed class PaginationMetadata
    {
        /// <summary>
        /// Gets or sets the current page number
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Gets or sets the page size
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the total count of items
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Gets or sets the total number of pages
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Gets or sets whether there is a previous page
        /// </summary>
        public bool HasPreviousPage { get; set; }

        /// <summary>
        /// Gets or sets whether there is a next page
        /// </summary>
        public bool HasNextPage { get; set; }
    }
}