using System.Collections.Generic;

namespace SpatialLite.Core.Api;

/// <summary>
/// Represents a polygon.
/// </summary>
public interface IPolygon : IGeometry
{
    /// <summary>
    /// Gets the exterior boundary of the polygon.
    /// </summary>
    ICoordinateSequence ExteriorRing { get; }

    /// <summary>
    /// Gets a collection of interior boundaries that define holes in the polygon.
    /// </summary>
    IEnumerable<ICoordinateSequence> InteriorRings { get; }
}
