using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xunit;

using SpatialLite.Core;
using SpatialLite.Core.API;
using SpatialLite.Core.Geometries;

namespace Tests.SpatialLite.Core.Geometries {
	public class GeometryCollectionTests {
		#region Test Data

		Point[] _geometries;


		Coordinate[] _coordinatesXYZM = new Coordinate[] {
				new Coordinate(12,10,100, 1000),
				new Coordinate(22,20,200, 2000),
				new Coordinate(32,30,300, 3000)
		};

		#endregion

		public GeometryCollectionTests() {
			_geometries = new Point[3];
			_geometries[0] = new Point(1, 2, 3);
			_geometries[1] = new Point(1.1, 2.1, 3.1);
			_geometries[2] = new Point(1.2, 2.2, 3.2);
		}

		private void CheckGeometries(GeometryCollection<Geometry> target, Geometry[] geometries) {
			Assert.Equal(geometries.Length, target.Geometries.Count);

			for (int i = 0; i < geometries.Length; i++) {
				Assert.Same(geometries[i], target.Geometries[i]);
			}
		}

		#region Constructors tests

		#region Default constructors tests

		[Fact]
		public void Constructor__CreatesNewEmptyCollectionInWSG84() {
			GeometryCollection<Geometry> target = new GeometryCollection<Geometry>();

			Assert.Equal(SRIDList.WSG84, target.Srid);
			Assert.NotNull(target.Geometries);
			Assert.Empty(target.Geometries);
		}

		#endregion

		#region Constructor(SRID) tests

		[Fact]
		public void Constructor_SRID_CreatesEmptyCollectionInSpecifiedSRID() {
			int srid = 1;
			GeometryCollection<Geometry> target = new GeometryCollection<Geometry>(srid);

			Assert.Equal(srid, target.Srid);
			Assert.NotNull(target.Geometries);
			Assert.Empty(target.Geometries);
		}

		#endregion

		#region Constructor(IEnumerable<IGeometry>)

		[Fact]
		public void Constructor_IEnumerable_CreateNewCollectionWithDataInWSG84() {
			GeometryCollection<Geometry> target = new GeometryCollection<Geometry>(_geometries);

			Assert.Equal(SRIDList.WSG84, target.Srid);
			CheckGeometries(target, _geometries);
		}

		#endregion

		#region Constructor(SRID, IEnumerable)

		[Fact]
		public void Constructor_SRIDIEnumerable_CreateNewCollectionWithDataInWSpecifiedSRID() {
			int srid = 1;
			GeometryCollection<Geometry> target = new GeometryCollection<Geometry>(srid, _geometries);

			Assert.Equal(srid, target.Srid);
			CheckGeometries(target, _geometries);
		}

		#endregion

		#endregion

		#region Is3D tests

		[Fact]
		public void Is3D_ReturnsFalseForEmptyCollection() {
			GeometryCollection<Geometry> target = new GeometryCollection<Geometry>();

			Assert.False(target.Is3D);
		}

		[Fact]
		public void Is3D_ReturnsFalseForCollectionOf2DObjects() {
			var members2d = new Geometry[] { new Point(1, 2), new Point(2, 3) };
			GeometryCollection<Geometry> target = new GeometryCollection<Geometry>(members2d);

			Assert.False(target.Is3D);
		}

		[Fact]
		public void Is3D_ReturnsTrueForCollectionWithAtLeastOne3DObject() {
			GeometryCollection<Geometry> target = new GeometryCollection<Geometry>(_geometries);

			Assert.True(target.Is3D);
		}

		#endregion

		#region IsMeasured tests

		[Fact]
		public void IsMeasured_ReturnsFalseForEmptyCollection() {
			GeometryCollection<Geometry> target = new GeometryCollection<Geometry>();

			Assert.False(target.IsMeasured);
		}

		[Fact]
		public void IsMeasured_ReturnsFalseForCollectionOfNonMeasuredObjects() {
			GeometryCollection<Geometry> target = new GeometryCollection<Geometry>(_geometries);

			Assert.False(target.IsMeasured);
		}

		[Fact]
		public void IsMeasured_ReturnsTrueForCollectionWithAtLeastOneMeasuredObject() {
			var members = new Geometry[] { new Point(1, 2), new Point(2, 3, 4, 5) };
			GeometryCollection<Geometry> target = new GeometryCollection<Geometry>(members);

			Assert.True(target.IsMeasured);
		}

		#endregion

		#region GetEnvelope tests

		[Fact]
		public void GetEnvelopeReturnsEmptyEnvelopeForEmptyCollection() {
			GeometryCollection<Geometry> target = new GeometryCollection<Geometry>();

			Assert.Equal(Envelope.Empty, target.GetEnvelope());
		}

		[Fact]
		public void GetEnvelopeReturnsUnionOfMembersEnvelopes() {
			GeometryCollection<Geometry> target = new GeometryCollection<Geometry>(_geometries);
			Envelope expected = new Envelope(new Coordinate[] {_geometries[0].Position, _geometries[1].Position, _geometries[2].Position});

			Assert.Equal(expected, target.GetEnvelope());
		}

		#endregion

		#region GetBoundary tests

		[Fact]
		public void GetBoundary_ReturnsGeometryCollectionWithBoundariesOfObjectsInCollection() {
			int srid = 1;
			GeometryCollection<Geometry> target = new GeometryCollection<Geometry>(srid);
			target.Geometries.Add(new LineString(srid, _coordinatesXYZM));

			IGeometryCollection<IGeometry> boundary = target.GetBoundary() as IGeometryCollection<IGeometry>;

			Assert.NotNull(boundary);
			Assert.Equal(srid, boundary.Srid);
			Assert.Equal(target.Geometries.Count, boundary.Geometries.Count());

			IMultiPoint expectedChildBoundary = target.Geometries[0].GetBoundary() as IMultiPoint;
			IMultiPoint childBoundary = boundary.Geometries.First() as IMultiPoint;;

			Assert.Equal(expectedChildBoundary.Geometries.First().Position, childBoundary.Geometries.First().Position);
			Assert.Equal(expectedChildBoundary.Geometries.Last().Position, childBoundary.Geometries.Last().Position);
		}

		#endregion
	}
}
