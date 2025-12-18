using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace AutoCare.Application.Common.Interfaces
{
    /// <summary>
    /// Marker interface for Queries in CQRS pattern
    /// Queries represent read operations that don't modify system state
    /// 
    /// Design Principles:
    /// - Interface Segregation: Separates queries from commands
    /// - Single Responsibility: Queries only read data
    /// - Command-Query Separation: Never modifies state
    /// 
    /// Usage: Implement this interface for all query operations
    /// Example: GetBookingByIdQuery, GetAllServiceCentersQuery
    /// </summary>
    /// <typeparam name="TResponse">Response type containing query results</typeparam>
    public interface IQuery<out TResponse> : IRequest<TResponse>
    {
    }
}