using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialLite.Core.API;

namespace SpatialLite.Gps {
    /// <summary>
    /// Defines common properties for all GpxGeometry types
    /// </summary>
    public interface IGpxGeometry : IGeometry {
        /// <summary>
        /// Get the type of geometry
        /// </summary>
        GpxGeometryType GeometryType { get; }
    }
}
