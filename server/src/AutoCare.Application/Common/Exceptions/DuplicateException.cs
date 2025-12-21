using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoCare.Application.Common.Exceptions
{
    /// <summary>
    /// Exception thrown when a duplicate entity is detected
    /// HTTP Status Code: 409 Conflict
    /// 
    /// Example: Email already exists, Plate number already registered
    /// </summary>
    public sealed class DuplicateException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of DuplicateException
        /// </summary>
        /// <param name="entityName">Name of the entity type</param>
        /// <param name="propertyName">Name of the property that has duplicate value</param>
        /// <param name="propertyValue">The duplicate value</param>
        public DuplicateException(string entityName, string propertyName, object propertyValue)
            : base(
                message: $"{entityName} with {propertyName} '{propertyValue}' already exists",
                errorCode: "DUPLICATE_ENTITY",
                errorDetails: new Dictionary<string, object>
                {
                    ["EntityName"] = entityName,
                    ["PropertyName"] = propertyName,
                    ["PropertyValue"] = propertyValue
                })
        {
        }
    }
}