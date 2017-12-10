using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xunit;
using Xunit.Extensions;
using Moq;

using SpatialLite.Core.API;
using SpatialLite.Core.Algorithms;

namespace Tests.SpatialLite.Core.Algorithms {
	public class Euclidean2DCalculatorTests {
		
		#region ComputeDistance(Coordinate, Coordinate) tests

		public static IEnumerable<object[]> CoordinatesSameXYOrdinates {
			get {
				yield return new object[] { new Coordinate(10.1, 100.2), new Coordinate(10.1, 100.2) };
				yield return new object[] { new Coordinate(10.1, 100.2), new Coordinate(10.1, 100.2, 1) };
				yield return new object[] { new Coordinate(10.1, 100.2), new Coordinate(10.1, 100.2, 1, 1) };
			}
		}

		[Theory]
		[MemberData("CoordinatesSameXYOrdinates")]
		public void ComputeDistance_CoordinateCoordinate_ReturnsZeroForPointsWithSameXYCoordinates(Coordinate c1, Coordinate c2) {
			Euclidean2DCalculator target = new Euclidean2DCalculator();

			double distance = target.CalculateDistance(c1, c2);

			Assert.Equal(0, distance);
		}

		public static IEnumerable<object[]> CoordinatesDistanceTestData {
			get {
				yield return new object[] { new Coordinate(-1, 0), new Coordinate(1, 0), 2 };
				yield return new object[] { new Coordinate(0, -1), new Coordinate(0, 1), 2 };
				yield return new object[] { new Coordinate(2, 1), new Coordinate(-1, -3), 5 };
			}
		}

		[Theory]
		[MemberData("CoordinatesDistanceTestData")]
		public void ComputeDistance_CoordinateCoordinate_ReturnsDistanceBetweenCoordinates(Coordinate c1, Coordinate c2, double expectedDistance) {
			Euclidean2DCalculator target = new Euclidean2DCalculator();

			double distance = target.CalculateDistance(c1, c2);

			Assert.Equal(expectedDistance, distance);
		}

		[Fact]
		public void ComputeDistance_CoordinateCoordinate_IgnoresZOrdinate() {
			Euclidean2DCalculator target = new Euclidean2DCalculator();

			double distance = target.CalculateDistance(new Coordinate(-1, 0, 100), new Coordinate(1, 0, -100));

			Assert.Equal(2, distance);
		}

		#endregion

		#region ComputeDistance(Coordinate, Coordinate, Coordinate, Mode) tests

		[Fact]
		public void ComputeDistance_CoordinateLineLineSegmentMode_ReturnsDistanceToEndPointsIfPointsProjectionLiesOutsideSegment() {
			Coordinate A = new Coordinate(1, 1);
			Coordinate B = new Coordinate(-1, -1);
			Coordinate c1 = new Coordinate(1,2);
			Coordinate c2 = new Coordinate(-1,-2);

			Euclidean2DCalculator target = new Euclidean2DCalculator();

			double distanceC1 = target.CalculateDistance(c1, A, B, LineMode.LineSegment);
			Assert.Equal(1, distanceC1);

			double distanceC2 = target.CalculateDistance(c2, A, B, LineMode.LineSegment);
			Assert.Equal(1, distanceC2);
		}

		public static IEnumerable<object[]> LineSegmentDistanceTestData {
			get {
				yield return new object[] { new Coordinate(0.5, 1),  new Coordinate(-1, 0), new Coordinate(1, 0), 1 };
				yield return new object[] { new Coordinate(1, 0.5), new Coordinate(0, -1), new Coordinate(0, 1), 1 };
				yield return new object[] { new Coordinate(-1, 1), new Coordinate(-5, -5), new Coordinate(5,5), Math.Sqrt(2) };
			}
		}

