using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Xunit;
using Xunit.Extensions;

using Tests.SpatialLite.Core.Data;

using SpatialLite.Core.API;
using SpatialLite.Core.Geometries;
using SpatialLite.Core.IO;


namespace Tests.SpatialLite.Core.IO {
	public class WkbWriterTests {

        [Fact]
        public void Construcotor_StreamSettings_SetsSettingsAndMakeThemReadOnly() {
            WkbWriterSettings settings = new WkbWriterSettings();
            using (WkbWriter target = new WkbWriter(new MemoryStream(), settings)) {
                Assert.Same(settings, target.Settings);
                Assert.True(settings.IsReadOnly);
            }
        }

        [Fact]
        public void Constructor_StreamSettings_ThrowsArgumentNullExceptionIfStreamIsNull() {
            Stream stream = null;
            Assert.Throws<ArgumentNullException>(() => new WkbWriter(stream, new WkbWriterSettings()));
        }

        [Fact]
        public void Constructor_StreamSettings_ThrowsArgumentNullExceptionIfSettingsIsNull() {
            Assert.Throws<ArgumentNullException>(() => new WkbWriter(new MemoryStream(), null));
        }

        [Fact]
        public void Construcotor_PathSettings_SetsSettingsAndMakeThemReadOnly() {
            WkbWriterSettings settings = new WkbWriterSettings();
            using (WkbWriter target = new WkbWriter(new MemoryStream(), settings)) {
                Assert.Same(settings, target.Settings);
                Assert.True(settings.IsReadOnly);
            }
        }

        [Fact]
        public void Constructor_PathSettings_CreatesOutputFile() {
            string filename = PathHelper.GetTempFilePath("wkbwriter-constructor-creates-output-test.bin");

            WkbWriterSettings settings = new WkbWriterSettings();
            using (WkbWriter target = new WkbWriter(filename, settings)) {
                ;
            }

            Assert.True(File.Exists(filename));
        }

        [Fact]
        public void Constructor_PathSettings_ThrowsArgumentNullExceptionIfStreamIsNull() {
            string path = null;
            Assert.Throws<ArgumentNullException>(() => new WkbWriter(path, new WkbWriterSettings()));
        }

        [Fact]
        public void Constructor_PathSettings_ThrowsArgumentNullExceptionIfSettingsIsNull() {
            string path = PathHelper.GetTempFilePath("wkbwriter-constructor-test.bin");

            Assert.Throws<ArgumentNullException>(() => new WkbWriter(path, null));
        }

        [Fact]
        public void Constructor_ThrowsExceptionIfEncodingIsSetToBingEndian() {
            MemoryStream stream = new MemoryStream();
            Assert.Throws<NotSupportedException>(() => new WkbWriter(stream, new WkbWriterSettings() { Encoding = BinaryEncoding.BigEndian }));
        }

        [Fact]
        public void Dispose_ClosesOutputStreamIfWritingToFiles() {
            string filename = PathHelper.GetTempFilePath("wkbwriter-closes-output-filestream-test.bin");

            WkbWriterSettings settings = new WkbWriterSettings();
            WkbWriter target = new WkbWriter(filename, settings);
            target.Dispose();

            FileStream testStream = null;
            testStream = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite);
            testStream.Dispose();
        }

        [Fact]
        public void Dispose_ClosesOutputStreamIfWritingToStream() {
            MemoryStream stream = new MemoryStream();

            WkbWriter target = new WkbWriter(stream, new WkbWriterSettings());
            target.Dispose();

            Assert.False(stream.CanRead);
        }

        [Fact]
        public void WkbWriter_Write_WritesLittleEndianEncodingByte() {
            MemoryStream stream = new MemoryStream();
            using (WkbWriter target = new WkbWriter(stream, new WkbWriterSettings() { Encoding = BinaryEncoding.LittleEndian })) {
                target.Write(new Point());

                CompareBytes(stream.ToArray(), 0, new byte[] { (byte)BinaryEncoding.LittleEndian });
            }
        }

        [Fact]
        public void Write_WritesEmptyPointAsEmptyGeoemtryCollection() {
            string wkt = "point empty";
            Point point = (Point)this.ParseWKT(wkt);

            MemoryStream stream = new MemoryStream();
            using (WkbWriter target = new WkbWriter(stream, new WkbWriterSettings())) {
                target.Write(point);

                this.CompareBytes(stream.ToArray(), TestDataReader.Read("collection-empty.wkb"));
            }
        }

