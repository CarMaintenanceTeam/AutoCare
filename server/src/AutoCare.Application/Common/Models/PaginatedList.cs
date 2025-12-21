using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoCare.Application.Common.Models
{
    /// <summary>
    /// Represents a paginated list of items with metadata
    /// Generic wrapper for paginated query results
    /// 
    /// Design Principles:
    /// - Single Responsibility: Encapsulates paginated data with metadata
    /// - Immutability: All properties are readonly
    /// - Type Safety: Generic type ensures type safety
    /// </summary>
    /// <typeparam name="T">Type of items in the list</typeparam>
    public sealed class PaginatedList<T>
    {
        /// <summary>
        /// Gets the items in the current page
        /// </summary>
        public IReadOnlyList<T> Items { get; }

        /// <summary>
        /// Gets the current page number (1-based)
        /// </summary>
        public int PageNumber { get; }

        /// <summary>
        /// Gets the page size
        /// </summary>
        public int PageSize { get; }

        /// <summary>
        /// Gets the total number of items across all pages
        /// </summary>
        public int TotalCount { get; }

        /// <summary>
        /// Gets the total number of pages
        /// </summary>
        public int TotalPages { get; }

        /// <summary>
        /// Gets a value indicating whether there is a previous page
        /// </summary>
        public bool HasPreviousPage => PageNumber > 1;

        /// <summary>
        /// Gets a value indicating whether there is a next page
        /// </summary>
        public bool HasNextPage => PageNumber < TotalPages;

        /// <summary>
        /// Gets a value indicating whether this is the first page
        /// </summary>
        public bool IsFirstPage => PageNumber == 1;

        /// <summary>
        /// Gets a value indicating whether this is the last page
        /// </summary>
        public bool IsLastPage => PageNumber == TotalPages;

        /// <summary>
        /// Private constructor to enforce factory pattern
        /// </summary>
        private PaginatedList(
            IReadOnlyList<T> items,
            int pageNumber,
            int pageSize,
            int totalCount)
        {
            Items = items ?? throw new ArgumentNullException(nameof(items));
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        }

        /// <summary>
        /// Creates a paginated list from items and pagination parameters
        /// </summary>
        /// <param name="items">Items for current page</param>
        /// <param name="totalCount">Total count of items across all pages</param>
        /// <param name="pagination">Pagination parameters</param>
        /// <returns>Paginated list with metadata</returns>
        /// <exception cref="ArgumentNullException">Thrown when items or pagination is null</exception>
        public static PaginatedList<T> Create(
            IEnumerable<T> items,
            int totalCount,
            PaginationParams pagination)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            if (pagination == null)
                throw new ArgumentNullException(nameof(pagination));

            if (totalCount < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(totalCount),
                    totalCount,
                    "Total count cannot be negative");

            return new PaginatedList<T>(
                items.ToList(),
                pagination.PageNumber,
                pagination.PageSize,
                totalCount);
        }

        /// <summary>
        /// Creates a paginated list with page number and page size
        /// </summary>
        /// <param name="items">Items for current page</param>
        /// <param name="totalCount">Total count of items across all pages</param>
        /// <param name="pageNumber">Current page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paginated list with metadata</returns>
        public static PaginatedList<T> Create(
            IEnumerable<T> items,
            int totalCount,
            int pageNumber,
            int pageSize)
        {
            var pagination = new PaginationParams(pageNumber, pageSize);
            return Create(items, totalCount, pagination);
        }

        /// <summary>
        /// Creates an empty paginated list
        /// </summary>
        /// <param name="pagination">Pagination parameters</param>
        /// <returns>Empty paginated list</returns>
        public static PaginatedList<T> Empty(PaginationParams? pagination = null)
        {
            pagination ??= PaginationParams.Default;
            return new PaginatedList<T>(
                Array.Empty<T>(),
                pagination.PageNumber,
                pagination.PageSize,
                0);
        }

        /// <summary>
        /// Maps items to a different type while preserving pagination metadata
        /// Implements Functor pattern
        /// </summary>
        /// <typeparam name="TResult">Target type</typeparam>
        /// <param name="mapper">Mapping function</param>
        /// <returns>Paginated list of mapped items</returns>
        public PaginatedList<TResult> Map<TResult>(Func<T, TResult> mapper)
        {
            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            var mappedItems = Items.Select(mapper).ToList();

            return new PaginatedList<TResult>(
                mappedItems,
                PageNumber,
                PageSize,
                TotalCount);
        }
    }
}