		[Theory]
		[MemberData("LineSegmentDistanceTestData")]
		public void ComputeDistance_CoordinateLineLineSegmentMode_ComputesPerpendicularDistanceToLineIfPointsProjectionLiesInside(Coordinate c, Coordinate A, Coordinate B, double expectedDistance) {
			Euclidean2DCalculator target = new Euclidean2DCalculator();

			double distance = target.CalculateDistance(c, A, B, LineMode.LineSegment);
			Assert.Equal(expectedDistance, distance);
		}

		public static IEnumerable<object[]> LineSegmentDistanceZeroDistanceTestData {
			get {
				yield return new object[] { new Coordinate(0, 0), new Coordinate(-1, 0), new Coordinate(1, 0) };
				yield return new object[] { new Coordinate(0, -1), new Coordinate(0, -1), new Coordinate(0, 1) };
				yield return new object[] { new Coordinate(0, 0), new Coordinate(0, -1), new Coordinate(0, 1) };
			}
		}

		[Theory]
		[MemberData("LineSegmentDistanceZeroDistanceTestData")]
		public void ComputeDistance_CoordinateLineLineSegmentMode_ReturnsZeroIfPointLiesOnLineSegment(Coordinate c, Coordinate A, Coordinate B) {
			Euclidean2DCalculator target = new Euclidean2DCalculator();

			double distance = target.CalculateDistance(c, A, B, LineMode.LineSegment);
			Assert.Equal(0, distance);
		}

		[Fact]
		public void ComputeDistance_CoordinateLineLineSegmentMode_ReturnsPointDistanceIfABAreEquals() {
			Coordinate A = new Coordinate(1, 1);
			Coordinate B = new Coordinate(1, 1);
			Coordinate c = new Coordinate(1, 2);
			
			Euclidean2DCalculator target = new Euclidean2DCalculator();

			double distance = target.CalculateDistance(c, A, B, LineMode.LineSegment);
			Assert.Equal(1, distance);
		}

		[Fact]
		public void ComputeDistance_CoordinateLineLineMode_ReturnsPointDistanceIfABAreEquals() {
			Coordinate A = new Coordinate(1, 1);
			Coordinate B = new Coordinate(1, 1);
			Coordinate c = new Coordinate(1, 2);

			Euclidean2DCalculator target = new Euclidean2DCalculator();

			double distance = target.CalculateDistance(c, A, B, LineMode.Line);
			Assert.Equal(1, distance);
		}

		public static IEnumerable<object[]> LineDistanceZeroDistanceTestData {
			get {
				yield return new object[] { new Coordinate(-2, -2), new Coordinate(-1, -1), new Coordinate(1, 1) };
				yield return new object[] { new Coordinate(2, 2), new Coordinate(-1, -1), new Coordinate(1, 1) };
				yield return new object[] { new Coordinate(0, 0), new Coordinate(-1, -1), new Coordinate(1, 1) };
			}
		}

		[Theory]
		[MemberData("LineDistanceZeroDistanceTestData")]
		public void ComputeDistance_CoordinateLineLineMode_ReturnsZeroIfPointLiesOnLine(Coordinate c, Coordinate A, Coordinate B) {
			Euclidean2DCalculator target = new Euclidean2DCalculator();

			double distance = target.CalculateDistance(c, A, B, LineMode.Line);
			Assert.Equal(0, distance);
		}

		public static IEnumerable<object[]> LineDistanceTestData {
			get {
				yield return new object[] { new Coordinate(-3, -1), new Coordinate(-1, -1), new Coordinate(1, 1), Math.Sqrt(2) };
				yield return new object[] { new Coordinate(-1, 1), new Coordinate(-1, -1), new Coordinate(1, 1), Math.Sqrt(2) };
				yield return new object[] { new Coordinate(3, 1), new Coordinate(-1, -1), new Coordinate(1, 1), Math.Sqrt(2) };
			}
		}

