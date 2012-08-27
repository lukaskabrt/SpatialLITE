using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialLite.Gps.Geometries;
using Xunit;

namespace Tests.SpatialLite.Gps.Geometries {
    public class GpxRouteTests {
        #region Constructor tests

        #region Constructor() tests

        [Fact]
        public void Constructor_Parameterless_CreatesEmptyRoute() {
            GpxRoute target = new GpxRoute();

            Assert.Empty(target.Points);
        }

        #endregion

        #region Constructor(IEnumerable<GpxPoint>) tests

        [Fact]
        public void Constructor_IEnumerablePoints_CreatesRouteWithPoints() {
            List<GpxPoint> points = new List<GpxPoint> {
			new GpxPoint(16.5, 45.9, 100, new DateTime(2011, 2, 24, 20, 00, 00)),
			new GpxPoint(16.6, 46.0, 110, new DateTime(2011, 2, 24, 20, 00, 10)),
			new GpxPoint(16.5, 46.1, 200, new DateTime(2011, 2, 24, 20, 00, 20))};

            GpxRoute target = new GpxRoute(points);

            Assert.Equal(points.Count, target.Points.Count);
            for (int i = 0; i < target.Points.Count; i++) {
                Assert.Same(points[i], target.Points[i]);
            }
        }

        #endregion

        #endregion
    }
}
