﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Xunit;
using Tests.SpatialLite.Core.Data;

using SpatialLite.Core.API;
using SpatialLite.Core.Geometries;
using SpatialLite.Core.IO;

namespace Tests.SpatialLite.Core.IO {
	public class WkbReaderTests {

		#region Constructor(Stream) tests

		[Fact]
		public void Constructor_Stream_ThrowsAgrumentNullExceptioIfStreamIsNull() {
			Stream stream = null;
			Assert.Throws<ArgumentNullException>(() => new WkbReader(stream));
		}

		#endregion

		#region Constructor(Path) tests

		[Fact]
		public void Constructor_Path_ThrowsFileNotFoundExceptioIfFileDoesNotExists() {
			Assert.Throws<FileNotFoundException>(() => new WkbReader("non-existing-file.wkb"));
		}

		#endregion

		#region Dispose() tests

		[Fact]
		public void Dispose_ClosesOutputStreamIfWritingToFiles() {
			string filename = "../../../Data/IO/point-3DM.wkb";

			WkbReader target = new WkbReader(filename);
			target.Dispose();

			FileStream testStream = null;
			testStream = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite);
			testStream.Dispose();
		}

		[Fact]
		public void Dispose_ClosesOutputStreamIfWritingToStream() {
			var stream = TestDataReader.Open("point-3DM.wkb");

			WkbReader target = new WkbReader(stream);
			target.Dispose();

			Assert.False(stream.CanRead);
		}

		#endregion

		#region Read() tests

		[Fact]
		public void Read_ReturnsNullIfStreamIsEmpty() {
			MemoryStream stream = new MemoryStream();

			WkbReader target = new WkbReader(stream);
			Geometry read = target.Read();

			Assert.Null(read);
		}

		[Fact]
		public void Read_ReadsGeometry() {
			Point expected = (Point)this.ParseWKT("point zm (-10.1 15.5 100.5 1000.5)");

			WkbReader target = new WkbReader(TestDataReader.Open("point-3DM.wkb"));
			Point parsed = (Point)target.Read();

			this.ComparePoints(parsed, expected);
		}

        [Fact]
        public void Read_ReadsMultipleGeometries() {
            Point expected1 = (Point)this.ParseWKT("point zm (-10.1 15.5 100.5 1000.5)");
            Point expected2 = (Point)this.ParseWKT("point zm (-10.2 15.6 100.6 1000.6)");

            WkbReader target = new WkbReader(TestDataReader.Open("two-points-3DM.wkb"));

            Point parsed1 = (Point)target.Read();
            this.ComparePoints(parsed1, expected1);

            Point parsed2 = (Point)target.Read();
            this.ComparePoints(parsed2, expected2);
        }

        [Fact]
        public void Read_ReturnsNullIfNoMoreGeometriesAreAvailable() {
            WkbReader target = new WkbReader(TestDataReader.Open("point-3DM.wkb"));

            target.Read();
            Geometry parsed = target.Read();

            Assert.Null(parsed);
        }

        [Fact]
        public void Read_ThrowsExceptionIfWKBDoesNotRepresentGeometry() {
            byte[] wkb = new byte[] { 12, 0, 0, 45, 78, 124, 36, 0 };
            using (MemoryStream ms = new MemoryStream(wkb)) {
                WkbReader target = new WkbReader(ms);

                Assert.Throws<WkbFormatException>(() => target.Read());
            }
        }

        #endregion

        #region Read<T>(WKB) tests

        [Fact]
        public void ReadT_ReturnsNullIfStreamIsEmpty() {
            MemoryStream stream = new MemoryStream();

            WkbReader target = new WkbReader(stream);
            Geometry read = target.Read<Geometry>();

            Assert.Null(read);
        }

        [Fact]
        public void ReadT_ReadsGeometry() {
            Point expected = (Point)this.ParseWKT("point zm (-10.1 15.5 100.5 1000.5)");

            WkbReader target = new WkbReader(TestDataReader.Open("point-3DM.wkb"));
            Point parsed = target.Read<Point>();

            this.ComparePoints(parsed, expected);
        }

