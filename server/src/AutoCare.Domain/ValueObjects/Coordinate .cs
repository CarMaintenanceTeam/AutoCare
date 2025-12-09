using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoCare.Domain.ValueObjects
{
    /// <summary>
    /// GPS Coordinate value object (Latitude, Longitude)
    /// Ensures coordinates are always valid
    /// </summary>
    public sealed class Coordinate : IEquatable<Coordinate>
    {


        /// <summary>
        /// Gets the latitude value (-90 to 90)
        /// </summary>
        public decimal Latitude { get; }

        /// <summary>
        /// Gets the longitude value (-180 to 180)
        /// </summary>
        public decimal Longitude { get; }

        /// <summary>
        /// Private constructor to enforce factory pattern
        /// </summary>
        private Coordinate(decimal latitude, decimal longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        /// <summary>
        /// Creates a new Coordinate value object with validation
        /// </summary>
        /// <param name="latitude">Latitude value</param>
        /// <param name="longitude">Longitude value</param>
        /// <returns>A validated Coordinate value object</returns>
        /// <exception cref="ArgumentException">Thrown when coordinates are out of range</exception>
        public static Coordinate Create(decimal latitude, decimal longitude)
        {
            // Validate latitude range
            if (latitude < -90 || latitude > 90)
                throw new ArgumentException(
                    "Latitude must be between -90 and 90 degrees",
                    nameof(latitude));

            // Validate longitude range
            if (longitude < -180 || longitude > 180)
                throw new ArgumentException(
                    "Longitude must be between -180 and 180 degrees",
                    nameof(longitude));

            return new Coordinate(latitude, longitude);
        }

        /// <summary>
        /// Calculates the distance to another coordinate using Haversine formula
        /// </summary>
        /// <param name="other">The other coordinate</param>
        /// <returns>Distance in kilometers</returns>
        public double DistanceTo(Coordinate other)
        {
            const double EarthRadiusKm = 6371.0;

            var dLat = ToRadians((double)(other.Latitude - Latitude));
            var dLon = ToRadians((double)(other.Longitude - Longitude));

            var lat1 = ToRadians((double)Latitude);
            var lat2 = ToRadians((double)other.Latitude);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2) *
                    Math.Cos(lat1) * Math.Cos(lat2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return EarthRadiusKm * c;
        }

        /// <summary>
        /// Converts degrees to radians
        /// </summary>
        private static double ToRadians(double degrees)
        {
            return degrees * (Math.PI / 180.0);
        }

        #region Equality & Comparison

        /// <summary>
        /// Determines whether two coordinates are equal
        /// </summary>
        public bool Equals(Coordinate? other)
        {
            if (other is null)
                return false;

            return Latitude == other.Latitude && Longitude == other.Longitude;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current coordinate
        /// </summary>
        public override bool Equals(object? obj)
        {
            return Equals(obj as Coordinate);
        }

        /// <summary>
        /// Returns the hash code for this coordinate
        /// </summary>
        public override int GetHashCode()
        {
            return HashCode.Combine(Latitude, Longitude);
        }

        /// <summary>
        /// Returns the coordinate as a string in format "(Lat, Lon)"
        /// </summary>
        public override string ToString()
        {
            return $"({Latitude:F6}, {Longitude:F6})";
        }

        #endregion

        #region Operators

        /// <summary>
        /// Equality operator
        /// </summary>
        public static bool operator ==(Coordinate? left, Coordinate? right)
        {
            if (left is null && right is null)
                return true;

            if (left is null || right is null)
                return false;

            return left.Equals(right);
        }

        /// <summary>
        /// Inequality operator
        /// </summary>
        public static bool operator !=(Coordinate? left, Coordinate? right)
        {
            return !(left == right);
        }

        #endregion

    }
}