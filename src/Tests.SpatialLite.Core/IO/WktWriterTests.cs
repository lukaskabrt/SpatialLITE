using SpatialLite.Core.API;
using SpatialLite.Core.Geometries;
using SpatialLite.Core.IO;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Tests.SpatialLite.Core.IO;

public class WktWriterTests
{


    private static readonly Coordinate[] _coordinatesXY = new Coordinate[] {
            new Coordinate(-10.1, 15.5), new Coordinate(20.2, -25.5), new Coordinate(30.3, 35.5)
    };

    private static readonly Coordinate[] _coordinatesXYZ = new Coordinate[] {
            new Coordinate(-10.1, 15.5, 100.5), new Coordinate(20.2, -25.5, 200.5), new Coordinate(30.3, 35.5, -300.5)
    };

    private static readonly Coordinate[] _coordinatesXYM = new Coordinate[] {
            new Coordinate(-10.1, 15.5, double.NaN, 1000.5), new Coordinate(20.2, -25.5, double.NaN, 2000.5), new Coordinate(30.3, 35.5, double.NaN, -3000.5)
    };

    private static readonly Coordinate[] _coordinatesXYZM = new Coordinate[] {
            new Coordinate(-10.1, 15.5, 100.5, 1000.5), new Coordinate(20.2, -25.5, 200.5, 2000.5), new Coordinate(30.3, 35.5, -300.5, -3000.5)
    };

    private static readonly Coordinate[] _coordinates2XYZM = new Coordinate[] {
            new Coordinate(-1.1, 1.5, 10.5, 100.5), new Coordinate(2.2, -2.5, 20.5, 200.5), new Coordinate(3.3, 3.5, -30.5, -300.5)
    };

    [Fact]
    public void Construcotor_StreamSettings_SetsSettingsAndMakeThemReadOnly()
    {
        WktWriterSettings settings = new WktWriterSettings();
        using (WktWriter target = new WktWriter(new MemoryStream(), settings))
        {
            Assert.Same(settings, target.Settings);
            Assert.True(settings.IsReadOnly);
        }
    }

    [Fact]
    public void Constructor_StreamSettings_ThrowsArgumentNullExceptionIfStreamIsNull()
    {
        Stream stream = null;
        Assert.Throws<ArgumentNullException>(() => new WktWriter(stream, new WktWriterSettings()));
    }

