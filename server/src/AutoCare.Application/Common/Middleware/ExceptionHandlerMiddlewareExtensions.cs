using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace AutoCare.Application.Common.Middleware
{
    /// <summary>
    /// Extension methods for registering exception handler middleware
    /// </summary>
    public static class ExceptionHandlerMiddlewareExtensions
    {
        // <summary>
        /// Adds global exception handler middleware to pipeline
        /// Must be called early in middleware pipeline (before MVC)
        /// </summary>
        /// <param name="builder">Application builder</param>
        /// <returns>Application builder for chaining</returns>
        public static IApplicationBuilder UseGlobalExceptionHandler(
            this IApplicationBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            return builder.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}