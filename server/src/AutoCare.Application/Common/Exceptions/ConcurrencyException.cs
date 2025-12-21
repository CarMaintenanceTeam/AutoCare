using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoCare.Application.Common.Exceptions
{
    /// <summary>
    /// Exception thrown when a concurrency conflict is detected
    /// HTTP Status Code: 409 Conflict
    /// 
    /// Example: Record was modified by another user
    /// </summary>
    public sealed class ConcurrencyException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of ConcurrencyException
        /// </summary>
        /// <param name="entityName">Name of the entity type</param>
        /// <param name="entityId">ID of the entity</param>
        public ConcurrencyException(string entityName, object entityId)
            : base(
                message: $"{entityName} with ID '{entityId}' was modified by another user. Please refresh and try again.",
                errorCode: "CONCURRENCY_CONFLICT",
                errorDetails: new Dictionary<string, object>
                {
                    ["EntityName"] = entityName,
                    ["EntityId"] = entityId
                })
        {
        }
    }
}