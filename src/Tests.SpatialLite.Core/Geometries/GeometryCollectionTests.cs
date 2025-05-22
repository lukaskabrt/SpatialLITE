using SpatialLite.Core.API;
using SpatialLite.Core.Geometries;
using Xunit;

namespace Tests.SpatialLite.Core.Geometries;

public class GeometryCollectionTests
{
    private readonly Point[] _geometries;
    private readonly Coordinate[] _coordinatesXYZM = new Coordinate[] {
            new Coordinate(12,10,100, 1000),
            new Coordinate(22,20,200, 2000),
            new Coordinate(32,30,300, 3000)
    };

    public GeometryCollectionTests()
    {
        _geometries = new Point[3];
        _geometries[0] = new Point(1, 2, 3);
        _geometries[1] = new Point(1.1, 2.1, 3.1);
        _geometries[2] = new Point(1.2, 2.2, 3.2);
    }

    private void CheckGeometries(GeometryCollection<Geometry> target, Geometry[] geometries)
    {
        Assert.Equal(geometries.Length, target.Geometries.Count);

        for (int i = 0; i < geometries.Length; i++)
        {
            Assert.Same(geometries[i], target.Geometries[i]);
        }
    }

    [Fact]
    public void Constructor__CreatesNewEmptyCollection()
    {
        GeometryCollection<Geometry> target = new GeometryCollection<Geometry>();

        Assert.NotNull(target.Geometries);
        Assert.Empty(target.Geometries);
    }

    [Fact]
    public void Constructor_IEnumerable_CreateNewCollectionWithData()
    {
        GeometryCollection<Geometry> target = new GeometryCollection<Geometry>(_geometries);

        CheckGeometries(target, _geometries);
    }

    [Fact]
    public void Is3D_ReturnsFalseForEmptyCollection()
    {
        GeometryCollection<Geometry> target = new GeometryCollection<Geometry>();

        Assert.False(target.Is3D);
    }

    [Fact]
    public void Is3D_ReturnsFalseForCollectionOf2DObjects()
    {
        var members2d = new Geometry[] { new Point(1, 2), new Point(2, 3) };
        GeometryCollection<Geometry> target = new GeometryCollection<Geometry>(members2d);

        Assert.False(target.Is3D);
    }

    [Fact]
    public void Is3D_ReturnsTrueForCollectionWithAtLeastOne3DObject()
    {
        GeometryCollection<Geometry> target = new GeometryCollection<Geometry>(_geometries);

        Assert.True(target.Is3D);
    }

    [Fact]
    public void IsMeasured_ReturnsFalseForEmptyCollection()
    {
        GeometryCollection<Geometry> target = new GeometryCollection<Geometry>();

        Assert.False(target.IsMeasured);
    }

    [Fact]
    public void IsMeasured_ReturnsFalseForCollectionOfNonMeasuredObjects()
    {
        GeometryCollection<Geometry> target = new GeometryCollection<Geometry>(_geometries);

        Assert.False(target.IsMeasured);
    }

    [Fact]
    public void IsMeasured_ReturnsTrueForCollectionWithAtLeastOneMeasuredObject()
    {
        var members = new Geometry[] { new Point(1, 2), new Point(2, 3, 4, 5) };
        GeometryCollection<Geometry> target = new GeometryCollection<Geometry>(members);

        Assert.True(target.IsMeasured);
    }

    [Fact]
    public void GetEnvelopeReturnsEmptyEnvelopeForEmptyCollection()
    {
        GeometryCollection<Geometry> target = new GeometryCollection<Geometry>();

        Assert.Equal(Envelope.Empty, target.GetEnvelope());
    }

    [Fact]
    public void GetEnvelopeReturnsUnionOfMembersEnvelopes()
    {
        GeometryCollection<Geometry> target = new GeometryCollection<Geometry>(_geometries);
        Envelope expected = new Envelope(new Coordinate[] { _geometries[0].Position, _geometries[1].Position, _geometries[2].Position });

        Assert.Equal(expected, target.GetEnvelope());
    }
}
