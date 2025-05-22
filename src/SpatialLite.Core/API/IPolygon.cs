using System.Collections.Generic;

namespace SpatialLite.Core.API
{
    /// <summary>
    /// Defines properties and methods for polygons.
    /// </summary>
    public interface IPolygon : IGeometry
    {
        /// <summary>
        /// Gets the exterior boundary of the polygon
        /// </summary>
        ICoordinateList ExteriorRing { get; }

        /// <summary>
        /// Gets a collection of interior boundaries that define holes in the polygon
        /// </summary>
        IEnumerable<ICoordinateList> InteriorRings { get; }
    }
}