        [Fact]
        public void ReadT_ReturnsNullIfNoMoreGeometriesAreAvailable() {
            WkbReader target = new WkbReader(TestDataReader.Open("point-3DM.wkb"));

            target.Read<Point>();
            Geometry parsed = target.Read<Point>();

            Assert.Null(parsed);
        }

        [Fact]
        public void ReadT_ThrowsExceptionIfWKBDoesNotRepresentGeometry() {
            byte[] wkb = new byte[] { 12, 0, 0, 45, 78, 124, 36, 0 };
            using (MemoryStream ms = new MemoryStream(wkb)) {
                WkbReader target = new WkbReader(ms);

                Assert.Throws<WkbFormatException>(() => target.Read<Point>());
            }
        }

        [Fact]
        public void ReadT_ThrowsExceptionIfWKBDoesNotRepresentSpecifiecGeometryType() {
            WkbReader target = new WkbReader(TestDataReader.Open("point-3DM.wkb"));
            Assert.Throws<WkbFormatException>(() => target.Read<LineString>());
        }

        #endregion

        #region Parse(WKB) tests

        [Fact]
        public void Parse_ThrowsExceptionIfWKBDoesNotRepresentGeometry() {
            byte[] wkb = new byte[] { 12, 0, 0, 45, 78, 124, 36, 0 };

            Assert.Throws<WkbFormatException>(() => WkbReader.Parse(wkb));
        }

        [Fact]
        public void Parse_ReturnsNullForEmptyInput() {
            byte[] wkb = new byte[0];

            Assert.Null(WkbReader.Parse(wkb));
        }

        [Fact]
        public void Parse_ThrowsArgumentNullExceptionIfDataIsNull() {
            Assert.Throws<ArgumentNullException>(() => WkbReader.Parse(null));
        }

        [Fact]
        public void Parse_ReturnsParsedGeometry() {
            string wkt = "point m (-10.1 15.5 1000.5)";
            Point expected = (Point)this.ParseWKT(wkt);
            Point parsed = (Point)WkbReader.Parse(TestDataReader.Read("point-2DM.wkb"));

            this.ComparePoints(parsed, expected);
        }

        #endregion

        #region Parse<T>(WKB) tests

        [Fact]
        public void ParseT_ThrowsExceptionIfWKBDoesNotRepresentSpecifiedType() {
            byte[] wkb = TestDataReader.Read("linestring-2D.wkb");

            Assert.Throws<WkbFormatException>(() => WkbReader.Parse<Point>(wkb));
        }

        [Fact]
        public void ParseT_ReturnsNullForEmptyInput() {
            byte[] wkb = new byte[0];

            Assert.Null(WkbReader.Parse<Geometry>(wkb));
        }

        [Fact]
        public void ParseT_ThrowsArgumentNullExceptionIfDataIsNull() {
            Assert.Throws<ArgumentNullException>(() => WkbReader.Parse<Point>(null));
        }

        #endregion

        #region Parse<Point>(WKB) tests

        [Fact]
        public void ParsePoint_Parses2DPoint() {
            string wkt = "point (-10.1 15.5)";
            byte[] wkb = TestDataReader.Read("point-2D.wkb");

            this.TestParsePoint(wkb, wkt);
        }

        [Fact]
        public void ParsePoint_Parses2DMeasuredPoint() {
            string wkt = "point m (-10.1 15.5 1000.5)";
            byte[] wkb = TestDataReader.Read("point-2DM.wkb");

            this.TestParsePoint(wkb, wkt);
        }

        [Fact]
        public void ParsePoint_Parses3DPoint() {
            string wkt = "point z (-10.1 15.5 100.5)";
            byte[] wkb = TestDataReader.Read("point-3D.wkb");

            this.TestParsePoint(wkb, wkt);
        }

        [Fact]
        public void ParsePoint_Parses3DMeasuredPoint() {
            string wkt = "point zm (-10.1 15.5 100.5 1000.5)";
            byte[] wkb = TestDataReader.Read("point-3DM.wkb");

            this.TestParsePoint(wkb, wkt);
        }

        #endregion

        #region Parse<LineString>(WKB) tests

        [Fact]
        public void Parse_ParsesEmptyLineString() {
            string wkt = "linestring empty";
            byte[] wkb = TestDataReader.Read("linestring-empty.wkb");

            this.TestParseLineString(wkb, wkt);
        }

