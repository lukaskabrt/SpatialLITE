using SpatialLite.H3;

namespace SpatialLite.UnitTests.H3;

public partial class H3CellTests
{
    public class LatLngToCell
    {
        // All 18 behaviours: known location → expected cell
        [Theory]
        // Sunnyvale, CA — multiple resolutions
        [InlineData(37.3615593, -122.0553238, 0, "8029fffffffffff")]
        [InlineData(37.3615593, -122.0553238, 1, "81283ffffffffff")]
        [InlineData(37.3615593, -122.0553238, 2, "822837fffffffff")]
        [InlineData(37.3615593, -122.0553238, 3, "832834fffffffff")]
        [InlineData(37.3615593, -122.0553238, 4, "8428347ffffffff")]
        [InlineData(37.3615593, -122.0553238, 5, "85283473fffffff")]
        [InlineData(37.3615593, -122.0553238, 10, "8a283470d927fff")]
        [InlineData(37.3615593, -122.0553238, 15, "8f283470d921c65")]
        // (0°, 0°) — pentagon base cell 58
        [InlineData(0.0, 0.0, 0, "8075fffffffffff")]
        [InlineData(0.0, 0.0, 5, "85754e67fffffff")]
        // North pole
        [InlineData(90.0, 0.0, 0, "8001fffffffffff")]
        [InlineData(90.0, 0.0, 5, "85032623fffffff")]
        // South pole
        [InlineData(-90.0, 0.0, 0, "80f3fffffffffff")]
        // Cities
        [InlineData(51.5074, -0.1278, 5, "85194ad3fffffff")] // London
        [InlineData(35.6762, 139.6503, 5, "852f5a37fffffff")] // Tokyo
        [InlineData(-33.8688, 151.2093, 5, "85be0e37fffffff")] // Sydney
        // Antimeridian
        [InlineData(0.0, 180.0, 5, "857eb573fffffff")]
        // Statue of Liberty
        [InlineData(40.689167, -74.044444, 5, "852a1073fffffff")]
        public void ReturnsExpectedCell_ForKnownLocation(
            double lat, double lng, int resolution, string expectedHex)
        {
            // When
            var cell = H3Cell.LatLngToCell(lat, lng, resolution);

            // Then
            cell.Should().Be(H3Cell.Parse(expectedHex));
        }
    }
}
