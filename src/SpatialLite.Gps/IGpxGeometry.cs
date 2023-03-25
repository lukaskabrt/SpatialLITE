using SpatialLite.Core.Api;

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