		[Theory]
		[MemberData("LineDistanceTestData")]
		public void ComputeDistance_CoordinateLineLineMode_ComputesPerpendicularDistanceToLine(Coordinate c, Coordinate A, Coordinate B, double expectedDistance) {
			Euclidean2DCalculator target = new Euclidean2DCalculator();

			double distance = target.CalculateDistance(c, A, B, LineMode.Line);
			Assert.Equal(expectedDistance, distance);
		}

		#endregion

		#region ComputeArea(ICoordinateList) tests

		[Fact]
		public void ComputeArea_ThrowsExceptionIfNumberOfVerticesIsLesserThen3() {
			Mock<ICoordinateList> listM = new Mock<ICoordinateList>();
			listM.SetupGet(l => l.Count).Returns(2);
			Euclidean2DCalculator target = new Euclidean2DCalculator();

			Assert.Throws<ArgumentException>(() => target.CalculateArea(listM.Object));
		}

		[Fact]
		public void ComputeArea_ReturnsAreaOfConvexPolygon() {
			Coordinate[] coordinates = new Coordinate[] { new Coordinate(1, 1), new Coordinate(2, 0.5), new Coordinate(3, 1), new Coordinate(3, 2), new Coordinate(1, 2) };
			double expectedArea = 2.5;

			Mock<ICoordinateList> listM = new Mock<ICoordinateList>();
			listM.SetupGet(list => list.Count).Returns(5);
			listM.Setup(list => list[0]).Returns(coordinates[0]);
			listM.Setup(list => list[1]).Returns(coordinates[1]);
			listM.Setup(list => list[2]).Returns(coordinates[2]);
			listM.Setup(list => list[3]).Returns(coordinates[3]);
			listM.Setup(list => list[4]).Returns(coordinates[4]);
			Euclidean2DCalculator target = new Euclidean2DCalculator();

			double area = target.CalculateArea(listM.Object);
			Assert.Equal(expectedArea, area);
		}

		[Fact]
		public void ComputeArea_ReturnsAreaOfConcavePolygon() {
			Coordinate[] coordinates = new Coordinate[] { new Coordinate(1, 1), new Coordinate(2, 1.5), new Coordinate(3, 1), new Coordinate(3, 2), new Coordinate(1, 2) };
			double expectedArea = 1.5;

			Mock<ICoordinateList> listM = new Mock<ICoordinateList>();
			listM.SetupGet(list => list.Count).Returns(5);
			listM.Setup(list => list[0]).Returns(coordinates[0]);
			listM.Setup(list => list[1]).Returns(coordinates[1]);
			listM.Setup(list => list[2]).Returns(coordinates[2]);
			listM.Setup(list => list[3]).Returns(coordinates[3]);
			listM.Setup(list => list[4]).Returns(coordinates[4]);
			Euclidean2DCalculator target = new Euclidean2DCalculator();

			double area = target.CalculateArea(listM.Object);
			Assert.Equal(expectedArea, area);
		}

		[Fact]
		public void ComputeArea_ReturnsCorrectAreaIfLastCoordinateIsSameAsFirst() {
			Coordinate[] coordinates = new Coordinate[] { new Coordinate(1, 1), new Coordinate(2, 0.5), new Coordinate(3, 1), new Coordinate(3, 2), new Coordinate(1, 2), new Coordinate(1, 1) };
			double expectedArea = 2.5;

			Mock<ICoordinateList> listM = new Mock<ICoordinateList>();
			listM.SetupGet(list => list.Count).Returns(6);
			listM.Setup(list => list[0]).Returns(coordinates[0]);
			listM.Setup(list => list[1]).Returns(coordinates[1]);
			listM.Setup(list => list[2]).Returns(coordinates[2]);
			listM.Setup(list => list[3]).Returns(coordinates[3]);
			listM.Setup(list => list[4]).Returns(coordinates[4]);
			listM.Setup(list => list[5]).Returns(coordinates[5]);
			Euclidean2DCalculator target = new Euclidean2DCalculator();

			double area = target.CalculateArea(listM.Object);
			Assert.Equal(expectedArea, area);
		}
		#endregion
	}
}