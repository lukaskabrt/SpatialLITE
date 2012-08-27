using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

using SpatialLite.Gps.Geometries;
using SpatialLite.Core.API;

namespace Tests.SpatialLite.Gps.Geometries {
    public class GpxPointTests {
        #region Test data

		double _xOrdinate = 3.5;
		double _yOrdinate = 4.2;
		double _zOrdinate = 10.5;

		Coordinate _coordinate;
 
		#endregion

        public GpxPointTests() {
			_coordinate = new Coordinate(_xOrdinate, _yOrdinate, _zOrdinate);
        }

        #region Constructor tests

        #region Constructor() tests

        [Fact]
        public void Constructor__CreatesPointWithEmptyPositionAndNullTimestamp() {
            var target = new GpsPoint();

            Assert.Equal(Coordinate.Empty, target.Position);
            Assert.Null(target.Timestamp);
        }
        
        #endregion

        #region Constructor(Coordinate) tests

        [Fact]
        public void Constructor_Coordinate_CreatesPointWithPositionAndNullTimestamp() {
            var target = new GpxPoint(_coordinate);

            Assert.Equal(_coordinate, target.Position);
            Assert.Null(target.Timestamp);
        }

        #endregion

        #region Constructor(Coordinate, Timestamp) tests

        [Fact]
        public void Constructor_Coordinate_CreatesPointWithPositionAndTimestamp() {
            var timestamp = DateTime.Now;
            var target = new GpxPoint(_coordinate, timestamp);

            Assert.Equal(_coordinate, target.Position);
            Assert.Equal(timestamp, target.Timestamp);
        }

        
        #endregion

        #region Constructor(Lon, Lat, Elevation, Timestamp) tests

        [Fact]
        public void Constructor_LonLatElevationTimestamp_CreatesPointWithPositionAndTimestamp() {
            var timestamp = DateTime.Now;
            var target = new GpxPoint(_xOrdinate, _yOrdinate, _zOrdinate, timestamp);

            Assert.Equal(_coordinate, target.Position);
            Assert.Equal(timestamp, target.Timestamp);
        }

        #endregion

        #endregion
    }
}
