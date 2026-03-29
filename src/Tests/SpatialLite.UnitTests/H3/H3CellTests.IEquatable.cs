using SpatialLite.H3;

namespace SpatialLite.UnitTests.H3;

public partial class H3CellTests
{
    public class IEquatable
    {
        [Fact]
        public void Equals_ReturnsTrue_IfSameValueH3Cell()
        {
            // Given
            var a = new H3Cell(0x085283473fffffffUL);
            var b = new H3Cell(0x085283473fffffffUL);

            // When
            var result = a.Equals(b);

            // Then
            result.Should().BeTrue();
        }

        [Fact]
        public void Equals_ReturnsFalse_IfDifferentValueH3Cell()
        {
            // Given
            var a = new H3Cell(0x085283473fffffffUL);
            var b = new H3Cell(0x0821c37fffffffffUL);

            // When
            var result = a.Equals(b);

            // Then
            result.Should().BeFalse();
        }

        [Fact]
        public void Equals_ReturnsTrue_IfBoxedCellWithSameValue()
        {
            // Given
            var a = new H3Cell(0x085283473fffffffUL);
            object b = new H3Cell(0x085283473fffffffUL);

            // When
            var result = a.Equals(b);

            // Then
            result.Should().BeTrue();
        }

        [Fact]
        public void Equals_ReturnsFalse_IfNonH3CellObject()
        {
            // Given
            var a = new H3Cell(0x085283473fffffffUL);
            object other = "not an H3Cell";

            // When
            var result = a.Equals(other);

            // Then
            result.Should().BeFalse();
        }

        [Fact]
        public void EqualityOperator_ReturnsTrue_IfSameValue()
        {
            // Given
            var a = new H3Cell(0x085283473fffffffUL);
            var b = new H3Cell(0x085283473fffffffUL);

            // When
            var result = a == b;

            // Then
            result.Should().BeTrue();
        }

        [Fact]
        public void InequalityOperator_ReturnsTrue_IfDifferentValues()
        {
            // Given
            var a = new H3Cell(0x085283473fffffffUL);
            var b = new H3Cell(0x0821c37fffffffffUL);

            // When
            var result = a != b;

            // Then
            result.Should().BeTrue();
        }

        [Fact]
        public void GetHashCode_ReturnsSameHashCode_IfSameValue()
        {
            // Given
            var a = new H3Cell(0x085283473fffffffUL);
            var b = new H3Cell(0x085283473fffffffUL);

            // When
            var hashCodeA = a.GetHashCode();
            var hashCodeB = b.GetHashCode();

            // Then
            hashCodeA.Should().Be(hashCodeB);
        }
    }
}
