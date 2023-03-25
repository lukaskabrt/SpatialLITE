using FluentAssertions;
using FluentAssertions.Execution;
using SpatialLite.Core.Api;

namespace Tests.SpatialLite.Core.FluentAssertions
{
    public static class CoordinateExtensions
    {
        public static void ShouldHaveCoordinates(this Coordinate coordinate, double x, double y, double z)
        {
            using (new AssertionScope())
            {
                coordinate.X.Should().Be(x);
                coordinate.Y.Should().Be(y);
                coordinate.Z.Should().Be(z);
            }
        }

        public static void ShouldHaveSameCoordinates(this Coordinate coordinate, Coordinate expected)
        {
            using (new AssertionScope())
            {
                coordinate.X.Should().Be(expected.X);
                coordinate.Y.Should().Be(expected.Y);
                coordinate.Z.Should().Be(expected.Z);
            }
        }
    }
}
