using SpatialLite.Core.Algorithms;
using SpatialLite.Core.API;
using SpatialLite.Core.Geometries;
using SpatialLite.Core.IO;
using System;
using System.Collections.Generic;
using Xunit;

namespace Tests.SpatialLite.Core.Algorithms;

public class Euclidean2DLocatorTests
{

    [Fact]
    public void IsOnLine_ReturnsTrueIfPointIsOnAorB()
    {
        Coordinate a = new Coordinate(-10, 0);
        Coordinate b = new Coordinate(3, 2);
        Euclidean2DLocator target = new Euclidean2DLocator();

        Assert.True(target.IsOnLine(a, a, b, LineMode.Line));
        Assert.True(target.IsOnLine(b, a, b, LineMode.Line));
    }

    public static IEnumerable<object[]> PointsBetweenAB
    {
        get
        {
            yield return new object[] { new Coordinate(1, 1), new Coordinate(-10, 1), new Coordinate(10, 1), LineMode.Line };
            yield return new object[] { new Coordinate(1, 1), new Coordinate(1, 10), new Coordinate(1, -10), LineMode.Line };
            yield return new object[] { new Coordinate(1, 1), new Coordinate(2, 2), new Coordinate(-1, -1), LineMode.Line };
            yield return new object[] { new Coordinate(1, 1), new Coordinate(-10, 1), new Coordinate(10, 1), LineMode.LineSegment };
            yield return new object[] { new Coordinate(1, 1), new Coordinate(1, 10), new Coordinate(1, -10), LineMode.LineSegment };
            yield return new object[] { new Coordinate(1, 1), new Coordinate(2, 2), new Coordinate(-1, -1), LineMode.LineSegment };
        }
    }

    [Theory]
    [MemberData(nameof(PointsBetweenAB))]
    public void IsOnLine_ReturnsTrueIfPointLiesBetweenAAndB(Coordinate c, Coordinate a, Coordinate b, LineMode mode)
    {
        Euclidean2DLocator target = new Euclidean2DLocator();

        Assert.True(target.IsOnLine(c, a, b, mode));
    }

    public static IEnumerable<object[]> PointsOnAB
    {
        get
        {
            yield return new object[] { new Coordinate(-11, 1), new Coordinate(-10, 1), new Coordinate(10, 1) };
            yield return new object[] { new Coordinate(11, 1), new Coordinate(-10, 1), new Coordinate(10, 1) };
        }
    }

    [Theory]
    [MemberData(nameof(PointsOnAB))]
    public void IsOnLine_ReturnsTrueIfPointLiesOnABAndModeIsLine(Coordinate c, Coordinate a, Coordinate b)
    {
        Euclidean2DLocator target = new Euclidean2DLocator();

        Assert.True(target.IsOnLine(c, a, b, LineMode.Line));
    }

    [Theory]
    [MemberData(nameof(PointsOnAB))]
    public void IsOnLine_ReturnsFalseIfPointLiesOnABAndModeIsLineSegment(Coordinate c, Coordinate a, Coordinate b)
    {
        Euclidean2DLocator target = new Euclidean2DLocator();

        Assert.False(target.IsOnLine(c, a, b, LineMode.LineSegment));
    }

    public static IEnumerable<object[]> PointsNotOnAB
    {
        get
        {
            yield return new object[] { new Coordinate(1, 2), new Coordinate(-10, 1), new Coordinate(10, 1), LineMode.Line };
            yield return new object[] { new Coordinate(2, 1), new Coordinate(1, 10), new Coordinate(1, -10), LineMode.Line };
            yield return new object[] { new Coordinate(0, 1), new Coordinate(2, 2), new Coordinate(-1, -1), LineMode.Line };
            yield return new object[] { new Coordinate(1, 2), new Coordinate(-10, 1), new Coordinate(10, 1), LineMode.LineSegment };
            yield return new object[] { new Coordinate(2, 1), new Coordinate(1, 10), new Coordinate(1, -10), LineMode.LineSegment };
            yield return new object[] { new Coordinate(0, 1), new Coordinate(2, 2), new Coordinate(-1, -1), LineMode.LineSegment };
        }
    }

    [Theory]
    [MemberData(nameof(PointsNotOnAB))]
    public void IsOnLine_ReturnsFalseIfPointDoEsNotLieOnAB(Coordinate c, Coordinate a, Coordinate b, LineMode mode)
    {
        Euclidean2DLocator target = new Euclidean2DLocator();
        Assert.False(target.IsOnLine(c, a, b, mode));
    }

