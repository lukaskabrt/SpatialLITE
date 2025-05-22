using SpatialLite.Gps;
using SpatialLite.Gps.Geometries;
using Xunit;

namespace Tests.SpatialLite.Gps.Geometries;

public class GpxTrackTests
{
    [Fact]
    public void Constructor_Parameterless_CreateEmptyGpxTrack()
    {
        GpxTrack target = new GpxTrack();

        Assert.Empty(target.Geometries);
    }

    [Fact]
    public void Constructor_Segments_CreateGpxTrackWithSegments()
    {
        GpxTrackSegment[] segments = new GpxTrackSegment[] { new GpxTrackSegment(), new GpxTrackSegment(), new GpxTrackSegment() };

        GpxTrack target = new GpxTrack(segments);

        Assert.Equal(segments.Length, target.Geometries.Count);
        for (int i = 0; i < target.Geometries.Count; i++)
        {
            Assert.Same(segments[i], target.Geometries[i]);
        }
    }

    [Fact]
    public void GeometryType_ReturnsTrack()
    {
        GpxTrack target = new GpxTrack();

        Assert.Equal(GpxGeometryType.Track, target.GeometryType);
    }
}
