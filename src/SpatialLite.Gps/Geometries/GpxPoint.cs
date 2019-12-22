using System;
using SpatialLite.Core.API;

namespace SpatialLite.Gps.Geometries {
    /// <summary>
    /// Represents a Gpx point
    /// </summary>
    public class GpxPoint : GpsPoint, IGpxGeometry {

        /// <summary>
        /// Creates a new, empty instance of the GpxPoint
        /// </summary>
        public GpxPoint() {
        }

        /// <summary>
        /// Creates a new instance of the GpxPoint with given position
        /// </summary>
        /// <param name="position">The position of the point</param>
        public GpxPoint(Coordinate position)
            : base(position) {
        }

        /// <summary>
        /// Creates a new instance of the GpxPoint with given coordinates and timestamp
        /// </summary>
        /// <param name="longitude">The longitude of the point</param>
        /// <param name="latitude">The latitude of the point</param>
        /// <param name="elevation">The elevation of the point</param>
        /// <param name="time">The time when the point was recorded</param>
        public GpxPoint(double longitude, double latitude, double elevation, DateTime time)
            : base(longitude, latitude, elevation, time) {
        }

        /// <summary>
        /// Creates a new instance of the GpxPoint with given position and time
        /// </summary>
        /// <param name="position">The position of the point</param>
        /// <param name="time">The time when the point was recorded</param>
        public GpxPoint(Coordinate position, DateTime time)
            : base(position) {
            Timestamp = time;
        }

        /// <summary>
        /// Gets or sets additional information about point
        /// </summary>
        public GpxPointMetadata Metadata { get; set; }

        /// <summary>
        /// Get the type of geometry
        /// </summary>
        public GpxGeometryType GeometryType {
            get { return GpxGeometryType.Waypoint; }
        }
    }
}
