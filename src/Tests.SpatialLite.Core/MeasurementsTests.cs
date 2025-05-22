using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xunit;
using Moq;

using SpatialLite.Core;
using SpatialLite.Core.API;
using SpatialLite.Core.Geometries;
using SpatialLite.Core.Algorithms;

namespace Tests.SpatialLite.Core
{
    public class MeasurementsTests
    {

        [Fact]
        public void Euclidean2D_GetsInstanceOfMeasuremntsWithTheEuclidean2DCalculator()
        {
            Assert.IsType<Euclidean2DCalculator>(Measurements.Euclidean2D.DimensionsCalculator);
        }

        [Fact]
        public void Sphere2D_GetsInstanceOfMeasuremntsWithTheSphere2DCalculator()
        {
            Assert.IsType<Sphere2DCalculator>(Measurements.Sphere2D.DimensionsCalculator);
        }

        [Fact]
        public void Constructor_SetsCalculatorObject()
        {
            Mock<IDimensionsCalculator> calculatorM = new Mock<IDimensionsCalculator>();

            Measurements target = new Measurements(calculatorM.Object);

            Assert.Same(calculatorM.Object, target.DimensionsCalculator);
        }

        [Fact]
        public void ComputeDistance_CoordinateCoordinate_CallsIDistanceCalculatorWithCorrectParameters()
        {
            Coordinate c1 = new Coordinate(10.1, 20.1);
            Coordinate c2 = new Coordinate(10.2, 20.2);
            Mock<IDimensionsCalculator> calculatorM = new Mock<IDimensionsCalculator>();

            Measurements target = new Measurements(calculatorM.Object);
            double distance = target.ComputeDistance(c1, c2);

            calculatorM.Verify(calc => calc.CalculateDistance(c1, c2), Times.Once());
        }

        [Fact]
        public void ComputeDistance_PointPoint_ReturnsNaNIfPoint1IsEmpty()
        {
            Point p1 = new Point(Coordinate.Empty);
            Point p2 = new Point(new Coordinate(10.2, 20.2));
            Mock<IDimensionsCalculator> calculatorM = new Mock<IDimensionsCalculator>();

            Measurements target = new Measurements(calculatorM.Object);
            double distance = target.ComputeDistance(p1, p2);

            Assert.True(double.IsNaN(distance));
        }

        [Fact]
        public void ComputeDistance_PointPoint_ReturnsNaNIfPoint2IsEmpty()
        {
            Point p1 = new Point(new Coordinate(10.1, 20.1));
            Point p2 = new Point(Coordinate.Empty);
            Mock<IDimensionsCalculator> calculatorM = new Mock<IDimensionsCalculator>();

            Measurements target = new Measurements(calculatorM.Object);
            double distance = target.ComputeDistance(p1, p2);

            Assert.True(double.IsNaN(distance));
        }

        [Fact]
        public void ComputeDistance_PointPoint_CallsIDistanceCalculatorWithCorrectParameters()
        {
            Point p1 = new Point(new Coordinate(10.1, 20.1));
            Point p2 = new Point(new Coordinate(10.2, 20.2));
            Mock<IDimensionsCalculator> calculatorM = new Mock<IDimensionsCalculator>();

            Measurements target = new Measurements(calculatorM.Object);
            double distance = target.ComputeDistance(p1, p2);

            calculatorM.Verify(calc => calc.CalculateDistance(p1.Position, p2.Position), Times.Once());
        }

        [Fact]
        public void ComputeDistance_PointLineString_ReturnsNaNIfLineStringIsEmpty()
        {
            Point point = new Point(new Coordinate(10.1, 20.1));
            LineString linestring = new LineString();

            Mock<IDimensionsCalculator> calculatorM = new Mock<IDimensionsCalculator>();

            Measurements target = new Measurements(calculatorM.Object);
            double distance = target.ComputeDistance(point, linestring);

            Assert.True(double.IsNaN(distance));
        }

