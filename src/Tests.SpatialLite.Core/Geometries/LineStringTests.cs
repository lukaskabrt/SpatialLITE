using SpatialLite.Core.API;
using SpatialLite.Core.Geometries;
using System.Linq;
using Xunit;

namespace Tests.SpatialLite.Core.Geometries;

public class LineStringTests
{
    private readonly Coordinate[] _coordinatesXY = new Coordinate[] {
            new(12,10),
            new(22,20),
            new(32,30)
    };
    private readonly Coordinate[] _coordinatesXYZ = new Coordinate[] {
            new(12,10,100),
            new(22,20,200),
            new(32,30,300)
    };
    private readonly Coordinate[] _coordinatesXYZM = new Coordinate[] {
            new(12,10,100, 1000),
            new(22,20,200, 2000),
            new(32,30,300, 3000)
    };


    private void CheckCoordinates(LineString target, Coordinate[] expectedPoints)
    {
        Assert.Equal(expectedPoints.Length, target.Coordinates.Count);

        for (int i = 0; i < expectedPoints.Length; i++)
        {
            Assert.Equal(expectedPoints[i], target.Coordinates[i]);
        }
    }

    [Fact]
    public void Constructor__CreatesEmptyLineString()
    {
        LineString target = new();

        Assert.Equal(0, target.Coordinates.Count);
    }

    [Fact]
    public void Constructor_IEnumerable_CreatesLineStringFromCoordinates()
    {
        LineString target = new(_coordinatesXYZ);

        CheckCoordinates(target, _coordinatesXYZ);
    }

    [Fact]
    public void Is3D_ReturnsFalseForEmptyLineString()
    {
        LineString target = new();

        Assert.False(target.Is3D);
    }

    [Fact]
    public void Is3D_ReturnsFalseForAll2DCoords()
    {
        LineString target = new(_coordinatesXY);

        Assert.False(target.Is3D);
    }

    [Fact]
    public void Is3D_ReturnsTrueForAll3DCoords()
    {
        LineString target = new(_coordinatesXYZ);

        Assert.True(target.Is3D);
    }

    [Fact]
    public void IsMeasured_ReturnsFalseForEmptyLineString()
    {
        LineString target = new();

        Assert.False(target.IsMeasured);
    }

    [Fact]
    public void IsMeasured_ReturnsFalseForNonMeasuredCoords()
    {
        LineString target = new(_coordinatesXYZ);

        Assert.False(target.IsMeasured);
    }

    [Fact]
    public void IsMeasured_ReturnsTrueForMeasuredCoords()
    {
        LineString target = new(_coordinatesXYZM);

        Assert.True(target.IsMeasured);
    }

    [Fact]
    public void Start_ReturnsEmptyCoordinateForEmptyLineString()
    {
        LineString target = new();

        Assert.Equal(Coordinate.Empty, target.Start);
    }

    [Fact]
    public void Start_ReturnsFirstCoordinate()
    {
        LineString target = new(_coordinatesXYZ);

        Assert.Equal(_coordinatesXYZ.First(), target.Start);
    }

    [Fact]
    public void End_ReturnsEmptyCoordinateForEmptyLineString()
    {
        LineString target = new();

        Assert.Equal(Coordinate.Empty, target.End);
    }

    [Fact]
    public void End_ReturnsLastCoordinate()
    {
        LineString target = new(_coordinatesXYZ);

        Assert.Equal(_coordinatesXYZ.Last(), target.End);
    }

    [Fact]
    public void IsClosed_ReturnsTrueForClosedLineString()
    {
        LineString target = new(_coordinatesXYZ);
        target.Coordinates.Add(target.Coordinates[0]);

        Assert.True(target.IsClosed);
    }

    [Fact]
    public void IsClosed_ReturnsFalseForOpenLineString()
    {
        LineString target = new(_coordinatesXYZ);

        Assert.False(target.IsClosed);
    }

    [Fact]
    public void IsClosed_ReturnsFalseForEmptyLineString()
    {
        LineString target = new();

        Assert.False(target.IsClosed);
    }

    [Fact]
    public void GetEnvelope_ReturnsEmptyEnvelopeForEmptyLineString()
    {
        LineString target = new();
        Envelope envelope = target.GetEnvelope();

        Assert.Equal(Envelope.Empty, envelope);
    }

    [Fact]
    public void GetEnvelope_ReturnsEnvelopeOfLineString()
    {
        LineString target = new(_coordinatesXYZ);
        Envelope expected = new(_coordinatesXYZ);

        Assert.Equal(expected, target.GetEnvelope());
    }
}
