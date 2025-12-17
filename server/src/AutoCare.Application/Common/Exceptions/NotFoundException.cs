using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoCare.Application.Common.Exceptions
{
    /// <summary>
    /// Exception thrown when a requested entity is not found
    /// HTTP Status Code: 404 Not Found
    /// 
    /// Design Principle: Specific exception for specific scenario
    /// </summary>
    public sealed class NotFoundException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of NotFoundException
        /// </summary>
        /// <param name="entityName">Name of the entity type</param>
        /// <param name="entityId">ID of the entity that was not found</param>
        public NotFoundException(string entityName, object entityId)
            : base(
                message: $"{entityName} with ID '{entityId}' was not found",
                errorCode: "NOT_FOUND",
                errorDetails: new Dictionary<string, object>
                {
                    ["EntityName"] = entityName,
                    ["EntityId"] = entityId
                })
        {
        }

        /// <summary>
        /// Initializes a new instance of NotFoundException with custom message
        /// </summary>
        /// <param name="message">Custom error message</param>
        public NotFoundException(string message)
            : base(
                message: message,
                errorCode: "NOT_FOUND")
        {
        }
    }
}