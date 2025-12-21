using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoCare.Application.Common.Interfaces
{
    /// <summary>
    /// Service for hashing and verifying passwords using BCrypt
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// Hashes a password
        /// </summary>
        string HashPassword(string password);

        /// <summary>
        /// Verifies a password against a hash
        /// </summary>
        bool VerifyPassword(string password, string hash);
    }
}