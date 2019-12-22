using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SpatialLite.Core.API;
using SpatialLite.Gps;
using SpatialLite.Gps.Geometries;
using SpatialLite.Gps.IO;
using Xunit;
using Tests.SpatialLite.Gps.Data;

namespace Tests.SpatialLite.Gps.IO {
    public class GpxReaderTests {

        [Fact]
        public void Constructor_StringSettings_ThrowsExceptionIfFileDoesnotExist() {
            Assert.Throws<FileNotFoundException>(delegate { new GpxReader("non-existing-file.gpx", new GpxReaderSettings() { ReadMetadata = false }); });
        }

        [Fact]
        public void Constructor_StringSettings_SetsSettingsAndMakesItReadOnly() {
            string path = "../../../Data/Gpx/gpx-real-file.gpx";
            var settings = new GpxReaderSettings() { ReadMetadata = false };
            using (var target = new GpxReader(path, settings)) {
                Assert.Same(settings, target.Settings);
                Assert.True(settings.IsReadOnly);
            }
        }

        [Fact]
        public void Constructor_StreamSettings_SetsSettingsAndMakesItReadOnly() {
            var settings = new GpxReaderSettings() { ReadMetadata = false };
            using (var target = new GpxReader(TestDataReader.Open("gpx-real-file.gpx"), settings)) {
                Assert.Same(settings, target.Settings);
                Assert.True(settings.IsReadOnly);
            }
        }

        [Fact]
        public void Constructor_StreamSettings_ThrowsExceptionIfVersionIsnt10or11() {
            Assert.Throws<InvalidDataException>(delegate { new GpxReader(TestDataReader.Open("gpx-version-2_0.gpx"), new GpxReaderSettings() { ReadMetadata = false }); });
        }

        [Fact]
        public void Constructor_StreamSettings_ThrowsExceptionXmlContainsInvalidRootElement() {
            Assert.Throws<InvalidDataException>(delegate { new GpxReader(TestDataReader.Open("gpx-invalid-root-element.gpx"), new GpxReaderSettings() { ReadMetadata = false }); });
        }

        [Fact]
        public void Read_ThrowsExceptionIfWaypointHasntLat() {
            GpxReader target = new GpxReader(TestDataReader.Open("gpx-waypoint-without-lat.gpx"), new GpxReaderSettings() { ReadMetadata = false });

            Assert.Throws<InvalidDataException>(() => target.Read());
        }

        [Fact]
        public void Read_ThrowsExceptionIfWaypointHasntLon() {
            GpxReader target = new GpxReader(TestDataReader.Open("gpx-waypoint-without-lon.gpx"), new GpxReaderSettings() { ReadMetadata = false });

            Assert.Throws<InvalidDataException>(() => target.Read());
        }

        [Fact]
        public void Read_SetsMetadataIfReadMetadataIsTrue() {
            var data = TestDataReader.Open("gpx-waypoint-simple.gpx");
            var expectedCoordinate = new Coordinate(-71.119277, 42.438878);

            GpxReader target = new GpxReader(data, new GpxReaderSettings() { ReadMetadata = true });
            var result = target.Read() as GpxPoint;

            Assert.NotNull(result.Metadata);
        }

        [Fact]
        public void Read_DoesntSetMetadataIfReadMetadataIsFalse() {
            var data = TestDataReader.Open("gpx-waypoint-with-metadata.gpx");
            var expectedCoordinate = new Coordinate(-71.119277, 42.438878);

            GpxReader target = new GpxReader(data, new GpxReaderSettings() { ReadMetadata = false });
            var result = target.Read() as GpxPoint;

            Assert.Null(result.Metadata);
        }

        [Fact]
        public void Read_ParsesWaypointWithLatLonElevationAndTime() {
            var data = TestDataReader.Open("gpx-waypoint-with-metadata.gpx");
            Coordinate expectedCoordinate = new Coordinate(-71.119277, 42.438878, 44.586548);
            DateTime expectedTime = new DateTime(2001, 11, 28, 21, 5, 28, DateTimeKind.Utc);

            GpxReader target = new GpxReader(data, new GpxReaderSettings() { ReadMetadata = false });
            var result = target.Read() as GpxPoint;

            Assert.Equal(result.Position, expectedCoordinate);
            Assert.Equal(result.Timestamp, expectedTime);
        }

