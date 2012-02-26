using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xunit;
using Xunit.Extensions;

using SpatialLite.Core.API;

namespace Tests.SpatialLite.Core.API {
	public class CoordinateTests {

		#region Test data

		double xCoordinate = 3.5;
		double yCoordinate = 4.2;
		double zCoordinate = 10.5;
		double mValue = 100.4;

		#endregion

		#region Constructor tests

		[Fact]
		public void Constructor_XY_SetsXYValuesAndZMNaN() {
			Coordinate target = new Coordinate(xCoordinate, yCoordinate);

			Assert.Equal(xCoordinate, target.X);
			Assert.Equal(yCoordinate, target.Y);
			Assert.Equal(double.NaN, target.Z);
			Assert.Equal(double.NaN, target.M);
		}

		[Fact]
		public void Constructor_XYZ_SetsXYZValuesAndMNaN() {
			Coordinate target = new Coordinate(xCoordinate, yCoordinate, zCoordinate);

			Assert.Equal(xCoordinate, target.X);
			Assert.Equal(yCoordinate, target.Y);
			Assert.Equal(zCoordinate, target.Z);
			Assert.Equal(double.NaN, target.M);
		}

		[Fact]
		public void Constructor_XYZM_SetsXYZMValues() {
			Coordinate target = new Coordinate(xCoordinate, yCoordinate, zCoordinate, mValue);

			Assert.Equal(xCoordinate, target.X);
			Assert.Equal(yCoordinate, target.Y);
			Assert.Equal(zCoordinate, target.Z);
			Assert.Equal(mValue, target.M);
		}

		#endregion

		#region Is3D tests

		[Fact]
		public void Is3D_ReturnsFalseForNaNZCoordinate() {
			Coordinate target = new Coordinate(xCoordinate, yCoordinate);

			Assert.False(target.Is3D);
		}

		[Fact]
		public void Is3D_ReturnsTrueFor3D() {
			Coordinate target = new Coordinate(xCoordinate, yCoordinate, zCoordinate);

			Assert.True(target.Is3D);
		}

		#endregion

		#region IsMeasured tests

		[Fact]
		public void IsMeasured_ReturnsFalseForNaNMCoordinate() {
			Coordinate target = new Coordinate(xCoordinate, yCoordinate);

			Assert.False(target.IsMeasured);
		}

		[Fact]
		public void IsMeasured_ReturnsTrueForMeasuredCoordinate() {
			Coordinate target = new Coordinate(xCoordinate, yCoordinate, zCoordinate, mValue);

			Assert.True(target.IsMeasured);
		}

		#endregion

		#region Equals() tests

		[Fact]
		public void Equals_ReturnsTrueForCoordinateWithTheSameOrdinates() {
			Coordinate target = new Coordinate(xCoordinate, yCoordinate, zCoordinate, mValue);
			Coordinate other = new Coordinate(xCoordinate, yCoordinate, zCoordinate, mValue);

			Assert.True(target.Equals(other));
		}

		[Fact]
		public void Equals_ReturnsTrueForNaNCoordinates() {
			Coordinate target = new Coordinate(double.NaN, double.NaN, double.NaN, double.NaN);
			Coordinate other = new Coordinate(double.NaN, double.NaN, double.NaN, double.NaN);

			Assert.True(target.Equals(other));
		}

		[Fact]
		public void Equals_ReturnsFalseForNull() {
			Coordinate target = new Coordinate(xCoordinate, yCoordinate, zCoordinate, mValue);
			object other = null;

			Assert.False(target.Equals(other));
		}

		[Fact]
		public void Equals_ReturnsFalseForOtherObjectType() {
			Coordinate target = new Coordinate(xCoordinate, yCoordinate, zCoordinate, mValue);
			object other = "string";

			Assert.False(target.Equals(other));
		}

		[Fact]
		public void Equals_ReturnsFalseForCoordinateWithDifferentOrdinates() {
			Coordinate target = new Coordinate(xCoordinate, yCoordinate, zCoordinate, mValue);
			Coordinate other = new Coordinate(xCoordinate + 1, yCoordinate + 1, zCoordinate + 1, mValue + 1);

			Assert.False(target.Equals(other));
		}

