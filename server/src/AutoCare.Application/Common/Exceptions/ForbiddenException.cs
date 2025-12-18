using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoCare.Application.Common.Exceptions
{
    /// <summary>
    /// Exception thrown when user doesn't have permission to perform an action
    /// HTTP Status Code: 403 Forbidden
    /// </summary>
    public sealed class ForbiddenException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of ForbiddenException
        /// </summary>
        /// <param name="message">Forbidden action message</param>
        public ForbiddenException(string message)
            : base(
                message: message,
                errorCode: "FORBIDDEN")
        {
        }

        /// <summary>
        /// Initializes a new instance of ForbiddenException with resource details
        /// </summary>
        /// <param name="resource">Resource name</param>
        /// <param name="action">Action attempted</param>
        public ForbiddenException(string resource, string action)
            : base(
                message: $"You do not have permission to {action} {resource}",
                errorCode: "FORBIDDEN",
                errorDetails: new Dictionary<string, object>
                {
                    ["Resource"] = resource,
                    ["Action"] = action
                })
        {
        }
    }
}