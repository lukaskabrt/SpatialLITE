
using FluentAssertions;
using SpatialLite.Core.Api;
using Xunit;

namespace Tests.SpatialLite.Core.API
{
    public class CoordinateTests
    {
        const double xCoordinate = 3.5f;
        const double yCoordinate = 4.2f;
        const double zCoordinate = 10.5f;

        [Fact]
        public void Constructor_XY_SetsXYValuesAndZNaN()
        {
            var target = new Coordinate(xCoordinate, yCoordinate);

            target.X.Should().Be(xCoordinate);
            target.Y.Should().Be(yCoordinate);
            target.Z.Should().Be(double.NaN);
        }

        [Fact]
        public void Constructor_XYZ_SetsXYZValues()
        {
            var target = new Coordinate(xCoordinate, yCoordinate, zCoordinate);

            target.X.Should().Be(xCoordinate);
            target.Y.Should().Be(yCoordinate);
            target.Z.Should().Be(zCoordinate);
        }

        /* Is3D */

        [Fact]
        public void Is3D_ReturnsFalseForNaNZCoordinate()
        {
            var target = new Coordinate(xCoordinate, yCoordinate);

            target.Is3D.Should().BeFalse();
        }

        [Fact]
        public void Is3D_ReturnsTrueFor3D()
        {
            var target = new Coordinate(xCoordinate, yCoordinate, zCoordinate);

            target.Is3D.Should().BeTrue();
        }

        /* Equals */

        [Fact]
        public void Equals_ReturnsTrueForCoordinateWithTheSameOrdinates()
        {
            var target = new Coordinate(xCoordinate, yCoordinate, zCoordinate);
            var other = new Coordinate(xCoordinate, yCoordinate, zCoordinate);

            target.Equals(other).Should().BeTrue();
        }

        [Fact]
        public void Equals_ReturnsTrueForTwoEmptyCoordinates()
        {
            var target = Coordinate.Empty;
            var other = Coordinate.Empty;

            target.Equals(other).Should().BeTrue();
        }

        [Fact]
        public void Equals_ReturnsFalseForNull()
        {
            var target = new Coordinate(xCoordinate, yCoordinate, zCoordinate);
            object other = null;

            target.Equals(other).Should().BeFalse();
        }

        [Fact]
        public void Equals_ReturnsFalseForOtherObjectType()
        {
            var target = new Coordinate(xCoordinate, yCoordinate, zCoordinate);
            object other = "string";

            target.Equals(other).Should().BeFalse();
        }

        [Fact]
        public void Equals_ReturnsFalseForEmptyAndNonEmptyCoordinates()
        {
            var target = new Coordinate(xCoordinate, yCoordinate, zCoordinate); ;
            var other = Coordinate.Empty;

            target.Equals(other).Should().BeFalse();
        }

        [Theory]
        [InlineData(xCoordinate + 1, yCoordinate, zCoordinate)]
        [InlineData(xCoordinate, yCoordinate + 1, zCoordinate)]
        [InlineData(xCoordinate, yCoordinate, zCoordinate + 1)]
        public void Equals_ReturnsFalseForCoordinateWithDifferentOrdinates(double x, double y, double z)
        {
            var target = new Coordinate(xCoordinate, yCoordinate, zCoordinate);
            var other = new Coordinate(x, y, z);

            target.Equals(other).Should().BeFalse();
        }

        /* Equals2D(Coordinate) */

        [Fact]
        public void Equals2D_ReturnsTrueForCoordinateWithSameXYOrdinates()
        {
            var target = new Coordinate(xCoordinate, yCoordinate, zCoordinate);
            var other = new Coordinate(xCoordinate, yCoordinate, zCoordinate);

            target.Equals2D(other).Should().BeTrue();
        }

        [Fact]
        public void Equals2D_ReturnsTrueForCoordinateWithTheDifferentZOrdinates()
        {
            var target = new Coordinate(xCoordinate, yCoordinate, zCoordinate);
            var other = new Coordinate(xCoordinate, yCoordinate, zCoordinate + 1);

            target.Equals2D(other).Should().BeTrue();
        }

        [Fact]
        public void Equals2D_ReturnsTrueForEmptyCoordinates()
        {
            var target = Coordinate.Empty;
            var other = Coordinate.Empty;

            target.Equals2D(other).Should().BeTrue();
        }


        [Fact]
        public void Equals2D_ReturnsFalseForEmptyAndNonEmptyCoordinates()
        {
            var target = new Coordinate(xCoordinate, yCoordinate, zCoordinate); ;
            var other = Coordinate.Empty;

            target.Equals2D(other).Should().BeFalse();
        }

        [Theory]
        [InlineData(xCoordinate + 1, yCoordinate)]
        [InlineData(xCoordinate, yCoordinate + 1)]
        public void Equals2D_ReturnsFalseForCoordinateWithDifferentXYOrdinates(double x, double y)
        {
            var target = new Coordinate(xCoordinate, yCoordinate, zCoordinate);
            var other = new Coordinate(x, y, zCoordinate);

            target.Equals2D(other).Should().BeFalse();
        }
    }
}
