using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutoCare.Application.Common.Behaviors
{
    /// <summary>
    /// Pipeline behavior that validates requests using FluentValidation
    /// Implements:
    /// - Single Responsibility: Only handles validation
    /// - Open/Closed: Open for extension (add validators), closed for modification
    /// - Dependency Inversion: Depends on IValidator abstraction
    /// 
    /// Execution Order: Runs AFTER LoggingBehavior, BEFORE handler execution
    /// </summary>
    /// <typeparam name="TRequest">Request type implementing IRequest</typeparam>
    /// <typeparam name="TResponse">Response type</typeparam>
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        /// <summary>
        /// Initializes a new instance of ValidationBehavior
        /// </summary>
        /// <param name="validators">Collection of validators for the request type</param>
        /// <param name="logger">Logger for validation events</param>
        /// <exception cref="ArgumentNullException">Thrown when logger is null</exception>
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators, ILogger<ValidationBehavior<TRequest, TResponse>> logger)
        {
            _validators = validators ?? Enumerable.Empty<IValidator<TRequest>>();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Validates the request before passing to the next behavior/handler
        /// </summary>
        /// <param name="request">The request to validate</param>
        /// <param name="next">Next behavior/handler in pipeline</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Response from next behavior/handler</returns>
        /// <exception cref="ValidationException">Thrown when validation fails</exception>
        public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
        {
            // Log the validation attempt
            var requestName = typeof(TRequest).Name;

            // If there are no validators, proceed to the next behavior/handler
            if (!_validators.Any())
            {
                // Log absence of validators
                _logger.LogDebug(
                "No validators found for {RequestName}, skipping validation",
                requestName);

                return await next();
            }
            // Log number of validators found
            _logger.LogDebug(
                "Validating {RequestName} with {ValidatorCount} validator(s)",
                requestName,
                _validators.Count());

            // Create a validation context
            var context = new ValidationContext<TRequest>(request);

            // Execute all validators in parallel for better performance
            var validationResults = await Task.WhenAll(
                _validators.Select(validator =>
                validator.ValidateAsync(context, cancellationToken)));

            // Collect all validation failures
            var failures = validationResults
                .SelectMany(result => result.Errors)
                .Where(failure => failure != null)
                .ToList();

            // If there are validation failures, throw an exception
            if (failures.Count != 0)
            {
                // Log validation failures
                _logger.LogWarning(
                "Validation failed for {RequestName}. Errors: {ErrorCount}",
                requestName,
                failures.Count);

                throw new ValidationException(failures);
            }
            // Log successful validation
            _logger.LogDebug(
            "Validation succeeded for {RequestName}",
            requestName);

            // validation passed, Proceed to the next behavior/handler
            return await next();
        }


    }
}