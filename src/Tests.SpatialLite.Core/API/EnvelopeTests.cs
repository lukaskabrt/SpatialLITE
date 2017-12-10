using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xunit;
using Xunit.Extensions;

using SpatialLite.Core.API;
using SpatialLite.Core.Geometries;

namespace Tests.SpatialLite.Core.API {
	public class EnvelopeTests {
		#region Test data

		Coordinate[] _coordinates = new Coordinate[] {
				new Coordinate(1, 10, 100, 1000),
				new Coordinate(0, 0, 0, 0),
				new Coordinate(-1, -10, -100, -1000)
		};

		Coordinate _insideCoordinate = new Coordinate(0.5, 0.5, 0.5, 0.5);
		Coordinate _lowerValues = new Coordinate(-2, -20, -200, -2000);
		Coordinate _higherValues = new Coordinate(2, 20, 200, 2000);

		double[] _expectedBounds = new double[] { -1, 1, -10, 10, -100, 100, -1000, 1000 };

		static public IEnumerable<object[]> _XYZEnvelopeDifferentBounds {
			get {
				yield return new object[] { new Coordinate[] { new Coordinate(1 + 1, 2, 100, 1000), new Coordinate(5, 6, 200, 2000)} };
				yield return new object[] { new Coordinate[] { new Coordinate(1, 2 + 1, 100, 1000), new Coordinate(5, 6, 200, 2000) } };
				yield return new object[] { new Coordinate[] { new Coordinate(1, 2, 100 + 1, 1000), new Coordinate(5, 6, 200, 2000) } };
				yield return new object[] { new Coordinate[] { new Coordinate(1, 2, 100, 1000 + 1), new Coordinate(5, 6, 200, 2000) } };
				yield return new object[] { new Coordinate[] { new Coordinate(1, 2, 100, 1000), new Coordinate(5 + 1, 6, 200, 2000) } };
				yield return new object[] { new Coordinate[] { new Coordinate(1, 2, 100, 1000), new Coordinate(5, 6 + 1, 200, 2000) } };
				yield return new object[] { new Coordinate[] { new Coordinate(1, 2, 100, 1000), new Coordinate(5, 6, 200 + 1, 2000) } };
				yield return new object[] { new Coordinate[] { new Coordinate(1, 2, 100, 1000), new Coordinate(5, 6, 200, 2000 + 1) } };
			}
		}

        #endregion

        #region Hepler functions

        internal void CheckBoundaries(Envelope target, double minX, double maxX, double minY, double maxY, double minZ, double maxZ, double minM, double maxM) {
			Assert.Equal(minX, target.MinX);
			Assert.Equal(maxX, target.MaxX);
			Assert.Equal(minY, target.MinY);
			Assert.Equal(maxY, target.MaxY);
			Assert.Equal(minZ, target.MinZ);
			Assert.Equal(maxZ, target.MaxZ);
			Assert.Equal(minM, target.MinM);
			Assert.Equal(maxM, target.MaxM);
		}

		#endregion

		#region Constructors tests

		#region Default constructor tests

		[Fact]
		public void Constructor__InitializesBoundsToNaNValues() {
			Envelope target = new Envelope();

			Assert.Equal(double.NaN, target.MinX);
			Assert.Equal(double.NaN, target.MaxX);
			Assert.Equal(double.NaN, target.MinY);
			Assert.Equal(double.NaN, target.MaxY);
			Assert.Equal(double.NaN, target.MinZ);
			Assert.Equal(double.NaN, target.MaxZ);
			Assert.Equal(double.NaN, target.MinM);
			Assert.Equal(double.NaN, target.MaxM);
		}

		#endregion

		#region Constructor(Coordinate) tests

		[Fact]
		public void Constructor_Coordinate_InitializesXYZProperties() {
			Envelope target = new Envelope(_coordinates[0]);

			CheckBoundaries(target, _coordinates[0].X, _coordinates[0].X, _coordinates[0].Y, _coordinates[0].Y, _coordinates[0].Z, _coordinates[0].Z, _coordinates[0].M, _coordinates[0].M);
		}

		#endregion

		#region Constructor(IEnumerable<Coordinate>) tests

		[Fact]
		public void Constructor_IEnumerableCoordinate_SetsMinMaxValues() {
			Envelope source = new Envelope(_coordinates);

			Envelope target = new Envelope(source);

			CheckBoundaries(target, _expectedBounds[0], _expectedBounds[1], _expectedBounds[2], _expectedBounds[3],
				_expectedBounds[4], _expectedBounds[5], _expectedBounds[6], _expectedBounds[7]);
		}

		#endregion

		#region Constructor(Envelope) tests

		[Fact]
		public void Constructor_Envelope_CopiesMinMaxValues() {
			Envelope source = new Envelope(_coordinates);

			Envelope target = new Envelope(source);

			CheckBoundaries(target, _expectedBounds[0], _expectedBounds[1], _expectedBounds[2], _expectedBounds[3],
				_expectedBounds[4], _expectedBounds[5], _expectedBounds[6], _expectedBounds[7]);
		}

		#endregion

		#endregion

		#region Extend(Coordinate) tests

		[Fact]
		public void Extend_Coordinate_SetsMinMaxValuesOnEmptyEnvelope() {
			Envelope target = new Envelope();
			target.Extend(_coordinates[0]);

			CheckBoundaries(target, _coordinates[0].X, _coordinates[0].X, _coordinates[0].Y, _coordinates[0].Y,
				_coordinates[0].Z, _coordinates[0].Z, _coordinates[0].M, _coordinates[0].M);
		}