        [Fact]
        public void Write_Writes2DPoint() {
            string wkt = "point (-10.1 15.5)";
            Point point = (Point)this.ParseWKT(wkt);

            MemoryStream stream = new MemoryStream();
            using (WkbWriter target = new WkbWriter(stream, new WkbWriterSettings())) {
                target.Write(point);

                this.CompareBytes(stream.ToArray(), TestDataReader.Read("point-2D.wkb"));
            }
        }

        [Fact]
        public void Write_Writes2DMeasuredPoint() {
            string wkt = "point m (-10.1 15.5 1000.5)";
            Point point = (Point)this.ParseWKT(wkt);

            MemoryStream stream = new MemoryStream();
            using (WkbWriter target = new WkbWriter(stream, new WkbWriterSettings())) {
                target.Write(point);

                this.CompareBytes(stream.ToArray(), TestDataReader.Read("point-2DM.wkb"));
            }
        }

        [Fact]
        public void Write_Writes3DPoint() {
            string wkt = "point z (-10.1 15.5 100.5)";
            Point point = (Point)this.ParseWKT(wkt);

            MemoryStream stream = new MemoryStream();
            using (WkbWriter target = new WkbWriter(stream, new WkbWriterSettings())) {
                target.Write(point);

                this.CompareBytes(stream.ToArray(), TestDataReader.Read("point-3D.wkb"));
            }
        }

        [Fact]
        public void Write_Writes3DMeasuredPoint() {
            string wkt = "point zm (-10.1 15.5 100.5 1000.5)";
            Point point = (Point)this.ParseWKT(wkt);

            MemoryStream stream = new MemoryStream();
            using (WkbWriter target = new WkbWriter(stream, new WkbWriterSettings())) {
                target.Write(point);

                this.CompareBytes(stream.ToArray(), TestDataReader.Read("point-3DM.wkb"));
            }
        }

        [Fact]
        public void Write_WritesEmptyLineString() {
            string wkt = "linestring empty";
            LineString linestring = (LineString)this.ParseWKT(wkt);

            MemoryStream stream = new MemoryStream();
            using (WkbWriter target = new WkbWriter(stream, new WkbWriterSettings())) {
                target.Write(linestring);

                this.CompareBytes(stream.ToArray(), TestDataReader.Read("linestring-empty.wkb"));
            }
        }

        [Fact]
        public void Write_Writes2DLineString() {
            string wkt = "linestring (-10.1 15.5, 20.2 -25.5, 30.3 35.5)";
            LineString linestring = (LineString)this.ParseWKT(wkt);

            MemoryStream stream = new MemoryStream();
            using (WkbWriter target = new WkbWriter(stream, new WkbWriterSettings())) {
                target.Write(linestring);

                this.CompareBytes(stream.ToArray(), TestDataReader.Read("linestring-2D.wkb"));
            }
        }

        [Fact]
        public void Write_Writes2DMeasuredLineString() {
            string wkt = "linestring m (-10.1 15.5 1000.5, 20.2 -25.5 2000.5, 30.3 35.5 -3000.5)";
            LineString linestring = (LineString)this.ParseWKT(wkt);

            MemoryStream stream = new MemoryStream();
            using (WkbWriter target = new WkbWriter(stream, new WkbWriterSettings())) {
                target.Write(linestring);

                this.CompareBytes(stream.ToArray(), TestDataReader.Read("linestring-2DM.wkb"));
            }
        }

        [Fact]
        public void Write_Writes3DLineString() {
            string wkt = "linestring z (-10.1 15.5 100.5, 20.2 -25.5 200.5, 30.3 35.5 -300.5)";
            LineString linestring = (LineString)this.ParseWKT(wkt);

            MemoryStream stream = new MemoryStream();
            using (WkbWriter target = new WkbWriter(stream, new WkbWriterSettings())) {
                target.Write(linestring);

                this.CompareBytes(stream.ToArray(), TestDataReader.Read("linestring-3D.wkb"));
            }
        }

