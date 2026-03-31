namespace SpatialLite.H3.Internals;

/// <summary>
/// Represents a coordinate in the IJK integer coordinate system used for hierarchical hexagonal grid indexing.
/// </summary>
internal struct IJKCoordinate
{
    private const int InvalidDigit = 7;

    internal int I;
    internal int J;
    internal int K;

    /// <summary>
    /// Initializes a new instance of the <see cref="IJKCoordinate"/> struct with the specified I, J, and K components.
    /// </summary>
    /// <param name="i">The I component.</param>
    /// <param name="j">The J component.</param>
    /// <param name="k">The K component.</param>
    internal IJKCoordinate(int i, int j, int k)
    {
        I = i;
        J = j;
        K = k;
    }

    /// <summary>
    /// Normalizes the I, J, and K components so that all values are non-negative and at least one component is zero.
    /// </summary>
    /// <remarks>
    /// This method adjusts the components in place to ensure that none are negative and the smallest component is set
    /// to zero. This normalization is useful for representing the components in a canonical form.
    /// </remarks>
    internal void Normalize()
    {
        if (I < 0)
        {
            J -= I;
            K -= I;
            I = 0;
        }

        if (J < 0)
        {
            I -= J;
            K -= J;
            J = 0;
        }

        if (K < 0)
        {
            I -= K;
            J -= K;
            K = 0;
        }

        var min = Math.Min(I, Math.Min(J, K));
        if (min > 0)
        {
            I -= min;
            J -= min;
            K -= min;
        }
    }

    /// <summary>
    /// Finds the normalized ijk coordinates of the indexing parent of a cell in a counter-clockwise aperture 7 grid.
    /// Works in place.
    /// </summary>
    internal void UpAp7()
    {
        var i = I - K;
        var j = J - K;
        I = (int)Math.Round((3 * i - j) * (1.0 / 7.0), MidpointRounding.AwayFromZero);
        J = (int)Math.Round((i + 2 * j) * (1.0 / 7.0), MidpointRounding.AwayFromZero);
        K = 0;
        Normalize();
    }

    /// <summary>
    /// Find the normalized ijk coordinates of the indexing parent of a cell in a clockwise aperture 7 grid.Works in
    /// place.
    /// </summary>
    internal void UpAp7r()
    {
        var i = I - K;
        var j = J - K;
        I = (int)Math.Round((2 * i + j) * (1.0 / 7.0), MidpointRounding.AwayFromZero);
        J = (int)Math.Round((3 * j - i) * (1.0 / 7.0), MidpointRounding.AwayFromZero);
        K = 0;
        Normalize();
    }

    /// <summary>
    /// Finds the normalized ijk coordinates of the hex centered on the indicated hex at the next finer aperture 7
    /// counter-clockwise resolution. Works in place.
    /// </summary>
    internal void DownAp7()
    {
        var newI = 3 * I + 1 * J + 0 * K;
        var newJ = 0 * I + 3 * J + 1 * K;
        var newK = 1 * I + 0 * J + 3 * K;

        I = newI;
        J = newJ;
        K = newK;
        Normalize();
    }

    /// <summary>
    /// Finds the normalized ijk coordinates of the hex centered on the indicated hex at the next finer aperture 7
    /// clockwise resolution. Works in place.
    /// </summary>
    internal void DownAp7r()
    {
        var newI = 3 * I + 0 * J + 1 * K;
        var newJ = 1 * I + 3 * J + 0 * K;
        var newK = 0 * I + 1 * J + 3 * K;

        I = newI;
        J = newJ;
        K = newK;

        Normalize();
    }

    /// <summary>
    /// Rotates ijk coordinates 60 degrees counter-clockwise. Works in place.
    /// </summary>
    internal void Rotate60Ccw()
    {
        var newI = I + K;
        var newJ = I + J;
        var newK = J + K;

        I = newI;
        J = newJ;
        K = newK;

        Normalize();
    }

    /// <summary>
    /// Rotates ijk coordinates 60 degrees clockwise. Works in place.
    /// </summary>
    internal void Rotate60Cw()
    {
        var newI = I + J;
        var newJ = J + K;
        var newK = I + K;

        I = newI;
        J = newJ;
        K = newK;

        Normalize();
    }

    /// <summary>
    /// Determines the H3 digit corresponding to a unit vector or the zero vector in ijk coordinates.
    /// </summary>
    /// <returns>
    /// The H3 digit (0-6) corresponding to the ijk unit vector, zero vector, or INVALID_DIGIT (7) on failure.
    /// </returns>
    internal readonly int ToDigit()
    {
        var c = this;
        c.Normalize();
        return (c.I, c.J, c.K) switch
        {
            (0, 0, 0) => 0,
            (0, 0, 1) => 1,
            (0, 1, 0) => 2,
            (0, 1, 1) => 3,
            (1, 0, 0) => 4,
            (1, 0, 1) => 5,
            (1, 1, 0) => 6,
            _ => InvalidDigit
        };
    }

    /// <summary>
    /// Subtracts two IJK coordinates and returns the result as a new IJK coordinate.
    /// </summary>
    /// <param name="a">The first IJK coordinate.</param>
    /// <param name="b">The second IJK coordinate.</param>
    /// <returns>The result of subtracting <paramref name="b"/> from <paramref name="a"/>.</returns>
    internal static IJKCoordinate Subtract(in IJKCoordinate a, in IJKCoordinate b)
        => new(a.I - b.I, a.J - b.J, a.K - b.K);
}
