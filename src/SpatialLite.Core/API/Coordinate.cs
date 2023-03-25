using System;

namespace SpatialLite.Core.Api;

/// <summary>
/// Represents a location in the coordinate space.
/// </summary>
public record struct Coordinate : IEquatable<Coordinate>
{
    /// <summary>
    /// Represents an empty coordinate.
    /// </summary>
    /// <remarks>
    /// The empty coordinate has all coordinates equal to NaN.
    /// </remarks>
    public static readonly Coordinate Empty = new Coordinate(double.NaN, double.NaN, double.NaN);

    /// <summary>
    /// Gets the X-coordinate.
    /// </summary>
    public double X { get; private set; }

    /// <summary>
    /// Gets the Y-coordinate.
    /// </summary>
    public double Y { get; private set; }

    /// <summary>
    /// Gets the Z-coordinate.
    /// </summary>
    public double Z { get; private set; }

    /// <summary>
    /// Gets the M value
    /// </summary>
    public double M { get; private set; }

    /// <summary>
    /// Initializes a <see cref="Coordinate"/> with X, Y coordinates.
    /// </summary>
    /// <param name="x">X-coordinate value.</param>
    /// <param name="y">Y-coordinate value.</param>
    public Coordinate(double x, double y)
    {
        X = x;
        Y = y;
        Z = double.NaN;
        M = double.NaN;
    }

    /// <summary>
    /// Initializes a <see cref="Coordinate"/> with X, Y, Z coordinates.
    /// </summary>
    /// <param name="x">X-coordinate value.</param>
    /// <param name="y">Y-coordinate value.</param>
    /// <param name="z">Z-coordinate value.</param>
    public Coordinate(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
        M = double.NaN;
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="Coordinate"/>has assigned <see cref="Z"/> coordinate.
    /// </summary>
    public bool Is3D => !double.IsNaN(this.Z);

    /// <summary>
    /// Gets a value indicating whether this <see cref="Coordinate"/> represents <see cref="Coordinate.Empty"/>.
    /// </summary>
    public bool IsEmpty => double.IsNaN(this.X) || double.IsNaN(this.Y);

    /// <summary>
    /// Gets a value indicating whether this coordinate has assigned <see cref="Coordinate.M"/> value.
    /// </summary>
    public bool IsMeasured => !double.IsNaN(this.M);

    /// <summary>
    /// Returns a <c>string</c> representation of the current <see cref="Coordinate"/>.
    /// </summary>
    /// <returns>A <c>string</c> representation of the current <see cref="Coordinate"/></returns>
    public override string ToString()
    {
        return string.Format(System.Globalization.CultureInfo.InvariantCulture, "[{0}; {1}; {2}]", this.X, this.Y, this.Z);
    }

    /// <summary>
    /// Determines whether two <see cref="Coordinate"/> are equal in the 2D space.
    /// </summary>
    /// <param name="other">The <see cref="Coordinate"/> to compare with the current <see cref="Coordinate"/></param>
    /// <returns>true if the specified <see cref="Coordinate"/> is equal to the current <see cref="Coordinate"/> in the 2D space otherwise, false.</returns>
    public bool Equals2D(Coordinate other)
    {
        return (this.IsEmpty && other.IsEmpty) || ((this.X == other.X) && (this.Y == other.Y));
    }
}
