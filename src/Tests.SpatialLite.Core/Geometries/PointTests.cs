using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xunit;

using SpatialLite.Core;
using SpatialLite.Core.API;
using SpatialLite.Core.Geometries;

namespace Tests.SpatialLite.Core.Geometries {
	public class PointTests {

		double _xOrdinate = 3.5;
		double _yOrdinate = 4.2;
		double _zOrdinate = 10.5;
		double _mValue = 100.4;

		Coordinate _coordinate;
		public PointTests() {
			_coordinate = new Coordinate(_xOrdinate, _yOrdinate, _zOrdinate, _mValue);
		}

		private void ChenckPosition(Point target, double x, double y, double z, double m) {
			Assert.Equal(x, target.Position.X);
			Assert.Equal(y, target.Position.Y);
			Assert.Equal(z, target.Position.Z);
			Assert.Equal(m, target.Position.M);
		}

		[Fact]
		public void Constructor__CreatesPointWithEmptyPositionAndWSG84SRID() {
			Point target = new Point();

			Assert.Equal(Coordinate.Empty, target.Position);
			Assert.Equal(SRIDList.WSG84, target.Srid);
		}

		[Fact]
		public void Constructor_XY_SetsCoordinatesAndWSG84SRID() {
			Point target = new Point(_xOrdinate, _yOrdinate);

			ChenckPosition(target, _xOrdinate, _yOrdinate, double.NaN, double.NaN);
			Assert.Equal(SRIDList.WSG84, target.Srid);
		}

		[Fact]
		public void Constructor_XYZ_SetsCoordinatesAndWSG84SRID() {
			Point target = new Point(_xOrdinate, _yOrdinate, _zOrdinate);

			ChenckPosition(target, _xOrdinate, _yOrdinate, _zOrdinate, double.NaN);
			Assert.Equal(SRIDList.WSG84, target.Srid);
		}

		[Fact]
		public void Constructor_XYZM_SetsCoordinatesAndWSG84SRID() {
			Point target = new Point(_xOrdinate, _yOrdinate, _zOrdinate, _mValue);

			ChenckPosition(target, _xOrdinate, _yOrdinate, _zOrdinate, _mValue);
			Assert.Equal(SRIDList.WSG84, target.Srid);
		}

		[Fact]
		public void Constructor_Coordinate_SetsCoordinatesAndWSG84SRID() {
			Point target = new Point(_coordinate);

			Assert.Equal(_coordinate, target.Position);
			Assert.Equal(SRIDList.WSG84, target.Srid);
		}

		[Fact]
		public void Constructor_SridCoordinate_SetsCoordinatesAndWSG84SRID() {
			int srid = -1;
			Point target = new Point(srid, _coordinate);

			Assert.Equal(_coordinate, target.Position);
			Assert.Equal(srid, target.Srid);
		}

		[Fact]
		public void Is3D_ReturnsTrueFor3DPoint() {
			Point target = new Point(_xOrdinate, _yOrdinate, _zOrdinate);

			Assert.True(target.Is3D);
		}

		[Fact]
		public void Is3D_ReturnsFalseFor2DPoint() {
			Point target = new Point(_xOrdinate, _yOrdinate);

			Assert.False(target.Is3D);
		}

		[Fact]
		public void IsMeasured_ReturnsTrueForMeasuredPoint() {
			Point target = new Point(_xOrdinate, _yOrdinate, double.NaN, _mValue);

			Assert.True(target.IsMeasured);
		}

		[Fact]
		public void IsMeasured_ReturnsFalseForNonMeasuredPoint() {
			Point target = new Point(_xOrdinate, _yOrdinate);

			Assert.False(target.IsMeasured);
		}

		[Fact]
		public void GetEnvelope_ReturnsEmptyEnvelopeForEmptyPoint() {
			Point target = new Point();
			Envelope envelope = target.GetEnvelope();

			Assert.Equal(double.NaN, envelope.MinX);
			Assert.Equal(double.NaN, envelope.MaxX);
			Assert.Equal(double.NaN, envelope.MinY);
			Assert.Equal(double.NaN, envelope.MaxY);
			Assert.Equal(double.NaN, envelope.MinZ);
			Assert.Equal(double.NaN, envelope.MaxZ);
			Assert.Equal(double.NaN, envelope.MinM);
			Assert.Equal(double.NaN, envelope.MaxM);
		}

		[Fact]
		public void GetEnvelope_ReturnsEnvelopeThatCoversOnePoint() {
			Point target = new Point(_coordinate);
			Envelope envelope = target.GetEnvelope();

			Assert.Equal(_coordinate.X, envelope.MinX);
			Assert.Equal(_coordinate.X, envelope.MaxX);
			Assert.Equal(_coordinate.Y, envelope.MinY);
			Assert.Equal(_coordinate.Y, envelope.MaxY);
			Assert.Equal(_coordinate.Z, envelope.MinZ);
			Assert.Equal(_coordinate.Z, envelope.MaxZ);
			Assert.Equal(_coordinate.M, envelope.MinM);
			Assert.Equal(_coordinate.M, envelope.MaxM);
		}

		[Fact]
		public void GetBoundary_ReturnsEmptyGeometryCollection() {
			Point target = new Point(_coordinate);
			IGeometryCollection<IGeometry> boundary = target.GetBoundary() as IGeometryCollection<IGeometry>;

			Assert.NotNull(boundary);
			Assert.Empty(boundary.Geometries);
		}
	}
}
