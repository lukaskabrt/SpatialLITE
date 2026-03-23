using SpatialLite.Contracts;
using SpatialLite.Gpx.Geometries;

namespace SpatialLite.UnitTests.Gpx.Geometries;

public class GpxRouteTests
{
    [Fact]
    public void Constructor_Parameterless_InitializesEmptyRoute()
    {
        var route = new GpxRoute();

        Assert.NotNull(route.Points);
        Assert.Empty(route.Points);
    }

    [Fact]
    public void Constructor_Points_InitializesRouteWithPoints()
    {
        var points = new List<GpxPoint>
        {
            new(new Coordinate(1.0, 2.0)),
            new(new Coordinate(3.0, 4.0))
        };

        var route = new GpxRoute(points);

        Assert.Equal(points, route.Points);
    }
}
