
using FluentAssertions;
using SpatialLite.Core.Api;
using SpatialLite.Core.Geometries;
using Tests.SpatialLite.Core.FluentAssertions;
using Xunit;

namespace Tests.SpatialLite.Core.Geometries
{
    public class PolygonTests
    {

        Coordinate[] _coordinatesXY = new[] {
            new Coordinate(1, 2), new Coordinate(1.1f, 2.1f), new Coordinate(1.2f, 2.2f), new Coordinate(1.3f, 2.3f)
        };

        Coordinate[] _coordinatesXYZ = new[] {
            new Coordinate(1, 2, 3), new Coordinate(1.1f, 2.1f, 3.1f), new Coordinate(1.2f, 2.2f, 3.2f), new Coordinate(1.3f, 2.3f, 3.3f)
        };

        CoordinateSequence _exteriorRing3D;
        CoordinateSequence[] _interiorRings3D;

        public PolygonTests()
        {
            _exteriorRing3D = new CoordinateSequence(_coordinatesXYZ);

            _interiorRings3D = new CoordinateSequence[2];
            _interiorRings3D[0] = new CoordinateSequence(new Coordinate[] { _coordinatesXYZ[0], _coordinatesXYZ[1], _coordinatesXYZ[0] });
            _interiorRings3D[1] = new CoordinateSequence(new Coordinate[] { _coordinatesXYZ[1], _coordinatesXYZ[2], _coordinatesXYZ[1] });
        }

        [Fact]
        public void Constructor_CreatesEmptyPolygonAndInitializesProperties()
        {
            var target = new Polygon();

            target.ExteriorRing.Should().BeEmpty();
            target.InteriorRings.Should().BeEmpty();
        }

        [Fact]
        public void Constructor_CreatesPolygonWithExteriorBoundary()
        {
            var target = new Polygon(_exteriorRing3D);

            target.ExteriorRing.Should().BeEquivalentTo(_exteriorRing3D);
            target.InteriorRings.Should().BeEmpty();
        }

        [Fact]
        public void Is3D_ReturnsTrueFor3DExteriorRing()
        {
            var target = new Polygon(_exteriorRing3D);

            target.Is3D.Should().BeTrue();
        }

        [Fact]
        public void Is3D_ReturnsFalseForEmptyPolygon()
        {
            var target = new Polygon();

            target.Is3D.Should().BeFalse();
        }

        [Fact]
        public void Is3D_ReturnsFalseFor2DExteriorRing()
        {
            var target = new Polygon(new CoordinateSequence(_coordinatesXY));

            target.Is3D.Should().BeFalse();
        }

        [Fact]
        public void GetEnvelope_ReturnsEmptyEnvelopeForEmptyPolygon()
        {
            var target = new Polygon();

            var envelope = target.GetEnvelope();

            envelope.IsEmpty.Should().BeTrue();
        }

        [Fact]
        public void GetEnvelopeReturnsEnvelopeOfLineString()
        {
            var target = new Polygon(new CoordinateSequence(_coordinatesXY));

            var envelope = target.GetEnvelope();

            envelope.ShouldHaveSameBounds(new Envelope2D(_coordinatesXY));
        }
    }
}