    [Fact]
    public void Constructor_StreamSettings_ThrowsArgumentNullExceptionIfSettingsIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new WktWriter(new MemoryStream(), null));
    }

    [Fact]
    public void Construcotor_PathSettings_SetsSettingsAndMakeThemReadOnly()
    {
        WktWriterSettings settings = new WktWriterSettings();
        using (WktWriter target = new WktWriter(new MemoryStream(), settings))
        {
            Assert.Same(settings, target.Settings);
            Assert.True(settings.IsReadOnly);
        }
    }

    [Fact]
    public void Constructor_PathSettings_CreatesOutputFile()
    {
        string filename = PathHelper.GetTempFilePath("wktwriter-constructor-creates-output-test.wkt");
        File.Delete(filename);

        WktWriterSettings settings = new WktWriterSettings();
        using (WktWriter target = new WktWriter(filename, settings))
        {
            ;
        }

        Assert.True(File.Exists(filename));
    }

    [Fact]
    public void Constructor_PathSettings_ThrowsArgumentNullExceptionIfStreamIsNull()
    {
        string path = null;
        Assert.Throws<ArgumentNullException>(() => new WktWriter(path, new WktWriterSettings()));
    }

    [Fact]
    public void Constructor_PathSettings_ThrowsArgumentNullExceptionIfSettingsIsNull()
    {
        string path = PathHelper.GetTempFilePath("WktWriter-constructor-test.bin");

        Assert.Throws<ArgumentNullException>(() => new WktWriter(path, null));
    }

    public static IEnumerable<object[]> WriteToStringTestData
    {
        get
        {
            yield return new object[] { new Point(), "point empty" };
            yield return new object[] { new LineString(), "linestring empty" };
            yield return new object[] { new Polygon(), "polygon empty" };
            yield return new object[] { new MultiPoint(), "multipoint empty" };
            yield return new object[] { new MultiLineString(), "multilinestring empty" };
            yield return new object[] { new MultiPolygon(), "multipolygon empty" };
            yield return new object[] { new GeometryCollection<Geometry>(), "geometrycollection empty" };

        }
    }

    [Theory]
    [MemberData(nameof(WriteToStringTestData))]
    public void WriteToString_WritesAllGeometryTypes(Geometry toWrite, string expectedWkt)
    {
        TestWriteGeometry(toWrite, expectedWkt);
    }

    [Fact]
    public void Dispose_ClosesOutputStreamIfWritingToFiles()
    {
        string filename = PathHelper.GetTempFilePath("wktwriter-closes-output-filestream-test.wkt");

        WktWriterSettings settings = new WktWriterSettings();
        WktWriter target = new WktWriter(filename, settings);
        target.Dispose();
        FileStream testStream = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite);
        testStream.Dispose();
    }

    [Fact]
    public void Dispose_ClosesOutputStreamIfWritingToStream()
    {
        MemoryStream stream = new MemoryStream();

        WktWriter target = new WktWriter(stream, new WktWriterSettings());
        target.Dispose();

        Assert.False(stream.CanRead);
    }

    public static IEnumerable<object[]> Write_WritesPointsOfAllDimensionsTestData
    {
        get
        {
            yield return new object[] { new Point(), "point empty" };
            yield return new object[] { new Point(_coordinatesXY[0]), "point (-10.1 15.5)" };
            yield return new object[] { new Point(_coordinatesXYM[0]), "point m (-10.1 15.5 1000.5)" };
            yield return new object[] { new Point(_coordinatesXYZ[0]), "point z (-10.1 15.5 100.5)" };
            yield return new object[] { new Point(_coordinatesXYZM[0]), "point zm (-10.1 15.5 100.5 1000.5)" };
        }
    }

    [Theory]
    [MemberData(nameof(Write_WritesPointsOfAllDimensionsTestData))]
    public void Write_WritesPointsOfAllDimensions(Point toWrite, string expectedWkt)
    {
        TestWriteGeometry(toWrite, expectedWkt);
    }

    public static IEnumerable<object[]> Write_WritesLinestringOfAllDimensionsTestData
    {
        get
        {
            yield return new object[] { new LineString(), "linestring empty" };
            yield return new object[] { new LineString(_coordinatesXY), "linestring (-10.1 15.5, 20.2 -25.5, 30.3 35.5)" };
            yield return new object[] { new LineString(_coordinatesXYM), "linestring m (-10.1 15.5 1000.5, 20.2 -25.5 2000.5, 30.3 35.5 -3000.5)" };
            yield return new object[] { new LineString(_coordinatesXYZ), "linestring z (-10.1 15.5 100.5, 20.2 -25.5 200.5, 30.3 35.5 -300.5)" };
            yield return new object[] { new LineString(_coordinatesXYZM), "linestring zm (-10.1 15.5 100.5 1000.5, 20.2 -25.5 200.5 2000.5, 30.3 35.5 -300.5 -3000.5)" };
        }
    }

    [Theory]
    [MemberData(nameof(Write_WritesLinestringOfAllDimensionsTestData))]
    public void Write_WritesLinestringsOfAllDimensions(LineString toWrite, string expectedWkt)
    {
        TestWriteGeometry(toWrite, expectedWkt);
    }

    public static IEnumerable<object[]> Write_WritesPolygonsOfAllDimensionsTestData
    {
        get
        {
            yield return new object[] { new Polygon(), "polygon empty" };
            yield return new object[] { new Polygon(new CoordinateList(_coordinatesXY)), "polygon ((-10.1 15.5, 20.2 -25.5, 30.3 35.5))" };
            yield return new object[] { new Polygon(new CoordinateList(_coordinatesXYM)), "polygon m ((-10.1 15.5 1000.5, 20.2 -25.5 2000.5, 30.3 35.5 -3000.5))" };
            yield return new object[] { new Polygon(new CoordinateList(_coordinatesXYZ)), "polygon z ((-10.1 15.5 100.5, 20.2 -25.5 200.5, 30.3 35.5 -300.5))" };
            yield return new object[] { new Polygon(new CoordinateList(_coordinatesXYZM)), "polygon zm ((-10.1 15.5 100.5 1000.5, 20.2 -25.5 200.5 2000.5, 30.3 35.5 -300.5 -3000.5))" };
        }
    }

    [Theory]
    [MemberData(nameof(Write_WritesPolygonsOfAllDimensionsTestData))]
    public void Write_WritesPolygonsOfAllDimensions(Polygon toWrite, string expectedWkt)
    {
        TestWriteGeometry(toWrite, expectedWkt);
    }

    [Fact]
    public void Write_WritesComplexPolygonWitOuterAndInnerRings()
    {
        string wkt = "polygon zm ((-10.1 15.5 100.5 1000.5, 20.2 -25.5 200.5 2000.5, 30.3 35.5 -300.5 -3000.5),(-1.1 1.5 10.5 100.5, 2.2 -2.5 20.5 200.5, 3.3 3.5 -30.5 -300.5),(-1.1 1.5 10.5 100.5, 2.2 -2.5 20.5 200.5, 3.3 3.5 -30.5 -300.5))";
        Polygon polygon = new Polygon(new CoordinateList(_coordinatesXYZM));
        polygon.InteriorRings.Add(new CoordinateList(_coordinates2XYZM));
        polygon.InteriorRings.Add(new CoordinateList(_coordinates2XYZM));

        TestWriteGeometry(polygon, wkt);
    }

    public static IEnumerable<object[]> Write_WritesMultiPoinsOfAllDimensionsTestData
    {
        get
        {
            yield return new object[] { new MultiPoint(), "multipoint empty" };
            yield return new object[] { new MultiPoint(new Point[] { new Point(_coordinatesXY[0]), new Point(_coordinatesXY[1]) }), "multipoint ((-10.1 15.5),(20.2 -25.5))" };
            yield return new object[] { new MultiPoint(new Point[] { new Point(_coordinatesXYM[0]), new Point(_coordinatesXYM[1]) }), "multipoint m ((-10.1 15.5 1000.5),(20.2 -25.5 2000.5))" };
            yield return new object[] { new MultiPoint(new Point[] { new Point(_coordinatesXYZ[0]), new Point(_coordinatesXYZ[1]) }), "multipoint z ((-10.1 15.5 100.5),(20.2 -25.5 200.5))" };
            yield return new object[] { new MultiPoint(new Point[] { new Point(_coordinatesXYZM[0]), new Point(_coordinatesXYZM[1]) }), "multipoint zm ((-10.1 15.5 100.5 1000.5),(20.2 -25.5 200.5 2000.5))" };
        }
    }

    [Theory]
    [MemberData(nameof(Write_WritesMultiPoinsOfAllDimensionsTestData))]
    public void Write_WritesMultiPointsOfAllDimensions(MultiPoint toWrite, string expectedWkt)
    {
        TestWriteGeometry(toWrite, expectedWkt);
    }

    public static IEnumerable<object[]> Write_WritesMultiLineStringsOfAllDimensionsTestData
    {
        get
        {
            yield return new object[] { new MultiLineString(), "multilinestring empty" };
            yield return new object[] { new MultiLineString(new LineString[] { new LineString(_coordinatesXY), new LineString(_coordinatesXY) }),
                "multilinestring ((-10.1 15.5, 20.2 -25.5, 30.3 35.5),(-10.1 15.5, 20.2 -25.5, 30.3 35.5))" };
            yield return new object[] { new MultiLineString(new LineString[] { new LineString(_coordinatesXYM), new LineString(_coordinatesXYM) }),
                "multilinestring m ((-10.1 15.5 1000.5, 20.2 -25.5 2000.5, 30.3 35.5 -3000.5),(-10.1 15.5 1000.5, 20.2 -25.5 2000.5, 30.3 35.5 -3000.5))" };
            yield return new object[] { new MultiLineString(new LineString[] { new LineString(_coordinatesXYZ), new LineString(_coordinatesXYZ) }),
                "multilinestring z ((-10.1 15.5 100.5, 20.2 -25.5 200.5, 30.3 35.5 -300.5),(-10.1 15.5 100.5, 20.2 -25.5 200.5, 30.3 35.5 -300.5))" };
            yield return new object[] { new MultiLineString(new LineString[] { new LineString(_coordinatesXYZM), new LineString(_coordinatesXYZM) }),
                "multilinestring zm ((-10.1 15.5 100.5 1000.5, 20.2 -25.5 200.5 2000.5, 30.3 35.5 -300.5 -3000.5),(-10.1 15.5 100.5 1000.5, 20.2 -25.5 200.5 2000.5, 30.3 35.5 -300.5 -3000.5))" };
        }
    }

    [Theory]
    [MemberData(nameof(Write_WritesMultiLineStringsOfAllDimensionsTestData))]
    public void Write_WritesMultiLineStringsOfAllDimensions(MultiLineString toWrite, string expectedWkt)
    {
        TestWriteGeometry(toWrite, expectedWkt);
    }

    public static IEnumerable<object[]> Write_WritesMultiPolygonsOfAllDimensionsTestData
    {
        get
        {
            yield return new object[] { new MultiPolygon(), "multipolygon empty" };
            yield return new object[] { new MultiPolygon(new Polygon[] { new Polygon(new CoordinateList(_coordinatesXY)), new Polygon(new CoordinateList(_coordinatesXY)) }),
                "multipolygon (((-10.1 15.5, 20.2 -25.5, 30.3 35.5)),((-10.1 15.5, 20.2 -25.5, 30.3 35.5)))" };
            yield return new object[] { new MultiPolygon(new Polygon[] { new Polygon(new CoordinateList(_coordinatesXYM)), new Polygon(new CoordinateList(_coordinatesXYM)) }),
                "multipolygon m (((-10.1 15.5 1000.5, 20.2 -25.5 2000.5, 30.3 35.5 -3000.5)),((-10.1 15.5 1000.5, 20.2 -25.5 2000.5, 30.3 35.5 -3000.5)))" };
            yield return new object[] { new MultiPolygon(new Polygon[] { new Polygon(new CoordinateList(_coordinatesXYZ)), new Polygon(new CoordinateList(_coordinatesXYZ)) }),
                "multipolygon z (((-10.1 15.5 100.5, 20.2 -25.5 200.5, 30.3 35.5 -300.5)),((-10.1 15.5 100.5, 20.2 -25.5 200.5, 30.3 35.5 -300.5)))" };
            yield return new object[] { new MultiPolygon(new Polygon[] { new Polygon(new CoordinateList(_coordinatesXYZM)), new Polygon(new CoordinateList(_coordinatesXYZM)) }),
                "multipolygon zm (((-10.1 15.5 100.5 1000.5, 20.2 -25.5 200.5 2000.5, 30.3 35.5 -300.5 -3000.5)),((-10.1 15.5 100.5 1000.5, 20.2 -25.5 200.5 2000.5, 30.3 35.5 -300.5 -3000.5)))" };
        }
    }

    [Theory]
    [MemberData(nameof(Write_WritesMultiPolygonsOfAllDimensionsTestData))]
    public void Write_WritesMultiPolygonsOfAllDimensions(MultiPolygon toWrite, string expectedWkt)
    {
        TestWriteGeometry(toWrite, expectedWkt);
    }

    public static IEnumerable<object[]> Write_WritesGeometryCollectionOfAllDimensionsTestData
    {
        get
        {
            yield return new object[] { new GeometryCollection<Geometry>(), "geometrycollection empty" };
            yield return new object[] { new GeometryCollection<Geometry>(new Geometry[] { new Point(_coordinatesXY[0]) }), "geometrycollection (point (-10.1 15.5))" };
            yield return new object[] { new GeometryCollection<Geometry>(new Geometry[] { new Point(_coordinatesXYM[0]) }), "geometrycollection m (point m (-10.1 15.5 1000.5))" };
            yield return new object[] { new GeometryCollection<Geometry>(new Geometry[] { new Point(_coordinatesXYZ[0]) }), "geometrycollection z (point z (-10.1 15.5 100.5))" };
            yield return new object[] { new GeometryCollection<Geometry>(new Geometry[] { new Point(_coordinatesXYZM[0]) }), "geometrycollection zm (point zm (-10.1 15.5 100.5 1000.5))" };
        }
    }

    [Theory]
    [MemberData(nameof(Write_WritesGeometryCollectionOfAllDimensionsTestData))]
    public void Write_WritesGeometryCollectionOfAllDimensions(GeometryCollection<Geometry> toWrite, string expectedWkt)
    {
        TestWriteGeometry(toWrite, expectedWkt);
    }

    [Fact]
    public void Write_WritesCollectionWithAllGeometryTypes()
    {
        string wkt = "geometrycollection (point (-10.1 15.5),linestring (-10.1 15.5, 20.2 -25.5, 30.3 35.5),polygon ((-10.1 15.5, 20.2 -25.5, 30.3 35.5)),multipoint empty,multilinestring empty,multipolygon empty)";
        GeometryCollection<Geometry> collection = new GeometryCollection<Geometry>();
        collection.Geometries.Add(new Point(_coordinatesXY[0]));
        collection.Geometries.Add(new LineString(_coordinatesXY));
        collection.Geometries.Add(new Polygon(new CoordinateList(_coordinatesXY)));
        collection.Geometries.Add(new MultiPoint());
        collection.Geometries.Add(new MultiLineString());
        collection.Geometries.Add(new MultiPolygon());

        TestWriteGeometry(collection, wkt);
    }

    [Fact]
    public void Write_WritesNestedCollection()
    {
        string wkt = "geometrycollection (geometrycollection (point (-10.1 15.5)))";
        GeometryCollection<Geometry> collection = new GeometryCollection<Geometry>();
        GeometryCollection<Geometry> nested = new GeometryCollection<Geometry>();
        nested.Geometries.Add(new Point(_coordinatesXY[0]));
        collection.Geometries.Add(nested);

        TestWriteGeometry(collection, wkt);
    }

    private void TestWriteGeometry(IGeometry geometry, string expectedWkt)
    {
        MemoryStream stream = new MemoryStream();
        using (WktWriter writer = new WktWriter(stream, new WktWriterSettings()))
        {
            writer.Write(geometry);
        }

        using (TextReader tr = new StreamReader(new MemoryStream(stream.ToArray())))
        {
            string wkt = tr.ReadToEnd();
            Assert.Equal(expectedWkt, wkt, StringComparer.OrdinalIgnoreCase);
        }
    }
}