        [Fact]
        public void Read_ParsesWaypointWithExtensions() {
            var data = TestDataReader.Open("gpx-waypoint-extensions.gpx");
            Coordinate expectedCoordinate = new Coordinate(-71.119277, 42.438878, 44.586548);
            DateTime expectedTime = new DateTime(2001, 11, 28, 21, 5, 28, DateTimeKind.Utc);

            GpxReader target = new GpxReader(data, new GpxReaderSettings() { ReadMetadata = false });
            var result = target.Read() as GpxPoint;

            Assert.Equal(result.Position, expectedCoordinate);
            Assert.Equal(result.Timestamp, expectedTime);
        }

        [Fact]
        public void Read_ParsesMultipleWaypoints() {
            var data = TestDataReader.Open("gpx-waypoint-multiple.gpx");

            GpxReader target = new GpxReader(data, new GpxReaderSettings() { ReadMetadata = false });
            GpxPoint result = null;

            int count = 0;
            while ((result = target.Read() as GpxPoint) != null) {
                count++;
            }

            Assert.Equal(3, count);
        }

        [Fact]
        public void Read_ReadsWaypointMetadata() {
            var data = TestDataReader.Open("gpx-waypoint-with-metadata.gpx");

            GpxReader target = new GpxReader(data, new GpxReaderSettings() { ReadMetadata = true });
            var result = target.Read() as GpxPoint;

            Assert.Equal(0.98, result.Metadata.MagVar);
            Assert.Equal(12.5, result.Metadata.GeoidHeight);
            Assert.Equal(GpsFix.Fix3D, result.Metadata.Fix);
            Assert.Equal(8, result.Metadata.SatellitesCount);
            Assert.Equal(5.1, result.Metadata.Hdop);
            Assert.Equal(8.1, result.Metadata.Vdop);
            Assert.Equal(10.8, result.Metadata.Pdop);
            Assert.Equal(45, result.Metadata.AgeOfDgpsData);
            Assert.Equal(124, result.Metadata.DgpsId);

            Assert.Equal("WPT Comment", result.Metadata.Comment);
            Assert.Equal("WPT Description", result.Metadata.Description);
            Assert.Equal("WPT Name", result.Metadata.Name);
            Assert.Equal("WPT Source", result.Metadata.Source);

            Assert.Equal(1, result.Metadata.Links.Count);
            GpxLink link = result.Metadata.Links.Single();
            Assert.Equal("http://www.topografix.com", link.Url.OriginalString);
            Assert.Equal("Link text", link.Text);
            Assert.Equal("plain/text", link.Type);
        }

        [Fact]
        public void Read_ReadsWaypointUnsortedMetadataAndExtension() {
            var data = TestDataReader.Open("gpx-waypoint-with-metadata.gpx");

            GpxReader target = new GpxReader(data, new GpxReaderSettings() { ReadMetadata = true });
            var result = target.Read() as GpxPoint;

            Assert.Equal(0.98, result.Metadata.MagVar);
            Assert.Equal(12.5, result.Metadata.GeoidHeight);
            Assert.Equal(GpsFix.Fix3D, result.Metadata.Fix);
            Assert.Equal(8, result.Metadata.SatellitesCount);
            Assert.Equal(5.1, result.Metadata.Hdop);
            Assert.Equal(8.1, result.Metadata.Vdop);
            Assert.Equal(10.8, result.Metadata.Pdop);
            Assert.Equal(45, result.Metadata.AgeOfDgpsData);
            Assert.Equal(124, result.Metadata.DgpsId);

            Assert.Equal("WPT Comment", result.Metadata.Comment);
            Assert.Equal("WPT Description", result.Metadata.Description);
            Assert.Equal("WPT Name", result.Metadata.Name);
            Assert.Equal("WPT Source", result.Metadata.Source);

            Assert.Equal(1, result.Metadata.Links.Count);
            GpxLink link = result.Metadata.Links.Single();
            Assert.Equal("http://www.topografix.com", link.Url.OriginalString);
            Assert.Equal("Link text", link.Text);
            Assert.Equal("plain/text", link.Type);
        }

        [Fact]
        public void Read_ParsesTrackWithSingleSegment() {
            var data = TestDataReader.Open("gpx-track-single-track-segment.gpx");

            GpxReader target = new GpxReader(data, new GpxReaderSettings() { ReadMetadata = false });
            var result = target.Read() as GpxTrack;

            Assert.Single(result.Geometries);

            GpxTrackSegment segment = result.Geometries[0];
            Assert.Equal(new Coordinate(-76.638178825, 39.449270368), segment.Points[0].Position);
            Assert.Equal(new DateTime(1970, 1, 1, 7, 10, 23, DateTimeKind.Utc), segment.Points[0].Timestamp);
            Assert.Equal(new Coordinate(-76.638012528, 39.449130893), segment.Points[1].Position);
            Assert.Equal(new DateTime(1970, 1, 1, 7, 10, 28, DateTimeKind.Utc), segment.Points[1].Timestamp);
            Assert.Equal(new Coordinate(-76.637980342, 39.449098706), segment.Points[2].Position);
            Assert.Equal(new DateTime(1970, 1, 1, 7, 10, 33, DateTimeKind.Utc), segment.Points[2].Timestamp);
        }

