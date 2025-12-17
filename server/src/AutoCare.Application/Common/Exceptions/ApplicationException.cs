using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoCare.Application.Common.Exceptions
{
    /// <summary>
    /// Base exception class for all application-layer exceptions
    /// Implements Exception Hierarchy Pattern
    /// 
    /// Design Principles:
    /// - Single Responsibility: Base class for application exceptions
    /// - Open/Closed: Open for extension by specific exception types
    /// - Liskov Substitution: Can be used wherever Exception is expected
    /// 
    /// Usage: Inherit from this class for specific business exceptions
    /// </summary>
    public abstract class ApplicationException : Exception
    {
        /// <summary>
        /// Gets the error code for this exception
        /// Used for client-side error handling and logging
        /// </summary>
        public string ErrorCode { get; }

        /// <summary>
        /// Gets additional error details
        /// Optional structured data about the error
        /// </summary>
        public IDictionary<string, object>? ErrorDetails { get; }

        /// <summary>
        /// Initializes a new instance of ApplicationException
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="errorCode">Error code for categorization</param>
        /// <param name="errorDetails">Additional error details</param>
        protected ApplicationException(
            string message,
            string errorCode,
            IDictionary<string, object>? errorDetails = null)
            : base(message)
        {
            ErrorCode = errorCode ?? throw new ArgumentNullException(nameof(errorCode));
            ErrorDetails = errorDetails;
        }

        /// <summary>
        /// Initializes a new instance of ApplicationException with inner exception
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="errorCode">Error code for categorization</param>
        /// <param name="innerException">Inner exception</param>
        /// <param name="errorDetails">Additional error details</param>
        protected ApplicationException(
            string message,
            string errorCode,
            Exception innerException,
            IDictionary<string, object>? errorDetails = null)
            : base(message, innerException)
        {
            ErrorCode = errorCode ?? throw new ArgumentNullException(nameof(errorCode));
            ErrorDetails = errorDetails;
        }
    }
}