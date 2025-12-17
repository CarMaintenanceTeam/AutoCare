using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoCare.Application.Common.Exceptions
{
    /// <summary>
    /// Exception thrown when authentication fails
    /// HTTP Status Code: 401 Unauthorized
    /// </summary>
    public sealed class UnauthorizedException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of UnauthorizedException
        /// </summary>
        /// <param name="message">Authentication failure message</param>
        public UnauthorizedException(string message)
            : base(
                message: message,
                errorCode: "UNAUTHORIZED")
        {
        }

        /// <summary>
        /// Creates UnauthorizedException for invalid credentials
        /// </summary>
        public static UnauthorizedException InvalidCredentials()
        {
            return new UnauthorizedException("Invalid email or password");
        }

        /// <summary>
        /// Creates UnauthorizedException for expired token
        /// </summary>
        public static UnauthorizedException TokenExpired()
        {
            return new UnauthorizedException("Authentication token has expired");
        }

        /// <summary>
        /// Creates UnauthorizedException for invalid token
        /// </summary>
        public static UnauthorizedException InvalidToken()
        {
            return new UnauthorizedException("Invalid authentication token");
        }
    }
}