using SpatialLite.H3;

namespace SpatialLite.UnitTests.H3;

public partial class H3CellTests
{
    public class FromLatLng
    {
        private const double SfLat = 37.3615593;
        private const double SfLng = -122.0553238;

        [Fact]
        public void ReturnsCorrectCell_ForSfRes5()
        {
            // Given
            const ulong expected = 0x085283473fffffffUL;

            // When
            var result = H3Cell.FromLatLng(SfLat, SfLng, resolution: 5);

            // Then
            result.Value.Should().Be(expected);
        }

        [Fact]
        public void ReturnsCorrectCell_ForSfRes7()
        {
            // Given
            const ulong expected = 0x0872830828ffffffUL;

            // When
            var result = H3Cell.FromLatLng(SfLat, SfLng, resolution: 7);

            // Then
            result.Value.Should().Be(expected);
        }

        [Fact]
        public void ReturnsCorrectCell_ForSfRes10()
        {
            // Given
            const ulong expected = 0x08a2830828147fffUL;

            // When
            var result = H3Cell.FromLatLng(SfLat, SfLng, resolution: 10);

            // Then
            result.Value.Should().Be(expected);
        }

        [Fact]
        public void ReturnsCorrectCell_ForSfRes15()
        {
            // Given
            const ulong expected = 0x08f2830828052d25UL;

            // When
            var result = H3Cell.FromLatLng(SfLat, SfLng, resolution: 15);

            // Then
            result.Value.Should().Be(expected);
        }

        [Fact]
        public void ReturnsValidCellAtRes0_ForOriginCoordinate()
        {
            // Given
            const double lat = 0.0;
            const double lng = 0.0;

            // When
            var result = H3Cell.FromLatLng(lat, lng, resolution: 0);

            // Then
            result.IsValidCell.Should().BeTrue();
            result.GetResolution().Should().Be(0);
        }
    }
}