    [Fact]
    public void IsInRing_ThrowsArgumentExceptionIfRingHasLessThen3Points()
    {
        Coordinate c = new Coordinate(0, 0);
        CoordinateList ring = new CoordinateList(new Coordinate[] { new Coordinate(0, 1), new Coordinate(1, 0) });
        Euclidean2DLocator target = new Euclidean2DLocator();

        Assert.Throws<ArgumentException>(() => target.IsInRing(c, ring));
    }

    [Fact]
    public void IsInRing_ThrowsArgumentExceptionIfCoordinateListDoesNotRepresentRing()
    {
        Coordinate c = new Coordinate(0, 0);
        CoordinateList ring = new CoordinateList(new Coordinate[] { new Coordinate(0, 1), new Coordinate(1, 0), new Coordinate(1, 2) });
        Euclidean2DLocator target = new Euclidean2DLocator();

        Assert.Throws<ArgumentException>(() => target.IsInRing(c, ring));
    }

    [Fact]
    public void IsInRing_ReturnsTrueIfPointIsInSimpleRing()
    {
        string wktRing = "linestring (-1 -1, 1 -1, 1 1, -1 1, -1 -1)";
        LineString ring = WktReader.Parse<LineString>(wktRing);

        Coordinate c = new Coordinate(0.5, 0.5);
        Euclidean2DLocator target = new Euclidean2DLocator();

        Assert.True(target.IsInRing(c, ring.Coordinates));
    }

    [Fact]
    public void IsInRing_ReturnsTrueIfPointIsInRingAndCYCoordinateIsSameAsVertexYCoordinate()
    {
        string wktRing = "linestring (-1 -1, 1 -1, 1 0.5, 1 1, -1 1, -1 -1)";
        LineString ring = WktReader.Parse<LineString>(wktRing);

        Coordinate c = new Coordinate(0.5, 0.5);
        Euclidean2DLocator target = new Euclidean2DLocator();

        Assert.True(target.IsInRing(c, ring.Coordinates));
    }

    [Fact]
    public void IsInRing_ReturnsTrueIfPointIsInConcaveRing()
    {
        string wktRing = "linestring (0 0, 2 0, 1 1, 1 2, 0 0)";
        LineString ring = WktReader.Parse<LineString>(wktRing);

        Coordinate c = new Coordinate(1, 0.5);
        Euclidean2DLocator target = new Euclidean2DLocator();

        Assert.True(target.IsInRing(c, ring.Coordinates));
    }

    [Fact]
    public void IsInRing_ReturnsFalseIfPointIsOutsideSimpleRing()
    {
        string wktRing = "linestring (-1 -1, 1 -1, 1 1, -1 1, -1 -1)";
        LineString ring = WktReader.Parse<LineString>(wktRing);

        Coordinate c = new Coordinate(2, 2);
        Euclidean2DLocator target = new Euclidean2DLocator();

        Assert.False(target.IsInRing(c, ring.Coordinates));
    }

    [Fact]
    public void IsInRing_ReturnsFalseIfPointIsOutsideRing()
    {
        string wktRing = "linestring (0 0, 2 0, 1 1, 1 2, 0 0)";
        LineString ring = WktReader.Parse<LineString>(wktRing);

        Coordinate c = new Coordinate(3, 0);
        Euclidean2DLocator target = new Euclidean2DLocator();

        Assert.False(target.IsInRing(c, ring.Coordinates));
    }

    [Fact]
    public void IsInRing_ReturnsFalseIfPointIsInRingAndCYCoordinateIsSameAsVertexYCoordinate()
    {
        string wktRing = "linestring (-1 -1, 1 -1, 1 0.5, 1 1, -1 1, -1 -1)";
        LineString ring = WktReader.Parse<LineString>(wktRing);

        Coordinate c = new Coordinate(-2, 0.5);
        Euclidean2DLocator target = new Euclidean2DLocator();

        Assert.False(target.IsInRing(c, ring.Coordinates));
    }



    [Fact]
    public void Intersects_ReturnsFalseForParallelLines()
    {
        Coordinate A1 = new Coordinate(1.0, 1.0);
        Coordinate B1 = new Coordinate(2.0, 2.0);

        Coordinate A2 = new Coordinate(2.0, 1.0);
        Coordinate B2 = new Coordinate(3.0, 2.0);

        Euclidean2DLocator target = new Euclidean2DLocator();

        Assert.False(target.Intersects(A1, B1, LineMode.Line, A2, B2, LineMode.Line));
    }

