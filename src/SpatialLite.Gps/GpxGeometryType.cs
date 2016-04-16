using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpatialLite.Gps {
    /// <summary>
    /// Defines possible types of GpxGeometry
    /// </summary>
    public enum GpxGeometryType {
        /// <summary>
        /// Waypoint
        /// </summary>
        Waypoint,

        /// <summary>
        /// Route
        /// </summary>
        Route,

        /// <summary>
        /// Recorder track
        /// </summary>
        Track
    }
}
