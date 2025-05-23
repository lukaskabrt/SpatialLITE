using SpatialLite.Gps;
using SpatialLite.Gps.Geometries;
using System;
using System.Collections.Generic;
using Xunit;

namespace Tests.SpatialLite.Gps.Geometries;

public class GpxRouteTests
{
    [Fact]
    public void Constructor_Parameterless_CreatesEmptyRoute()
    {
        GpxRoute target = new();

        Assert.Empty(target.Points);
    }

    [Fact]
    public void Constructor_IEnumerablePoints_CreatesRouteWithPoints()
    {
        List<GpxPoint> points = new()
        {
        new GpxPoint(16.5, 45.9, 100, new DateTime(2011, 2, 24, 20, 00, 00)),
        new GpxPoint(16.6, 46.0, 110, new DateTime(2011, 2, 24, 20, 00, 10)),
        new GpxPoint(16.5, 46.1, 200, new DateTime(2011, 2, 24, 20, 00, 20))};

        GpxRoute target = new(points);

        Assert.Equal(points.Count, target.Points.Count);
        for (int i = 0; i < target.Points.Count; i++)
        {
            Assert.Same(points[i], target.Points[i]);
        }
    }

    [Fact]
    public void GeometryType_ReturnsRoute()
    {
        GpxRoute target = new();

        Assert.Equal(GpxGeometryType.Route, target.GeometryType);
    }
}
