using SpatialLite.Core.API;
using SpatialLite.Core.Geometries;
using SpatialLite.Core.IO;
using System;
using System.Collections.Generic;
using System.IO;
using Tests.SpatialLite.Core.Data;
using Xunit;

namespace Tests.SpatialLite.Core.IO;


public class WktReaderTests
{

    private readonly Coordinate[] _coordinatesXY = new Coordinate[] {
            new Coordinate(-10.1, 15.5), new Coordinate(20.2, -25.5), new Coordinate(30.3, 35.5)
    };

    private readonly Coordinate[] _coordinatesXYZ = new Coordinate[] {
            new Coordinate(-10.1, 15.5, 100.5), new Coordinate(20.2, -25.5, 200.5), new Coordinate(30.3, 35.5, -300.5)
    };

    private readonly Coordinate[] _coordinatesXYM = new Coordinate[] {
            new Coordinate(-10.1, 15.5, double.NaN, 1000.5), new Coordinate(20.2, -25.5, double.NaN, 2000.5), new Coordinate(30.3, 35.5, double.NaN, -3000.5)
    };

    private readonly Coordinate[] _coordinatesXYZM = new Coordinate[] {
            new Coordinate(-10.1, 15.5, 100.5, 1000.5), new Coordinate(20.2, -25.5, 200.5, 2000.5), new Coordinate(30.3, 35.5, -300.5, -3000.5)
    };

    private readonly Coordinate[] _coordinates2XYZM = new Coordinate[] {
            new Coordinate(-1.1, 1.5, 10.5, 100.5), new Coordinate(2.2, -2.5, 20.5, 200.5), new Coordinate(3.3, 3.5, -30.5, -300.5)
    };

    [Fact]
    public void Constructor_Stream_ThrowsAgrumentNullExceptioIfStreamIsNull()
    {
        Stream stream = null;
        Assert.Throws<ArgumentNullException>(() => new WktReader(stream));
    }

    [Fact]
    public void Constructor_Path_ThrowsFileNotFoundExceptioIfFileDoesNotExists()
    {
        Assert.Throws<FileNotFoundException>(() => new WktReader("non-existing-file.wkb"));
    }

