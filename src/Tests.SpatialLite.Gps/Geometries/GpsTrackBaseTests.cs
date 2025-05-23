using SpatialLite.Core.Geometries;
using SpatialLite.Gps.Geometries;
using System;
using System.Collections.Generic;
using Xunit;

namespace Tests.SpatialLite.Gps.Geometries;

public class GpsTrackBaseTests
{
    private readonly List<GpsPoint> _points;

    public GpsTrackBaseTests()
    {
        _points = new List<GpsPoint> {
        new(16.5, 45.9, 100, new DateTime(2011, 2, 24, 20, 00, 00)),
        new(16.6, 46.0, 110, new DateTime(2011, 2, 24, 20, 00, 10)),
        new(16.5, 46.1, 200, new DateTime(2011, 2, 24, 20, 00, 20))};

    }

    [Fact]
    public void Constructor__CreatesEmptyGpsTrack()
    {
        GpsTrackBase<GpsPoint> target = new();

        Assert.Equal(0, target.Coordinates.Count);
    }

    [Fact]
    public void Constructor_IEnumerablePoints_CreatesGpsTrackWithPoints()
    {

        GpsTrackBase<GpsPoint> target = new(_points);

        Assert.Equal(_points.Count, target.Points.Count);
        for (int i = 0; i < target.Points.Count; i++)
        {
            Assert.Same(_points[i], target.Points[i]);
        }
    }

    [Fact]
    public void Coordinates_GetsPositionOfPoints()
    {
        GpsTrackBase<GpsPoint> target = new(_points);

        Assert.Equal(_points.Count, target.Coordinates.Count);
        for (int i = 0; i < _points.Count; i++)
        {
            Assert.Equal(_points[i].Position, target.Coordinates[i]);
        }
    }

    [Fact]
    public void Coordinates_GetsPositionOfPointsIfWayCastedToLineString()
    {
        GpsTrackBase<GpsPoint> line = new(_points);
        LineString target = line;

        Assert.Equal(_points.Count, target.Coordinates.Count);
        for (int i = 0; i < _points.Count; i++)
        {
            Assert.Equal(_points[i].Position, target.Coordinates[i]);
        }
    }
}
