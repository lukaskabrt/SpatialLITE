using SpatialLite.Core.API;
using System.Collections.Generic;

namespace SpatialLite.Core.Geometries;

/// <summary>
/// Represents a collection of Points
/// </summary>
public class MultiPoint : GeometryCollection<Point>, IMultiPoint
{

    /// <summary>
    /// Initializes a new instance of the MultiPoint class that is empty and has assigned WSG84 coordinate reference system.
    /// </summary>
    public MultiPoint()
        : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the MultiPoint class with specified points.
    /// </summary>
    /// <param name="points">The collection of points to be copied to the new MultiPoint.</param>
    public MultiPoint(IEnumerable<Point> points)
        : base(points)
    {
    }

    /// <summary>
    /// Gets collection of points from this Multipoint as the collection of IPoint objects.
    /// </summary>
    IEnumerable<IPoint> IGeometryCollection<IPoint>.Geometries
    {
        get { return Geometries; }
    }
}