        [Fact]
        public void Parse_Parses2DLineString() {
            string wkt = "linestring (-10.1 15.5, 20.2 -25.5, 30.3 35.5)";
            byte[] wkb = TestDataReader.Read("linestring-2D.wkb");

            this.TestParseLineString(wkb, wkt);
        }

        [Fact]
        public void Parse_Parses2DMeasuredLineString() {
            string wkt = "linestring m (-10.1 15.5 1000.5, 20.2 -25.5 2000.5, 30.3 35.5 -3000.5)";
            byte[] wkb = TestDataReader.Read("linestring-2DM.wkb");

            this.TestParseLineString(wkb, wkt);
        }

        [Fact]
        public void Parse_Parses3DLineString() {
            string wkt = "linestring z (-10.1 15.5 100.5, 20.2 -25.5 200.5, 30.3 35.5 -300.5)";
            byte[] wkb = TestDataReader.Read("linestring-3D.wkb");
            this.TestParseLineString(wkb, wkt);
        }

        [Fact]
        public void Parse_Parses3DMeasuredLineString() {
            string wkt = "linestring zm (-10.1 15.5 100.5 1000.5, 20.2 -25.5 200.5 2000.5, 30.3 35.5 -300.5 -3000.5)";
            byte[] wkb = TestDataReader.Read("linestring-3DM.wkb");

            this.TestParseLineString(wkb, wkt);
        }

        #endregion

        #region Parse<Polygon>(WKB) tests

        [Fact]
        public void Parse_ParsesEmptyPolygon() {
            string wkt = "polygon empty";
            byte[] wkb = TestDataReader.Read("polygon-empty.wkb");

            this.TestParsePolygon(wkb, wkt);
        }

        [Fact]
        public void Parse_Parses2DPolygonOnlyExteriorRing() {
            string wkt = "polygon ((-10.1 15.5, 20.2 -25.5, 30.3 35.5))";
            byte[] wkb = TestDataReader.Read("polygon-ext-2D.wkb");

            this.TestParsePolygon(wkb, wkt);
        }

        [Fact]
        public void Parse_Parses2DMeasuredPolygonOnlyExteriorRing() {
            string wkt = "polygon m ((-10.1 15.5 1000.5, 20.2 -25.5 2000.5, 30.3 35.5 -3000.5))";
            byte[] wkb = TestDataReader.Read("polygon-ext-2DM.wkb");

            this.TestParsePolygon(wkb, wkt);
        }

        [Fact]
        public void Parse_Parses3DPolygonOnlyExteriorRing() {
            string wkt = "polygon z ((-10.1 15.5 100.5, 20.2 -25.5 200.5, 30.3 35.5 -300.5))";
            byte[] wkb = TestDataReader.Read("polygon-ext-3D.wkb");

            this.TestParsePolygon(wkb, wkt);
        }

        [Fact]
        public void Parse_Parses3DMeasuredPolygonOnlyExteriorRing() {
            string wkt = "polygon zm ((-10.1 15.5 100.5 1000.5, 20.2 -25.5 200.5 2000.5, 30.3 35.5 -300.5 -3000.5))";
            byte[] wkb = TestDataReader.Read("polygon-ext-3DM.wkb");

            this.TestParsePolygon(wkb, wkt);
        }

        [Fact]
        public void Parse_Parses3DMeasuredPolygon() {
            string wkt = "polygon zm ((-10.1 15.5 100.5 1000.5, 20.2 -25.5 200.5 2000.5, 30.3 35.5 -300.5 -3000.5),(-1.1 1.5 10.5 100.5, 2.2 -2.5 20.5 200.5, 3.3 3.5 -30.5 -300.5),(-1.1 1.5 10.5 100.5, 2.2 -2.5 20.5 200.5, 3.3 3.5 -30.5 -300.5))";
            byte[] wkb = TestDataReader.Read("polygon-3DM.wkb");

            this.TestParsePolygon(wkb, wkt);
        }

        #endregion

        #region Parse<MultiPoint>(WKB) tests

