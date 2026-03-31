namespace SpatialLite.H3.Internals;

/// <summary>
/// Represents a 3D vector with double-precision components.
/// </summary>
internal readonly struct Vec3d
{
    /// <summary>
    /// Initializes a new instance of the Vec3d structure with the specified X, Y, and Z coordinates.
    /// </summary>
    /// <param name="x">The value to assign to the X coordinate.</param>
    /// <param name="y">The value to assign to the Y coordinate.</param>
    /// <param name="z">The value to assign to the Z coordinate.</param>
    internal Vec3d(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    /// <summary>
    /// Gets the X-coordinate value.
    /// </summary>
    internal double X { get; }

    /// <summary>
    /// Gets the Y-coordinate value.
    /// </summary>
    internal double Y { get; }

    /// <summary>
    /// Gets the Z-coordinate value.
    /// </summary>
    internal double Z { get; }

    /// <summary>
    /// Calculates the squared Euclidean distance between this vector and the specified vector.
    /// </summary>
    /// <remarks>This method avoids computing the square root, making it more efficient when only relative
    /// distances are needed.</remarks>
    /// <param name="other">The vector to which the squared distance is calculated.</param>
    /// <returns>The squared distance between this vector and the <paramref name="other"/> vector.</returns>
    internal double SquareDistance(in Vec3d other)
    {
        var dx = X - other.X;
        var dy = Y - other.Y;
        var dz = Z - other.Z;

        return dx * dx + dy * dy + dz * dz;
    }
}