		#endregion

		#region Equals2D(Coordinate) tests

		[Fact]
		public void Equals2D_ReturnsTrueForCoordinateWithTheSameOrdinates() {
			Coordinate target = new Coordinate(xCoordinate, yCoordinate, zCoordinate, mValue);
			Coordinate other = new Coordinate(xCoordinate, yCoordinate, zCoordinate, mValue);

			Assert.True(target.Equals2D(other));
		}

		[Fact]
		public void Equals2D_ReturnsTrueForCoordinateWithTheDifferentZMOrdinates() {
			Coordinate target = new Coordinate(xCoordinate, yCoordinate, zCoordinate, mValue);
			Coordinate other = new Coordinate(xCoordinate, yCoordinate, zCoordinate + 1, mValue + 1);

			Assert.True(target.Equals2D(other));
		}

		[Fact]
		public void Equals2D_ReturnsTrueForNaNCoordinates() {
			Coordinate target = new Coordinate(double.NaN, double.NaN, double.NaN, double.NaN);
			Coordinate other = new Coordinate(double.NaN, double.NaN, double.NaN, double.NaN);

			Assert.True(target.Equals2D(other));
		}

		[Fact]
		public void Equals2D_ReturnsFalseForCoordinateWithDifferentXYOrdinates() {
			Coordinate target = new Coordinate(xCoordinate, yCoordinate, zCoordinate, mValue);
			Coordinate other = new Coordinate(xCoordinate + 1, yCoordinate + 1, zCoordinate, mValue);

			Assert.False(target.Equals2D(other));
		}

		#endregion

		#region operator == tests

		[Fact]
		public void EqualsOperator_ReturnsTrueForCoordinateWithTheSameOrdinates() {
			Coordinate target = new Coordinate(xCoordinate, yCoordinate, zCoordinate, mValue);
			Coordinate other = new Coordinate(xCoordinate, yCoordinate, zCoordinate, mValue);

			Assert.True(target == other);
		}

		[Fact]
		public void EqualsOperator_ReturnsTrueForNaNCoordinates() {
			Coordinate target = new Coordinate(double.NaN, double.NaN, double.NaN, double.NaN);
			Coordinate other = new Coordinate(double.NaN, double.NaN, double.NaN, double.NaN);

			Assert.True(target == other);
		}

		[Fact]
		public void EqualsOperator_ReturnsFalseForCoordinateWithDifferentOrdinates() {
			Coordinate target = new Coordinate(xCoordinate, yCoordinate, zCoordinate, mValue);
			Coordinate other = new Coordinate(xCoordinate + 1, yCoordinate + 1, zCoordinate + 1, mValue + 1);

			Assert.False(target == other);
		}

		#endregion

		#region operator != tests

		[Fact]
		public void NotEqualsOperator_ReturnsFalseForCoordinateWithTheSameOrdinates() {
			Coordinate target = new Coordinate(xCoordinate, yCoordinate, zCoordinate, mValue);
			Coordinate other = new Coordinate(xCoordinate, yCoordinate, zCoordinate, mValue);

			Assert.False(target != other);
		}

		[Fact]
		public void NotEqualsOperator_ReturnsFalseForNaNCoordinates() {
			Coordinate target = new Coordinate(double.NaN, double.NaN, double.NaN, double.NaN);
			Coordinate other = new Coordinate(double.NaN, double.NaN, double.NaN, double.NaN);

			Assert.False(target != other);
		}

		[Fact]
		public void NotEqualsOperator_ReturnsTrueForCoordinateWithDifferentOrdinates() {
			Coordinate target = new Coordinate(xCoordinate, yCoordinate, zCoordinate, mValue);
			Coordinate other = new Coordinate(xCoordinate + 1, yCoordinate + 1, zCoordinate + 1, mValue + 1);

			Assert.True(target != other);
		}

		#endregion
	}
}
