using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutoCare.Application.Common.Behaviors
{
    /// <summary>
    /// Pipeline behavior that monitors request execution performance
    /// Implements:
    /// - Single Responsibility: Only handles performance monitoring
    /// - Open/Closed: Open for extension (custom thresholds), closed for modification
    /// 
    /// Execution Order: Runs LAST in pipeline (wraps entire execution)
    /// 
    /// Features:
    /// - Measures total execution time
    /// - Logs warnings for slow requests (> threshold)
    /// - Provides performance metrics for monitoring
    /// </summary>
    /// <typeparam name="TRequest">Request type implementing IRequest</typeparam>
    /// <typeparam name="TResponse">Response type</typeparam>
    public sealed class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;

        /// <summary>
        /// Performance threshold in milliseconds
        /// Requests exceeding this threshold will be logged as warnings
        /// </summary>
        private const int PerformanceThresholdMs = 500;

        /// <summary>
        /// Initializes a new instance of PerformanceBehavior
        /// </summary>
        /// <param name="logger">Logger instance for performance metrics</param>
        /// <exception cref="ArgumentNullException">Thrown when logger is null</exception>
        public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Monitors request execution performance
        /// </summary>
        /// <param name="request">The request being processed</param>
        /// <param name="next">Next behavior/handler in pipeline</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Response from next behavior/handler</returns>
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var stopwatch = Stopwatch.StartNew();

            try
            {
                // Execute next behavior/handler
                var response = await next();

                stopwatch.Stop();

                // Check if execution exceeded performance threshold
                var elapsedMs = stopwatch.ElapsedMilliseconds;

                if (elapsedMs > PerformanceThresholdMs)
                {
                    LogSlowRequest(requestName, elapsedMs);
                }
                else
                {
                    LogNormalRequest(requestName, elapsedMs);
                }

                return response;
            }
            catch (Exception)
            {
                stopwatch.Stop();

                // Log performance even for failed requests
                _logger.LogWarning(
                    "Request {RequestName} failed after {ElapsedMs}ms",
                    requestName,
                    stopwatch.ElapsedMilliseconds);

                throw;
            }
        }

        /// <summary>
        /// Logs slow request warning with detailed performance metrics
        /// </summary>
        /// <param name="requestName">Name of the request type</param>
        /// <param name="elapsedMs">Elapsed time in milliseconds</param>
        private void LogSlowRequest(string requestName, long elapsedMs)
        {
            _logger.LogWarning(
                "Slow Request: {RequestName} took {ElapsedMs}ms (Threshold: {ThresholdMs}ms). " +
                "Consider optimization: check database queries, external API calls, or add caching.",
                requestName,
                elapsedMs,
                PerformanceThresholdMs);
        }

        /// <summary>
        /// Logs normal request completion with performance metrics
        /// </summary>
        /// <param name="requestName">Name of the request type</param>
        /// <param name="elapsedMs">Elapsed time in milliseconds</param>
        private void LogNormalRequest(string requestName, long elapsedMs)
        {
            _logger.LogInformation(
                "Request {RequestName} completed in {ElapsedMs}ms",
                requestName,
                elapsedMs);
        }
    }
}