        [Fact]
        public void ParseMultiPoint_ParsesEmptyMultipoint() {
            string wkt = "multipoint empty";
            byte[] wkb = TestDataReader.Read("multipoint-empty.wkb");

            this.TestParseMultiPoint(wkb, wkt);
        }

        [Fact]
        public void ParseMultiPoint_Parses2DMultiPoint() {
            string wkt = "multipoint ((-10.1 15.5),(20.2 -25.5))";
            byte[] wkb = TestDataReader.Read("multipoint-2D.wkb");

            this.TestParseMultiPoint(wkb, wkt);
        }

        [Fact]
        public void ParseMultiPoint_Parses2DMeasuredMultiPoint() {
            string wkt = "multipoint m ((-10.1 15.5 1000.5),(20.2 -25.5 2000.5))";
            byte[] wkb = TestDataReader.Read("multipoint-2DM.wkb");

            this.TestParseMultiPoint(wkb, wkt);
        }

        [Fact]
        public void ParseMultiPoint_Parses3DMultiPoint() {
            string wkt = "multipoint z ((-10.1 15.5 100.5),(20.2 -25.5 200.5))";
            byte[] wkb = TestDataReader.Read("multipoint-3D.wkb");

            this.TestParseMultiPoint(wkb, wkt);
        }

        [Fact]
        public void ParseMultiPoint_Parses3DMeasuredMultiPoint() {
            string wkt = "multipoint zm ((-10.1 15.5 100.5 1000.5),(20.2 -25.5 200.5 2000.5))";
            byte[] wkb = TestDataReader.Read("multipoint-3DM.wkb");

            this.TestParseMultiPoint(wkb, wkt);
        }

        #endregion

        #region Parse<MuliLineString>(WKB) test

        [Fact]
        public void ParseMultiLineString_ParsesEmptyMultiLineString() {
            string wkt = "multilinestring empty";
            byte[] wkb = TestDataReader.Read("multilinestring-empty.wkb");

            this.TestParseMultiLineString(wkb, wkt);
        }

        [Fact]
        public void ParseMultiLineString_Parses2DMultiLineString() {
            string wkt = "multilinestring ((-10.1 15.5, 20.2 -25.5, 30.3 35.5),(-10.1 15.5, 20.2 -25.5, 30.3 35.5))";
            byte[] wkb = TestDataReader.Read("multilinestring-2D.wkb");

            this.TestParseMultiLineString(wkb, wkt);
        }

        [Fact]
        public void ParseMultiLineString_Parses2DMeasuredMultiLineString() {
            string wkt = "multilinestring m ((-10.1 15.5 1000.5, 20.2 -25.5 2000.5, 30.3 35.5 -3000.5),(-10.1 15.5 1000.5, 20.2 -25.5 2000.5, 30.3 35.5 -3000.5))";
            byte[] wkb = TestDataReader.Read("multilinestring-2DM.wkb");

            this.TestParseMultiLineString(wkb, wkt);
        }

        [Fact]
        public void ParseMultiLineString_Parses3DMultiLineString() {
            string wkt = "multilinestring z ((-10.1 15.5 100.5, 20.2 -25.5 200.5, 30.3 35.5 -300.5),(-10.1 15.5 100.5, 20.2 -25.5 200.5, 30.3 35.5 -300.5))";
            byte[] wkb = TestDataReader.Read("multilinestring-3D.wkb");

            this.TestParseMultiLineString(wkb, wkt);
        }

        [Fact]
        public void ParseMultiLineString_Parses3DMeasuredMultiLineString() {
            string wkt = "multilinestring zm ((-10.1 15.5 100.5 1000.5, 20.2 -25.5 200.5 2000.5, 30.3 35.5 -300.5 -3000.5),(-10.1 15.5 100.5 1000.5, 20.2 -25.5 200.5 2000.5, 30.3 35.5 -300.5 -3000.5))";
            byte[] wkb = TestDataReader.Read("multilinestring-3DM.wkb");

            this.TestParseMultiLineString(wkb, wkt);
        }

        #endregion

        #region Parse<MultiPolygon>(WKB) tests

