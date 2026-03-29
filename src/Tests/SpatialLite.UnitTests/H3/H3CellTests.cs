using SpatialLite.H3;

namespace SpatialLite.UnitTests.H3;

public class H3CellTests
{
    // ── GetResolution ──────────────────────────────────────────────────────────

    [Theory]
    [InlineData(0x085283473fffffffUL, 5)]
    [InlineData(0x0821c37fffffffffUL, 2)]
    [InlineData(0x08001fffffffffffUL, 0)]
    [InlineData(0x08f2830828052d25UL, 15)]
    public void GetResolution_KnownCell_ReturnsExpectedResolution(ulong rawValue, int expectedResolution)
    {
        // Given
        var cell = new H3Cell(rawValue);

        // When
        var resolution = cell.GetResolution();

        // Then
        Assert.Equal(expectedResolution, resolution);
    }

    // ── H3ToString ────────────────────────────────────────────────────────────

    [Fact]
    public void H3ToString_ValidCell_ReturnsLowercaseHexWithoutPadding()
    {
        // Given
        var cell = new H3Cell(0x085283473fffffffUL);

        // When
        var result = cell.H3ToString();

        // Then
        Assert.Equal("85283473fffffff", result);
    }

    // ── ToString ──────────────────────────────────────────────────────────────

    [Fact]
    public void ToString_ValidCell_ReturnsSameAsH3ToString()
    {
        // Given
        var cell = new H3Cell(0x085283473fffffffUL);

        // When
        var result = cell.ToString();

        // Then
        Assert.Equal("85283473fffffff", result);
    }

    // ── StringToH3 ────────────────────────────────────────────────────────────

    [Theory]
    [InlineData("85283473fffffff", 0x085283473fffffffUL)]
    [InlineData("821c37fffffffff", 0x0821c37fffffffffUL)]
    public void StringToH3_ValidHexString_ReturnsCellWithCorrectValue(string hex, ulong expectedValue)
    {
        // Given / When
        var cell = H3Cell.StringToH3(hex);

        // Then
        Assert.Equal(expectedValue, cell.Value);
    }

    [Fact]
    public void StringToH3_InvalidString_ThrowsArgumentException()
    {
        // Given / When / Then
        Assert.Throws<ArgumentException>(() => H3Cell.StringToH3("xyz"));
    }

    [Fact]
    public void StringToH3_NullString_ThrowsArgumentException()
    {
        // Given / When / Then
        Assert.Throws<ArgumentException>(() => H3Cell.StringToH3(null!));
    }

    // ── Parse ─────────────────────────────────────────────────────────────────

    [Fact]
    public void Parse_ValidHexString_ReturnsCellWithCorrectValue()
    {
        // Given / When
        var cell = H3Cell.Parse("85283473fffffff");

        // Then
        Assert.Equal(0x085283473fffffffUL, cell.Value);
    }

    // ── TryParse ──────────────────────────────────────────────────────────────

    [Fact]
    public void TryParse_ValidHexString_ReturnsTrueAndCorrectCell()
    {
        // Given / When
        var success = H3Cell.TryParse("85283473fffffff", out var cell);

        // Then
        Assert.True(success);
        Assert.Equal(0x085283473fffffffUL, cell.Value);
    }

    [Theory]
    [InlineData("xyz")]
    [InlineData(null)]
    public void TryParse_InvalidOrNullString_ReturnsFalse(string? input)
    {
        // Given / When
        var success = H3Cell.TryParse(input, out _);

        // Then
        Assert.False(success);
    }

    // ── IEquatable<H3Cell>.Equals ──────────────────────────────────────────────

    [Fact]
    public void Equals_H3Cell_SameValue_ReturnsTrue()
    {
        // Given
        var a = new H3Cell(0x085283473fffffffUL);
        var b = new H3Cell(0x085283473fffffffUL);

        // When / Then
        Assert.True(a.Equals(b));
    }

