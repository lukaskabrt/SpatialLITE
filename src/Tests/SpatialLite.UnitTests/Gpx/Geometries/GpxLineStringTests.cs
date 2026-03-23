using SpatialLite.Contracts;
using SpatialLite.Gpx.Geometries;

namespace SpatialLite.UnitTests.Gpx.Geometries;

public class GpxLineStringTests
{
    [Fact]
    public void Constructor_Parameterless_InitializesClassWithEmptyPointsAndCoordinatesList()
    {
        var lineString = new GpxLineString();

        Assert.NotNull(lineString.Points);
        Assert.NotNull(lineString.Coordinates);

        Assert.Empty(lineString.Points);
        Assert.Empty(lineString.Coordinates);
    }

    [Fact]
    public void Start_ReturnsEmptyCoordinate_IfLineStringIsEmpty()
    {
        var target = new GpxLineString();

        Assert.Equal(Coordinate.Empty, target.Start);
    }

    [Fact]
    public void Start_ReturnsFirstCoordinate()
    {
        var points = new List<GpxPoint>
        {
            new(new Coordinate(1, 2)),
            new(new Coordinate(3, 4))
        };

        var target = new GpxLineString(points);

        Assert.Equal(points.First().Position, target.Start);
    }

    [Fact]
    public void End_ReturnsEmptyCoordinate_IfLineStringIsEmpty()
    {
        var target = new GpxLineString();

        Assert.Equal(Coordinate.Empty, target.End);
    }

    [Fact]
    public void End_ReturnsLastCoordinate()
    {
        var points = new List<GpxPoint>
        {
            new(new Coordinate(1, 2)),
            new(new Coordinate(3, 4))
        };

        var target = new GpxLineString(points);

        Assert.Equal(points.Last().Position, target.End);
    }

    [Fact]
    public void IsClosed_ReturnsTrue_IfLineStringIsClosed()
    {
        var points = new List<GpxPoint>
        {
            new(new Coordinate(1, 2)),
            new(new Coordinate(3, 4)),
            new(new Coordinate(1, 2))
        };

        var target = new GpxLineString(points);

        Assert.True(target.IsClosed);
    }

    [Fact]
    public void IsClosed_ReturnsFalse_IfLineStringIsOpen()
    {
        var points = new List<GpxPoint>
        {
            new(new Coordinate(1, 2)),
            new(new Coordinate(3, 4))
        };

        var target = new GpxLineString(points);

        Assert.False(target.IsClosed);
    }

    [Fact]
    public void IsClosed_ReturnsFalse_IfLineStringIsEmpty()
    {
        var target = new GpxLineString();

        Assert.False(target.IsClosed);
    }

    [Fact]
    public void GetEnvelope_ReturnsEmptyEnvelopeForEmptyLineString()
    {
        var target = new GpxLineString();
        var envelope = target.GetEnvelope();

        Assert.Equal(Envelope.Empty, envelope);
    }

    [Fact]
    public void GetEnvelope_ReturnsEnvelopeOfLineString()
    {
        var points = new List<GpxPoint>
        {
            new(new Coordinate(1, 2)),
            new(new Coordinate(3, 4)),
            new(new Coordinate(1, 2))
        };

        var target = new GpxLineString(points);

        var expected = new Envelope(points.Select(p => p.Position));

        Assert.Equal(expected, target.GetEnvelope());
    }
}
