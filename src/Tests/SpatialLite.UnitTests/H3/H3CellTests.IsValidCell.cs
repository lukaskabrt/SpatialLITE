using SpatialLite.H3;

namespace SpatialLite.UnitTests.H3;

public partial class H3CellTests
{
    public class IsValidCell
    {
        [Theory]
        [InlineData(0x085283473fffffffUL)] // res-5 SF hexagon
        [InlineData(0x0821c37fffffffffUL)] // res-2
        [InlineData(0x08001fffffffffffUL)] // res-0 base cell 0
        [InlineData(0x08f2830828052d25UL)] // res-15
        public void ReturnsTrue_IfKnownValidCell(ulong rawValue)
        {
            // Given
            var cell = new H3Cell(rawValue);

            // When
            var result = cell.IsValidCell;

            // Then
            result.Should().BeTrue();
        }

        [Fact]
        public void ReturnsTrue_IfPentagonBaseCell4Res0AllDigits7()
        {
            // Given – mode=1, res=0, base=4, all 15 digits = 7
            var value = (1UL << 59) | (4UL << 45) | ((1UL << 45) - 1UL);
            var cell = new H3Cell(value);

            // When
            var result = cell.IsValidCell;

            // Then
            result.Should().BeTrue();
        }

        [Fact]
        public void ReturnsTrue_IfPentagonBaseCell4Res1Digit1IsTwo()
        {
            // Given – mode=1, res=1, base=4, digit1=2 (not K_AXES=1), digits2-15=7
            var value = (1UL << 59) | (1UL << 52) | (4UL << 45) | (2UL << 42) | ((1UL << 42) - 1UL);
            var cell = new H3Cell(value);

            // When
            var result = cell.IsValidCell;

            // Then
            result.Should().BeTrue();
        }

        [Fact]
        public void ReturnsFalse_IfZeroValue()
        {
            // Given – mode = 0, not H3_CELL_MODE
            var cell = new H3Cell(0UL);

            // When
            var result = cell.IsValidCell;

            // Then
            result.Should().BeFalse();
        }

        [Fact]
        public void ReturnsFalse_IfHighBitSet()
        {
            // Given – reserved bit 63 is set
            var cell = new H3Cell(0x085283473fffffffUL | (1UL << 63));

            // When
            var result = cell.IsValidCell;

            // Then
            result.Should().BeFalse();
        }

        [Fact]
        public void ReturnsFalse_IfReservedBitsNonzero()
        {
            // Given – mode-specific reserved bit 56 is set
            var cell = new H3Cell(0x085283473fffffffUL | (1UL << 56));

            // When
            var result = cell.IsValidCell;

            // Then
            result.Should().BeFalse();
        }

        [Fact]
        public void ReturnsFalse_IfBaseCellOutOfRange()
        {
            // Given – base cell 122 is out of valid range 0-121
            var value = (1UL << 59) | (122UL << 45) | ((1UL << 45) - 1UL);
            var cell = new H3Cell(value);

            // When
            var result = cell.IsValidCell;

            // Then
            result.Should().BeFalse();
        }

        [Fact]
        public void ReturnsFalse_IfRes0CellWithNon7PaddingDigit()
        {
            // Given – res=0 cell but digit at r=1 (offset 42) is 0 instead of 7
            var value = 0x08001fffffffffffUL & ~(7UL << 42);
            var cell = new H3Cell(value);

            // When
            var result = cell.IsValidCell;

            // Then
            result.Should().BeFalse();
        }

        [Fact]
        public void ReturnsFalse_IfPentagonBaseCell4Res1Digit1IsKAxes()
        {
            // Given – pentagon base cell 4, res=1, first used digit=1 (K_AXES_DIGIT) → invalid
            var value = (1UL << 59) | (1UL << 52) | (4UL << 45) | (1UL << 42) | ((1UL << 42) - 1UL);
            var cell = new H3Cell(value);

            // When
            var result = cell.IsValidCell;

            // Then
            result.Should().BeFalse();
        }

        [Fact]
        public void ReturnsFalse_IfWrongMode()
        {
            // Given – mode bits set to 2, not H3_CELL_MODE (1)
            var cell = new H3Cell(2UL << 59);

            // When
            var result = cell.IsValidCell;

            // Then
            result.Should().BeFalse();
        }
    }
}