		[Fact]
		public void Extend_Coordinate_DoNothingIfCoordinateIsEmpty() {
			Envelope target = new Envelope(_coordinates);

			target.Extend(Coordinate.Empty);

			CheckBoundaries(target, _expectedBounds[0], _expectedBounds[1], _expectedBounds[2], _expectedBounds[3],
				_expectedBounds[4], _expectedBounds[5], _expectedBounds[6], _expectedBounds[7]);
		}

		[Fact]
		public void Extend_Coordinate_ExtendsEnvelopeToLowerValues() {
			Envelope target = new Envelope(_coordinates);

			target.Extend(_lowerValues);

			CheckBoundaries(target, _lowerValues.X, _expectedBounds[1], _lowerValues.Y, _expectedBounds[3], _lowerValues.Z, _expectedBounds[5], _lowerValues.M, _expectedBounds[7]);
		}

		[Fact]
		public void Extend_Coordinate_ExtendsEnvelopeToHigherValues() {
			Envelope target = new Envelope(_coordinates);

			target.Extend(_higherValues);

			CheckBoundaries(target, _expectedBounds[0], _higherValues.X, _expectedBounds[2], _higherValues.Y,	_expectedBounds[4], _higherValues.Z, _expectedBounds[6], _higherValues.M);
		}

		[Fact]
		public void Extend_Coordinate_DoNothingForCoordinateInsideEnvelope() {
			Envelope target = new Envelope(_coordinates);

			target.Extend(_insideCoordinate);

			CheckBoundaries(target, _expectedBounds[0], _expectedBounds[1], _expectedBounds[2], _expectedBounds[3],
				_expectedBounds[4], _expectedBounds[5], _expectedBounds[6], _expectedBounds[7]);
		}

		#endregion

		#region Extend(IEnumerable<Coordinate>) tests

		[Fact]
		public void Extend_IEnumerableCoordinate_SetsMinMaxValuesOnEmptyEnvelope() {
			Envelope target = new Envelope();

			target.Extend(_coordinates);

			CheckBoundaries(target, _expectedBounds[0], _expectedBounds[1], _expectedBounds[2], _expectedBounds[3],
				_expectedBounds[4], _expectedBounds[5], _expectedBounds[6], _expectedBounds[7]);
		}

		[Fact]
		public void Extend_IEnumerableCoordinate_DoNothingForEmptyCollection() {
			Envelope target = new Envelope(_coordinates);

			target.Extend(new Coordinate[] {});

			CheckBoundaries(target, _expectedBounds[0], _expectedBounds[1], _expectedBounds[2], _expectedBounds[3],
				_expectedBounds[4], _expectedBounds[5], _expectedBounds[6], _expectedBounds[7]);
		}

		[Fact]
		public void Extend_IEnumerableCoordinate_ExtendsEnvelope() {
			Envelope target = new Envelope(_coordinates);

			target.Extend(new Coordinate[] {_lowerValues, _higherValues });

			CheckBoundaries(target, _lowerValues.X, _higherValues.X, _lowerValues.Y, _higherValues.Y, _lowerValues.Z, _higherValues.Z, _lowerValues.M, _higherValues.M);
		}

		#endregion

		#region Extend(Envelope) tests

		[Fact]
		public void Extend_Envelope_SetsMinMaxValuesOnEmptyEnvelope() {
			Envelope target = new Envelope();

			target.Extend(new Envelope(_coordinates));

			CheckBoundaries(target, _expectedBounds[0], _expectedBounds[1], _expectedBounds[2], _expectedBounds[3],
				_expectedBounds[4], _expectedBounds[5], _expectedBounds[6], _expectedBounds[7]);
		}

		[Fact]
		public void Extend_Envelope_DoNothingIfEnvelopeIsInsideTargetEnvelope() {
			Envelope target = new Envelope(_coordinates);

			target.Extend(new Envelope(_coordinates[1]));

			CheckBoundaries(target, _expectedBounds[0], _expectedBounds[1], _expectedBounds[2], _expectedBounds[3],
				_expectedBounds[4], _expectedBounds[5], _expectedBounds[6], _expectedBounds[7]);
		}

		[Fact]
		public void Extend_Envelope_ExtendsEnvelope() {
			Envelope target = new Envelope(_coordinates);

			target.Extend(new Envelope(new Coordinate[] {_lowerValues, _higherValues}));

			CheckBoundaries(target, _lowerValues.X, _higherValues.X, _lowerValues.Y, _higherValues.Y, _lowerValues.Z, _higherValues.Z, _lowerValues.M, _higherValues.M);
		}

		#endregion

		#region Equals(object), Equals(Envelope) tests

		[Fact]
		public void Equals_ReturnsTrueForSameObjectInstance() {
			Envelope target = new Envelope(_coordinates);

			Assert.True(target.Equals(target));
		}

		[Fact]
		public void Equals_ReturnsTrueForTheEnvelopeWithTheSameBounds() {
			Envelope target = new Envelope(_coordinates);
			Envelope other = new Envelope(target);

			Assert.True(target.Equals(other));
		}

		[Fact]
		public void Equals_ReturnsFalseForNull() {
			Envelope target = new Envelope(_coordinates);
			object other = null;

			Assert.False(target.Equals(other));
		}

		[Fact]
		public void Equals_ReturnsFalseForOtherObjectType() {
			Envelope target = new Envelope(_coordinates);
			object other = "string";

			Assert.False(target.Equals(other));
		}

		[Theory]
		[MemberData(nameof(_XYZEnvelopeDifferentBounds))]
		public void Equals_ReturnsFalseForTheEnvelopeWithDifferentBounds(Coordinate[] corners) {
			Envelope target = new Envelope(_coordinates);
			Envelope other = new Envelope(corners);

			Assert.False(target.Equals(other));
		}

		#endregion
	}
}
