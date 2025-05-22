using SpatialLite.Gps;
using SpatialLite.Gps.Geometries;
using Xunit;

namespace Tests.SpatialLite.Gps.Geometries;

public class GpxTrackTests
{
    [Fact]
    public void Constructor_Parameterless_CreateEmptyGpxTrack()
    {
        GpxTrack target = new();

        Assert.Empty(target.Geometries);
    }

    [Fact]
    public void Constructor_Segments_CreateGpxTrackWithSegments()
    {
        GpxTrackSegment[] segments = new GpxTrackSegment[] { new(), new(), new() };

        GpxTrack target = new(segments);

        Assert.Equal(segments.Length, target.Geometries.Count);
        for (int i = 0; i < target.Geometries.Count; i++)
        {
            Assert.Same(segments[i], target.Geometries[i]);
        }
    }

    [Fact]
    public void GeometryType_ReturnsTrack()
    {
        GpxTrack target = new();

        Assert.Equal(GpxGeometryType.Track, target.GeometryType);
    }
}