        [Fact]
        public void ParseMultiPolygon_ParsesEmptyMultiPolygon() {
            string wkt = "multipolygon empty";
            byte[] wkb = TestDataReader.Read("multipolygon-empty.wkb");

            this.TestParseMultiPolygon(wkb, wkt);
        }

        [Fact]
        public void ParseMultiPolygon_Parses2DMultiPolygon() {
            string wkt = "multipolygon (((-10.1 15.5, 20.2 -25.5, 30.3 35.5)),((-10.1 15.5, 20.2 -25.5, 30.3 35.5)))";
            byte[] wkb = TestDataReader.Read("multipolygon-2D.wkb");

            this.TestParseMultiPolygon(wkb, wkt);
        }

        [Fact]
        public void ParseMultiPolygon_Parses2DMeasuredMultiPolygon() {
            string wkt = "multipolygon m (((-10.1 15.5 1000.5, 20.2 -25.5 2000.5, 30.3 35.5 -3000.5)),((-10.1 15.5 1000.5, 20.2 -25.5 2000.5, 30.3 35.5 -3000.5)))";
            byte[] wkb = TestDataReader.Read("multipolygon-2DM.wkb");

            this.TestParseMultiPolygon(wkb, wkt);
        }

        [Fact]
        public void ParseMultiPolygon_Parses3DMultiPolygon() {
            string wkt = "multipolygon z (((-10.1 15.5 100.5, 20.2 -25.5 200.5, 30.3 35.5 -300.5)),((-10.1 15.5 100.5, 20.2 -25.5 200.5, 30.3 35.5 -300.5)))";
            byte[] wkb = TestDataReader.Read("multipolygon-3D.wkb");

            this.TestParseMultiPolygon(wkb, wkt);
        }

        [Fact]
        public void ParseMultiPolygon_Parses3DMeasuredMultiPolygon() {
            string wkt = "multipolygon zm (((-10.1 15.5 100.5 1000.5, 20.2 -25.5 200.5 2000.5, 30.3 35.5 -300.5 -3000.5)),((-10.1 15.5 100.5 1000.5, 20.2 -25.5 200.5 2000.5, 30.3 35.5 -300.5 -3000.5)))";
            byte[] wkb = TestDataReader.Read("multipolygon-3DM.wkb");

            this.TestParseMultiPolygon(wkb, wkt);
        }

        #endregion

        #region Parse<GeometryCollection>(WKB) tests

        [Fact]
        public void ParseGeometryCollection_ParsesEmptyGeometryCollection() {
            string wkt = "geometrycollection empty";
            GeometryCollection<Geometry> expected = (GeometryCollection<Geometry>)this.ParseWKT(wkt);
            GeometryCollection<Geometry> parsed = WkbReader.Parse<GeometryCollection<Geometry>>(TestDataReader.Read("collection-empty.wkb"));

            Assert.Empty(parsed.Geometries);
        }

        [Fact]
        public void ParseGeometryCollection_Parses2DGeometryCollection() {
            string wkt = "geometrycollection (point (-10.1 15.5))";
            GeometryCollection<Geometry> expected = (GeometryCollection<Geometry>)this.ParseWKT(wkt);
            GeometryCollection<Geometry> parsed = WkbReader.Parse<GeometryCollection<Geometry>>(TestDataReader.Read("collection-2D.wkb"));

            Assert.Equal(expected.Geometries.Count, parsed.Geometries.Count);
            this.ComparePoints((Point)parsed.Geometries[0], (Point)expected.Geometries[0]);
        }

        [Fact]
        public void ParseGeometryCollection_Parses2DMeasuredGeometryCollection() {
            string wkt = "geometrycollection m (point m (-10.1 15.5 1000.5))";
            GeometryCollection<Geometry> expected = (GeometryCollection<Geometry>)this.ParseWKT(wkt);
            GeometryCollection<Geometry> parsed = WkbReader.Parse<GeometryCollection<Geometry>>(TestDataReader.Read("collection-2DM.wkb"));

            Assert.Equal(expected.Geometries.Count, parsed.Geometries.Count);
            this.ComparePoints((Point)parsed.Geometries[0], (Point)expected.Geometries[0]);
        }

