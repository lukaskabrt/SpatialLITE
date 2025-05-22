using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xunit;

using SpatialLite.Core;
using SpatialLite.Core.API;
using SpatialLite.Core.Geometries;

namespace Tests.SpatialLite.Core.Geometries
{
    public class LineStringTests
    {

        Coordinate[] _coordinatesXY = new Coordinate[] {
                new Coordinate(12,10),
                new Coordinate(22,20),
                new Coordinate(32,30)
        };

        Coordinate[] _coordinatesXYZ = new Coordinate[] {
                new Coordinate(12,10,100),
                new Coordinate(22,20,200),
                new Coordinate(32,30,300)
        };

        Coordinate[] _coordinatesXYZM = new Coordinate[] {
                new Coordinate(12,10,100, 1000),
                new Coordinate(22,20,200, 2000),
                new Coordinate(32,30,300, 3000)
        };


        private void CheckCoordinates(LineString target, Coordinate[] expectedPoints)
        {
            Assert.Equal(expectedPoints.Length, target.Coordinates.Count);

            for (int i = 0; i < expectedPoints.Length; i++)
            {
                Assert.Equal(expectedPoints[i], target.Coordinates[i]);
            }
        }

        [Fact]
        public void Constructor__CreatesEmptyLineString()
        {
            LineString target = new LineString();

            Assert.Equal(0, target.Coordinates.Count);
        }

        [Fact]
        public void Constructor_IEnumerable_CreatesLineStringFromCoordinates()
        {
            LineString target = new LineString(_coordinatesXYZ);

            CheckCoordinates(target, _coordinatesXYZ);
        }

        [Fact]
        public void Is3D_ReturnsFalseForEmptyLineString()
        {
            LineString target = new LineString();

            Assert.False(target.Is3D);
        }

        [Fact]
        public void Is3D_ReturnsFalseForAll2DCoords()
        {
            LineString target = new LineString(_coordinatesXY);

            Assert.False(target.Is3D);
        }

        [Fact]
        public void Is3D_ReturnsTrueForAll3DCoords()
        {
            LineString target = new LineString(_coordinatesXYZ);

            Assert.True(target.Is3D);
        }

        [Fact]
        public void IsMeasured_ReturnsFalseForEmptyLineString()
        {
            LineString target = new LineString();

            Assert.False(target.IsMeasured);
        }

        [Fact]
        public void IsMeasured_ReturnsFalseForNonMeasuredCoords()
        {
            LineString target = new LineString(_coordinatesXYZ);

            Assert.False(target.IsMeasured);
        }

        [Fact]
        public void IsMeasured_ReturnsTrueForMeasuredCoords()
        {
            LineString target = new LineString(_coordinatesXYZM);

            Assert.True(target.IsMeasured);
        }

        [Fact]
        public void Start_ReturnsEmptyCoordinateForEmptyLineString()
        {
            LineString target = new LineString();

            Assert.Equal(Coordinate.Empty, target.Start);
        }

        [Fact]
        public void Start_ReturnsFirstCoordinate()
        {
            LineString target = new LineString(_coordinatesXYZ);

            Assert.Equal(_coordinatesXYZ.First(), target.Start);
        }

        [Fact]
        public void End_ReturnsEmptyCoordinateForEmptyLineString()
        {
            LineString target = new LineString();

            Assert.Equal(Coordinate.Empty, target.End);
        }

        [Fact]
        public void End_ReturnsLastCoordinate()
        {
            LineString target = new LineString(_coordinatesXYZ);

            Assert.Equal(_coordinatesXYZ.Last(), target.End);
        }

        [Fact]
        public void IsClosed_ReturnsTrueForClosedLineString()
        {
            LineString target = new LineString(_coordinatesXYZ);
            target.Coordinates.Add(target.Coordinates[0]);

            Assert.True(target.IsClosed);
        }

        [Fact]
        public void IsClosed_ReturnsFalseForOpenLineString()
        {
            LineString target = new LineString(_coordinatesXYZ);

            Assert.False(target.IsClosed);
        }

        [Fact]
        public void IsClosed_ReturnsFalseForEmptyLineString()
        {
            LineString target = new LineString();

            Assert.False(target.IsClosed);
        }

        [Fact]
        public void GetEnvelope_ReturnsEmptyEnvelopeForEmptyLineString()
        {
            LineString target = new LineString();
            Envelope envelope = target.GetEnvelope();

            Assert.Equal(Envelope.Empty, envelope);
        }

        [Fact]
        public void GetEnvelope_ReturnsEnvelopeOfLineString()
        {
            LineString target = new LineString(_coordinatesXYZ);
            Envelope expected = new Envelope(_coordinatesXYZ);

            Assert.Equal(expected, target.GetEnvelope());
        }
    }
}