        [Fact]
        public void Write_Writes3DMeasuredLineString() {
            string wkt = "linestring zm (-10.1 15.5 100.5 1000.5, 20.2 -25.5 200.5 2000.5, 30.3 35.5 -300.5 -3000.5)";
            LineString linestring = (LineString)this.ParseWKT(wkt);

            MemoryStream stream = new MemoryStream(); using (WkbWriter target = new WkbWriter(stream, new WkbWriterSettings())) {
                target.Write(linestring);

                this.CompareBytes(stream.ToArray(), TestDataReader.Read("linestring-3DM.wkb"));
            }
        }

        [Fact]
        public void Write_WritesEmptyPolygon() {
            string wkt = "polygon empty";
            Polygon polygon = (Polygon)this.ParseWKT(wkt);

            MemoryStream stream = new MemoryStream();
            using (WkbWriter target = new WkbWriter(stream, new WkbWriterSettings())) {
                target.Write(polygon);

                this.CompareBytes(stream.ToArray(), TestDataReader.Read("polygon-empty.wkb"));
            }
        }

        [Fact]
        public void Write_Writes2DPolygonOnlyExteriorRing() {
            string wkt = "polygon ((-10.1 15.5, 20.2 -25.5, 30.3 35.5))";
            Polygon polygon = (Polygon)this.ParseWKT(wkt);

            MemoryStream stream = new MemoryStream();
            using (WkbWriter target = new WkbWriter(stream, new WkbWriterSettings())) {
                target.Write(polygon);

                this.CompareBytes(stream.ToArray(), TestDataReader.Read("polygon-ext-2D.wkb"));
            }
        }

        [Fact]
        public void Write_Writes2DMeasuredPolygonOnlyExteriorRing() {
            string wkt = "polygon m ((-10.1 15.5 1000.5, 20.2 -25.5 2000.5, 30.3 35.5 -3000.5))";
            Polygon polygon = (Polygon)this.ParseWKT(wkt);

            MemoryStream stream = new MemoryStream();
            using (WkbWriter target = new WkbWriter(stream, new WkbWriterSettings())) {
                target.Write(polygon);

                this.CompareBytes(stream.ToArray(), TestDataReader.Read("polygon-ext-2DM.wkb"));
            }
        }

        [Fact]
        public void Write_Writes3DPolygonOnlyExteriorRing() {
            string wkt = "polygon z ((-10.1 15.5 100.5, 20.2 -25.5 200.5, 30.3 35.5 -300.5))";
            Polygon polygon = (Polygon)this.ParseWKT(wkt);

            MemoryStream stream = new MemoryStream();
            using (WkbWriter target = new WkbWriter(stream, new WkbWriterSettings())) {
                target.Write(polygon);

                this.CompareBytes(stream.ToArray(), TestDataReader.Read("polygon-ext-3D.wkb"));
            }
        }

        [Fact]
        public void Write_Writes3DMeasuredPolygonOnlyExteriorRing() {
            string wkt = "polygon zm ((-10.1 15.5 100.5 1000.5, 20.2 -25.5 200.5 2000.5, 30.3 35.5 -300.5 -3000.5))";
            Polygon polygon = (Polygon)this.ParseWKT(wkt);

            MemoryStream stream = new MemoryStream();
            using (WkbWriter target = new WkbWriter(stream, new WkbWriterSettings())) {
                target.Write(polygon);

                this.CompareBytes(stream.ToArray(), TestDataReader.Read("polygon-ext-3DM.wkb"));
            }
        }

        [Fact]
        public void Write_Writes3DMeasuredPolygon() {
            string wkt = "polygon zm ((-10.1 15.5 100.5 1000.5, 20.2 -25.5 200.5 2000.5, 30.3 35.5 -300.5 -3000.5),(-1.1 1.5 10.5 100.5, 2.2 -2.5 20.5 200.5, 3.3 3.5 -30.5 -300.5),(-1.1 1.5 10.5 100.5, 2.2 -2.5 20.5 200.5, 3.3 3.5 -30.5 -300.5))";
            Polygon polygon = (Polygon)this.ParseWKT(wkt);

            MemoryStream stream = new MemoryStream();
            using (WkbWriter target = new WkbWriter(stream, new WkbWriterSettings())) {
                target.Write(polygon);

                this.CompareBytes(stream.ToArray(), TestDataReader.Read("polygon-3DM.wkb"));
            }
        }