    [Fact]
    public void Intersects_ReturnsTrueForLinesThatIntersectsOutsideLineSegments()
    {
        Coordinate A1 = new Coordinate(1.0, 1.0);
        Coordinate B1 = new Coordinate(2.0, 1.0);

        Coordinate A2 = new Coordinate(2.0, 0.0);
        Coordinate B2 = new Coordinate(3.0, 1.0);

        Euclidean2DLocator target = new Euclidean2DLocator();

        Assert.True(target.Intersects(A1, B1, LineMode.Line, A2, B2, LineMode.Line));
    }

    [Fact]
    public void Intersects_ReturnsFalseForLineAndLineSegmentThatIntersectsOutsideLineSegment1()
    {
        Coordinate A1 = new Coordinate(1.0, 1.0);
        Coordinate B1 = new Coordinate(2.0, 1.0);

        Coordinate A2 = new Coordinate(2.0, 0.0);
        Coordinate B2 = new Coordinate(3.0, 1.0);

        Euclidean2DLocator target = new Euclidean2DLocator();

        Assert.False(target.Intersects(A1, B1, LineMode.LineSegment, A2, B2, LineMode.Line));
    }

    [Fact]
    public void Intersects_ReturnsFalseForLineAndLineSegmentThatIntersectsOutsideLineSegment2()
    {
        Coordinate A1 = new Coordinate(1.0, 1.0);
        Coordinate B1 = new Coordinate(2.0, 1.0);

        Coordinate A2 = new Coordinate(2.0, 0.0);
        Coordinate B2 = new Coordinate(3.0, 0.5);

        Euclidean2DLocator target = new Euclidean2DLocator();

        Assert.False(target.Intersects(A1, B1, LineMode.Line, A2, B2, LineMode.LineSegment));
    }

    [Fact]
    public void Intersects_ReturnsFalseForNonIntersectingLineSegments()
    {
        Coordinate A1 = new Coordinate(1.0, 1.0);
        Coordinate B1 = new Coordinate(2.0, 1.0);

        Coordinate A2 = new Coordinate(2.0, 0.0);
        Coordinate B2 = new Coordinate(3.0, 1.0);

        Euclidean2DLocator target = new Euclidean2DLocator();

        Assert.False(target.Intersects(A1, B1, LineMode.LineSegment, A2, B2, LineMode.LineSegment));
    }

    [Fact]
    public void Intersects_ReturnsTrueForIntersectingLineSegments()
    {
        Coordinate A1 = new Coordinate(1.0, 1.0);
        Coordinate B1 = new Coordinate(3.0, 1.0);

        Coordinate A2 = new Coordinate(1.0, 0.0);
        Coordinate B2 = new Coordinate(3.0, 2.0);

        Euclidean2DLocator target = new Euclidean2DLocator();

        Assert.True(target.Intersects(A1, B1, LineMode.LineSegment, A2, B2, LineMode.LineSegment));
    }

    [Fact]
    public void Intersects_LineLine_ReturnsaFalseForNonIntersectingLines()
    {
        ICoordinateList line1 = new CoordinateList(new Coordinate[] { new Coordinate(1.0, 1.0), new Coordinate(2.0, 1.0), new Coordinate(2.0, 2.0) });
        ICoordinateList line2 = new CoordinateList(new Coordinate[] { new Coordinate(1.0, 0.0), new Coordinate(2.0, 0.0), new Coordinate(2.0, -1.0) });

        Euclidean2DLocator target = new Euclidean2DLocator();

        Assert.False(target.Intersects(line1, line2));
    }

    [Fact]
    public void Intersects_LineLine_ReturnsaTrueForIntersectingLines()
    {
        ICoordinateList line1 = new CoordinateList(new Coordinate[] { new Coordinate(1.0, 1.0), new Coordinate(2.0, 1.0), new Coordinate(2.0, 2.0) });
        ICoordinateList line2 = new CoordinateList(new Coordinate[] { new Coordinate(1.0, 0.0), new Coordinate(1.5, 2.0), new Coordinate(2.0, -1.0) });

        Euclidean2DLocator target = new Euclidean2DLocator();

        Assert.True(target.Intersects(line1, line2));
    }
}
