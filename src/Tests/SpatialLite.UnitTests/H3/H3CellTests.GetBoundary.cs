using SpatialLite.H3;

namespace SpatialLite.UnitTests.H3;

public partial class H3CellTests
{
    public class GetBoundary
    {
        // ── Shared constants ────────────────────────────────────────────────
        private const double Tolerance = 0.001;

        // SF res-5 cell (hexagonal)
        private static readonly H3Cell SfRes5Cell = new(0x085283473fffffffUL);

        // Pentagon: mode=1, res=0, base-cell=4, all 15 digit slots = 7 (padding)
        private static readonly H3Cell PentagonCell =
            new((1UL << 59) | (4UL << 45) | ((1UL << 45) - 1UL));

        // Anti-meridian cell (res 4, near lng ≈ 179.9°)
        private static readonly H3Cell AntiMeridianCell = H3Cell.Parse("847eb51ffffffff");

        // ── Behavior 1: hexagonal cell returns 6 vertices ───────────────────

        [Fact]
        public void Returns6Vertices_ForHexagonalCell()
        {
            // Given
            var cell = SfRes5Cell;

            // When
            var boundary = cell.GetBoundary();

            // Then
            boundary.Count.Should().Be(6);
        }

        // ── Behavior 2: pentagon cell returns 5 vertices ────────────────────

        [Fact]
        public void Returns5Vertices_ForPentagonCell()
        {
            // Given
            var cell = PentagonCell;

            // When
            var boundary = cell.GetBoundary();

            // Then
            boundary.Count.Should().Be(5);
        }

        // ── Behavior 3: SF res-5 vertex coordinates match H3 reference ──────

        [Theory]
        [InlineData(0, -121.9150803271, 37.2713558667)]
        [InlineData(1, -121.8622232890, 37.3539264509)]
        [InlineData(2, -121.9235499963, 37.4283411861)]
        [InlineData(3, -122.0377349643, 37.4201286777)]
        [InlineData(4, -122.0904289290, 37.3375560844)]
        [InlineData(5, -122.0291013092, 37.2631979746)]
        public void ReturnsCorrectVertexCoordinates_ForSfRes5Cell(
            int vertexIndex, double expectedLng, double expectedLat)
        {
            // Given
            var cell = SfRes5Cell;

            // When
            var boundary = cell.GetBoundary();

            // Then
            boundary[vertexIndex].X.Should().BeApproximately(expectedLng, Tolerance);
            boundary[vertexIndex].Y.Should().BeApproximately(expectedLat, Tolerance);
        }

        // ── Behavior 4: pentagon vertex coordinates match H3 reference ───────

        [Theory]
        [InlineData(0, -10.4449775448, 63.0950540775)]
        [InlineData(1, 5.5236465493, 55.7067684652)]
        [InlineData(2, 25.0827223267, 58.4015448704)]
        [InlineData(3, 31.8312804991, 68.9299578819)]
        [InlineData(4, 0.3256103519, 73.3102236854)]
        public void ReturnsCorrectVertexCoordinates_ForPentagonCell(
            int vertexIndex, double expectedLng, double expectedLat)
        {
            // Given
            var cell = PentagonCell;

            // When
            var boundary = cell.GetBoundary();

            // Then
            boundary[vertexIndex].X.Should().BeApproximately(expectedLng, Tolerance);
            boundary[vertexIndex].Y.Should().BeApproximately(expectedLat, Tolerance);
        }

        // ── Behavior 5: anti-meridian cell spans both sides of 180° ─────────

        [Fact]
        public void Returns6Vertices_AndSpansBothSidesOf180_ForAntiMeridianCell()
        {
            // Given
            var cell = AntiMeridianCell;

            // When
            var boundary = cell.GetBoundary();

            // Then – 6 vertices
            boundary.Count.Should().Be(6);

            // And – raw coordinates straddle the anti-meridian without wrapping
            var minLng = boundary.Min(c => c.X);
            var maxLng = boundary.Max(c => c.X);
            minLng.Should().BeLessThan(-170.0);
            maxLng.Should().BeGreaterThan(170.0);
        }
    }
}
