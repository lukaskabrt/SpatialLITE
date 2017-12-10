﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SpatialLite.Core.API;
using SpatialLite.Core.Algorithms;

using Xunit;
using Moq;

namespace Tests.SpatialLite.Core.Algorithms {
	public class Sphere2DCalculatorTests {

		#region CalculateDistance(Coordinate, Coordinate) Tests

		[Fact]
		public void CalculateDistance_CoordinateCoordinate_Returns0ForSameCoordinate() {
			Coordinate c = new Coordinate(10.1, 100.2);
			Sphere2DCalculator target = new Sphere2DCalculator();

			double distance = target.CalculateDistance(c, c);

			Assert.Equal(0, distance);
		}

		//TODO more test cases
		[Fact]
		public void CalculateDistance_CoordinateCoordinate_ReturnsDistanceOf2Points() {
			Coordinate c1 = new Coordinate(0, 0);
			Coordinate c2 = new Coordinate(0, 90);
			Sphere2DCalculator target = new Sphere2DCalculator();

			double distance = target.CalculateDistance(c1, c2);
			double expectedDistance = Math.PI * 2 * Sphere2DCalculator.EarthRadius / 4;

			Assert.InRange<double>(distance, expectedDistance * 0.995, expectedDistance * 1.005);
		}

		[Fact]
		public void CalculateDsitance_CoordinateCoordinate_CalculateDistancesAcross0DegBoundary() {
			Coordinate c1 = new Coordinate(-45, 0);
			Coordinate c2 = new Coordinate(45, 0);
			Sphere2DCalculator target = new Sphere2DCalculator();

			double distance = target.CalculateDistance(c1, c2);
			double expectedDistance = Math.PI * 2 * Sphere2DCalculator.EarthRadius / 4;

			Assert.InRange<double>(distance, expectedDistance * 0.995, expectedDistance * 1.005);
		}

		[Fact]
		public void CalculateDsitance_CoordinateCoordinate_CalculateDistancesAcross180DegBoundary() {
			Coordinate c1 = new Coordinate(-135, 0);
			Coordinate c2 = new Coordinate(135, 0);
			Sphere2DCalculator target = new Sphere2DCalculator();

			double distance = target.CalculateDistance(c1, c2);
			double expectedDistance = Math.PI * 2 * Sphere2DCalculator.EarthRadius / 4;

			Assert.InRange<double>(distance, expectedDistance * 0.995, expectedDistance * 1.005);
		}

		#endregion

		#region CalculateDistance(Coordinate, Coordinate, Coordinate, LineMode) Tests

		[Fact]
		public void CalculateDistance_LineCoordinate_ReturnsDistanceToInfiniteLine() {
			Coordinate a = new Coordinate(0, 0);
			Coordinate b = new Coordinate(90, 0);
			Coordinate c = new Coordinate(120, 45);

			Sphere2DCalculator target = new Sphere2DCalculator();

			double distance = target.CalculateDistance(c, a, b, LineMode.Line);
			double expectedDistance = Math.PI * 2 * Sphere2DCalculator.EarthRadius / 8;

			Assert.InRange<double>(distance, expectedDistance * 0.995, expectedDistance * 1.005);
		}

		[Fact]
		public void CalculateDistance_LineCoordinate_ReturnsDistanceToLineSegment() {
			Coordinate a = new Coordinate(0, 0);
			Coordinate b = new Coordinate(90, 0);
			Coordinate c = new Coordinate(45, 45);

			Sphere2DCalculator target = new Sphere2DCalculator();

			double distance = target.CalculateDistance(c, a, b, LineMode.LineSegment);
			double expectedDistance = Math.PI * 2 * Sphere2DCalculator.EarthRadius / 8;

			Assert.InRange<double>(distance, expectedDistance * 0.995, expectedDistance * 1.005);
		}

		[Fact]
		public void CalculateDistance_LineCoordinate_ReturnsDistanceToEndPointIfPointIsOutsideLineSegment() {
			Coordinate a = new Coordinate(0, 0);
			Coordinate b = new Coordinate(90, 0);
			Coordinate c = new Coordinate(95, 45);

			Sphere2DCalculator target = new Sphere2DCalculator();

			double distance = target.CalculateDistance(c, a, b, LineMode.LineSegment);
			double expectedDistance = target.CalculateDistance(b, c);

			Assert.InRange<double>(distance, expectedDistance * 0.995, expectedDistance * 1.005);
		}

		[Fact]
		public void CalculateDistance_LineCoordinate_ReturnsDistanceToEndPointIfPointIsOutsideLineSegment2() {
			Coordinate a = new Coordinate(0, 0);
			Coordinate b = new Coordinate(90, 0);
			Coordinate c = new Coordinate(5, 45);

			Sphere2DCalculator target = new Sphere2DCalculator();

			double distance = target.CalculateDistance(c, a, b, LineMode.LineSegment);
			double expectedDistance = target.CalculateDistance(a, c);

			Assert.InRange<double>(distance, expectedDistance * 0.995, expectedDistance * 1.005);
		}

		#endregion

		#region CalculateArea(ICoordinateList) Tests
        
		[Fact]
		public void CalculateArea_CalculatesAreaOfPolygonPointFirstPointDoesNotEqualLast() {
			Coordinate[] coordinates = new Coordinate[] { new Coordinate(0, 0), new Coordinate(90, 0), new Coordinate(0, 90) };

			Mock<ICoordinateList> listM = new Mock<ICoordinateList>();
			listM.SetupGet(list => list.Count).Returns(3);
			listM.Setup(list => list[0]).Returns(coordinates[0]);
			listM.Setup(list => list[1]).Returns(coordinates[1]);
			listM.Setup(list => list[2]).Returns(coordinates[2]);

			double expectedArea = 4 * Math.PI * Sphere2DCalculator.EarthRadius * Sphere2DCalculator.EarthRadius / 8;

			Sphere2DCalculator target = new Sphere2DCalculator();

			double area = target.CalculateArea(listM.Object);
			Assert.InRange<double>(area, expectedArea * 0.995, expectedArea * 1.005);
		}

		[Fact]
		public void CalculateArea_CalculatesAreaOfPolygonPointFirstPointEqualsLast() {
			Coordinate[] coordinates = new Coordinate[] { new Coordinate(0, 0), new Coordinate(90, 0), new Coordinate(0, 90) };

			Mock<ICoordinateList> listM = new Mock<ICoordinateList>();
			listM.SetupGet(list => list.Count).Returns(4);
			listM.Setup(list => list[0]).Returns(coordinates[0]);
			listM.Setup(list => list[1]).Returns(coordinates[1]);
			listM.Setup(list => list[2]).Returns(coordinates[2]);
			listM.Setup(list => list[3]).Returns(coordinates[0]);

			double expectedArea = 4 * Math.PI * Sphere2DCalculator.EarthRadius * Sphere2DCalculator.EarthRadius / 8;

			Sphere2DCalculator target = new Sphere2DCalculator();

			double area = target.CalculateArea(listM.Object);
			Assert.InRange<double>(area, expectedArea * 0.995, expectedArea * 1.005);
		}

		#endregion
	}
}