        [Fact]
        public void ComputeDistance_PointLineString_ReturnsNaNIfPointIsEmpty()
        {
            Point point = new Point(Coordinate.Empty);
            LineString linestring = new LineString(new Coordinate[] { new Coordinate(10.1, 20.1), new Coordinate(10.2, 20.2), new Coordinate(10.3, 20.3) });

            Mock<IDimensionsCalculator> calculatorM = new Mock<IDimensionsCalculator>();

            Measurements target = new Measurements(calculatorM.Object);
            double distance = target.ComputeDistance(point, linestring);

            Assert.True(double.IsNaN(distance));
        }

        [Fact]
        public void ComputeDistance_PointMultiLineString_ReturnsNaNIfMultiLineStringIsEmpty()
        {
            Point point = new Point(new Coordinate(10.1, 20.1));
            MultiLineString multilinestring = new MultiLineString();

            Mock<IDimensionsCalculator> calculatorM = new Mock<IDimensionsCalculator>();

            Measurements target = new Measurements(calculatorM.Object);
            double distance = target.ComputeDistance(point, multilinestring);

            Assert.True(double.IsNaN(distance));
        }

        [Fact]
        public void ComputeDistance_PointMultiLineString_ReturnsNaNIfPointIsEmpty()
        {
            Point point = new Point(Coordinate.Empty);
            LineString linestring = new LineString(new Coordinate[] { new Coordinate(10.1, 20.1), new Coordinate(10.2, 20.2), new Coordinate(10.3, 20.3) });
            MultiLineString multilinestring = new MultiLineString(new LineString[] { linestring, linestring });

            Mock<IDimensionsCalculator> calculatorM = new Mock<IDimensionsCalculator>();

            Measurements target = new Measurements(calculatorM.Object);
            double distance = target.ComputeDistance(point, multilinestring);

            Assert.True(double.IsNaN(distance));
        }

        [Fact]
        public void ComputeLength_LineString_RetursZeroForLineStringWithoutPoints()
        {
            LineString linestring = new LineString();

            Mock<IDimensionsCalculator> calculatorM = new Mock<IDimensionsCalculator>();
            Measurements target = new Measurements(calculatorM.Object);

            double length = target.ComputeLength(linestring);

            Assert.Equal(0, length);
        }

        [Fact]
        public void ComputeLength_LineString_RetursZeroForLineStringWithoutOne()
        {
            LineString linestring = new LineString(new Coordinate[] { new Coordinate(10.1, 20.1) });

            Mock<IDimensionsCalculator> calculatorM = new Mock<IDimensionsCalculator>();
            Measurements target = new Measurements(calculatorM.Object);

            double length = target.ComputeLength(linestring);

            Assert.Equal(0, length);
        }

        [Fact]
        public void ComputeLength_LineString_RetursSumOfSegmentsLengths()
        {
            Random generator = new Random();
            double segment1Length = generator.Next(100);
            double segment2Length = generator.Next(100);
            double sum = segment1Length + segment2Length;

            LineString linestring = new LineString(new Coordinate[] { new Coordinate(10.1, 20.1), new Coordinate(10.2, 20.2), new Coordinate(10.3, 20.3) });
            Mock<IDimensionsCalculator> calculatorM = new Mock<IDimensionsCalculator>();
            calculatorM.Setup(calc => calc.CalculateDistance(linestring.Coordinates[0], linestring.Coordinates[1])).Returns(segment1Length);
            calculatorM.Setup(calc => calc.CalculateDistance(linestring.Coordinates[1], linestring.Coordinates[2])).Returns(segment2Length);

            Measurements target = new Measurements(calculatorM.Object);
            double length = target.ComputeLength(linestring);

            Assert.Equal(sum, length);
        }

