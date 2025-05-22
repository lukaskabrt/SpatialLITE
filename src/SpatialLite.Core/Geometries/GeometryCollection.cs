using SpatialLite.Core.API;
using System.Collections.Generic;
using System.Linq;

namespace SpatialLite.Core.Geometries;

/// <summary>
/// Represents generic collection of geometry objects.
/// </summary>
/// <remarks>All objects should be in the same spatial reference system, but it isn't enforced by this class.</remarks>
/// <typeparam name="T">The type of objects in the collection</typeparam>
public class GeometryCollection<T> : Geometry, IGeometryCollection<T> where T : IGeometry
{
    private readonly List<T> _geometries;

    /// <summary>
    /// Initializes a new instance of the <c>GeometryCollection</c> class that is empty and has assigned WSG84 coordinate reference system.
    /// </summary>
    public GeometryCollection()
        : base()
    {
        _geometries = new List<T>();
    }

    /// <summary>
    /// Initializes a new instance of the <c>GeometryCollection</c> class in WSG84 coordinate reference system and fills it with specified geometries.
    /// </summary>
    /// <param name="geometries">Geometry objects to be added to the collection</param>
    public GeometryCollection(IEnumerable<T> geometries)
        : base()
    {
        _geometries = new List<T>(geometries);
    }

    /// <summary>
    /// Gets a value indicating whether the this <see cref="GeometryCollection{T}"/>"/> has Z ordinates set.
    /// </summary>
    /// <remarks>
    /// Is3D returns <c>true</c> if any of the geometries contained in this <c>GeometryCollection</c> has Z ordinate set.
    /// </remarks>
    public override bool Is3D
    {
        get
        {
            return _geometries.Any(geometry => geometry.Is3D);
        }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="Geometry"/> has M values.
    /// </summary>
    /// <remarks>
    /// IsMeasured returns <c>true</c> if any of the geometries in this <c>GeometryCollection</c> has M value set.
    /// </remarks>
    public override bool IsMeasured
    {
        get { return _geometries.Any(c => c.IsMeasured); }
    }

    /// <summary>
    /// Gets the list of IGeometry objects in this collection
    /// </summary>
    public List<T> Geometries
    {
        get
        {
            return _geometries;
        }
    }

    /// <summary>
    /// Gets collection of geometry obejcts from this GeometryCollection as the collection of IGeometry objects.
    /// </summary>
    IEnumerable<T> IGeometryCollection<T>.Geometries
    {
        get { return _geometries; }
    }

    /// <summary>
    /// Computes envelope of the <c>GeometryCollection</c> object. The envelope is defined as a minimal bounding box for a geometry.
    /// </summary>
    /// <returns>
    /// Returns an <see cref="Envelope"/> object that specifies the minimal bounding box of the <c>GeometryCollection</c> object.
    /// </returns>
    public override Envelope GetEnvelope()
    {
        Envelope result = new();
        foreach (var item in _geometries)
        {
            result.Extend(item.GetEnvelope());
        }

        return result;
    }

    /// <summary>
    /// Gets collection of all <see cref="Coordinate"/> of this IGeometry object
    /// </summary>
    /// <returns>the collection of all <see cref="Coordinate"/> of this object</returns>
    public override IEnumerable<Coordinate> GetCoordinates()
    {
        return Geometries.SelectMany(o => o.GetCoordinates());
    }
}