        [Fact]
        public void ParseGeometryCollection_Parses3DGeometryCollection() {
            string wkt = "geometrycollection z (point z (-10.1 15.5 100.5))";
            GeometryCollection<Geometry> expected = (GeometryCollection<Geometry>)this.ParseWKT(wkt);
            GeometryCollection<Geometry> parsed = WkbReader.Parse<GeometryCollection<Geometry>>(TestDataReader.Read("collection-3D.wkb"));

            Assert.Equal(expected.Geometries.Count, parsed.Geometries.Count);
            this.ComparePoints((Point)parsed.Geometries[0], (Point)expected.Geometries[0]);
        }

        [Fact]
        public void ParseGeometryCollection_Parses3DMeasuredGeometryCollection() {
            string wkt = "geometrycollection zm (point zm (-10.1 15.5 100.5 1000.5))";
            GeometryCollection<Geometry> expected = (GeometryCollection<Geometry>)this.ParseWKT(wkt);
            GeometryCollection<Geometry> parsed = WkbReader.Parse<GeometryCollection<Geometry>>(TestDataReader.Read("collection-3DM.wkb"));

            Assert.Equal(expected.Geometries.Count, parsed.Geometries.Count);
            this.ComparePoints((Point)parsed.Geometries[0], (Point)expected.Geometries[0]);
        }

        [Fact]
        public void ParseGeometryCollection_ParsesCollectionWithPointLineStringAndPolygon() {
            string wkt = "geometrycollection (point (-10.1 15.5),linestring (-10.1 15.5, 20.2 -25.5, 30.3 35.5),polygon ((-10.1 15.5, 20.2 -25.5, 30.3 35.5)))";
            GeometryCollection<Geometry> expected = (GeometryCollection<Geometry>)this.ParseWKT(wkt);
            GeometryCollection<Geometry> parsed = WkbReader.Parse<GeometryCollection<Geometry>>(TestDataReader.Read("collection-pt-ls-poly.wkb"));

            Assert.Equal(expected.Geometries.Count, parsed.Geometries.Count);
            this.ComparePoints((Point)parsed.Geometries[0], (Point)expected.Geometries[0]);
            this.CompareLineStrings((LineString)parsed.Geometries[1], (LineString)expected.Geometries[1]);
            this.ComparePolygons((Polygon)parsed.Geometries[2], (Polygon)expected.Geometries[2]);
        }

        [Fact]
        public void ParseGeometryCollection_ParsesCollectionWithMultiGeometries() {
            string wkt = "geometrycollection (multipoint empty,multilinestring empty,multipolygon empty)";
            GeometryCollection<Geometry> expected = (GeometryCollection<Geometry>)this.ParseWKT(wkt);
            GeometryCollection<Geometry> parsed = WkbReader.Parse<GeometryCollection<Geometry>>(TestDataReader.Read("collection-multi.wkb"));

            Assert.Equal(expected.Geometries.Count, parsed.Geometries.Count);
            this.CompareMultiPoints((MultiPoint)parsed.Geometries[0], (MultiPoint)expected.Geometries[0]);
            this.CompareMultiLineStrings((MultiLineString)parsed.Geometries[1], (MultiLineString)expected.Geometries[1]);
            this.CompareMultiPolygons((MultiPolygon)parsed.Geometries[2], (MultiPolygon)expected.Geometries[2]);
        }

        [Fact]
        public void ParseGeometryCollection_ParsesNestedCollection() {
            string wkt = "geometrycollection (geometrycollection (point (-10.1 15.5)))";
            GeometryCollection<Geometry> expected = (GeometryCollection<Geometry>)this.ParseWKT(wkt);
            GeometryCollection<Geometry> parsed = WkbReader.Parse<GeometryCollection<Geometry>>(TestDataReader.Read("collection-nested.wkb"));

            Assert.Equal(expected.Geometries.Count, parsed.Geometries.Count);
            Assert.Equal(((GeometryCollection<Geometry>)expected.Geometries[0]).Geometries.Count, ((GeometryCollection<Geometry>)parsed.Geometries[0]).Geometries.Count);
            this.ComparePoints((Point)((GeometryCollection<Geometry>)parsed.Geometries[0]).Geometries[0], (Point)((GeometryCollection<Geometry>)expected.Geometries[0]).Geometries[0]);
        }

