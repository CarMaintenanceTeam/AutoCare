using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;

namespace AutoCare.Application.Common.Mappings
{
    /// <summary>
    /// Base interface for mappings
    /// Implement this in feature folders for feature-specific mappings
    /// 
    /// Usage:
    /// public class BookingMappingConfig : IRegister
    /// {
    ///     public void Register(TypeAdapterConfig config)
    ///     {
    ///         config.NewConfig&lt;Booking, BookingDto&gt;()
    ///             .Map(dest => dest.CustomerName, src => src.Customer.User.FullName);
    ///     }
    /// }
    /// </summary>
    public interface IMapsterRegister
    {
        /// <summary>
        /// Registers mapping configuration
        /// </summary>
        /// <param name="config">Type adapter configuration</param>
        void Register(TypeAdapterConfig config);
    }
}