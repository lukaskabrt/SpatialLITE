using SpatialLite.H3;

namespace SpatialLite.UnitTests.H3;

public partial class H3CellTests
{
    public class LatLngToCellValidation
    {
        // ── resolution bounds ────────────────────────────────────────────────

        [Fact]
        public void ThrowsArgumentOutOfRange_WhenResolutionIsNegative()
        {
            // Given
            static void Act() => H3Cell.LatLngToCell(0, 0, -1);

            // When
            var exception = Record.Exception(Act);

            // Then
            exception.Should().BeOfType<ArgumentOutOfRangeException>()
                .Which.ParamName.Should().Be("resolution");
        }

        [Fact]
        public void ThrowsArgumentOutOfRange_WhenResolutionIsAboveMax()
        {
            // Given
            static void Act() => H3Cell.LatLngToCell(0, 0, 16);

            // When
            var exception = Record.Exception(Act);

            // Then
            exception.Should().BeOfType<ArgumentOutOfRangeException>()
                .Which.ParamName.Should().Be("resolution");
        }

        [Fact]
        public void DoesNotThrow_WhenResolutionIsZero()
        {
            // Given / When
            var exception = Record.Exception(() => H3Cell.LatLngToCell(0, 0, 0));

            // Then
            exception.Should().BeNull();
        }

        [Fact]
        public void DoesNotThrow_WhenResolutionIsFifteen()
        {
            // Given / When
            var exception = Record.Exception(() => H3Cell.LatLngToCell(37.3615593, -122.0553238, 15));

            // Then
            exception.Should().BeNull();
        }

        // ── lat validation ───────────────────────────────────────────────────

        [Fact]
        public void ThrowsArgumentException_WhenLatIsNaN()
        {
            // Given
            static void Act() => H3Cell.LatLngToCell(double.NaN, 0, 5);

            // When
            var exception = Record.Exception(Act);

            // Then
            exception.Should().BeOfType<ArgumentException>()
                .Which.ParamName.Should().Be("lat");
        }

        [Fact]
        public void ThrowsArgumentException_WhenLatIsPositiveInfinity()
        {
            // Given
            static void Act() => H3Cell.LatLngToCell(double.PositiveInfinity, 0, 5);

            // When
            var exception = Record.Exception(Act);

            // Then
            exception.Should().BeOfType<ArgumentException>()
                .Which.ParamName.Should().Be("lat");
        }

        // ── lng validation ───────────────────────────────────────────────────

        [Fact]
        public void ThrowsArgumentException_WhenLngIsNaN()
        {
            // Given
            static void Act() => H3Cell.LatLngToCell(0, double.NaN, 5);

            // When
            var exception = Record.Exception(Act);

            // Then
            exception.Should().BeOfType<ArgumentException>()
                .Which.ParamName.Should().Be("lng");
        }

        [Fact]
        public void ThrowsArgumentException_WhenLngIsNegativeInfinity()
        {
            // Given
            static void Act() => H3Cell.LatLngToCell(0, double.NegativeInfinity, 5);

            // When
            var exception = Record.Exception(Act);

            // Then
            exception.Should().BeOfType<ArgumentException>()
                .Which.ParamName.Should().Be("lng");
        }
    }
}
