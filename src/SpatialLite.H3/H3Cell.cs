using System.Globalization;
using SpatialLite.Contracts;

namespace SpatialLite.H3;

/// <summary>
/// Represents an H3 cell index as a 64-bit unsigned integer.
/// </summary>
public readonly struct H3Cell : IEquatable<H3Cell>
{
    // ── Bit-layout constants ────────────────────────────────────────────────
    private const int ModeBitShift = 59;
    private const ulong ModeMask = 0xFUL;
    private const ulong H3CellMode = 1UL;

    private const int ReservedBitShift = 56;
    private const ulong ReservedMask = 0x7UL;

    private const int ResolutionBitShift = 52;
    private const ulong ResolutionMask = 0xFUL;

    private const int BaseCellBitShift = 45;
    private const ulong BaseCellMask = 0x7FUL;
    private const int MaxBaseCell = 121;

    private const ulong DigitMask = 0x7UL;
    private const int InvalidDigit = 7;   // padding sentinel
    private const int KAxesDigit = 1;   // forbidden first digit for pentagons
    private const int CenterDigit = 0;  // center digit sentinel (CENTER_DIGIT)

    // ── Static data ─────────────────────────────────────────────────────────
    private static readonly HashSet<int> PentagonBaseCells =
        new() { 4, 14, 24, 38, 49, 58, 63, 72, 83, 97, 107, 117 };

    // ── Instance field ──────────────────────────────────────────────────────
    private readonly ulong _value;

    // ── Constructor ─────────────────────────────────────────────────────────

    /// <summary>
    /// Initializes a new instance of the <see cref="H3Cell"/> struct with the specified cell index value.
    /// </summary>
    /// <param name="value">The 64-bit H3 cell index.</param>
    public H3Cell(ulong value)
    {
        _value = value;
    }

    // ── Public properties ───────────────────────────────────────────────────

    /// <summary>
    /// Gets the raw 64-bit H3 cell index value.
    /// </summary>
    public ulong Value => _value;

    /// <summary>
    /// Gets a value indicating whether this cell index has a valid H3 bit layout.
    /// Checks the high bit, mode nibble, reserved bits, resolution, base cell,
    /// digit padding, and the K-axes pentagon constraint.
    /// </summary>
    public bool IsValidCell
    {
        get
        {
            if ((_value >> 63) != 0UL)
            {
                return false;
            }

            var mode = (_value >> ModeBitShift) & ModeMask;
            if (mode != H3CellMode)
            {
                return false;
            }

            var reserved = (_value >> ReservedBitShift) & ReservedMask;
            if (reserved != 0UL)
            {
                return false;
            }

            var res = GetResolution();
            var baseCell = (int)((_value >> BaseCellBitShift) & BaseCellMask);

            if (baseCell > MaxBaseCell)
            {
                return false;
            }

            var isPentagon = PentagonBaseCells.Contains(baseCell);
            var firstUsedDigit = -1;

            for (var r = 1; r <= 15; r++)
            {
                var digit = (int)((_value >> DigitOffset(r)) & DigitMask);

                if (r <= res)
                {
                    if (digit == InvalidDigit)
                    {
                        return false;
                    }

                    if (isPentagon && firstUsedDigit < 0)
                    {
                        firstUsedDigit = digit;
                    }
                }
                else
                {
                    if (digit != InvalidDigit)
                    {
                        return false;
                    }
                }
            }

            if (isPentagon && firstUsedDigit == KAxesDigit)
            {
                return false;
            }

            return true;
        }
    }

    // ── Public methods ──────────────────────────────────────────────────────

    /// <summary>Returns the H3 resolution (0-15) encoded in bits 55-52.</summary>
    public int GetResolution() => (int)((_value >> ResolutionBitShift) & ResolutionMask);

    /// <summary>
    /// Returns the geographic polygon boundary of this H3 cell as a list of vertices.
    /// Hexagonal cells return 6 vertices; pentagon cells return 5.
    /// Each <see cref="Coordinate"/> has X=longitude and Y=latitude in degrees.
    /// </summary>
    public IReadOnlyList<Coordinate> GetBoundary() =>
        Internals.BoundaryConverter.CellToBoundary(_value, GetResolution(), IsPentagon);

    /// <inheritdoc/>
    public override string ToString() => _value.ToString("x", CultureInfo.InvariantCulture);

    /// <inheritdoc/>
    public bool Equals(H3Cell other) => _value == other._value;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is H3Cell other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => _value.GetHashCode();

    // ── Static factory methods ──────────────────────────────────────────────

    /// <summary>Parses a lowercase hex H3 address string into an <see cref="H3Cell"/>.</summary>
    /// <param name="s">The lowercase hexadecimal H3 address string (e.g. <c>"85283473fffffff"</c>).</param>
    /// <returns>The <see cref="H3Cell"/> whose value corresponds to <paramref name="s"/>.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="s"/> is <c>null</c> or not a valid hexadecimal string.</exception>
    public static H3Cell Parse(string s)
    {
        if (s is null || !ulong.TryParse(s, NumberStyles.HexNumber, null, out var v))
        {
            throw new ArgumentException($"'{s}' is not a valid H3 address.", nameof(s));
        }

        return new H3Cell(v);
    }

    /// <summary>
    /// Attempts to parse a hex H3 address string into an <see cref="H3Cell"/> without throwing on failure.
    /// </summary>
    /// <param name="s">The hexadecimal H3 address string to parse, or <c>null</c>.</param>
    /// <param name="cell">
    /// When this method returns <c>true</c>, the parsed <see cref="H3Cell"/>; otherwise the default value.
    /// </param>
    /// <returns><c>true</c> if <paramref name="s"/> was parsed successfully; otherwise <c>false</c>.</returns>
    public static bool TryParse(string? s, out H3Cell cell)
    {
        if (s != null && ulong.TryParse(s, NumberStyles.HexNumber, null, out var v))
        {
            cell = new H3Cell(v);
            return true;
        }

        cell = default;
        return false;
    }

    /// <summary>
    /// Converts a geographic coordinate to the containing H3 cell index at the given resolution.
    /// </summary>
    /// <param name="lat">Latitude in degrees.</param>
    /// <param name="lng">Longitude in degrees.</param>
    /// <param name="resolution">H3 resolution level (0–15).</param>
    /// <returns>The <see cref="H3Cell"/> that contains the specified coordinate.</returns>
    public static H3Cell FromLatLng(double lat, double lng, int resolution)
    {
        return new H3Cell(SpatialLite.H3.Internals.LatLngConverter.LatLngToCell(lat, lng, resolution));
    }

    // ── Operator overloads ──────────────────────────────────────────────────

    /// <summary>Returns <c>true</c> when both cells have the same value.</summary>
    public static bool operator ==(H3Cell left, H3Cell right) => left.Equals(right);

    /// <summary>Returns <c>true</c> when both cells have different values.</summary>
    public static bool operator !=(H3Cell left, H3Cell right) => !(left == right);

    // ── Private helpers ─────────────────────────────────────────────────────

    /// <summary>Returns the bit-shift offset for resolution digit <paramref name="r"/> (1-based).</summary>
    private static int DigitOffset(int r) => (15 - r) * 3;

    // ── Internal properties ─────────────────────────────────────────────────

    /// <summary>
    /// Gets a value indicating whether this cell is a pentagon cell
    /// (valid cell, pentagon base cell, all used digits are CENTER_DIGIT=0).
    /// </summary>
    internal bool IsPentagon
    {
        get
        {
            if (!IsValidCell)
            {
                return false;
            }

            var baseCell = (int)((_value >> BaseCellBitShift) & BaseCellMask);
            if (!PentagonBaseCells.Contains(baseCell))
            {
                return false;
            }

            var res = (int)((_value >> ResolutionBitShift) & ResolutionMask);
            for (var r = 1; r <= res; r++)
            {
                var digit = (int)((_value >> DigitOffset(r)) & DigitMask);
                if (digit != CenterDigit)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
