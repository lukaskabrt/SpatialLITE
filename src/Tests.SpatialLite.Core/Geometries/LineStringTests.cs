using FluentAssertions;
using SpatialLite.Core.Api;
using SpatialLite.Core.Geometries;
using System.Linq;
using Xunit;

namespace Tests.SpatialLite.Core.Geometries
{
    public class LineStringTests
    {

        Coordinate[] _coordinatesXY = new[] {
                new Coordinate(12,10),
                new Coordinate(22,20),
                new Coordinate(32,30)
        };

        Coordinate[] _coordinatesXYZ = new[] {
                new Coordinate(12,10,100),
                new Coordinate(22,20,200),
                new Coordinate(32,30,300)
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
        public void Constructor_CreatesEmptyLineString()
        {
            var target = new LineString();

            target.Coordinates.Should().BeEmpty();
        }

        [Fact]
        public void Constructor_CreatesLineStringFromCoordinates()
        {
            var target = new LineString(_coordinatesXYZ);

            target.Coordinates.Should().BeEquivalentTo(_coordinatesXYZ);
        }

        [Fact]
        public void Is3D_ReturnsFalseForEmptyLineString()
        {
            var target = new LineString();

            target.Is3D.Should().BeFalse();
        }

        [Fact]
        public void Is3D_ReturnsFalseForAll2DCoords()
        {
            var target = new LineString(_coordinatesXY);

            target.Is3D.Should().BeFalse();
        }

        [Fact]
        public void Is3D_ReturnsTrueForAll3DCoords()
        {
            var target = new LineString(_coordinatesXYZ);

            target.Is3D.Should().BeTrue();
        }

        [Fact]
        public void Start_ReturnsEmptyCoordinateForEmptyLineString()
        {
            var target = new LineString();

            target.Start.IsEmpty.Should().BeTrue();
        }

        [Fact]
        public void Start_ReturnsFirstCoordinate()
        {
            var target = new LineString(_coordinatesXYZ);

            target.Start.Should().Equals(_coordinatesXYZ.First());
        }

        [Fact]
        public void End_ReturnsEmptyCoordinateForEmptyLineString()
        {
            var target = new LineString();

            target.End.IsEmpty.Should().BeTrue();
        }

        [Fact]
        public void End_ReturnsLastCoordinate()
        {
            LineString target = new LineString(_coordinatesXYZ);

            target.End.Should().Equals(_coordinatesXYZ.Last());
        }

        [Fact]
        public void IsClosed_ReturnsTrueForClosedLineString()
        {
            var coordinates = new[] { new Coordinate(0, 0), new Coordinate(1, 0), new Coordinate(1, 1), new Coordinate(0, 0) };
            var target = new LineString(coordinates);

            target.IsClosed.Should().BeTrue();
        }

        [Fact]
        public void IsClosed_ReturnsFalseForOpenLineString()
        {
            var target = new LineString(_coordinatesXYZ);

            target.IsClosed.Should().BeFalse();
        }

        [Fact]
        public void IsClosed_ReturnsFalseForEmptyLineString()
        {
            var target = new LineString();

            target.IsClosed.Should().BeFalse();
        }

        [Fact]
        public void GetEnvelope_ReturnsEmptyEnvelopeForEmptyLineString()
        {
            var target = new LineString();

            var envelope = target.GetEnvelope();

            envelope.IsEmpty.Should().BeTrue();
        }

        [Fact]
        public void GetEnvelope_ReturnsEnvelopeOfLineString()
        {
            var target = new LineString(_coordinatesXYZ);

            var envelope = target.GetEnvelope();

            envelope.Should().Equals(new Envelope2D(_coordinatesXYZ));
        }
    }
}