        [Fact]
        public void ComputeLength_MultiLineString_RetursZeroForMultiLineStringWithoutMembers()
        {
            MultiLineString multilinestring = new MultiLineString();

            Mock<IDimensionsCalculator> calculatorM = new Mock<IDimensionsCalculator>();
            Measurements target = new Measurements(calculatorM.Object);

            double length = target.ComputeLength(multilinestring);

            Assert.Equal(0, length);
        }

        [Fact]
        public void ComputeLength_MultiLineString_RetursSumOfLineStringsLengths()
        {
            Random generator = new Random();
            double segment1Length = generator.Next(100);
            double segment2Length = generator.Next(100);
            double sum = 2 * (segment1Length + segment2Length);

            LineString linestring = new LineString(new Coordinate[] { new Coordinate(10.1, 20.1), new Coordinate(10.2, 20.2), new Coordinate(10.3, 20.3) });
            MultiLineString multilinestring = new MultiLineString(new LineString[] { linestring, linestring });

            Mock<IDimensionsCalculator> calculatorM = new Mock<IDimensionsCalculator>();
            calculatorM.Setup(calc => calc.CalculateDistance(linestring.Coordinates[0], linestring.Coordinates[1])).Returns(segment1Length);
            calculatorM.Setup(calc => calc.CalculateDistance(linestring.Coordinates[1], linestring.Coordinates[2])).Returns(segment2Length);

            Measurements target = new Measurements(calculatorM.Object);
            double length = target.ComputeLength(multilinestring);

            Assert.Equal(sum, length);
        }

        [Fact]
        public void ComputeArea_IPolygon_ReturnsAreaOfSimplePolygonCalculatedByIDimensionsCalculator()
        {
            Polygon polygon = new Polygon(new CoordinateList());
            Random generator = new Random();
            double expectedArea = generator.Next(100);

            Mock<IDimensionsCalculator> calculatorM = new Mock<IDimensionsCalculator>();
            calculatorM.Setup(calc => calc.CalculateArea(polygon.ExteriorRing)).Returns(expectedArea);
            Measurements target = new Measurements(calculatorM.Object);

            double area = target.ComputeArea(polygon);

            Assert.Equal(expectedArea, area);
        }

        [Fact]
        public void ComputeArea_IPolygon_ReturnsAreaOfPolygonWithoutHolesCalculatedByIDimensionsCalculator()
        {
            // Create polygon with interior ring
            Polygon polygon = new Polygon(new CoordinateList());
            polygon.InteriorRings.Add(new CoordinateList());

            // Fixed test values
            double exteriorArea = 50;
            double interiorArea = 10;
            double expectedArea = exteriorArea - interiorArea;

            // Setup mock 
            Mock<IDimensionsCalculator> calculatorM = new Mock<IDimensionsCalculator>();
            calculatorM.Setup(calc => calc.CalculateArea(It.IsAny<ICoordinateList>()))
                .Returns<ICoordinateList>(coords =>
                {
                    if (coords == polygon.ExteriorRing) return exteriorArea;
                    if (coords == polygon.InteriorRings[0]) return interiorArea;
                    return 0;
                });

            // Create the object being tested
            Measurements target = new Measurements(calculatorM.Object);

            // Execute the method being tested
            double area = target.ComputeArea(polygon);

            // Verify the result
            Assert.Equal(expectedArea, area);
        }

        [Fact]
        public void ComputeArea_IMultiPolygon_ReturnsSumOfPolygonAreas()
        {
            Random generator = new Random();

            Polygon polygon = new Polygon(new CoordinateList());
            double polygonArea = generator.Next(100);
            MultiPolygon multipolygon = new MultiPolygon(new Polygon[] { polygon, polygon });

            Mock<IDimensionsCalculator> calculatorM = new Mock<IDimensionsCalculator>();
            calculatorM.Setup(calc => calc.CalculateArea(polygon.ExteriorRing)).Returns(() => polygonArea);

            Measurements target = new Measurements(calculatorM.Object);

            double area = target.ComputeArea(multipolygon);

            Assert.Equal(2 * polygonArea, area);
        }
    }
}
