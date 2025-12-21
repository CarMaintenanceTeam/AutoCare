using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoCare.Application.Common.Models
{
    /// <summary>
    /// Response wrapper for API responses
    /// Provides consistent response structure across all endpoints
    /// 
    /// Design Principles:
    /// - Consistency: All API responses follow same structure
    /// - Clarity: Clear success/failure indication
    /// - Extensibility: Can add metadata without breaking clients
    /// </summary>
    /// <typeparam name="T">Type of data in response</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Gets or sets a value indicating whether the request succeeded
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the response data
        /// Null if request failed
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Gets or sets the error message if request failed
        /// Null if request succeeded
        /// </summary>
        public string? Error { get; set; }

        /// <summary>
        /// Gets or sets the collection of validation errors
        /// Empty if no validation errors
        /// </summary>
        public List<string> Errors { get; set; } = new();

        /// <summary>
        /// Gets or sets the error code for categorization
        /// Null if request succeeded
        /// </summary>
        public string? ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets additional metadata about the response
        /// </summary>
        public Dictionary<string, object>? Metadata { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the response
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        #region Factory Methods

        /// <summary>
        /// Creates a successful API response with data
        /// </summary>
        /// <param name="data">Response data</param>
        /// <param name="metadata">Optional metadata</param>
        /// <returns>Successful API response</returns>
        public static ApiResponse<T> SuccessResponse(T data, Dictionary<string, object>? metadata = null)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Metadata = metadata
            };
        }

        /// <summary>
        /// Creates a failed API response with single error
        /// </summary>
        /// <param name="error">Error message</param>
        /// <param name="errorCode">Error code</param>
        /// <returns>Failed API response</returns>
        public static ApiResponse<T> ErrorResponse(string error, string? errorCode = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Error = error,
                ErrorCode = errorCode,
                Errors = new List<string> { error }
            };
        }

        /// <summary>
        /// Creates a failed API response with multiple errors
        /// </summary>
        /// <param name="errors">Collection of error messages</param>
        /// <param name="errorCode">Error code</param>
        /// <returns>Failed API response</returns>
        public static ApiResponse<T> ErrorResponse(IEnumerable<string> errors, string? errorCode = null)
        {
            var errorList = errors.ToList();
            return new ApiResponse<T>
            {
                Success = false,
                Error = errorList.FirstOrDefault(),
                ErrorCode = errorCode,
                Errors = errorList
            };
        }

        /// <summary>
        /// Creates a failed API response from Result
        /// </summary>
        /// <param name="result">Failed result</param>
        /// <returns>Failed API response</returns>
        public static ApiResponse<T> FromResult(Result result)
        {
            return ErrorResponse(
                result.Errors.Any() ? result.Errors : new[] { result.Error! },
                "OPERATION_FAILED");
        }

        /// <summary>
        /// Creates a failed API response from Result with value
        /// </summary>
        /// <param name="result">Failed result with value</param>
        /// <returns>Failed API response</returns>
        public static ApiResponse<T> FromResult(Result<T> result)
        {
            if (result.IsSuccess)
                return SuccessResponse(result.Value!);

            return ErrorResponse(
                result.Errors.Any() ? result.Errors : new[] { result.Error! },
                "OPERATION_FAILED");
        }

        #endregion
    }
}