        #endregion


        #region TestParse*(WKB, ExpectedAsWKT)

        private void TestParsePoint(byte[] wkb, string expectedAsWkt) {
			Point expected = (Point)this.ParseWKT(expectedAsWkt);
			Point parsed = WkbReader.Parse<Point>(wkb);

			this.ComparePoints(parsed, expected);
		}

		private void TestParseLineString(byte[] wkb, string expectedAsWkt) {
			LineString expected = (LineString)this.ParseWKT(expectedAsWkt);
			LineString parsed = WkbReader.Parse<LineString>(wkb);

			this.CompareLineStrings(parsed, expected);
		}

		private void TestParsePolygon(byte[] wkb, string expectedAsWkt) {
			Polygon expected = (Polygon)this.ParseWKT(expectedAsWkt);
			Polygon parsed = WkbReader.Parse<Polygon>(wkb);

			this.ComparePolygons(parsed, expected);
		}

		private void TestParseMultiPoint(byte[] wkb, string expectedAsWkt) {
			MultiPoint expected = (MultiPoint)this.ParseWKT(expectedAsWkt);
			MultiPoint parsed = WkbReader.Parse<MultiPoint>(wkb);

			this.CompareMultiPoints(parsed, expected);
		}

		private void TestParseMultiLineString(byte[] wkb, string expectedAsWkt) {
			MultiLineString expected = (MultiLineString)this.ParseWKT(expectedAsWkt);
			MultiLineString parsed = WkbReader.Parse<MultiLineString>(wkb);

			this.CompareMultiLineStrings(parsed, expected);
		}

		private void TestParseMultiPolygon(byte[] wkb, string expectedAsWkt) {
			MultiPolygon expected = (MultiPolygon)this.ParseWKT(expectedAsWkt);
			MultiPolygon parsed = WkbReader.Parse<MultiPolygon>(wkb);

			this.CompareMultiPolygons(parsed, expected);
		}

		#endregion

		#region Compare*(Actual, Expected)

		private void ComparePoints(Point point, Point expected) {
			Assert.Equal(expected.Position, point.Position);
		}

		private void CompareCoordinateLists(ICoordinateList list, ICoordinateList expected) {
			Assert.Equal(expected.Count, list.Count);
			for (int i = 0; i < expected.Count; i++) {
				Assert.Equal(expected[i], list[i]);
			}
		}

		private void CompareLineStrings(LineString linestring, LineString expected) {
			this.CompareCoordinateLists(linestring.Coordinates, expected.Coordinates);
		}

		private void ComparePolygons(Polygon polygon, Polygon expected) {
			this.CompareCoordinateLists(polygon.ExteriorRing, expected.ExteriorRing);

			Assert.Equal(expected.InteriorRings.Count, polygon.InteriorRings.Count);
			for (int i = 0; i < expected.InteriorRings.Count; i++) {
				this.CompareCoordinateLists(polygon.InteriorRings[i], expected.InteriorRings[i]);
			}
		}

		private void CompareMultiPoints(MultiPoint multipoint, MultiPoint expected) {
			Assert.Equal(expected.Geometries.Count, multipoint.Geometries.Count);

			for (int i = 0; i < expected.Geometries.Count; i++) {
				this.ComparePoints(multipoint.Geometries[i], expected.Geometries[i]);
			}
		}

		private void CompareMultiLineStrings(MultiLineString multilinestring, MultiLineString expected) {
			Assert.Equal(expected.Geometries.Count, multilinestring.Geometries.Count);

			for (int i = 0; i < expected.Geometries.Count; i++) {
				this.CompareLineStrings(multilinestring.Geometries[i], expected.Geometries[i]);
			}
		}

		private void CompareMultiPolygons(MultiPolygon multipolygon, MultiPolygon expected) {
			Assert.Equal(expected.Geometries.Count, multipolygon.Geometries.Count);

			for (int i = 0; i < expected.Geometries.Count; i++) {
				this.ComparePolygons(multipolygon.Geometries[i], expected.Geometries[i]);
			}
		}

		#endregion

		private Geometry ParseWKT(string wkt) {
			return WktReader.Parse(wkt);
		}
	}
}
