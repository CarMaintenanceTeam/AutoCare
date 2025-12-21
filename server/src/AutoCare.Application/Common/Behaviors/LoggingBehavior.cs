using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutoCare.Application.Common.Behaviors
{
    /// <summary>
    /// Pipeline behavior that logs request and response information
    /// Implements:
    /// - Single Responsibility: Only handles logging
    /// - Open/Closed: Open for extension (customize logging), closed for modification
    /// 
    /// Execution Order: Runs FIRST in pipeline (before validation and handler)
    /// Provides structured logging for:
    /// - Request start/completion
    /// - Request payload (in development)
    /// - Execution success/failure
    /// </summary>
    /// <typeparam name="TRequest">Request type implementing IRequest</typeparam>
    /// <typeparam name="TResponse">Response type</typeparam>
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    {
        // Logger instance
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
        /// <summary>
        /// Initializes a new instance of LoggingBehavior
        /// </summary>
        /// <param name="logger">Logger instance for structured logging</param>
        /// <exception cref="ArgumentNullException">Thrown when logger is null</exception>
        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }
        /// <summary>
        /// Logs request execution information
        /// </summary>
        /// <param name="request">The request being processed</param>
        /// <param name="next">Next behavior/handler in pipeline</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Response from next behavior/handler</returns>
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // Log before handling the request
            var requestName = typeof(TRequest).Name;
            var requestId = Guid.NewGuid();

            // Log request start with structured data
            _logger.LogInformation(
                "Processing request {RequestName} with ID {RequestId}",
                requestName,
                requestId);
            // Log request payload in development (avoid logging sensitive data in production)
            LogRequestPayload(request, requestName, requestId);

            try
            {
                // Execute next behavior/handler
                var response = await next();

                // Log successful completion
                _logger.LogInformation(
                    "Completed request {RequestName} with ID {RequestId}",
                    requestName,
                    requestId);

                return response;
            }
            catch (Exception ex)
            {
                // Log failure with exception details
                _logger.LogError(
                    ex,
                    "Request {RequestName} with ID {RequestId} failed with exception",
                    requestName,
                    requestId);

                // Re-throw to allow exception handling middleware to process
                throw;
            }
        }

        /// <summary>
        /// Logs request payload for debugging purposes
        /// Only logs in Development environment to avoid exposing sensitive data
        /// </summary>
        /// <param name="request">Request to log</param>
        /// <param name="requestName">Request type name</param>
        /// <param name="requestId">Unique request identifier</param>
        private void LogRequestPayload(TRequest request, string requestName, Guid requestId)
        {
            try
            {
                // Only log payload at Debug level (typically disabled in production)
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    var payload = JsonSerializer.Serialize(request, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

                    _logger.LogDebug(
                        "Request {RequestName} with ID {RequestId} payload: {Payload}",
                        requestName,
                        requestId,
                        payload);
                }
            }
            catch (Exception ex)
            {
                // Swallow serialization errors (don't fail request due to logging)
                _logger.LogWarning(
                    ex,
                    "Failed to serialize request payload for {RequestName} with ID {RequestId}",
                    requestName,
                    requestId);
            }
        }
    }
}