        [Fact]
        public void Write_WritesEmptyMultipoint() {
            string wkt = "multipoint empty";
            MultiPoint multipoint = (MultiPoint)this.ParseWKT(wkt);

            MemoryStream stream = new MemoryStream();
            using (WkbWriter target = new WkbWriter(stream, new WkbWriterSettings())) {
                target.Write(multipoint);

                this.CompareBytes(stream.ToArray(), TestDataReader.Read("multipoint-empty.wkb"));
            }
        }

        [Fact]
        public void Write_Writes2DMultiPoint() {
            string wkt = "multipoint ((-10.1 15.5),(20.2 -25.5))";
            MultiPoint multipoint = (MultiPoint)this.ParseWKT(wkt);

            MemoryStream stream = new MemoryStream();
            using (WkbWriter target = new WkbWriter(stream, new WkbWriterSettings())) {
                target.Write(multipoint);

                this.CompareBytes(stream.ToArray(), TestDataReader.Read("multipoint-2D.wkb"));
            }
        }

        [Fact]
        public void Write_Writes2DMeasuredMultiPoint() {
            string wkt = "multipoint m ((-10.1 15.5 1000.5),(20.2 -25.5 2000.5))";
            MultiPoint multipoint = (MultiPoint)this.ParseWKT(wkt);

            MemoryStream stream = new MemoryStream();
            using (WkbWriter target = new WkbWriter(stream, new WkbWriterSettings())) {
                target.Write(multipoint);

                this.CompareBytes(stream.ToArray(), TestDataReader.Read("multipoint-2DM.wkb"));
            }
        }

        [Fact]
        public void Write_Writes3DMultiPoint() {
            string wkt = "multipoint z ((-10.1 15.5 100.5),(20.2 -25.5 200.5))";
            MultiPoint multipoint = (MultiPoint)this.ParseWKT(wkt);

            MemoryStream stream = new MemoryStream();
            using (WkbWriter target = new WkbWriter(stream, new WkbWriterSettings())) {
                target.Write(multipoint);

                this.CompareBytes(stream.ToArray(), TestDataReader.Read("multipoint-3D.wkb"));
            }
        }

        [Fact]
        public void Write_Writes3DMeasuredMultiPoint() {
            string wkt = "multipoint zm ((-10.1 15.5 100.5 1000.5),(20.2 -25.5 200.5 2000.5))";
            MultiPoint multipoint = (MultiPoint)this.ParseWKT(wkt);

            MemoryStream stream = new MemoryStream();
            using (WkbWriter target = new WkbWriter(stream, new WkbWriterSettings())) {
                target.Write(multipoint);

                this.CompareBytes(stream.ToArray(), TestDataReader.Read("multipoint-3DM.wkb"));
            }
        }

        [Fact]
        public void Write_WritesEmptyMultiLineString() {
            string wkt = "multilinestring empty";
            MultiLineString multilinestring = (MultiLineString)this.ParseWKT(wkt);

            MemoryStream stream = new MemoryStream();
            using (WkbWriter target = new WkbWriter(stream, new WkbWriterSettings())) {
                target.Write(multilinestring);

                this.CompareBytes(stream.ToArray(), TestDataReader.Read("multilinestring-empty.wkb"));
            }
        }

        [Fact]
        public void Write_Writes2DMultiLineString() {
            string wkt = "multilinestring ((-10.1 15.5, 20.2 -25.5, 30.3 35.5),(-10.1 15.5, 20.2 -25.5, 30.3 35.5))";
            MultiLineString multilinestring = (MultiLineString)this.ParseWKT(wkt);

            MemoryStream stream = new MemoryStream();
            using (WkbWriter target = new WkbWriter(stream, new WkbWriterSettings())) {
                target.Write(multilinestring);

                this.CompareBytes(stream.ToArray(), TestDataReader.Read("multilinestring-2D.wkb"));
            }
        }

        [Fact]
        public void Write_Writes2DMeasuredMultiLineString() {
            string wkt = "multilinestring m ((-10.1 15.5 1000.5, 20.2 -25.5 2000.5, 30.3 35.5 -3000.5),(-10.1 15.5 1000.5, 20.2 -25.5 2000.5, 30.3 35.5 -3000.5))";
            MultiLineString multilinestring = (MultiLineString)this.ParseWKT(wkt);

            MemoryStream stream = new MemoryStream();
            using (WkbWriter target = new WkbWriter(stream, new WkbWriterSettings())) {
                target.Write(multilinestring);

                this.CompareBytes(stream.ToArray(), TestDataReader.Read("multilinestring-2DM.wkb"));
            }
        }

