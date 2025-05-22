using System;

using Xunit;

using SpatialLite.Gps.Geometries;
using SpatialLite.Core.API;
using SpatialLite.Gps;

namespace Tests.SpatialLite.Gps.Geometries
{
    public class GpxPointTests
    {

        double _xOrdinate = 3.5;
        double _yOrdinate = 4.2;
        double _zOrdinate = 10.5;

        Coordinate _coordinate;
        public GpxPointTests()
        {
            _coordinate = new Coordinate(_xOrdinate, _yOrdinate, _zOrdinate);
        }

        [Fact]
        public void Constructor__CreatesPointWithEmptyPositionAndNullTimestamp()
        {
            var target = new GpsPoint();

            Assert.Equal(Coordinate.Empty, target.Position);
            Assert.Null(target.Timestamp);
        }

        [Fact]
        public void Constructor_Coordinate_CreatesPointWithPositionAndNullTimestamp()
        {
            var target = new GpxPoint(_coordinate);

            Assert.Equal(_coordinate, target.Position);
            Assert.Null(target.Timestamp);
        }

        [Fact]
        public void Constructor_Coordinate_CreatesPointWithPositionAndTimestamp()
        {
            var timestamp = DateTime.Now;
            var target = new GpxPoint(_coordinate, timestamp);

            Assert.Equal(_coordinate, target.Position);
            Assert.Equal(timestamp, target.Timestamp);
        }

        [Fact]
        public void Constructor_LonLatElevationTimestamp_CreatesPointWithPositionAndTimestamp()
        {
            var timestamp = DateTime.Now;
            var target = new GpxPoint(_xOrdinate, _yOrdinate, _zOrdinate, timestamp);

            Assert.Equal(_coordinate, target.Position);
            Assert.Equal(timestamp, target.Timestamp);
        }

        [Fact]
        public void GeometryType_ReturnsWaypoint()
        {
            GpxPoint target = new GpxPoint();

            Assert.Equal(GpxGeometryType.Waypoint, target.GeometryType);
        }
    }
}
