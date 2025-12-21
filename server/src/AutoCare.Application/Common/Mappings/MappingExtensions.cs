using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Models;
using Mapster;

namespace AutoCare.Application.Common.Mappings
{
    public static class MappingExtensions
    {
        /// <summary>
        /// Maps source object to destination type
        /// </summary>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <returns>Mapped destination object</returns>
        public static TDestination MapTo<TDestination>(this object source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source.Adapt<TDestination>();
        }

        /// <summary>
        /// Maps source collection to destination collection
        /// </summary>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source collection</param>
        /// <returns>Mapped destination collection</returns>
        public static List<TDestination> MapToList<TDestination>(this IEnumerable<object> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source.Select(x => x.Adapt<TDestination>()).ToList();
        }

        /// <summary>
        /// Maps paginated list to DTO paginated list
        /// Preserves pagination metadata
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source paginated list</param>
        /// <returns>Mapped destination paginated list</returns>
        public static PaginatedList<TDestination> MapToPaginated<TSource, TDestination>(
            this PaginatedList<TSource> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source.Map(item => item.Adapt<TDestination>());
        }
    }
}