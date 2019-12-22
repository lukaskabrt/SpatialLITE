using System.Collections.Generic;

namespace SpatialLite.Gps.Geometries {
    /// <summary>
    /// Represents route - an ordered list of waypoints representing a series of turn points leading to a destination.
    /// </summary>
    public class GpxRoute : GpxTrackSegment, IGpxGeometry {
        /// <summary>
        /// Get or sets additional info about the route
        /// </summary>
        public GpxTrackMetadata Metadata { get; set; }

        /// <summary>
        /// Get the type of geometry
        /// </summary>
        public GpxGeometryType GeometryType {
            get { return GpxGeometryType.Route; }
        }

        /// <summary>
        /// Creates a new, empty instance of the GpxRoute class
        /// </summary>
        public GpxRoute()
            : base() {
        }

        /// <summary>
        /// Creates a new instance of the GpxRoute class with given points
        /// </summary>
        /// <param name="points">The points to be added to the route</param>
        public GpxRoute(IEnumerable<GpxPoint> points)
            : base(points) {
        }
    }
}
