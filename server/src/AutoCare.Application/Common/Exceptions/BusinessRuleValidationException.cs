using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoCare.Application.Common.Exceptions
{
    /// <summary>
    /// Exception thrown when a business rule validation fails
    /// HTTP Status Code: 400 Bad Request
    /// 
    /// Example: Cannot cancel a completed booking, Cannot book in the past
    /// </summary>
    public sealed class BusinessRuleValidationException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of BusinessRuleValidationException
        /// </summary>
        /// <param name="message">Business rule violation message</param>
        public BusinessRuleValidationException(string message)
            : base(
                message: message,
                errorCode: "BUSINESS_RULE_VIOLATION")
        {
        }

        /// <summary>
        /// Initializes a new instance of BusinessRuleValidationException with details
        /// </summary>
        /// <param name="message">Business rule violation message</param>
        /// <param name="errorDetails">Additional error details</param>
        public BusinessRuleValidationException(string message, IDictionary<string, object> errorDetails)
            : base(
                message: message,
                errorCode: "BUSINESS_RULE_VIOLATION",
                errorDetails: errorDetails)
        {
        }
    }
}