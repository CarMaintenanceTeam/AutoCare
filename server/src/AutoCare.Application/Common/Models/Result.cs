using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoCare.Application.Common.Models
{

    /// <summary>
    /// Represents the result of an operation without a return value
    /// Implements Result Pattern for better error handling without exceptions
    /// 
    /// Design Principles:
    /// - Single Responsibility: Encapsulates operation outcome
    /// - Fail Fast: Explicitly indicates success or failure
    /// - Explicit over Implicit: Clear success/failure states
    /// 
    /// Benefits:
    /// - Avoids expensive exceptions for business logic errors
    /// - Forces explicit error handling
    /// - Better testability
    /// - Clear API contracts
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Gets a value indicating whether the operation succeeded
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Gets a value indicating whether the operation failed
        /// </summary>
        public bool IsFailure => !IsSuccess;

        /// <summary>
        /// Gets the error message if operation failed
        /// Null if operation succeeded
        /// </summary>
        public string? Error { get; }

        /// <summary>
        /// Gets the collection of error messages if operation failed
        /// Empty if operation succeeded
        /// </summary>
        public IReadOnlyList<string> Errors { get; }

        /// <summary>
        /// Private constructor to enforce factory pattern
        /// </summary>
        /// <param name="isSuccess">Success indicator</param>
        /// <param name="error">Error message</param>
        /// <param name="errors">Collection of error messages</param>
        protected Result(bool isSuccess, string? error, IReadOnlyList<string>? errors = null)
        {
            // Invariant: Success means no errors, Failure means at least one error
            if (isSuccess && error != null)
                throw new InvalidOperationException("Successful result cannot have an error");

            if (!isSuccess && error == null && (errors == null || !errors.Any()))
                throw new InvalidOperationException("Failed result must have an error");

            IsSuccess = isSuccess;
            Error = error;
            Errors = errors ?? Array.Empty<string>();
        }

        #region Factory Methods

        /// <summary>
        /// Creates a successful result
        /// </summary>
        /// <returns>Success result</returns>
        public static Result Success()
        {
            return new Result(true, null);
        }

        /// <summary>
        /// Creates a failed result with a single error message
        /// </summary>
        /// <param name="error">Error message describing the failure</param>
        /// <returns>Failure result</returns>
        /// <exception cref="ArgumentNullException">Thrown when error is null or empty</exception>
        public static Result Failure(string error)
        {
            if (string.IsNullOrWhiteSpace(error))
                throw new ArgumentNullException(nameof(error), "Error message cannot be null or empty");

            return new Result(false, error);
        }

        /// <summary>
        /// Creates a failed result with multiple error messages
        /// </summary>
        /// <param name="errors">Collection of error messages</param>
        /// <returns>Failure result</returns>
        /// <exception cref="ArgumentNullException">Thrown when errors collection is null or empty</exception>
        public static Result Failure(IEnumerable<string> errors)
        {
            var errorList = errors?.Where(e => !string.IsNullOrWhiteSpace(e)).ToList();

            if (errorList == null || !errorList.Any())
                throw new ArgumentNullException(nameof(errors), "Errors collection cannot be null or empty");

            return new Result(false, errorList.First(), errorList);
        }

        /// <summary>
        /// Creates a failed result from FluentValidation errors
        /// </summary>
        /// <param name="validationErrors">Validation errors</param>
        /// <returns>Failure result</returns>
        public static Result ValidationFailure(IEnumerable<string> validationErrors)
        {
            return Failure(validationErrors);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Executes action if result is successful
        /// Implements fluent interface pattern
        /// </summary>
        /// <param name="action">Action to execute on success</param>
        /// <returns>Current result for chaining</returns>
        public Result OnSuccess(Action action)
        {
            if (IsSuccess)
                action();

            return this;
        }

        /// <summary>
        /// Executes action if result is failed
        /// Implements fluent interface pattern
        /// </summary>
        /// <param name="action">Action to execute on failure</param>
        /// <returns>Current result for chaining</returns>
        public Result OnFailure(Action<string> action)
        {
            if (IsFailure)
                action(Error!);

            return this;
        }

        #endregion
    }

    /// <summary>
    /// Represents the result of an operation with a return value
    /// Generic version of Result pattern
    /// </summary>
    /// <typeparam name="TValue">Type of the value returned on success</typeparam>
    public class Result<TValue> : Result
    {
        /// <summary>
        /// Gets the value returned by successful operation
        /// Null if operation failed
        /// </summary>
        public TValue? Value { get; }

        /// <summary>
        /// Private constructor to enforce factory pattern
        /// </summary>
        /// <param name="isSuccess">Success indicator</param>
        /// <param name="value">Return value</param>
        /// <param name="error">Error message</param>
        /// <param name="errors">Collection of error messages</param>
        private Result(bool isSuccess, TValue? value, string? error, IReadOnlyList<string>? errors = null)
            : base(isSuccess, error, errors)
        {
            // Invariant: Success means value exists, Failure means no value
            if (isSuccess && value == null)
                throw new InvalidOperationException("Successful result must have a value");

            if (!isSuccess && value != null)
                throw new InvalidOperationException("Failed result cannot have a value");

            Value = value;
        }

        #region Factory Methods

        /// <summary>
        /// Creates a successful result with a value
        /// </summary>
        /// <param name="value">Value to return</param>
        /// <returns>Success result with value</returns>
        /// <exception cref="ArgumentNullException">Thrown when value is null</exception>
        public static Result<TValue> Success(TValue value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value), "Value cannot be null for successful result");

            return new Result<TValue>(true, value, null);
        }

        /// <summary>
        /// Creates a failed result with a single error message
        /// </summary>
        /// <param name="error">Error message</param>
        /// <returns>Failure result</returns>
        public new static Result<TValue> Failure(string error)
        {
            if (string.IsNullOrWhiteSpace(error))
                throw new ArgumentNullException(nameof(error), "Error message cannot be null or empty");

            return new Result<TValue>(false, default, error);
        }

        /// <summary>
        /// Creates a failed result with multiple error messages
        /// </summary>
        /// <param name="errors">Collection of error messages</param>
        /// <returns>Failure result</returns>
        public new static Result<TValue> Failure(IEnumerable<string> errors)
        {
            var errorList = errors?.Where(e => !string.IsNullOrWhiteSpace(e)).ToList();

            if (errorList == null || !errorList.Any())
                throw new ArgumentNullException(nameof(errors), "Errors collection cannot be null or empty");

            return new Result<TValue>(false, default, errorList.First(), errorList);
        }

        /// <summary>
        /// Creates a failed result from FluentValidation errors
        /// </summary>
        /// <param name="validationErrors">Validation errors</param>
        /// <returns>Failure result</returns>
        public new static Result<TValue> ValidationFailure(IEnumerable<string> validationErrors)
        {
            return Failure(validationErrors);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Executes function if result is successful and returns new result
        /// Implements monadic bind (flatMap) pattern
        /// </summary>
        /// <typeparam name="TNewValue">Type of new value</typeparam>
        /// <param name="func">Function to execute on success</param>
        /// <returns>New result</returns>
        public Result<TNewValue> Bind<TNewValue>(Func<TValue, Result<TNewValue>> func)
        {
            if (IsFailure)
                return Result<TNewValue>.Failure(Error!);

            return func(Value!);
        }

        /// <summary>
        /// Maps successful value to new value
        /// Implements functor map pattern
        /// </summary>
        /// <typeparam name="TNewValue">Type of new value</typeparam>
        /// <param name="func">Mapping function</param>
        /// <returns>New result with mapped value</returns>
        public Result<TNewValue> Map<TNewValue>(Func<TValue, TNewValue> func)
        {
            if (IsFailure)
                return Result<TNewValue>.Failure(Error!);

            return Result<TNewValue>.Success(func(Value!));
        }

        /// <summary>
        /// Executes action if result is successful
        /// </summary>
        /// <param name="action">Action to execute with value</param>
        /// <returns>Current result for chaining</returns>
        public Result<TValue> OnSuccess(Action<TValue> action)
        {
            if (IsSuccess)
                action(Value!);

            return this;
        }

        /// <summary>
        /// Executes action if result is failed
        /// </summary>
        /// <param name="action">Action to execute with error</param>
        /// <returns>Current result for chaining</returns>
        public new Result<TValue> OnFailure(Action<string> action)
        {
            if (IsFailure)
                action(Error!);

            return this;
        }

        #endregion

        #region Implicit Operators

        /// <summary>
        /// Implicit conversion from value to successful result
        /// Enables: Result&lt;int&gt; result = 42;
        /// </summary>
        public static implicit operator Result<TValue>(TValue value)
        {
            return Success(value);
        }

        #endregion
    }
}