        [Fact]
        public void Read_ParsesTrackWithSingleSegmentAndExtensions() {
            var data = TestDataReader.Open("gpx-track-single-track-segment.gpx");

            GpxReader target = new GpxReader(data, new GpxReaderSettings() { ReadMetadata = false });
            var result = target.Read() as GpxTrack;

            Assert.Single(result.Geometries);

            GpxTrackSegment segment = result.Geometries[0];
        }

        [Fact]
        public void Read_ParsesTrackWithMultipleSegments() {
            var data = TestDataReader.Open("gpx-track-2-track-segments.gpx");

            GpxReader target = new GpxReader(data, new GpxReaderSettings() { ReadMetadata = false });
            var result = target.Read() as GpxTrack;

            //segments
            Assert.Equal(2, result.Geometries.Count);
            //points in segments
            Assert.Equal(3, result.Geometries[0].Points.Count);
            Assert.Equal(2, result.Geometries[1].Points.Count);
        }

        [Fact]
        public void Read_ParsesMultipleTracks() {
            var data = TestDataReader.Open("gpx-track-multiple-tracks.gpx");

            GpxReader target = new GpxReader(data, new GpxReaderSettings() { ReadMetadata = false });
            var result1 = target.Read() as GpxTrack;
            var result2 = target.Read() as GpxTrack;

            //segments - first track
            Assert.Equal(2, result1.Geometries.Count);
            //points in segments - first track
            Assert.Equal(3, result1.Geometries[0].Points.Count);
            Assert.Equal(2, result1.Geometries[1].Points.Count);

            //segments - second track
            Assert.Single(result2.Geometries);
            //points in segments - second track
            Assert.Equal(2, result2.Geometries[0].Points.Count);
        }

        [Fact]
        public void Read_ParsesEmptyTrack() {
            var data = TestDataReader.Open("gpx-track-empty.gpx");

            GpxReader target = new GpxReader(data, new GpxReaderSettings() { ReadMetadata = false });
            var result = target.Read() as GpxTrack;

            Assert.Empty(result.Geometries);
        }

        [Fact]
        public void Read_ParsesTrackWithEmptySegment() {
            var data = TestDataReader.Open("gpx-track-empty-track-segment.gpx");

            GpxReader target = new GpxReader(data, new GpxReaderSettings() { ReadMetadata = false });
            var result = target.Read() as GpxTrack;

            Assert.Single(result.Geometries);
            Assert.Empty(result.Geometries[0].Points);
        }

        [Fact]
        public void Read_ParsesTrackMetadata() {
            var data = TestDataReader.Open("gpx-track-with-metadata.gpx");

            GpxReader target = new GpxReader(data, new GpxReaderSettings() { ReadMetadata = true });
            var result = target.Read() as GpxTrack;

            Assert.Equal("TRK Comment", result.Metadata.Comment);
            Assert.Equal("TRK Description", result.Metadata.Description);
            Assert.Equal("TRK Name", result.Metadata.Name);
            Assert.Equal("TRK Source", result.Metadata.Source);
            Assert.Equal("TRK Type", result.Metadata.Type);

            GpxLink link = result.Metadata.Links.Single();
            Assert.Equal("http://www.topografix.com", link.Url.OriginalString);
            Assert.Equal("Link text", link.Text);
            Assert.Equal("plain/text", link.Type);
        }

        [Fact]
        public void Read_SetsTrackMetadataToNullIfReadMetadataIsFalse() {
            var data = TestDataReader.Open("gpx-track-with-metadata.gpx");

            GpxReader target = new GpxReader(data, new GpxReaderSettings() { ReadMetadata = false });
            var result = target.Read() as GpxTrack;

            Assert.Null(result.Metadata);
        }

        [Fact]
        public void Read_ParsesEmptyRoute() {
            var data = TestDataReader.Open("gpx-route-empty.gpx");
            GpxReader target = new GpxReader(data, new GpxReaderSettings() {ReadMetadata = false});

            var result = target.Read() as GpxRoute;

            Assert.Empty(result.Points);
        }

