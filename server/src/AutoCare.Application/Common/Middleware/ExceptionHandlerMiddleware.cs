using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using AutoCare.Application.Common.Models;
using AutoCare.Application.Common.Exceptions;
using System.Net;
using FluentValidation;



namespace AutoCare.Application.Common.Middleware
{
    /// <summary>
    /// Global exception handler middleware
    /// Implements centralized exception handling for all API requests
    /// 
    /// Design Principles:
    /// - Single Responsibility: Only handles exceptions
    /// - Don't Repeat Yourself: Centralized error handling
    /// - Fail Fast: Catches and handles all exceptions
    /// 
    /// Features:
    /// - Maps exceptions to appropriate HTTP status codes
    /// - Logs all exceptions with context
    /// - Returns consistent error responses
    /// - Handles validation errors specially
    /// </summary>
    public sealed class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        /// <summary>
        /// Initializes a new instance of ExceptionHandlerMiddleware
        /// </summary>
        /// <param name="next">Next middleware in pipeline</param>
        /// <param name="logger">Logger instance</param>
        public ExceptionHandlerMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Invokes the middleware
        /// </summary>
        /// <param name="context">HTTP context</param>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Pass request to next middleware
                await _next(context);
            }
            catch (Exception ex)
            {
                // Handle exception and return error response
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Handles exception and returns appropriate error response
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <param name="exception">Exception to handle</param>
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Log exception with full details
            LogException(context, exception);

            // Prepare error response
            var (statusCode, errorResponse) = MapExceptionToResponse(exception);

            // Set response properties
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            // Serialize and write response
            var json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }

        /// <summary>
        /// Maps exception to HTTP status code and error response
        /// Implements Strategy Pattern for different exception types
        /// </summary>
        /// <param name="exception">Exception to map</param>
        /// <returns>HTTP status code and error response</returns>
        private (HttpStatusCode StatusCode, ApiResponse<object> Response) MapExceptionToResponse(
            Exception exception)
        {
            return exception switch
            {
                // FluentValidation errors (400 Bad Request)
                ValidationException validationEx => (
                    HttpStatusCode.BadRequest,
                    ApiResponse<object>.ErrorResponse(
                        validationEx.Errors.Select(e => e.ErrorMessage),
                        "VALIDATION_ERROR")),

                // Not Found (404)
                NotFoundException notFoundEx => (
                    HttpStatusCode.NotFound,
                    ApiResponse<object>.ErrorResponse(
                        notFoundEx.Message,
                        notFoundEx.ErrorCode)),

                // Business Rule Violation (400 Bad Request)
                BusinessRuleValidationException businessEx => (
                    HttpStatusCode.BadRequest,
                    ApiResponse<object>.ErrorResponse(
                        businessEx.Message,
                        businessEx.ErrorCode)),

                // Duplicate Entity (409 Conflict)
                DuplicateException duplicateEx => (
                    HttpStatusCode.Conflict,
                    ApiResponse<object>.ErrorResponse(
                        duplicateEx.Message,
                        duplicateEx.ErrorCode)),

                // Unauthorized (401)
                UnauthorizedException unauthorizedEx => (
                    HttpStatusCode.Unauthorized,
                    ApiResponse<object>.ErrorResponse(
                        unauthorizedEx.Message,
                        unauthorizedEx.ErrorCode)),

                // Forbidden (403)
                ForbiddenException forbiddenEx => (
                    HttpStatusCode.Forbidden,
                    ApiResponse<object>.ErrorResponse(
                        forbiddenEx.Message,
                        forbiddenEx.ErrorCode)),

                // Concurrency Conflict (409)
                ConcurrencyException concurrencyEx => (
                    HttpStatusCode.Conflict,
                    ApiResponse<object>.ErrorResponse(
                        concurrencyEx.Message,
                        concurrencyEx.ErrorCode)),

                // External Service Error (503 Service Unavailable)
                ExternalServiceException serviceEx => (
                    HttpStatusCode.ServiceUnavailable,
                    ApiResponse<object>.ErrorResponse(
                        serviceEx.Message,
                        serviceEx.ErrorCode)),

                // Generic Application Exception (400 Bad Request)
                Exceptions.ApplicationException appEx => (
                    HttpStatusCode.BadRequest,
                    ApiResponse<object>.ErrorResponse(
                        appEx.Message,
                        appEx.ErrorCode)),

                // Unhandled exceptions (500 Internal Server Error)
                _ => (
                    HttpStatusCode.InternalServerError,
                    ApiResponse<object>.ErrorResponse(
                        "An unexpected error occurred. Please try again later.",
                        "INTERNAL_SERVER_ERROR"))
            };
        }

        /// <summary>
        /// Logs exception with context information
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <param name="exception">Exception to log</param>
        private void LogException(HttpContext context, Exception exception)
        {
            // Determine log level based on exception type
            var logLevel = exception switch
            {
                ValidationException => LogLevel.Warning,
                NotFoundException => LogLevel.Warning,
                BusinessRuleValidationException => LogLevel.Warning,
                UnauthorizedException => LogLevel.Warning,
                ForbiddenException => LogLevel.Warning,
                Exceptions.ApplicationException => LogLevel.Error,
                _ => LogLevel.Error
            };

            // Log with context
            _logger.Log(
                logLevel,
                exception,
                "Exception occurred while processing request {Method} {Path}. User: {User}",
                context.Request.Method,
                context.Request.Path,
                context.User?.Identity?.Name ?? "Anonymous");
        }
    }
}