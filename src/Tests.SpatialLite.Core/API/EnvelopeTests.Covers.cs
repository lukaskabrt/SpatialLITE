using FluentAssertions;
using SpatialLite.Core.Api;
using System;
using Xunit;

namespace Tests.SpatialLite.Core.API
{
    public partial class Envelope2DTests
    {
        /* Covers(Coordinate) */

        [Fact]
        public void Covers_ReturnsTrueForCoordinateInsideEnvelope()
        {
            var coordinate = new Coordinate(0.5f, 0.5f);
            var target = new Envelope2D(new[] { new Coordinate(1, 2), new Coordinate(-1, -2) });

            target.Covers(coordinate).Should().BeTrue();
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(-1, 0)]
        [InlineData(0, 2)]
        [InlineData(0, -2)]
        public void Covers_ReturnsTrueForCoordinateOnBoundary(double x, double y)
        {
            var coordinate = new Coordinate(x, y);
            var target = new Envelope2D(new[] { new Coordinate(1, 2), new Coordinate(-1, -2) });

            target.Covers(coordinate).Should().BeTrue();
        }

        [Fact]
        public void Covers_ReturnsFalseForEmptyCoordinate()
        {
            var target = new Envelope2D(new[] { new Coordinate(1, 2), new Coordinate(-1, -2) });

            target.Covers(Coordinate.Empty).Should().BeFalse();
        }

        [Fact]
        public void Covers_ReturnsFalseForEmptyEnvelope()
        {
            var coordinate = new Coordinate(0, 0);
            var target = Envelope2D.Empty;

            target.Covers(Coordinate.Empty).Should().BeFalse();
        }

        [Theory]
        [InlineData(10, 0)]
        [InlineData(-10, 0)]
        [InlineData(0, 20)]
        [InlineData(0, -20)]
        public void Covers_ReturnsFalseForCoordinateOutsideEnvelope(double x, double y)
        {
            var coordinate = new Coordinate(x, y);
            var target = new Envelope2D(new[] { new Coordinate(1, 2), new Coordinate(-1, -2) });

            target.Covers(coordinate).Should().BeFalse();
        }

        /* Covers(Envelope2D) */

        [Fact]
        public void Covers_ReturnsTrueForEnvelopeInsideTargetEnvelope()
        {
            var target = new Envelope2D(new[] { new Coordinate(10, 20), new Coordinate(-10, -20) });
            var other = new Envelope2D(new[] { new Coordinate(1, 2), new Coordinate(-1, -2) });

            target.Covers(other).Should().BeTrue();
        }

        [Fact]
        public void Covers_ReturnsTrueForEnvelopeWithSameBounds()
        {
            var target = new Envelope2D(new[] { new Coordinate(1, 2), new Coordinate(-1, -2) });
            var other = new Envelope2D(new[] { new Coordinate(1, 2), new Coordinate(-1, -2) });

            target.Covers(other).Should().BeTrue();
        }

        [Fact]
        public void Covers_ReturnsTrueForEmptyEnvelope()
        {
            var target = new Envelope2D(new[] { new Coordinate(10, 20), new Coordinate(-10, -20) });

            target.Covers(Envelope2D.Empty).Should().BeFalse();
        }

        [Fact]
        public void Covers_ReturnsFalseForEmptyTargetEnvelope()
        {
            var other = new Envelope2D(new Coordinate(0, 0));

            Envelope2D.Empty.Covers(other).Should().BeFalse();
        }

        [Fact]
        public void Covers_ReturnsFalseForEnvelopeOutsideTargetEnvelope()
        {
            var target = new Envelope2D(new[] { new Coordinate(1, 2), new Coordinate(-1, -2) });
            var other = new Envelope2D(new[] { new Coordinate(10, 20), new Coordinate(-10, -20) });

            target.Covers(other).Should().BeFalse();
        }

        [Fact]
        public void Covers_ReturnsFalseForPartiallyIntersectingEnvelope()
        {
            var target = new Envelope2D(new[] { new Coordinate(1, 2), new Coordinate(-1, -2) });
            var other = new Envelope2D(new[] { new Coordinate(10, 20), new Coordinate(0, 0) });

            target.Covers(other).Should().BeFalse();
        }
    }
}