        [Fact]
        public void Read_ParsesSingleRoute() {
            var data = TestDataReader.Open("gpx-route-single-route.gpx");
            GpxReader target = new GpxReader(data, new GpxReaderSettings() { ReadMetadata = false });

            var result = target.Read() as GpxRoute;

            Assert.Equal(new Coordinate(-76.638178825, 39.449270368), result.Points[0].Position);
            Assert.Equal(new DateTime(1970, 1, 1, 7, 10, 23, DateTimeKind.Utc), result.Points[0].Timestamp);
            Assert.Equal(new Coordinate(-76.638012528, 39.449130893), result.Points[1].Position);
            Assert.Equal(new DateTime(1970, 1, 1, 7, 10, 28, DateTimeKind.Utc), result.Points[1].Timestamp);
            Assert.Equal(new Coordinate(-76.637980342, 39.449098706), result.Points[2].Position);
            Assert.Equal(new DateTime(1970, 1, 1, 7, 10, 33, DateTimeKind.Utc), result.Points[2].Timestamp);
        }

        [Fact]
        public void Read_ParsesSingleRouteWithExtensions() {
            var data = TestDataReader.Open("gpx-route-with-metadata-and-extensions.gpx");
            GpxReader target = new GpxReader(data, new GpxReaderSettings() { ReadMetadata = false });

            var result = target.Read() as GpxRoute;

            Assert.Equal(3, result.Points.Count);
        }

        [Fact]
        public void Read_ParsesMultipleRoutes() {
            var data = TestDataReader.Open("gpx-route-multiple-routes.gpx");
            GpxReader target = new GpxReader(data, new GpxReaderSettings() { ReadMetadata = false });

            var result1 = target.Read() as GpxRoute;
            var result2 = target.Read() as GpxRoute;

            Assert.Equal(3, result1.Points.Count);
            Assert.Equal(2, result2.Points.Count);
        }

        [Fact]
        public void Read_ParsesRouteWithMetadata() {
            var data = TestDataReader.Open("gpx-route-with-metadata.gpx");
            GpxReader target = new GpxReader(data, new GpxReaderSettings() { ReadMetadata = true });

            var result = target.Read() as GpxRoute;

            Assert.Equal("RTE Comment", result.Metadata.Comment);
            Assert.Equal("RTE Description", result.Metadata.Description);
            Assert.Equal("RTE Name", result.Metadata.Name);
            Assert.Equal("RTE Source", result.Metadata.Source);
            Assert.Equal("RTE Type", result.Metadata.Type);

            Assert.Equal(1, result.Metadata.Links.Count);
            GpxLink link = result.Metadata.Links.Single();
            Assert.Equal("http://www.topografix.com", link.Url.OriginalString);
            Assert.Equal("Link text", link.Text);
            Assert.Equal("plain/text", link.Type);
        }

        [Fact]
        public void Read_SetsRouteMetadataToNullIfReadMetadataIsFalse() {
            var data = TestDataReader.Open("gpx-route-with-metadata.gpx");
            GpxReader target = new GpxReader(data, new GpxReaderSettings() { ReadMetadata = false });

            var result = target.Read() as GpxRoute;

            Assert.Null(result.Metadata);
        }

        [Fact]
        public void Dispose_ClosesOutputStreamIfWritingToFiles() {
            string filename = "../../../Data/Gpx/gpx-real-file.gpx";

            var target = new GpxReader(filename, new GpxReaderSettings() { ReadMetadata = false });
            target.Dispose();

            FileStream testStream = null;
            testStream = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite);
            testStream.Dispose();
        }

        [Fact]
        public void Read_ReadsAllEntitiesFromRealGpxFile() {
            var data = TestDataReader.Open("gpx-real-file.gpx");
            List<IGpxGeometry> parsed = new List<IGpxGeometry>();

            GpxReader target = new GpxReader(data, new GpxReaderSettings() { ReadMetadata = true });

            IGpxGeometry geometry = null;
            while ((geometry = target.Read()) != null) {
                parsed.Add(geometry);
            }

            // waypoints
            Assert.Equal(3, parsed.Where(g => g.GeometryType == GpxGeometryType.Waypoint).Count());

            // routes
            Assert.Equal(2, parsed.Where(g => g.GeometryType == GpxGeometryType.Route).Count());

            // tracks
            Assert.Single(parsed.Where(g => g.GeometryType == GpxGeometryType.Track));
        }
    }
}
