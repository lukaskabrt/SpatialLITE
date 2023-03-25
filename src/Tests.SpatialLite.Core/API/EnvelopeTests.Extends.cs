using SpatialLite.Core.Api;
using System;
using Tests.SpatialLite.Core.FluentAssertions;
using Xunit;

namespace Tests.SpatialLite.Core.API
{
    public partial class Envelope2DTests
    {
        /* Extend(Coordinate) */

        [Fact]
        public void Extend_SetsMinMaxValuesOnEmptyEnvelope2DForSingleCoordinate()
        {
            var source = Envelope2D.Empty;
            var coordinate = new Coordinate(1, 2);

            var target = source.Extend(coordinate);

            target.ShouldHaveBounds(coordinate.X, coordinate.X, coordinate.Y, coordinate.Y);
        }

        [Fact]
        public void Extend_DoesNothingIfCoordinateIsEmpty()
        {
            var source = new Envelope2D(new[] { new Coordinate(1, 2), new Coordinate(-1, -2) });

            var target = source.Extend(Coordinate.Empty);

            target.ShouldHaveSameBounds(source);
        }

        public static TheoryData<Coordinate, double, double, double, double> CoordinateWithExtendedBounds => new()
        {
            { new Coordinate(10, 0), -1, 10, -2, 2 },
            { new Coordinate(-10, 0), -10, 1, -2, 2 },
            { new Coordinate(0, 10), -1, 1, -2, 10 },
            { new Coordinate(0, -10), -1, 1, -10, 2 }
        };

        [Theory]
        [MemberData(nameof(CoordinateWithExtendedBounds))]
        public void Extend_ExtendsEnvelope2DWithSingleCoordinate(Coordinate coordinate, double minX, double maxX, double minY, double maxY)
        {
            var source = new Envelope2D(new[] { new Coordinate(1, 2), new Coordinate(-1, -2) });

            var target = source.Extend(coordinate);

            target.ShouldHaveBounds(minX, maxX, minY, maxY);
        }

        [Fact]
        public void Extend_DoesNothingForSingleCoordinateInsideEnvelope2D()
        {
            var source = new Envelope2D(new[] { new Coordinate(1, 2), new Coordinate(-1, -2) });

            var target = source.Extend(new Coordinate(0.5f, 0.5f));

            target.ShouldHaveSameBounds(source);
        }

        /* Extend(IReadOnlyList<Coordinate>) */

        [Fact]
        public void Extend_SetsMinMaxValuesOnEmptyEnvelope2DForMultipleCoordinates()
        {
            var source = Envelope2D.Empty;

            var target = source.Extend(new[] { new Coordinate(1, 2), new Coordinate(-1, -2) });

            target.ShouldHaveBounds(-1, 1, -2, 2);
        }

        [Fact]
        public void Extend_DoesNothingForEmptyCollection()
        {
            var source = new Envelope2D(new[] { new Coordinate(1, 2), new Coordinate(-1, -2) });

            var target = source.Extend(Array.Empty<Coordinate>());

            target.ShouldHaveSameBounds(source);
        }

        [Fact]
        public void Extend_DoesNothingForCoordinatesInsideEnvelope2D()
        {
            var source = new Envelope2D(new[] { new Coordinate(1, 2), new Coordinate(-1, -2) });

            var target = source.Extend(new[] { new Coordinate(0.5f, 0.5f), new Coordinate(0.25f, 0.25f) });

            target.ShouldHaveSameBounds(source);
        }

        [Fact]
        public void Extend_Coordinates_ExtendsEnvelope2D()
        {
            var source = new Envelope2D(new[] { new Coordinate(1, 2), new Coordinate(-1, -2) });

            var target = source.Extend(new[] { new Coordinate(-10, -20), new Coordinate(10, 20) });

            target.ShouldHaveBounds(-10, 10, -20, 20);
        }

        /* Extend(Envelope2D) */

        [Fact]
        public void Extend_SetsMinMaxValuesOnEmptyEnvelope2D()
        {
            var source = Envelope2D.Empty;
            var other = new Envelope2D(new[] { new Coordinate(1, 2), new Coordinate(-1, -2) });

            var target = source.Extend(other);

            target.ShouldHaveSameBounds(other);
        }

        [Fact]
        public void Extend_DoesNothingIfOtherEnvelope2DIsEmpty()
        {
            var source = new Envelope2D(new[] { new Coordinate(1, 2), new Coordinate(-1, -2) });
            var other = Envelope2D.Empty;

            var target = source.Extend(other);

            target.ShouldHaveSameBounds(source);
        }

        [Fact]
        public void Extend_DoesNothingIfEnvelope2DIsInsideTargetEnvelope2D()
        {
            var source = new Envelope2D(new[] { new Coordinate(10, 20), new Coordinate(-10, -20) });
            var other = new Envelope2D(new[] { new Coordinate(1, 2), new Coordinate(-1, -2) });

            var target = source.Extend(other);

            target.ShouldHaveSameBounds(source);
        }

        [Fact]
        public void Extend_ExtendsEnvelope2DWithOtherEnvelope2D()
        {
            var source = new Envelope2D(new[] { new Coordinate(1, 2), new Coordinate(-1, -2) });
            var other = new Envelope2D(new[] { new Coordinate(10, 20), new Coordinate(-10, -20) });

            var target = source.Extend(other);

            target.ShouldHaveSameBounds(other);
        }
    }
}
