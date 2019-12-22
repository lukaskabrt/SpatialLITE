using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xunit;

using SpatialLite.Core;
using SpatialLite.Core.API;
using SpatialLite.Core.Geometries;

namespace Tests.SpatialLite.Core.Geometries {
	public class GeometryTests {

		class GeometryMock : Geometry {

			public GeometryMock()
				: base() {
			}

			public GeometryMock(int srid)
				: base(srid) {
			}

			public override bool Is3D {
				get { throw new NotImplementedException(); }
			}

			public override bool IsMeasured {
				get { throw new NotImplementedException(); }
			}

			public override Envelope GetEnvelope() {
				throw new NotImplementedException();
			}

			public override IGeometry GetBoundary() {
				throw new NotImplementedException();
			}

            public override IEnumerable<Coordinate> GetCoordinates() {
                throw new NotImplementedException();
            }

            public override void Apply(ICoordinateFilter filter) {
                throw new NotImplementedException();
            }
        }

		[Fact]
		public void Constructor_SetsDefaultSRID() {
			GeometryMock target = new GeometryMock();

			Assert.Equal(SRIDList.WSG84, target.Srid);
		}

		[Fact]
		public void Constructor_SRID_SetsSRID() {
			int srid = 1;
			GeometryMock target = new GeometryMock(srid);

			Assert.Equal(srid, target.Srid);
		}
	}
}
