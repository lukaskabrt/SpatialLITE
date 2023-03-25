
using FluentAssertions;
using SpatialLite.Core.Api;
using SpatialLite.Core.Geometries;
using Xunit;

namespace Tests.SpatialLite.Core.Geometries
{
    public class GeometryCollectionTests
    {

        Point[] _geometries;

        Coordinate[] _coordinatesXYZ = new Coordinate[] {
                new Coordinate(12,10,100),
                new Coordinate(22,20,200),
                new Coordinate(32,30,300)
        };

        public GeometryCollectionTests()
        {
            _geometries = new Point[2];
            _geometries[0] = new Point(1, 2, 3);
            _geometries[1] = new Point(2, 3, 4);
        }

        [Fact]
        public void Constructor_CreatesNewEmptyCollection()
        {
            var target = new GeometryCollection<Geometry>();

            target.Geometries.Should().BeEmpty();
        }

        [Fact]
        public void Constructor_CreateNewCollectionWithData()
        {
            var target = new GeometryCollection<Geometry>(_geometries);

            target.Geometries.Should().BeEquivalentTo(_geometries);
        }

        [Fact]
        public void Is3D_ReturnsFalseForEmptyCollection()
        {
            var target = new GeometryCollection<Geometry>();

            target.Is3D.Should().BeFalse();
        }

        [Fact]
        public void Is3D_ReturnsFalseForCollectionOf2DObjects()
        {
            var members2d = new Geometry[] { new Point(1, 2), new Point(2, 3) };
            var target = new GeometryCollection<Geometry>(members2d);

            target.Is3D.Should().BeFalse();
        }

        [Fact]
        public void Is3D_ReturnsTrueForCollectionWithAtLeastOne3DObject()
        {
            var target = new GeometryCollection<Geometry>(_geometries);

            target.Is3D.Should().BeTrue();
        }

        [Fact]
        public void GetEnvelopeReturnsEmptyEnvelopeForEmptyCollection()
        {
            var target = new GeometryCollection<Geometry>();

            var envelope = target.GetEnvelope();

            envelope.IsEmpty.Should().BeTrue();
        }

        [Fact]
        public void GetEnvelopeReturnsUnionOfMembersEnvelopes()
        {
            var expected = new Envelope2D(new Coordinate[] { _geometries[0].Position, _geometries[1].Position });
            var target = new GeometryCollection<Geometry>(_geometries);

            var envelope = target.GetEnvelope();

            envelope.Should().Equals(expected);
        }
    }
}
