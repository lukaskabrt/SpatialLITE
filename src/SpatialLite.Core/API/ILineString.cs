namespace SpatialLite.Core.Api;

/// <summary>
/// Represents a line strings. The line string is a curve with the linear connection between consecutive points.
/// </summary>
public interface ILineString : IGeometry
{
    /// <summary>
    /// Gets a value indicating whether the <see cref="ILineString"/> is closed.
    /// </summary>
    /// <remarks>
    /// The ILineString is closed if <see cref="ILineString.Start"/> and <see cref="ILineString.End"/> are identical.
    /// </remarks>
    bool IsClosed { get; }

    /// <summary>
    /// Gets the first coordinate of the <see cref="ILineString"/> object.
    /// </summary>
    Coordinate Start { get; }

    /// <summary>
    /// Gets the last coordinate of the <see cref="ILineString"/> object.
    /// </summary>
    Coordinate End { get; }

    /// <summary>
    /// Gets the list of coordinates that defines <see cref="ILineString"/>.
    /// </summary>
    ICoordinateSequence Coordinates { get; }
}
