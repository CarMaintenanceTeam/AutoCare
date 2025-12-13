using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoCare.Infrastructure.Identity
{
    /// <summary>
    /// Implementation of password hasher using BCrypt
    /// </summary>
    public class PasswordHasher : IPasswordHasher
    {
        /// <summary>
        /// Hashes a password using BCrypt with work factor 12
        /// </summary>
        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty", nameof(password));

            // Work factor 12 = 2^12 iterations (good balance between security and performance)
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }

        /// <summary>
        /// Verifies if password matches the hash
        /// </summary>
        public bool VerifyPassword(string password, string hash)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash))
                return false;

            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            catch
            {
                return false;
            }
        }
    }
}