using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xunit;
using Xunit.Extensions;

using SpatialLite.Core.API;

namespace Tests.SpatialLite.Core.API {
	public class CoordinateTests {
		double xCoordinate = 3.5;
		double yCoordinate = 4.2;
		double zCoordinate = 10.5;
		double mValue = 100.4;

		[Fact]
		public void Constructor_XY_SetsXYValues() {
			Coordinate target = new Coordinate(xCoordinate, yCoordinate);

			Assert.Equal(xCoordinate, target.X);
			Assert.Equal(yCoordinate, target.Y);
		}

		[Fact]
		public void Equals_ReturnsTrueForCoordinateWithTheSameOrdinates() {
			Coordinate target = new Coordinate(xCoordinate, yCoordinate);
			Coordinate other = new Coordinate(xCoordinate, yCoordinate);

			Assert.True(target.Equals(other));
		}

		[Fact]
		public void Equals_ReturnsTrueForNaNCoordinates() {
			Coordinate target = new Coordinate(double.NaN, double.NaN);
			Coordinate other = new Coordinate(double.NaN, double.NaN);

			Assert.True(target.Equals(other));
		}

		[Fact]
		public void Equals_ReturnsFalseForNull() {
			Coordinate target = new Coordinate(xCoordinate, yCoordinate);
			object other = null;

			Assert.False(target.Equals(other));
		}

		[Fact]
		public void Equals_ReturnsFalseForOtherObjectType() {
			Coordinate target = new Coordinate(xCoordinate, yCoordinate);
			object other = "string";

			Assert.False(target.Equals(other));
		}

		[Fact]
		public void Equals_ReturnsFalseForCoordinateWithDifferentOrdinates() {
			Coordinate target = new Coordinate(xCoordinate, yCoordinate);
			Coordinate other = new Coordinate(xCoordinate + 1, yCoordinate + 1);

			Assert.False(target.Equals(other));
		}

		[Fact]
		public void Equals2D_ReturnsTrueForCoordinateWithTheSameOrdinates() {
			Coordinate target = new Coordinate(xCoordinate, yCoordinate);
			Coordinate other = new Coordinate(xCoordinate, yCoordinate);

			Assert.True(target.Equals2D(other));
		}

		[Fact]
		public void Equals2D_ReturnsTrueForNaNCoordinates() {
			Coordinate target = new Coordinate(double.NaN, double.NaN);
			Coordinate other = new Coordinate(double.NaN, double.NaN);

			Assert.True(target.Equals2D(other));
		}

		[Fact]
		public void Equals2D_ReturnsFalseForCoordinateWithDifferentXYOrdinates() {
			Coordinate target = new Coordinate(xCoordinate, yCoordinate);
			Coordinate other = new Coordinate(xCoordinate + 1, yCoordinate + 1);

			Assert.False(target.Equals2D(other));
		}

		[Fact]
		public void EqualsOperator_ReturnsTrueForCoordinateWithTheSameOrdinates() {
			Coordinate target = new Coordinate(xCoordinate, yCoordinate);
			Coordinate other = new Coordinate(xCoordinate, yCoordinate);

			Assert.True(target == other);
		}

		[Fact]
		public void EqualsOperator_ReturnsTrueForNaNCoordinates() {
			Coordinate target = new Coordinate(double.NaN, double.NaN);
			Coordinate other = new Coordinate(double.NaN, double.NaN);

			Assert.True(target == other);
		}

		[Fact]
		public void EqualsOperator_ReturnsFalseForCoordinateWithDifferentOrdinates() {
			Coordinate target = new Coordinate(xCoordinate, yCoordinate);
			Coordinate other = new Coordinate(xCoordinate + 1, yCoordinate + 1);

			Assert.False(target == other);
		}

		[Fact]
		public void NotEqualsOperator_ReturnsFalseForCoordinateWithTheSameOrdinates() {
			Coordinate target = new Coordinate(xCoordinate, yCoordinate);
			Coordinate other = new Coordinate(xCoordinate, yCoordinate);

			Assert.False(target != other);
		}

		[Fact]
		public void NotEqualsOperator_ReturnsFalseForNaNCoordinates() {
			Coordinate target = new Coordinate(double.NaN, double.NaN);
			Coordinate other = new Coordinate(double.NaN, double.NaN);

			Assert.False(target != other);
		}

		[Fact]
		public void NotEqualsOperator_ReturnsTrueForCoordinateWithDifferentOrdinates() {
			Coordinate target = new Coordinate(xCoordinate, yCoordinate);
			Coordinate other = new Coordinate(xCoordinate + 1, yCoordinate + 1);

			Assert.True(target != other);
		}
	}
}