        [Fact]
        public void Write_Writes3DMultiLineString() {
            string wkt = "multilinestring z ((-10.1 15.5 100.5, 20.2 -25.5 200.5, 30.3 35.5 -300.5),(-10.1 15.5 100.5, 20.2 -25.5 200.5, 30.3 35.5 -300.5))";
            MultiLineString multilinestring = (MultiLineString)this.ParseWKT(wkt);

            MemoryStream stream = new MemoryStream();
            using (WkbWriter target = new WkbWriter(stream, new WkbWriterSettings())) {
                target.Write(multilinestring);

                this.CompareBytes(stream.ToArray(), TestDataReader.Read("multilinestring-3D.wkb"));
            }
        }

        [Fact]
        public void Write_Writes3DMeasuredMultiLineString() {
            string wkt = "multilinestring zm ((-10.1 15.5 100.5 1000.5, 20.2 -25.5 200.5 2000.5, 30.3 35.5 -300.5 -3000.5),(-10.1 15.5 100.5 1000.5, 20.2 -25.5 200.5 2000.5, 30.3 35.5 -300.5 -3000.5))";
            MultiLineString multilinestring = (MultiLineString)this.ParseWKT(wkt);

            MemoryStream stream = new MemoryStream();
            using (WkbWriter target = new WkbWriter(stream, new WkbWriterSettings())) {
                target.Write(multilinestring);

                this.CompareBytes(stream.ToArray(), TestDataReader.Read("multilinestring-3DM.wkb"));
            }
        }

        [Fact]
        public void Write_WritesEmptyMultiPolygon() {
            string wkt = "multipolygon empty";
            MultiPolygon multipolygon = (MultiPolygon)this.ParseWKT(wkt);

            MemoryStream stream = new MemoryStream();
            using (WkbWriter target = new WkbWriter(stream, new WkbWriterSettings())) {
                target.Write(multipolygon);

                this.CompareBytes(stream.ToArray(), TestDataReader.Read("multipolygon-empty.wkb"));
            }
        }

        [Fact]
        public void Write_Writes2DMultiPolygon() {
            string wkt = "multipolygon (((-10.1 15.5, 20.2 -25.5, 30.3 35.5)),((-10.1 15.5, 20.2 -25.5, 30.3 35.5)))";
            MultiPolygon multipolygon = (MultiPolygon)this.ParseWKT(wkt);

            MemoryStream stream = new MemoryStream();
            using (WkbWriter target = new WkbWriter(stream, new WkbWriterSettings())) {
                target.Write(multipolygon);

                this.CompareBytes(stream.ToArray(), TestDataReader.Read("multipolygon-2D.wkb"));
            }
        }

        [Fact]
        public void Write_Writes2DMeasuredMultiPolygon() {
            string wkt = "multipolygon m (((-10.1 15.5 1000.5, 20.2 -25.5 2000.5, 30.3 35.5 -3000.5)),((-10.1 15.5 1000.5, 20.2 -25.5 2000.5, 30.3 35.5 -3000.5)))";
            MultiPolygon multipolygon = (MultiPolygon)this.ParseWKT(wkt);

            MemoryStream stream = new MemoryStream();
            using (WkbWriter target = new WkbWriter(stream, new WkbWriterSettings())) {
                target.Write(multipolygon);

                this.CompareBytes(stream.ToArray(), TestDataReader.Read("multipolygon-2DM.wkb"));
            }
        }

        [Fact]
        public void Write_Writes3DMultiPolygon() {
            string wkt = "multipolygon z (((-10.1 15.5 100.5, 20.2 -25.5 200.5, 30.3 35.5 -300.5)),((-10.1 15.5 100.5, 20.2 -25.5 200.5, 30.3 35.5 -300.5)))";
            MultiPolygon multipolygon = (MultiPolygon)this.ParseWKT(wkt);

            MemoryStream stream = new MemoryStream();
            using (WkbWriter target = new WkbWriter(stream, new WkbWriterSettings())) {
                target.Write(multipolygon);

                this.CompareBytes(stream.ToArray(), TestDataReader.Read("multipolygon-3D.wkb"));
            }
        }

