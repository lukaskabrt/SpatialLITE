using System;
using SpatialLite.Core.API;
using SpatialLite.Core.Geometries;

namespace SpatialLite.Gps.Geometries {
    /// <summary>
    /// Represents location on the earth surface with timestamp that defines time when the point was recorded 
    /// </summary>
    public class GpsPoint : Point, IGpsPoint {

        /// <summary>
        /// Creates a new, empty instance of the GpsPoint
        /// </summary>
        public GpsPoint() {
        }

        /// <summary>
        /// Creates a new instance of the GpsPoint with given position
        /// </summary>
        /// <param name="position">The position of the point</param>
        public GpsPoint(Coordinate position)
            : base(position) {
        }

        /// <summary>
        /// Creates a new instance of the GpsPoint with given coordinates and timestamp
        /// </summary>
        /// <param name="longitude">The longitude of the point</param>
        /// <param name="latitude">The latitude of the point</param>
        /// <param name="elevation">The elevation of the point</param>
        /// <param name="time">The time when the point was recorded</param>
        public GpsPoint(double longitude, double latitude, double elevation, DateTime time)
            : base(longitude, latitude, elevation) {
            Timestamp = time;
        }

        /// <summary>
        /// Creates a new instance of the GpsPoint with given position and time
        /// </summary>
        /// <param name="position">The position of the point</param>
        /// <param name="time">The time when the point was recorded</param>
        public GpsPoint(Coordinate position, DateTime time)
            : base(position) {
            Timestamp = time;
        }

        /// <summary>
        /// Gets or sets time when the point was recorded.
        /// </summary>
        public DateTime? Timestamp { get; set; }        
    }
}
