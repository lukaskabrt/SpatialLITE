namespace SpatialLite.H3;

/// <summary>
/// Represents an H3 cell index as a 64-bit unsigned integer.
/// </summary>
public readonly struct H3Cell
{
    private readonly ulong _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="H3Cell"/> struct with the specified cell index value.
    /// </summary>
    /// <param name="value">The 64-bit H3 cell index.</param>
    public H3Cell(ulong value)
    {
        _value = value;
    }

    /// <summary>
    /// Gets the raw 64-bit H3 cell index value.
    /// </summary>
    public ulong Value => _value;
}