        [Fact]
        public void Write_Writes3DMeasuredMultiPolygon() {
            string wkt = "multipolygon zm (((-10.1 15.5 100.5 1000.5, 20.2 -25.5 200.5 2000.5, 30.3 35.5 -300.5 -3000.5)),((-10.1 15.5 100.5 1000.5, 20.2 -25.5 200.5 2000.5, 30.3 35.5 -300.5 -3000.5)))";
            MultiPolygon multipolygon = (MultiPolygon)this.ParseWKT(wkt);

            MemoryStream stream = new MemoryStream();
            using (WkbWriter target = new WkbWriter(stream, new WkbWriterSettings())) {
                target.Write(multipolygon);

                this.CompareBytes(stream.ToArray(), TestDataReader.Read("multipolygon-3DM.wkb"));
            }
        }

        [Fact]
        public void Write_WritesEmptyGeometryCollection() {
            string wkt = "geometrycollection empty";
            GeometryCollection<Geometry> collection = (GeometryCollection<Geometry>)this.ParseWKT(wkt);

            MemoryStream stream = new MemoryStream();
            using (WkbWriter target = new WkbWriter(stream, new WkbWriterSettings())) {
                target.Write(collection);

                this.CompareBytes(stream.ToArray(), TestDataReader.Read("collection-empty.wkb"));
            }
        }

        [Fact]
        public void Write_Writes2DGeometryCollection() {
            string wkt = "geometrycollection (point (-10.1 15.5))";
            GeometryCollection<Geometry> collection = (GeometryCollection<Geometry>)this.ParseWKT(wkt);

            MemoryStream stream = new MemoryStream();
            using (WkbWriter target = new WkbWriter(stream, new WkbWriterSettings())) {
                target.Write(collection);

                this.CompareBytes(stream.ToArray(), TestDataReader.Read("collection-2D.wkb"));
            }
        }

        [Fact]
        public void Write_Writes2DMeasuredGeometryCollection() {
            string wkt = "geometrycollection m (point m (-10.1 15.5 1000.5))";
            GeometryCollection<Geometry> collection = (GeometryCollection<Geometry>)this.ParseWKT(wkt);

            MemoryStream stream = new MemoryStream();
            using (WkbWriter target = new WkbWriter(stream, new WkbWriterSettings())) {
                target.Write(collection);

                this.CompareBytes(stream.ToArray(), TestDataReader.Read("collection-2DM.wkb"));
            }
        }

        [Fact]
        public void Write_Writes3DGeometryCollection() {
            string wkt = "geometrycollection z (point z (-10.1 15.5 100.5))";
            GeometryCollection<Geometry> collection = (GeometryCollection<Geometry>)this.ParseWKT(wkt);

            MemoryStream stream = new MemoryStream();
            using (WkbWriter target = new WkbWriter(stream, new WkbWriterSettings())) {
                target.Write(collection);

                this.CompareBytes(stream.ToArray(), TestDataReader.Read("collection-3D.wkb"));
            }
        }

        [Fact]
        public void Write_Writes3DMeasuredGeometryCollection() {
            string wkt = "geometrycollection zm (point zm (-10.1 15.5 100.5 1000.5))";
            GeometryCollection<Geometry> collection = (GeometryCollection<Geometry>)this.ParseWKT(wkt);

            MemoryStream stream = new MemoryStream();
            using (WkbWriter target = new WkbWriter(stream, new WkbWriterSettings())) {
                target.Write(collection);

                this.CompareBytes(stream.ToArray(), TestDataReader.Read("collection-3DM.wkb"));
            }
        }

        [Fact]
        public void Write_WritesCollectionWithPointLineStringAndPolygon() {
            string wkt = "geometrycollection (point (-10.1 15.5),linestring (-10.1 15.5, 20.2 -25.5, 30.3 35.5),polygon ((-10.1 15.5, 20.2 -25.5, 30.3 35.5)))";
            GeometryCollection<Geometry> collection = (GeometryCollection<Geometry>)this.ParseWKT(wkt);

            MemoryStream stream = new MemoryStream();
            using (WkbWriter target = new WkbWriter(stream, new WkbWriterSettings())) {
                target.Write(collection);

                this.CompareBytes(stream.ToArray(), TestDataReader.Read("collection-pt-ls-poly.wkb"));
            }
        }

