using SpatialLite.H3;

namespace SpatialLite.UnitTests.H3;

public partial class H3CellTests
{
    public class IsPentagon
    {
        [Fact]
        public void ReturnsTrue_IfPentagonBaseCell4Res0()
        {
            // Given – mode=1, res=0, base=4, all 15 digits = 7 (padding)
            var value = (1UL << 59) | (4UL << 45) | ((1UL << 45) - 1UL);
            var cell = new H3Cell(value);

            // When
            var result = cell.IsPentagon;

            // Then
            result.Should().BeTrue();
        }

        [Fact]
        public void ReturnsTrue_IfPentagonBaseCell4Res1Digit1IsZero()
        {
            // Given – mode=1, res=1, base=4, digit[1]=0 (CENTER_DIGIT), digits 2-15=7
            var value = (1UL << 59) | (1UL << 52) | (4UL << 45) | ((1UL << 42) - 1UL);
            var cell = new H3Cell(value);

            // When
            var result = cell.IsPentagon;

            // Then
            result.Should().BeTrue();
        }

        [Fact]
        public void ReturnsFalse_IfNonPentagonBaseCell0Res0()
        {
            // Given – base cell 0 is NOT a pentagon base cell
            var cell = new H3Cell(0x08001fffffffffffUL);

            // When
            var result = cell.IsPentagon;

            // Then
            result.Should().BeFalse();
        }

        [Fact]
        public void ReturnsFalse_IfPentagonBaseCell4Res1Digit1IsTwo()
        {
            // Given – mode=1, res=1, base=4, digit[1]=2 (not CENTER_DIGIT=0) → not the pentagon center
            var value = (1UL << 59) | (1UL << 52) | (4UL << 45) | (2UL << 42) | ((1UL << 42) - 1UL);
            var cell = new H3Cell(value);

            // When
            var result = cell.IsPentagon;

            // Then
            result.Should().BeFalse();
        }

        [Fact]
        public void ReturnsFalse_IfInvalidZeroCell()
        {
            // Given – zero value has wrong mode → IsValidCell is false
            var cell = new H3Cell(0UL);

            // When
            var result = cell.IsPentagon;

            // Then
            result.Should().BeFalse();
        }
    }
}
