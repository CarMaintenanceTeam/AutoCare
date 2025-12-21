using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoCare.Application.Common.Models
{
    /// <summary>
    /// Represents pagination parameters for queries
    /// Implements Value Object pattern - immutable and validated
    /// 
    /// Design Principles:
    /// - Single Responsibility: Encapsulates pagination logic
    /// - Immutability: All properties are readonly
    /// - Self-Validation: Validates parameters in constructor
    /// 
    /// Usage: Include in query requests that support pagination
    /// </summary>
    public sealed class PaginationParams
    {
        /// <summary>
        /// Default page number (first page)
        /// </summary>
        public const int DefaultPageNumber = 1;

        /// <summary>
        /// Default page size
        /// </summary>
        public const int DefaultPageSize = 10;

        /// <summary>
        /// Maximum allowed page size to prevent performance issues
        /// </summary>
        public const int MaxPageSize = 100;

        /// <summary>
        /// Gets the page number (1-based)
        /// </summary>
        public int PageNumber { get; }

        /// <summary>
        /// Gets the page size (number of items per page)
        /// </summary>
        public int PageSize { get; }

        /// <summary>
        /// Gets the number of items to skip
        /// Calculated property for use in database queries
        /// </summary>
        public int Skip => (PageNumber - 1) * PageSize;

        /// <summary>
        /// Gets the number of items to take
        /// Alias for PageSize for clarity in queries
        /// </summary>
        public int Take => PageSize;

        /// <summary>
        /// Initializes a new instance of PaginationParams with validation
        /// </summary>
        /// <param name="pageNumber">Page number (must be >= 1)</param>
        /// <param name="pageSize">Page size (must be between 1 and MaxPageSize)</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when parameters are invalid</exception>
        public PaginationParams(int pageNumber = DefaultPageNumber, int pageSize = DefaultPageSize)
        {
            if (pageNumber < 1)
                throw new ArgumentOutOfRangeException(
                    nameof(pageNumber),
                    pageNumber,
                    "Page number must be greater than or equal to 1");

            if (pageSize < 1)
                throw new ArgumentOutOfRangeException(
                    nameof(pageSize),
                    pageSize,
                    "Page size must be greater than or equal to 1");

            if (pageSize > MaxPageSize)
                throw new ArgumentOutOfRangeException(
                    nameof(pageSize),
                    pageSize,
                    $"Page size must not exceed {MaxPageSize}");

            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        /// <summary>
        /// Creates default pagination parameters
        /// </summary>
        public static PaginationParams Default => new(DefaultPageNumber, DefaultPageSize);
    }
}