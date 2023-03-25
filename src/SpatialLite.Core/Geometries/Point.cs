using SpatialLite.Core.Api;
using System.Collections.Generic;

namespace SpatialLite.Core.Geometries;

/// <summary>
/// Represents a location in the coordinate space.
/// </summary>
public class Point : Geometry, IPoint
{

    private Coordinate _position = Coordinate.Empty;

    /// <summary>
    /// Initializes a new instance of the <c>Point</c> class that is empty.
    /// </summary>
    public Point()
        : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <c>Point</c> class with specified X and Y coordinates.
    /// </summary>
    /// <param name="x">The X-coordinate of the <c>Point</c></param>
    /// <param name="y">The Y-coordinate of the <c>Point</c></param>
    public Point(double x, double y)
        : base()
    {
        _position = new Coordinate(x, y);
    }

    /// <summary>
    /// Initializes a new instance of the <c>Point</c> class with specified X, Y and Z coordinates.
    /// </summary>
    /// <param name="x">The X-coordinate of the <c>Point</c></param>
    /// <param name="y">The Y-coordinate of the <c>Point</c></param>
    /// <param name="z">The Z-coordinate of the <c>Point</c></param>
    public Point(double x, double y, double z)
        : base()
    {
        _position = new Coordinate(x, y, z);
    }

    /// <summary>
    /// Initializes a new instance of the <c>Point</c> class with specific <c>Position</c> in WSG84 coordinate reference system.
    /// </summary>
    /// <param name="position">The position of this <c>Point</c></param>
    public Point(Coordinate position)
        : base()
    {
        _position = position;
    }

    /// <summary>
    /// Gets or sets position of this <c>Point</c>
    /// </summary>
    public Coordinate Position => _position;

    /// <summary>
    /// Gets a value indicating whether the this <c>Point</c> has Z-coordinate set.
    /// </summary>
    public override bool Is3D => _position.Is3D;

    /// <summary>
    /// Returns Envelope, that covers this Point.
    /// </summary>
    /// <returns>Envelope, that covers this Point.</returns>
    public override Envelope2D GetEnvelope() => new Envelope2D(_position);

    /// <summary>
    /// Gets collection of all <see cref="Coordinate"/> of this IGeometry object
    /// </summary>
    /// <returns>the collection of all <see cref="Coordinate"/> of this object</returns>
    public override IReadOnlyList<Coordinate> GetCoordinates()
    {
        return new[] { _position };
    }
}
