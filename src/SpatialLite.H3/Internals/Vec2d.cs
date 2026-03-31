namespace SpatialLite.H3.Internals;

/// <summary>
/// Represents a 2D vector with double-precision components.
/// </summary>
internal readonly struct Vec2d
{
    /// <summary>
    /// Initializes a new instance of the Vec2d structure with the specified X and Y coordinates.
    /// </summary>
    /// <param name="x">The X coordinate of the vector.</param>
    /// <param name="y">The Y coordinate of the vector.</param>
    internal Vec2d(double x, double y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// Gets the X-coordinate value.
    /// </summary>
    internal double X { get; }

    /// <summary>
    /// Gets the Y-coordinate value.
    /// </summary>
    internal double Y { get; }
}
