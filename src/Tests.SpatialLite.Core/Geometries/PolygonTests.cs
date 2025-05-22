using SpatialLite.Core.API;
using SpatialLite.Core.Geometries;
using Xunit;

namespace Tests.SpatialLite.Core.Geometries;

public class PolygonTests
{
    private readonly Coordinate[] _coordinatesXY = new Coordinate[] {
                new(1, 2), new(1.1, 2.1), new(1.2, 2.2), new(1.3, 2.3)
    };
    private readonly Coordinate[] _coordinatesXYZ = new Coordinate[] {
                new(1, 2, 3), new(1.1, 2.1, 3.1), new(1.2, 2.2, 3.2), new(1.3, 2.3, 3.3)
    };
    private readonly Coordinate[] _coordinatesXYZM = new Coordinate[] {
                new(1, 2, 3, 10), new(1.1, 2.1, 3.1, 20), new(1.2, 2.2, 3.2, 30), new(1.3, 2.3, 3.3, 40)
    };
    private readonly CoordinateList _exteriorRing3D;
    private readonly CoordinateList[] _interiorRings3D;

    public PolygonTests()
    {
        _exteriorRing3D = new CoordinateList(_coordinatesXYZ);

        _interiorRings3D = new CoordinateList[2];
        _interiorRings3D[0] = new CoordinateList(new Coordinate[] { _coordinatesXYZ[0], _coordinatesXYZ[1], _coordinatesXYZ[0] });
        _interiorRings3D[1] = new CoordinateList(new Coordinate[] { _coordinatesXYZ[1], _coordinatesXYZ[2], _coordinatesXYZ[1] });
    }

    private void CheckInteriorRings(Polygon target, CoordinateList[] expected)
    {
        Assert.Equal(expected.Length, target.InteriorRings.Count);
        for (int i = 0; i < expected.Length; i++)
        {
            Assert.Same(expected[i], target.InteriorRings[i]);
        }
    }

    [Fact]
    public void Constructor__CreatesEmptyPolygonAndInitializesProperties()
    {
        Polygon target = new();

        Assert.NotNull(target.ExteriorRing);
        Assert.Empty(target.ExteriorRing);

        Assert.NotNull(target.InteriorRings);
        Assert.Empty(target.InteriorRings);
    }

    [Fact]
    public void Constructor_ExteriorRing_CreatesPolygonWithExteriorBoundary()
    {
        Polygon target = new(_exteriorRing3D);

        Assert.Same(_exteriorRing3D, target.ExteriorRing);

        Assert.NotNull(target.InteriorRings);
        Assert.Empty(target.InteriorRings);
    }

    [Fact]
    public void Is3D_ReturnsTrueFor3DExteriorRing()
    {
        Polygon target = new(_exteriorRing3D);

        Assert.True(target.Is3D);
    }

    [Fact]
    public void Is3D_ReturnsFalseForEmptyPolygon()
    {
        Polygon target = new();

        Assert.False(target.Is3D);
    }

    [Fact]
    public void Is3D_ReturnsFalseFor2DExteriorRing()
    {
        Polygon target = new(new CoordinateList(_coordinatesXY));

        Assert.False(target.Is3D);
    }

    [Fact]
    public void IsMeasured_ReturnsTrueForMeasuredExteriorRing()
    {
        Polygon target = new(new CoordinateList(_coordinatesXYZM));

        Assert.True(target.IsMeasured);
    }

    [Fact]
    public void IsMeasured_ReturnsFalseForEmptyPolygon()
    {
        Polygon target = new();

        Assert.False(target.IsMeasured);
    }

    [Fact]
    public void IsMeasured_ReturnsFalseForNonMeasuredExteriorRing()
    {
        Polygon target = new(new CoordinateList(_coordinatesXYZ));

        Assert.False(target.IsMeasured);
    }

    [Fact]
    public void GetEnvelope_ReturnsEmptyEnvelopeForEmptyPolygon()
    {
        Polygon target = new();
        Envelope envelope = target.GetEnvelope();

        Assert.Equal(Envelope.Empty, envelope);
    }

    [Fact]
    public void GetEnvelopeReturnsEnvelopeOfLineString()
    {
        Envelope expectedEnvelope = new(_coordinatesXYZ);

        Polygon target = new(_exteriorRing3D);
        Envelope envelope = target.GetEnvelope();

        Assert.Equal(expectedEnvelope, envelope);
    }
}