    [Fact]
    public void Dispose_ClosesOutputStreamIfWritingToFiles()
    {
        string filename = "../../../Data/IO/wkt-point-3DM.wkt";

        WktReader target = new WktReader(filename);
        target.Dispose();
        FileStream testStream = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite);
        testStream.Dispose();
    }

    [Fact]
    public void Dispose_ClosesOutputStreamIfWritingToStream()
    {
        var stream = TestDataReader.Open("wkt-point-3DM.wkt");

        WktReader target = new WktReader(stream);
        target.Dispose();

        Assert.False(stream.CanRead);
    }
    public static IEnumerable<object[]> Read_ReadsAllGeometryTypesTestData
    {
        get
        {
            yield return new object[] { TestDataReader.Read("wkt-point-3DM.wkt") };
            yield return new object[] { TestDataReader.Read("wkt-linestring-3DM.wkt") };
            yield return new object[] { TestDataReader.Read("wkt-polygon-3DM.wkt") };
            yield return new object[] { TestDataReader.Read("wkt-multipoint-3DM.wkt") };
            yield return new object[] { TestDataReader.Read("wkt-multilinestring-3DM.wkt") };
            yield return new object[] { TestDataReader.Read("wkt-multipolygon-3DM.wkt") };
            yield return new object[] { TestDataReader.Read("wkt-geometry-collection-3DM.wkt") };
        }
    }

    [Theory]
    [MemberData(nameof(Read_ReadsAllGeometryTypesTestData))]
    public void Read_ReadsAllGeometryTypes(byte[] data)
    {
        using (WktReader target = new WktReader(new MemoryStream(data)))
        {
            IGeometry readGeometry = target.Read();
            Assert.NotNull(readGeometry);
        }
    }

    [Fact]
    public void Read_ReturnsNullIfStreamIsEmpty()
    {
        using (WktReader target = new WktReader(new MemoryStream()))
        {
            IGeometry readGeometry = target.Read();

            Assert.Null(readGeometry);
        }
    }

    [Fact]
    public void Read_ReturnsNullIfNoMoreGeometriesAreAvailableInInputStream()
    {
        using (WktReader target = new WktReader(TestDataReader.Open("wkt-point-3DM.wkt")))
        {
            target.Read();
            IGeometry readGeometry = target.Read();

            Assert.Null(readGeometry);
        }
    }

    [Fact]
    public void Read_ReadsMultipleGeometries()
    {
        using (WktReader target = new WktReader(TestDataReader.Open("wkt-point-and-linestring-3DM.wkt")))
        {
            IGeometry readGeometry = target.Read();
            Assert.True(readGeometry is Point);

            readGeometry = target.Read();
            Assert.True(readGeometry is LineString);
        }
    }

    [Fact]
    public void ReadT_ReadsGeometry()
    {
        using (WktReader target = new WktReader(TestDataReader.Open("wkt-point-3DM.wkt")))
        {
            Point read = target.Read<Point>();
            Assert.NotNull(read);
        }
    }

    [Fact]
    public void ReadT_ReturnsNullIfStreamIsEmpty()
    {
        using (WktReader target = new WktReader(new MemoryStream()))
        {
            IGeometry readGeometry = target.Read<Point>();

            Assert.Null(readGeometry);
        }
    }

    [Fact]
    public void ReadT_ThrowsExceptionIfWKTDoesNotRepresentGeometryOfSpecificType()
    {
        using (WktReader target = new WktReader(TestDataReader.Open("wkt-point-3DM.wkt")))
        {
            Assert.Throws<WktParseException>(target.Read<LineString>);
        }
    }

    [Fact]
    public void Parse_ParsesPoint()
    {
        string wkt = "point empty";

        Point parsed = (Point)WktReader.Parse(wkt);
        CompareCoordinate(Coordinate.Empty, parsed.Position);
    }

    [Fact]
    public void Parse_ParsesLineString()
    {
        string wkt = "linestring empty";

        LineString parsed = (LineString)WktReader.Parse(wkt);

        Assert.NotNull(parsed);
        Assert.Empty(parsed.Coordinates);
    }

    [Fact]
    public void Parse_ParsesPolygon()
    {
        string wkt = "polygon empty";

        Polygon parsed = (Polygon)WktReader.Parse(wkt);

        Assert.NotNull(parsed);
        Assert.Empty(parsed.ExteriorRing);
    }

    [Fact]
    public void Parse_ParsesGeometryCollection()
    {
        string wkt = "geometrycollection empty";

        GeometryCollection<Geometry> parsed = (GeometryCollection<Geometry>)WktReader.Parse(wkt);

        Assert.NotNull(parsed);
        Assert.Empty(parsed.Geometries);
    }

    [Fact]
    public void Parse_ParsesMultiPoint()
    {
        string wkt = "multipoint empty";

        MultiPoint parsed = (MultiPoint)WktReader.Parse(wkt);

        Assert.NotNull(parsed);
        Assert.Empty(parsed.Geometries);
    }

    [Fact]
    public void Parse_ParsesMultiLineString()
    {
        string wkt = "multilinestring empty";

        MultiLineString parsed = (MultiLineString)WktReader.Parse(wkt);

        Assert.NotNull(parsed);
        Assert.Empty(parsed.Geometries);
    }

    [Fact]
    public void Parse_ParsesMultiPolygon()
    {
        string wkt = "multipolygon empty";

        MultiPolygon parsed = (MultiPolygon)WktReader.Parse(wkt);

        Assert.NotNull(parsed);
        Assert.Empty(parsed.Geometries);
    }

    [Fact]
    public void Parse_ThrowsExceptionIfWktDoNotRepresentGeometry()
    {
        string wkt = "invalid string";

        Assert.Throws<WktParseException>(() => WktReader.Parse(wkt));
    }

    [Theory]
    [InlineData("point  zm \t(-10.1 15.5 100.5 \n1000.5)")]
    [InlineData("\n\rlinestring (-10.1 15.5, 20.2 -25.5, \t30.3  35.5)")]
    [InlineData("polygon\n\r m\t ((-10.1 15.5 1000.5,    20.2 -25.5  2000.5, 30.3 35.5 -3000.5))")]
    [InlineData("multipoint\t\r z ((-10.1 15.5 100.5),(20.2 -25.5 200.5))")]
    [InlineData("\nmultilinestring \tempty")]
    [InlineData("multipolygon\n\r (((-10.1\t 15.5,  20.2 -25.5,  30.3 35.5)),((-10.1  15.5, 20.2 -25.5, 30.3 35.5)))")]
    [InlineData("geometrycollection \t\n\r(point  (-10.1  15.5))")]
    public void Parse_ParsesGeometriesWithMultipleWhitespacesInsteadOneSpace(string wkt)
    {
        WktReader.Parse(wkt);
    }

    [Theory]
    [InlineData("point  zm ( -10.1 15.5 100.5 1000.5 )")]
    [InlineData("linestring ( -10.1 15.5 , 20.2 -25.5 , 30.3  35.5 )")]
    [InlineData("polygon m ( ( -10.1 15.5 1000.5 , 20.2 -25.5 2000.5,  30.3 35.5 -3000.5 ) )")]
    [InlineData("multipoint z ( ( -10.1 15.5 100.5 ) , ( 20.2 -25.5 200.5 ) )")]
    [InlineData("multilinestring ( ( -10.1 15.5 , 20.2 -25.5 , 30.3 35.5 ) , ( -10.1 15.5, 20.2 -25.5, 30.3 35.5 ) ) ")]
    [InlineData("multipolygon ( ( ( -10.1 15.5 , 20.2 -25.5 ,  30.3 35.5 ) ) , ( ( -10.1  15.5 , 20.2 -25.5 , 30.3 35.5 ) ) )")]
    [InlineData("geometrycollection ( point ( -10.1  15.5 ) )")]
    public void Parse_ParsesGeometriesWithWhitespacesAtPlacesWhereTheyAreNotExpected(string wkt)
    {
        WktReader.Parse(wkt);
    }

    [Fact]
    public void Parse_ParsesEmptyPoint()
    {
        string wkt = "point empty";

        Point parsed = WktReader.Parse<Point>(wkt);

        CompareCoordinate(Coordinate.Empty, parsed.Position);
    }

    [Fact]
    public void Parse_Parses2DPoint()
    {
        string wkt = "point (-10.1 15.5)";

        Point parsed = WktReader.Parse<Point>(wkt);

        Assert.False(parsed.Is3D);
        Assert.False(parsed.IsMeasured);
        CompareCoordinate(_coordinatesXY[0], parsed.Position);
    }

    [Fact]
    public void Parse_Parses2DMeasuredPoint()
    {
        string wkt = "point m (-10.1 15.5 1000.5)";

        Point parsed = WktReader.Parse<Point>(wkt);

        Assert.NotNull(parsed);
        Assert.False(parsed.Is3D);
        CompareCoordinate(_coordinatesXYM[0], parsed.Position);
    }

    [Fact]
    public void Parse_Parses3DPoint()
    {
        string wkt = "point z (-10.1 15.5 100.5)";

        Point parsed = WktReader.Parse<Point>(wkt);

        Assert.NotNull(parsed);
        Assert.False(parsed.IsMeasured);
        CompareCoordinate(_coordinatesXYZ[0], parsed.Position);
    }

    [Fact]
    public void Parse_Parses3DMeasuredPoint()
    {
        string wkt = "point zm (-10.1 15.5 100.5 1000.5)";

        Point parsed = WktReader.Parse<Point>(wkt);

        Assert.NotNull(parsed);
        CompareCoordinate(_coordinatesXYZM[0], parsed.Position);
    }

    [Fact]
    public void Parse_ThrowsExceptionIfWktDoNotRepresentPoint()
    {
        string wkt = "linestring empty";

        Assert.Throws<WktParseException>(() => WktReader.Parse<Point>(wkt));
    }

    [Fact]
    public void Parse_ParsesEmptyLineString()
    {
        string wkt = "linestring empty";

        LineString parsed = WktReader.Parse<LineString>(wkt);

        Assert.NotNull(parsed);
        Assert.Empty(parsed.Coordinates);
    }

    [Fact]
    public void Parse_Parses2DLineString()
    {
        string wkt = "linestring (-10.1 15.5, 20.2 -25.5, 30.3 35.5)";

        LineString parsed = WktReader.Parse<LineString>(wkt);

        Assert.NotNull(parsed);
        CompareCoordinates(_coordinatesXY, parsed.Coordinates);
    }

    [Fact]
    public void Parse_Parses2DMeasuredLineString()
    {
        string wkt = "linestring m (-10.1 15.5 1000.5, 20.2 -25.5 2000.5, 30.3 35.5 -3000.5)";

        LineString parsed = WktReader.Parse<LineString>(wkt);

        Assert.NotNull(parsed);
        CompareCoordinates(_coordinatesXYM, parsed.Coordinates);
    }

    [Fact]
    public void Parse_Parses3DLineString()
    {
        string wkt = "linestring z (-10.1 15.5 100.5, 20.2 -25.5 200.5, 30.3 35.5 -300.5)";

        LineString parsed = WktReader.Parse<LineString>(wkt);

        Assert.NotNull(parsed);
        CompareCoordinates(_coordinatesXYZ, parsed.Coordinates);
    }

    [Fact]
    public void Parse_Parses3DMeasuredLineString()
    {
        string wkt = "linestring zm (-10.1 15.5 100.5 1000.5, 20.2 -25.5 200.5 2000.5, 30.3 35.5 -300.5 -3000.5)";

        LineString parsed = WktReader.Parse<LineString>(wkt);

        Assert.NotNull(parsed);
        CompareCoordinates(_coordinatesXYZM, parsed.Coordinates);
    }

    [Fact]
    public void Parse_ThrowsExceptionIfWktDoNotRepresentLineString()
    {
        string wkt = "point empty";

        Assert.Throws<WktParseException>(() => WktReader.Parse<LineString>(wkt));
    }

    [Fact]
    public void ParsePolygon_ParsesEmptyPolygon()
    {
        string wkt = "polygon empty";

        Polygon parsed = WktReader.Parse<Polygon>(wkt);

        Assert.NotNull(parsed);
        Assert.Empty(parsed.ExteriorRing);
        Assert.Empty(parsed.InteriorRings);
    }

    [Fact]
    public void ParsePolygon_Parses2DPolygonOnlyExteriorRing()
    {
        string wkt = "polygon ((-10.1 15.5, 20.2 -25.5, 30.3 35.5))";

        Polygon parsed = WktReader.Parse<Polygon>(wkt);

        Assert.NotNull(parsed);
        CompareCoordinates(_coordinatesXY, parsed.ExteriorRing);
        Assert.Empty(parsed.InteriorRings);
    }

    [Fact]
    public void ParsePolygon_Parses3DPolygonOnlyExteriorRing()
    {
        string wkt = "polygon z ((-10.1 15.5 100.5, 20.2 -25.5 200.5, 30.3 35.5 -300.5))";

        Polygon parsed = WktReader.Parse<Polygon>(wkt);

        Assert.NotNull(parsed);
        CompareCoordinates(_coordinatesXYZ, parsed.ExteriorRing);
        Assert.Empty(parsed.InteriorRings);
    }

    [Fact]
    public void ParsePolygon_Parses2DMeauredPolygonOnlyExteriorRing()
    {
        string wkt = "polygon m ((-10.1 15.5 1000.5, 20.2 -25.5 2000.5, 30.3 35.5 -3000.5))";

        Polygon parsed = WktReader.Parse<Polygon>(wkt);

        Assert.NotNull(parsed);
        CompareCoordinates(_coordinatesXYM, parsed.ExteriorRing);
        Assert.Empty(parsed.InteriorRings);
    }

    [Fact]
    public void ParsePolygon_Parses3DMeasuredPolygonOnlyExteriorRing()
    {
        string wkt = "polygon zm ((-10.1 15.5 100.5 1000.5, 20.2 -25.5 200.5 2000.5, 30.3 35.5 -300.5 -3000.5))";

        Polygon parsed = WktReader.Parse<Polygon>(wkt);

        Assert.NotNull(parsed);
        CompareCoordinates(_coordinatesXYZM, parsed.ExteriorRing);
        Assert.Empty(parsed.InteriorRings);
    }

    [Fact]
    public void ParsePolygon_Parses3DMeasuredPolygonWithInteriorRings()
    {
        string wkt = "polygon zm ((-10.1 15.5 100.5 1000.5, 20.2 -25.5 200.5 2000.5, 30.3 35.5 -300.5 -3000.5),(-1.1 1.5 10.5 100.5, 2.2 -2.5 20.5 200.5, 3.3 3.5 -30.5 -300.5),(-1.1 1.5 10.5 100.5, 2.2 -2.5 20.5 200.5, 3.3 3.5 -30.5 -300.5))";

        Polygon parsed = WktReader.Parse<Polygon>(wkt);

        Assert.NotNull(parsed);
        CompareCoordinates(_coordinatesXYZM, parsed.ExteriorRing);
        Assert.Equal(2, parsed.InteriorRings.Count);
        CompareCoordinates(_coordinates2XYZM, parsed.InteriorRings[0]);
        CompareCoordinates(_coordinates2XYZM, parsed.InteriorRings[1]);
    }

    [Fact]
    public void ParsePolygong_ThrowsExceptionIfWktDoNotRepresentPolygon()
    {
        string wkt = "point empty";

        Assert.Throws<WktParseException>(() => WktReader.Parse<Polygon>(wkt));
    }

    [Fact]
    public void Parse_ParsesEmptyMultiPoint()
    {
        string wkt = "multipoint empty";

        MultiPoint parsed = WktReader.Parse<MultiPoint>(wkt);

        Assert.NotNull(parsed);
        Assert.Empty(parsed.Geometries);
    }

    [Fact]
    public void Parse_Parses2DMultiPoint()
    {
        string wkt = "multipoint ((-10.1 15.5),(20.2 -25.5))";

        MultiPoint parsed = WktReader.Parse<MultiPoint>(wkt);

        Assert.NotNull(parsed);
        Assert.Equal(2, parsed.Geometries.Count);
        CompareCoordinate(_coordinatesXY[0], parsed.Geometries[0].Position);
        CompareCoordinate(_coordinatesXY[1], parsed.Geometries[1].Position);
    }

    [Fact]
    public void Parse_Parses2DMeasuredMultiPoint()
    {
        string wkt = "multipoint m ((-10.1 15.5 1000.5),(20.2 -25.5 2000.5))";

        MultiPoint parsed = WktReader.Parse<MultiPoint>(wkt);

        Assert.NotNull(parsed);
        Assert.Equal(2, parsed.Geometries.Count);
        CompareCoordinate(_coordinatesXYM[0], parsed.Geometries[0].Position);
        CompareCoordinate(_coordinatesXYM[1], parsed.Geometries[1].Position);
    }

    [Fact]
    public void Parse_Parses3DMultiPoint()
    {
        string wkt = "multipoint z ((-10.1 15.5 100.5),(20.2 -25.5 200.5))";

        MultiPoint parsed = WktReader.Parse<MultiPoint>(wkt);

        Assert.NotNull(parsed);
        Assert.Equal(2, parsed.Geometries.Count);
        CompareCoordinate(_coordinatesXYZ[0], parsed.Geometries[0].Position);
        CompareCoordinate(_coordinatesXYZ[1], parsed.Geometries[1].Position);
    }

    [Fact]
    public void Parse_Parses3DMeasuredMultiPoint()
    {
        string wkt = "multipoint zm ((-10.1 15.5 100.5 1000.5),(20.2 -25.5 200.5 2000.5))";

        MultiPoint parsed = WktReader.Parse<MultiPoint>(wkt);

        Assert.NotNull(parsed);
        Assert.Equal(2, parsed.Geometries.Count);
        CompareCoordinate(_coordinatesXYZM[0], parsed.Geometries[0].Position);
        CompareCoordinate(_coordinatesXYZM[1], parsed.Geometries[1].Position);
    }

    [Fact]
    public void Parse_ThrowsExceptionIfWktDoNotRepresentMultiPoint()
    {
        string wkt = "point empty";

        Assert.Throws<WktParseException>(() => WktReader.Parse<MultiPoint>(wkt));
    }

    [Fact]
    public void Parse_ParsesEmptyMultiLineString()
    {
        string wkt = "multilinestring empty";

        MultiLineString parsed = WktReader.Parse<MultiLineString>(wkt);

        Assert.NotNull(parsed);
        Assert.Empty(parsed.Geometries);
    }

    [Fact]
    public void Parse_Parses2DMultiLineString()
    {
        string wkt = "multilinestring ((-10.1 15.5, 20.2 -25.5, 30.3 35.5),(-10.1 15.5, 20.2 -25.5, 30.3 35.5))";

        MultiLineString parsed = WktReader.Parse<MultiLineString>(wkt);

        Assert.NotNull(parsed);
        Assert.Equal(2, parsed.Geometries.Count);
        CompareCoordinates(_coordinatesXY, parsed.Geometries[0].Coordinates);
        CompareCoordinates(_coordinatesXY, parsed.Geometries[1].Coordinates);
    }

    [Fact]
    public void Parse_Parses2DMeasuredMultiLineString()
    {
        string wkt = "multilinestring m ((-10.1 15.5 1000.5, 20.2 -25.5 2000.5, 30.3 35.5 -3000.5),(-10.1 15.5 1000.5, 20.2 -25.5 2000.5, 30.3 35.5 -3000.5))";

        MultiLineString parsed = WktReader.Parse<MultiLineString>(wkt);

        Assert.NotNull(parsed);
        Assert.Equal(2, parsed.Geometries.Count);
        CompareCoordinates(_coordinatesXYM, parsed.Geometries[0].Coordinates);
        CompareCoordinates(_coordinatesXYM, parsed.Geometries[1].Coordinates);
    }

    [Fact]
    public void Parse_Parses3DMultiLineString()
    {
        string wkt = "multilinestring z ((-10.1 15.5 100.5, 20.2 -25.5 200.5, 30.3 35.5 -300.5),(-10.1 15.5 100.5, 20.2 -25.5 200.5, 30.3 35.5 -300.5))";

        MultiLineString parsed = WktReader.Parse<MultiLineString>(wkt);

        Assert.NotNull(parsed);
        Assert.Equal(2, parsed.Geometries.Count);
        CompareCoordinates(_coordinatesXYZ, parsed.Geometries[0].Coordinates);
        CompareCoordinates(_coordinatesXYZ, parsed.Geometries[1].Coordinates);
    }

    [Fact]
    public void Parse_Parses3DMeasuredMultiLineString()
    {
        string wkt = "multilinestring zm ((-10.1 15.5 100.5 1000.5, 20.2 -25.5 200.5 2000.5, 30.3 35.5 -300.5 -3000.5),(-10.1 15.5 100.5 1000.5, 20.2 -25.5 200.5 2000.5, 30.3 35.5 -300.5 -3000.5))";

        MultiLineString parsed = WktReader.Parse<MultiLineString>(wkt);

        Assert.NotNull(parsed);
        Assert.Equal(2, parsed.Geometries.Count);
        CompareCoordinates(_coordinatesXYZM, parsed.Geometries[0].Coordinates);
        CompareCoordinates(_coordinatesXYZM, parsed.Geometries[1].Coordinates);
    }

    [Fact]
    public void Parse_ThrowsExceptionIfWktDoNotRepresentMultiLineString()
    {
        string wkt = "point empty";

        Assert.Throws<WktParseException>(() => WktReader.Parse<MultiLineString>(wkt));
    }

    [Fact]
    public void Parse_ParsesEmptyMultiPolygon()
    {
        string wkt = "multipolygon empty";

        MultiPolygon parsed = WktReader.Parse<MultiPolygon>(wkt);

        Assert.NotNull(parsed);
        Assert.Empty(parsed.Geometries);
    }

    [Fact]
    public void Parse_Parses2DMultiPolygon()
    {
        string wkt = "multipolygon (((-10.1 15.5, 20.2 -25.5, 30.3 35.5)),((-10.1 15.5, 20.2 -25.5, 30.3 35.5)))";

        MultiPolygon parsed = WktReader.Parse<MultiPolygon>(wkt);

        Assert.NotNull(parsed);
        Assert.Equal(2, parsed.Geometries.Count);
        CompareCoordinates(_coordinatesXY, parsed.Geometries[0].ExteriorRing);
        CompareCoordinates(_coordinatesXY, parsed.Geometries[1].ExteriorRing);
    }

    [Fact]
    public void Parse_Parses2DMeasuredMultiPolygon()
    {
        string wkt = "multipolygon m (((-10.1 15.5 1000.5, 20.2 -25.5 2000.5, 30.3 35.5 -3000.5)),((-10.1 15.5 1000.5, 20.2 -25.5 2000.5, 30.3 35.5 -3000.5)))";

        MultiPolygon parsed = WktReader.Parse<MultiPolygon>(wkt);

        Assert.NotNull(parsed);
        Assert.Equal(2, parsed.Geometries.Count);
        CompareCoordinates(_coordinatesXYM, parsed.Geometries[0].ExteriorRing);
        CompareCoordinates(_coordinatesXYM, parsed.Geometries[1].ExteriorRing);
    }

    [Fact]
    public void Parse_Parses3DMultiPolygon()
    {
        string wkt = "multipolygon z (((-10.1 15.5 100.5, 20.2 -25.5 200.5, 30.3 35.5 -300.5)),((-10.1 15.5 100.5, 20.2 -25.5 200.5, 30.3 35.5 -300.5)))";

        MultiPolygon parsed = WktReader.Parse<MultiPolygon>(wkt);

        Assert.NotNull(parsed);
        Assert.Equal(2, parsed.Geometries.Count);
        CompareCoordinates(_coordinatesXYZ, parsed.Geometries[0].ExteriorRing);
        CompareCoordinates(_coordinatesXYZ, parsed.Geometries[1].ExteriorRing);
    }

    [Fact]
    public void Parse_Parses3DMeasuredMultiPolygon()
    {
        string wkt = "multipolygon zm (((-10.1 15.5 100.5 1000.5, 20.2 -25.5 200.5 2000.5, 30.3 35.5 -300.5 -3000.5)),((-10.1 15.5 100.5 1000.5, 20.2 -25.5 200.5 2000.5, 30.3 35.5 -300.5 -3000.5)))";

        MultiPolygon parsed = WktReader.Parse<MultiPolygon>(wkt);

        Assert.NotNull(parsed);
        Assert.Equal(2, parsed.Geometries.Count);
        CompareCoordinates(_coordinatesXYZM, parsed.Geometries[0].ExteriorRing);
        CompareCoordinates(_coordinatesXYZM, parsed.Geometries[1].ExteriorRing);
    }

    [Fact]
    public void Parse_ThrowsExceptionIfWktDoNotRepresentMultiPolygon()
    {
        string wkt = "point empty";

        Assert.Throws<WktParseException>(() => WktReader.Parse<MultiPolygon>(wkt));
    }

    [Fact]
    public void Parse_ParsesEmptyGeometryCollection()
    {
        string wkt = "geometrycollection empty";

        GeometryCollection<Geometry> parsed = WktReader.Parse<GeometryCollection<Geometry>>(wkt);

        Assert.NotNull(parsed);
        Assert.Empty(parsed.Geometries);
    }

    [Fact]
    public void Parse_Parses2DGeometryCollectionWithPoint()
    {
        string wkt = "geometrycollection (point (-10.1 15.5))";

        GeometryCollection<Geometry> parsed = WktReader.Parse<GeometryCollection<Geometry>>(wkt);

        Assert.NotNull(parsed);
        Assert.Single(parsed.Geometries);
        CompareCoordinate(_coordinatesXY[0], ((Point)parsed.Geometries[0]).Position);
    }

    [Fact]
    public void Parse_Parses2DMeasuredGeometryCollectionWithPoint()
    {
        string wkt = "geometrycollection m (point m (-10.1 15.5 1000.5))";

        GeometryCollection<Geometry> parsed = WktReader.Parse<GeometryCollection<Geometry>>(wkt);

        Assert.NotNull(parsed);
        Assert.Single(parsed.Geometries);
        CompareCoordinate(_coordinatesXYM[0], ((Point)parsed.Geometries[0]).Position);
    }

    [Fact]
    public void Parse_Parses3DGeometryCollectionWithPoint()
    {
        string wkt = "geometrycollection z (point z (-10.1 15.5 100.5))";

        GeometryCollection<Geometry> parsed = WktReader.Parse<GeometryCollection<Geometry>>(wkt);

        Assert.NotNull(parsed);
        Assert.Single(parsed.Geometries);
        CompareCoordinate(_coordinatesXYZ[0], ((Point)parsed.Geometries[0]).Position);
    }

    [Fact]
    public void Parse_Parses3DMeasuredGeometryCollectionWithPoint()
    {
        string wkt = "geometrycollection zm (point zm (-10.1 15.5 100.5 1000.5))";

        GeometryCollection<Geometry> parsed = WktReader.Parse<GeometryCollection<Geometry>>(wkt);

        Assert.NotNull(parsed);
        Assert.Single(parsed.Geometries);
        CompareCoordinate(_coordinatesXYZM[0], ((Point)parsed.Geometries[0]).Position);
    }

    [Fact]
    public void Parse_ParsesCollectionWithPointLineStringAndPolygon()
    {
        string wkt = "geometrycollection (point (-10.1 15.5),linestring (-10.1 15.5, 20.2 -25.5, 30.3 35.5),polygon ((-10.1 15.5, 20.2 -25.5, 30.3 35.5)))";

        GeometryCollection<Geometry> parsed = WktReader.Parse<GeometryCollection<Geometry>>(wkt);

        Assert.NotNull(parsed);
        Assert.Equal(3, parsed.Geometries.Count);
        CompareCoordinate(_coordinatesXY[0], ((Point)parsed.Geometries[0]).Position);
        CompareCoordinates(_coordinatesXY, ((LineString)parsed.Geometries[1]).Coordinates);
        CompareCoordinates(_coordinatesXY, ((Polygon)parsed.Geometries[2]).ExteriorRing);
    }

    [Fact]
    public void Parse_ParsesNestedCollections()
    {
        string wkt = "geometrycollection (geometrycollection (point (-10.1 15.5)))";

        GeometryCollection<Geometry> parsed = WktReader.Parse<GeometryCollection<Geometry>>(wkt);

        Assert.NotNull(parsed);
        Assert.Single(parsed.Geometries);
        GeometryCollection<Geometry> nested = (GeometryCollection<Geometry>)parsed.Geometries[0];
        CompareCoordinate(_coordinatesXY[0], ((Point)nested.Geometries[0]).Position);
    }

    [Fact]
    public void Parse_ThrowsExceptionIfWktDoNotRepresentGeometryCollection()
    {
        string wkt = "point empty";

        Assert.Throws<WktParseException>(() => WktReader.Parse<GeometryCollection<Geometry>>(wkt));
    }

    private void CompareCoordinate(Coordinate expected, Coordinate actual)
    {
        Assert.Equal(expected, actual);
    }

    private void CompareCoordinates(Coordinate[] expected, ICoordinateList actual)
    {
        Assert.Equal(expected.Length, actual.Count);

        for (int i = 0; i < expected.Length; i++)
        {
            Assert.Equal(expected[i], actual[i]);
        }
    }
}
