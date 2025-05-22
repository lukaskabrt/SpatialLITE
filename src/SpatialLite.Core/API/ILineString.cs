namespace SpatialLite.Core.API;

/// <summary>
/// Defiens properties and methods for line strings. A line string is a curve with linear connection between concesutive points.
/// </summary>
public interface ILineString : IGeometry
{
    /// <summary>
    /// Gets a value indicating whether the <c>ILineString</c> is closed.
    /// </summary>
    /// <remarks>
    /// The ILineString is closed if <see cref="ILineString.Start"/> and <see cref="ILineString.End"/> are identical.
    /// </remarks>
    bool IsClosed { get; }

    /// <summary>
    /// Gets the first coordinate of the <c>ILineString</c> object.
    /// </summary>
    Coordinate Start { get; }

    /// <summary>
    /// Gets the last coordinate of the <c>ILineString</c> object.
    /// </summary>
    Coordinate End { get; }

    /// <summary>
    /// Gets the list of çoordinates that define this LisneString
    /// </summary>
    ICoordinateList Coordinates { get; }
}
