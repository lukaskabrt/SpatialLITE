using System.Collections.Generic;

namespace SpatialLite.Core.Api;

/// <summary>
/// Represents minimal bounding box of a set of coordinates.
/// </summary>
public readonly record struct Envelope2D
{
    /// <summary>
    /// Empty Envelope.
    /// </summary>
    /// <remarks>
    /// Empty envelope has its min values greater that max values.
    /// </remarks>
    public static readonly Envelope2D Empty = new Envelope2D(0, -1, 0, -1);

    /// <summary>
    /// Gets Envelope's minimal x-coordinate.
    /// </summary>
    public readonly double MinX { get; }

    /// <summary>
    /// Gets Envelope's maximal x-coordinate.
    /// </summary>
    public readonly double MaxX { get; }

    /// <summary>
    /// Gets Envelope's minimal y-coordinate.
    /// </summary>
    public readonly double MinY { get; }

    /// <summary>
    /// Gets Envelope's maximal y-coordinate.
    /// </summary>
    public readonly double MaxY { get; }

    /// <summary>
    /// Returns the difference between the maximum and minimum X coordinates.
    /// </summary>
    /// <returns>Width of the envelope.</returns>
    public double Width => this.IsEmpty ? 0 : MaxX - MinX;

    /// <summary>
    /// Returns the difference between the maximum and minimum y coordinates.
    /// </summary>
    /// <returns>Height of the envelope.</returns>
    public double Height => this.IsEmpty ? 0 : MaxY - MinY;

    /// <summary>
    /// Checks if this Envelope equals the empty Envelope.
    /// </summary>
    public bool IsEmpty => MinX > MaxX;

    /// <summary>
    /// Initializes an <see cref="Envelope2D"/> with the single coordinate.
    /// </summary>
    /// <param name="coord">The coordinate used initialize <see cref="Envelope2D"/></param>
    public Envelope2D(Coordinate coord)
    {
        if (coord.IsEmpty)
        {
            MinX = MinY = 0;
            MaxX = MaxY = -1;
            return;
        }

        MinX = coord.X;
        MaxX = coord.X;
        MinY = coord.Y;
        MaxY = coord.Y;
    }

    /// <summary>
    /// Initializes an <see cref="Envelope2D"/> as copy of the specified envelope.
    /// </summary>
    /// <param name="source">The envelope to be copied.</param>
    public Envelope2D(Envelope2D source)
    {
        MinX = source.MinX;
        MaxX = source.MaxX;
        MinY = source.MinY;
        MaxY = source.MaxY;
    }

    /// <summary>
    /// Initializes an <see cref="Envelope2D"/> that covers specified coordinates.
    /// </summary>
    /// <param name="coords">The coordinates to be covered.</param>
    public Envelope2D(IReadOnlyList<Coordinate> coords)
    {
        MinX = MinY = 0;
        MaxX = MaxY = -1;

        for (int i = 0; i < coords.Count; i++)
        {
            if (coords[i].IsEmpty) continue;

            if (this.IsEmpty)
            {
                MinX = coords[i].X;
                MaxX = coords[i].X;
                MinY = coords[i].Y;
                MaxY = coords[i].Y;
            }
            else
            {
                MinX = MinX < coords[i].X ? MinX : coords[i].X;
                MaxX = MaxX > coords[i].X ? MaxX : coords[i].X;
                MinY = MinY < coords[i].Y ? MinY : coords[i].Y;
                MaxY = MaxY > coords[i].Y ? MaxY : coords[i].Y;
            }
        }
    }

    /// <summary>
    /// Initializes an <see cref="Envelope2D"/> with specified values.
    /// </summary>
    /// <param name="minX">Minimal X ordinate.</param>
    /// <param name="maxX">Maximal X ordinate.</param>
    /// <param name="minY">Minimal Y ordinate.</param>
    /// <param name="maxY">Maximal Y ordinate.</param>
    private Envelope2D(double minX, double maxX, double minY, double maxY)
    {
        MinX = minX;
        MaxX = maxX;
        MinY = minY;
        MaxY = maxY;
    }

    /// <summary>
    /// Extends this <c>Envelope</c> to cover specified <c>Coordinate</c>.
    /// </summary>
    /// <param name="coord">The <c>Coordinate</c> to be covered by extended Envelope.</param>
    public Envelope2D Extend(Coordinate coord)
    {
        if (this.Covers(coord) || coord.IsEmpty) return this;

        if (this.IsEmpty) return new Envelope2D(coord);

        return new Envelope2D(
            MinX < coord.X ? MinX : coord.X,
            MaxX > coord.X ? MaxX : coord.X,
            MinY < coord.Y ? MinY : coord.Y,
            MaxY > coord.Y ? MaxY : coord.Y
       );
    }

    /// <summary>
    /// Extends this <c>Envelope</c> to cover specified <c>Coordinates</c>.
    /// </summary>
    /// <param name="coords">The collection of Coordinates to be covered by extended Envelope.</param>
    public Envelope2D Extend(IReadOnlyList<Coordinate> coords)
    {
        var envelop = new Envelope2D(coords);
        return Extend(envelop);
    }

    /// <summary>
    /// Extends this <c>Envelope</c> to cover specified <c>Envelope</c>.
    /// </summary>
    /// <param name="envelope">The <c>Envelope</c> to be covered by extended Envelope.</param>
    public Envelope2D Extend(Envelope2D envelope)
    {
        if (this.Covers(envelope) || envelope.IsEmpty) return this;

        return new Envelope2D(
            MinX < envelope.MinX ? MinX : envelope.MinX,
            MaxX > envelope.MaxX ? MaxX : envelope.MaxX,
            MinY < envelope.MinY ? MinY : envelope.MinY,
            MaxY > envelope.MaxY ? MaxY : envelope.MaxY
       );
    }

    /// <summary>
    /// Check if the region defined by <c>other</c>
    /// overlaps (intersects) the region of this <c>Envelope</c>.
    /// </summary>
    /// <param name="other"> the <c>Envelope</c> which this <c>Envelope</c> is
    /// being checked for overlapping.
    /// </param>
    /// <returns>
    /// <c>true</c> if the <c>Envelope</c>s overlap.
    /// </returns>
    public bool Intersects(Envelope2D other)
    {
        if (this.IsEmpty || other.IsEmpty) return false;

        return !(other.MinX > this.MaxX || other.MaxX < this.MinX || other.MinY > this.MaxY || other.MaxY < other.MinY);
    }

    ///<summary>
    /// Tests whether the given <see cref="Coordinate"/> lies in or on the envelope.
    ///</summary>
    /// <param name="coordinate">the <see cref="Coordinate"/> to check.</param>
    /// <returns>True if the <paramref name="coordinate"/> lies in the interior or on the boundary of this envelope.</returns>
    public bool Covers(Coordinate coordinate)
    {
        if (this.IsEmpty) return false;

        return
            coordinate.X >= this.MinX &&
            coordinate.X <= this.MaxX &&
            coordinate.Y >= this.MinY &&
            coordinate.Y <= this.MaxY;
    }

    ///<summary>
    /// Tests if an <see cref="Envelope2D"/> lies inside this envelope (inclusive of the boundary).
    ///</summary>
    /// <param name="other">The <c>Envelope</c> to check.</param>
    /// <returns>True if this envelope covers the <paramref name="other"/></returns>
    public bool Covers(Envelope2D other)
    {
        if (this.IsEmpty || other.IsEmpty) return false;

        return
            other.MinX >= this.MinX &&
            other.MaxX <= this.MaxX &&
            other.MinY >= this.MinY &&
            other.MaxY <= this.MaxY;
    }

    /// <summary>
    /// Expands <see cref="Envelope2D"/> on every side by the specified distance.
    /// </summary>
    /// <param name="dx">Distance in the X-axis direction.</param>
    /// <param name="dy">Distance in the Y-axis direction.</param>
    /// <returns>Expanded <see cref="Envelope2D"/>.</returns>
    /// <remarks>Both positive and negative values for <paramref name="dx"/> and <paramref name="dy"/> are allowed.</remarks>
    public Envelope2D Expand(double dx, double dy)
    {
        if (this.IsEmpty) return Envelope2D.Empty;

        return new Envelope2D(MinX - dx, MaxX + dx, MinY - dy, MaxY + dy);
    }
}