using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoCare.Application.Common.Models
{
    /// <summary>
    /// Extension methods for creating paginated lists from IQueryable
    /// </summary>
    public static class PaginationExtensions
    {
        /// <summary>
        /// Converts IQueryable to paginated list asynchronously
        /// Executes COUNT and SELECT queries efficiently
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="query">Source query</param>
        /// <param name="pagination">Pagination parameters</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated list</returns>
        public static async Task<PaginatedList<T>> ToPaginatedListAsync<T>(
            this IQueryable<T> query,
            PaginationParams pagination,
            CancellationToken cancellationToken = default)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            if (pagination == null)
                throw new ArgumentNullException(nameof(pagination));

            // Get total count (executes COUNT query)
            var totalCount = await Task.Run(() => query.Count(), cancellationToken);

            // Get items for current page (executes SELECT query with SKIP/TAKE)
            var items = await Task.Run(() =>
                query.Skip(pagination.Skip)
                     .Take(pagination.Take)
                     .ToList(),
                cancellationToken);

            return PaginatedList<T>.Create(items, totalCount, pagination);
        }
    }
}