    [Fact]
    public void Equals_H3Cell_DifferentValue_ReturnsFalse()
    {
        // Given
        var a = new H3Cell(0x085283473fffffffUL);
        var b = new H3Cell(0x0821c37fffffffffUL);

        // When / Then
        Assert.False(a.Equals(b));
    }

    // ── operator == / != ──────────────────────────────────────────────────────

    [Fact]
    public void EqualityOperator_SameValue_ReturnsTrue()
    {
        // Given
        var a = new H3Cell(0x085283473fffffffUL);
        var b = new H3Cell(0x085283473fffffffUL);

        // When / Then
        Assert.True(a == b);
    }

    [Fact]
    public void InequalityOperator_DifferentValues_ReturnsTrue()
    {
        // Given
        var a = new H3Cell(0x085283473fffffffUL);
        var b = new H3Cell(0x0821c37fffffffffUL);

        // When / Then
        Assert.True(a != b);
    }

    // ── IsValidCell ───────────────────────────────────────────────────────────

    [Theory]
    [InlineData(0x085283473fffffffUL)] // res-5 SF hexagon
    [InlineData(0x0821c37fffffffffUL)] // res-2
    [InlineData(0x08001fffffffffffUL)] // res-0 base cell 0
    [InlineData(0x08f2830828052d25UL)] // res-15
    public void IsValidCell_KnownValidCell_ReturnsTrue(ulong rawValue)
    {
        // Given
        var cell = new H3Cell(rawValue);

        // When
        var result = cell.IsValidCell;

        // Then
        Assert.True(result);
    }

    [Fact]
    public void IsValidCell_PentagonBaseCell4Res0AllDigits7_ReturnsTrue()
    {
        // Given – mode=1, res=0, base=4, all 15 digits = 7
        var value = (1UL << 59) | (4UL << 45) | ((1UL << 45) - 1UL);
        var cell = new H3Cell(value);

        // When
        var result = cell.IsValidCell;

        // Then
        Assert.True(result);
    }

    [Fact]
    public void IsValidCell_PentagonBaseCell4Res1Digit1IsTwo_ReturnsTrue()
    {
        // Given – mode=1, res=1, base=4, digit1=2 (not K_AXES=1), digits2-15=7
        var value = (1UL << 59) | (1UL << 52) | (4UL << 45) | (2UL << 42) | ((1UL << 42) - 1UL);
        var cell = new H3Cell(value);

        // When
        var result = cell.IsValidCell;

        // Then
        Assert.True(result);
    }

    [Fact]
    public void IsValidCell_ZeroValue_ReturnsFalse()
    {
        // Given – mode = 0, not H3_CELL_MODE
        var cell = new H3Cell(0UL);

        // When
        var result = cell.IsValidCell;

        // Then
        Assert.False(result);
    }

    [Fact]
    public void IsValidCell_HighBitSet_ReturnsFalse()
    {
        // Given – reserved bit 63 is set
        var cell = new H3Cell(0x085283473fffffffUL | (1UL << 63));

        // When
        var result = cell.IsValidCell;

        // Then
        Assert.False(result);
    }

    [Fact]
    public void IsValidCell_ReservedBitsNonzero_ReturnsFalse()
    {
        // Given – mode-specific reserved bit 56 is set
        var cell = new H3Cell(0x085283473fffffffUL | (1UL << 56));

        // When
        var result = cell.IsValidCell;

        // Then
        Assert.False(result);
    }

    [Fact]
    public void IsValidCell_BaseCellOutOfRange_ReturnsFalse()
    {
        // Given – base cell 122 is out of valid range 0-121
        var value = (1UL << 59) | (122UL << 45) | ((1UL << 45) - 1UL);
        var cell = new H3Cell(value);

        // When
        var result = cell.IsValidCell;

        // Then
        Assert.False(result);
    }

    [Fact]
    public void IsValidCell_Res0CellWithNon7PaddingDigit_ReturnsFalse()
    {
        // Given – res=0 cell but digit at r=1 (offset 42) is 0 instead of 7
        var value = 0x08001fffffffffffUL & ~(7UL << 42);
        var cell = new H3Cell(value);

        // When
        var result = cell.IsValidCell;

        // Then
        Assert.False(result);
    }