        [Fact]
        public void Write_WritesCollectionWithMultiGeometries() {
            string wkt = "geometrycollection (multipoint empty,multilinestring empty,multipolygon empty)";
            GeometryCollection<Geometry> collection = (GeometryCollection<Geometry>)this.ParseWKT(wkt);

            MemoryStream stream = new MemoryStream();
            using (WkbWriter target = new WkbWriter(stream, new WkbWriterSettings())) {
                target.Write(collection);

                this.CompareBytes(stream.ToArray(), TestDataReader.Read("collection-multi.wkb"));
            }
        }

        [Fact]
        public void Write_WritesNestedCollection() {
            string wkt = "geometrycollection (geometrycollection (point (-10.1 15.5)))";
            GeometryCollection<Geometry> collection = (GeometryCollection<Geometry>)this.ParseWKT(wkt);

            MemoryStream stream = new MemoryStream();
            using (WkbWriter target = new WkbWriter(stream, new WkbWriterSettings())) {
                target.Write(collection);

                this.CompareBytes(stream.ToArray(), TestDataReader.Read("collection-nested.wkb"));
            }
        }

        public static IEnumerable<object[]> WriteToArrayTestData {
            get {
                yield return new object[] { "point zm (-10.1 15.5 100.5 1000.5)", TestDataReader.Read("point-3DM.wkb") };
                yield return new object[] { "linestring zm (-10.1 15.5 100.5 1000.5, 20.2 -25.5 200.5 2000.5, 30.3 35.5 -300.5 -3000.5)", TestDataReader.Read("linestring-3DM.wkb") };
                yield return new object[] { "polygon zm ((-10.1 15.5 100.5 1000.5, 20.2 -25.5 200.5 2000.5, 30.3 35.5 -300.5 -3000.5),(-1.1 1.5 10.5 100.5, 2.2 -2.5 20.5 200.5, 3.3 3.5 -30.5 -300.5),(-1.1 1.5 10.5 100.5, 2.2 -2.5 20.5 200.5, 3.3 3.5 -30.5 -300.5))", TestDataReader.Read("polygon-3DM.wkb") };
                yield return new object[] { "multipoint zm ((-10.1 15.5 100.5 1000.5),(20.2 -25.5 200.5 2000.5))", TestDataReader.Read("multipoint-3DM.wkb") };
                yield return new object[] { "multilinestring zm ((-10.1 15.5 100.5 1000.5, 20.2 -25.5 200.5 2000.5, 30.3 35.5 -300.5 -3000.5),(-10.1 15.5 100.5 1000.5, 20.2 -25.5 200.5 2000.5, 30.3 35.5 -300.5 -3000.5))", TestDataReader.Read("multilinestring-3DM.wkb") };
                yield return new object[] { "multipolygon zm (((-10.1 15.5 100.5 1000.5, 20.2 -25.5 200.5 2000.5, 30.3 35.5 -300.5 -3000.5)),((-10.1 15.5 100.5 1000.5, 20.2 -25.5 200.5 2000.5, 30.3 35.5 -300.5 -3000.5)))", TestDataReader.Read("multipolygon-3DM.wkb") };
                yield return new object[] { "geometrycollection zm (point zm (-10.1 15.5 100.5 1000.5))", TestDataReader.Read("collection-3DM.wkb") };

            }
        }

        [Theory]
        [MemberData(nameof(WriteToArrayTestData))]
        public void WriteToArray_WritesAllGeometryTypes(string wkt, byte[] expectedWkb) {
            IGeometry geometry = this.ParseWKT(wkt);

            byte[] wkb = WkbWriter.WriteToArray(geometry);
            this.CompareBytes(wkb, expectedWkb);
        }

        private void CompareBytes(byte[] array, byte[] expected) {
			Assert.Equal(array.Length, expected.Length);

			for (int i = 0; i < expected.Length; i++) {
				Assert.Equal(expected[i], array[i]);
			}
		}

		private void CompareBytes(byte[] array, int offset, byte[] expected) {
			for (int i = 0; i < expected.Length; i++) {
				Assert.Equal(expected[i], array[i + offset]);
			}
		}

		private Geometry ParseWKT(string wkt) {
			return WktReader.Parse(wkt);
		}
	}
}
