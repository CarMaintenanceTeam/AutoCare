using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoCare.Application.Common.Exceptions
{
    /// <summary>
    /// Exception thrown when an external service call fails
    /// HTTP Status Code: 503 Service Unavailable
    /// 
    /// Example: Email service down, SMS service unavailable
    /// </summary>
    public sealed class ExternalServiceException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of ExternalServiceException
        /// </summary>
        /// <param name="serviceName">Name of the external service</param>
        /// <param name="message">Error message</param>
        /// <param name="innerException">Original exception from service</param>
        public ExternalServiceException(string serviceName, string message, Exception innerException)
            : base(
                message: $"External service '{serviceName}' failed: {message}",
                errorCode: "EXTERNAL_SERVICE_ERROR",
                innerException: innerException,
                errorDetails: new Dictionary<string, object>
                {
                    ["ServiceName"] = serviceName
                })
        {
        }
    }
}