    [Fact]
    public void IsValidCell_PentagonBaseCell4Res1Digit1IsKAxes_ReturnsFalse()
    {
        // Given – pentagon base cell 4, res=1, first used digit=1 (K_AXES_DIGIT) → invalid
        var value = (1UL << 59) | (1UL << 52) | (4UL << 45) | (1UL << 42) | ((1UL << 42) - 1UL);
        var cell = new H3Cell(value);

        // When
        var result = cell.IsValidCell;

        // Then
        Assert.False(result);
    }

    [Fact]
    public void IsValidCell_WrongMode2_ReturnsFalse()
    {
        // Given – mode bits set to 2, not H3_CELL_MODE (1)
        var cell = new H3Cell(2UL << 59);

        // When
        var result = cell.IsValidCell;

        // Then
        Assert.False(result);
    }

    // ── GetHashCode ───────────────────────────────────────────────────────────

    [Fact]
    public void GetHashCode_SameValue_ReturnsSameHashCode()
    {
        // Given
        var a = new H3Cell(0x085283473fffffffUL);
        var b = new H3Cell(0x085283473fffffffUL);

        // When / Then
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    // ── Equals(object?) ───────────────────────────────────────────────────────

    [Fact]
    public void Equals_BoxedCellWithSameValue_ReturnsTrue()
    {
        // Given
        var a = new H3Cell(0x085283473fffffffUL);
        object b = new H3Cell(0x085283473fffffffUL);

        // When / Then
        Assert.True(a.Equals(b));
    }

    [Fact]
    public void Equals_NonH3CellObject_ReturnsFalse()
    {
        // Given
        var a = new H3Cell(0x085283473fffffffUL);
        object other = "not an H3Cell";

        // When / Then
        Assert.False(a.Equals(other));
    }

    // ── IsPentagon ────────────────────────────────────────────────────────────

    [Fact]
    public void IsPentagon_PentagonBaseCell4Res0_ReturnsTrue()
    {
        // Given – mode=1, res=0, base=4, all 15 digits = 7 (padding); res=0 has no used digits → vacuously true
        var value = (1UL << 59) | (4UL << 45) | ((1UL << 45) - 1UL);
        var cell = new H3Cell(value);

        // When
        var result = cell.IsPentagon;

        // Then
        Assert.True(result);
    }

    [Fact]
    public void IsPentagon_PentagonBaseCell4Res1Digit1IsZero_ReturnsTrue()
    {
        // Given – mode=1, res=1, base=4, digit[1]=0 (CENTER_DIGIT), digits 2-15=7
        var value = (1UL << 59) | (1UL << 52) | (4UL << 45) | ((1UL << 42) - 1UL);
        var cell = new H3Cell(value);

        // When
        var result = cell.IsPentagon;

        // Then
        Assert.True(result);
    }

    [Fact]
    public void IsPentagon_NonPentagonBaseCell0Res0_ReturnsFalse()
    {
        // Given – base cell 0 is NOT a pentagon base cell
        var cell = new H3Cell(0x08001fffffffffffUL);

        // When
        var result = cell.IsPentagon;

        // Then
        Assert.False(result);
    }

    [Fact]
    public void IsPentagon_PentagonBaseCell4Res1Digit1IsTwo_ReturnsFalse()
    {
        // Given – mode=1, res=1, base=4, digit[1]=2 (not CENTER_DIGIT=0) → not the pentagon center
        var value = (1UL << 59) | (1UL << 52) | (4UL << 45) | (2UL << 42) | ((1UL << 42) - 1UL);
        var cell = new H3Cell(value);

        // When
        var result = cell.IsPentagon;

        // Then
        Assert.False(result);
    }

    [Fact]
    public void IsPentagon_InvalidZeroCell_ReturnsFalse()
    {
        // Given – zero value has wrong mode → IsValidCell is false
        var cell = new H3Cell(0UL);

        // When
        var result = cell.IsPentagon;

        // Then
        Assert.False(result);
    }
}
