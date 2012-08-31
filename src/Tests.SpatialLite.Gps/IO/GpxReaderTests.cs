using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialLite.Core.API;
using SpatialLite.Gps;
using SpatialLite.Gps.Geometries;
using SpatialLite.Gps.IO;
using Tests.SpatialLite.Gps.Data;
using Xunit;

namespace Tests.SpatialLite.Gps.IO {
    public class GpxReaderTests {
        #region Constructor(Path, Settings)

        [Fact]
        public void Constructor_StringSettings_ThrowsExceptionIfFileDoesnotExist() {
            Assert.Throws<FileNotFoundException>(delegate { new GpxReader("non-existing-file.gpx", new GpxReaderSettings() { ReadMetadata = false }); });
        }

        [Fact]
        public void Constructor_StringSettings_SetsSettingsAndMakesItReadOnly() {
            string path = "../../src/Tests.SpatialLite.Gps/Data/Gpx/gpx-real-file.gpx";
            var settings = new GpxReaderSettings() { ReadMetadata = false };
            using (var target = new GpxReader(path, settings)) {
                Assert.Same(settings, target.Settings);
                Assert.True(settings.IsReadOnly);
            }
        }

        #endregion

        #region Constructor(Stream, Settings) tests

        [Fact]
        public void Constructor_StreamSettings_SetsSettingsAndMakesItReadOnly() {
            var settings = new GpxReaderSettings() { ReadMetadata = false };
            using (var target = new GpxReader(new MemoryStream(GpxTestData.gpx_real_file), settings)) {
                Assert.Same(settings, target.Settings);
                Assert.True(settings.IsReadOnly);
            }
        }

        [Fact]
        public void Constructor_StreamSettings_ThrowsExceptionIfVersionIsnt10or11() {
            Assert.Throws<InvalidDataException>(delegate { new GpxReader(new MemoryStream(GpxTestData.gpx_version_2_0), new GpxReaderSettings() { ReadMetadata = false }); });
        }

        [Fact]
        public void Constructor_StreamSettings_ThrowsExceptionXmlContainsInvalidRootElement() {
            Assert.Throws<InvalidDataException>(delegate { new GpxReader(new MemoryStream(GpxTestData.gpx_invalid_root_element), new GpxReaderSettings() { ReadMetadata = false }); });
        }

        #endregion

        #region Read() Waypoint tests

        [Fact]
        public void Read_ThrowsExceptionIfWaypointHasntLat() {
            GpxReader target = new GpxReader(new MemoryStream(GpxTestData.gpx_waypoint_without_lat), new GpxReaderSettings() { ReadMetadata = false });

            Assert.Throws<InvalidDataException>(() => target.Read());
        }

        [Fact]
        public void Read_ThrowsExceptionIfWaypointHasntLon() {
            GpxReader target = new GpxReader(new MemoryStream(GpxTestData.gpx_waypoint_without_lon), new GpxReaderSettings() { ReadMetadata = false });

            Assert.Throws<InvalidDataException>(() => target.Read());
        }

        [Fact]
        public void Read_ParsesWaypointWitLatAndLon() {
            // <wpt lat="42.438878" lon="-71.119277"></wpt>
            var data = new MemoryStream(GpxTestData.gpx_waypoint_simple);
            var expectedCoordinate = new Coordinate(-71.119277, 42.438878);

            GpxReader target = new GpxReader(data, new GpxReaderSettings() { ReadMetadata = false });
            var result = target.Read() as GpxPoint;

            Assert.Equal(result.Position, expectedCoordinate);
        }

        [Fact]
        public void Read_SetsMetadataIfReadMetadataIsTrue() {
            var data = new MemoryStream(GpxTestData.gpx_waypoint_simple);
            var expectedCoordinate = new Coordinate(-71.119277, 42.438878);

            GpxReader target = new GpxReader(data, new GpxReaderSettings() { ReadMetadata = true });
            var result = target.Read() as GpxPoint;

            Assert.NotNull(result.Metadata);
        }

        [Fact]
        public void Read_DoesntSetMetadataIfReadMetadataIsFalse() {
            var data = new MemoryStream(GpxTestData.gpx_waypoint_with_metadata);
            var expectedCoordinate = new Coordinate(-71.119277, 42.438878);

            GpxReader target = new GpxReader(data, new GpxReaderSettings() { ReadMetadata = false });
            var result = target.Read() as GpxPoint;

            Assert.Null(result.Metadata);
        }

        [Fact]
        public void Read_ParsesWaypointWithLatLonElevationAndTime() {
            MemoryStream data = new MemoryStream(GpxTestData.gpx_waypoint_with_metadata);
            Coordinate expectedCoordinate = new Coordinate(-71.119277, 42.438878, 44.586548);
            DateTime expectedTime = new DateTime(2001, 11, 28, 21, 5, 28, DateTimeKind.Utc);

            GpxReader target = new GpxReader(data, new GpxReaderSettings() { ReadMetadata = false });
            var result = target.Read() as GpxPoint;

            Assert.Equal(result.Position, expectedCoordinate);
            Assert.Equal(result.Timestamp, expectedTime);
        }

        [Fact]
        public void Read_ParsesWaypointWithExtensions() {
            MemoryStream data = new MemoryStream(GpxTestData.gpx_waypoint_extensions);
            Coordinate expectedCoordinate = new Coordinate(-71.119277, 42.438878, 44.586548);
            DateTime expectedTime = new DateTime(2001, 11, 28, 21, 5, 28, DateTimeKind.Utc);

            GpxReader target = new GpxReader(data, new GpxReaderSettings() { ReadMetadata = false });
            var result = target.Read() as GpxPoint;

            Assert.Equal(result.Position, expectedCoordinate);
            Assert.Equal(result.Timestamp, expectedTime);
        }

        [Fact]
        public void Read_ParsesMultipleWaypoints() {
            MemoryStream data = new MemoryStream(GpxTestData.gpx_waypoint_multiple);

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
            MemoryStream data = new MemoryStream(GpxTestData.gpx_waypoint_with_metadata);

            GpxReader target = new GpxReader(data, new GpxReaderSettings() { ReadMetadata = true });
            var result = target.Read() as GpxPoint;

            Assert.Equal(0.98, result.Metadata.MagVar);
            Assert.Equal(12.5, result.Metadata.GeoidHeight);
            Assert.Equal(GpsFix.Fix3D, result.Metadata.Fix);
            Assert.Equal(8, result.Metadata.SatellitesCount);
            Assert.Equal(5, result.Metadata.Hdop);
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
            MemoryStream data = new MemoryStream(GpxTestData.gpx_waypoint_with_metadata);

            GpxReader target = new GpxReader(data, new GpxReaderSettings() { ReadMetadata = true });
            var result = target.Read() as GpxPoint;

            Assert.Equal(0.98, result.Metadata.MagVar);
            Assert.Equal(12.5, result.Metadata.GeoidHeight);
            Assert.Equal(GpsFix.Fix3D, result.Metadata.Fix);
            Assert.Equal(8, result.Metadata.SatellitesCount);
            Assert.Equal(5, result.Metadata.Hdop);
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

        #endregion

        #region Read() Track tests

        [Fact]
        public void Read_ParsesTrackWithSingleSegment() {
            MemoryStream data = new MemoryStream(GpxTestData.gpx_track_single_track_segment);

            GpxReader target = new GpxReader(data, new GpxReaderSettings() { ReadMetadata = false });
            var result = target.Read() as GpxTrack;

            Assert.Equal(1, result.Geometries.Count);

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
            MemoryStream data = new MemoryStream(GpxTestData.gpx_track_single_track_segment);

            GpxReader target = new GpxReader(data, new GpxReaderSettings() { ReadMetadata = false });
            var result = target.Read() as GpxTrack;

            Assert.Equal(1, result.Geometries.Count);

            GpxTrackSegment segment = result.Geometries[0];
        }

        [Fact]
        public void Read_ParsesTrackWithMultipleSegments() {
            MemoryStream data = new MemoryStream(GpxTestData.gpx_track_2_track_segments);

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
            MemoryStream data = new MemoryStream(GpxTestData.gpx_track_multiple_tracks);

            GpxReader target = new GpxReader(data, new GpxReaderSettings() { ReadMetadata = false });
            var result1 = target.Read() as GpxTrack;
            var result2 = target.Read() as GpxTrack;

            //segments - first track
            Assert.Equal(2, result1.Geometries.Count);
            //points in segments - first track
            Assert.Equal(3, result1.Geometries[0].Points.Count);
            Assert.Equal(2, result1.Geometries[1].Points.Count);

            //segments - second track
            Assert.Equal(1, result2.Geometries.Count);
            //points in segments - second track
            Assert.Equal(2, result2.Geometries[0].Points.Count);
        }

        [Fact]
        public void Read_ParsesEmptyTrack() {
            MemoryStream data = new MemoryStream(GpxTestData.gpx_track_empty);

            GpxReader target = new GpxReader(data, new GpxReaderSettings() { ReadMetadata = false });
            var result = target.Read() as GpxTrack;

            Assert.Equal(0, result.Geometries.Count);
        }

        [Fact]
        public void Read_ParsesTrackWithEmptySegment() {
            MemoryStream data = new MemoryStream(GpxTestData.gpx_track_empty_track_segment);

            GpxReader target = new GpxReader(data, new GpxReaderSettings() { ReadMetadata = false });
            var result = target.Read() as GpxTrack;

            Assert.Equal(1, result.Geometries.Count);
            Assert.Empty(result.Geometries[0].Points);
        }

        [Fact]
        public void Read_ParsesTrackMetadata() {
            MemoryStream data = new MemoryStream(GpxTestData.gpx_track_with_metadata);

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
            MemoryStream data = new MemoryStream(GpxTestData.gpx_track_with_metadata);

            GpxReader target = new GpxReader(data, new GpxReaderSettings() { ReadMetadata = false });
            var result = target.Read() as GpxTrack;

            Assert.Null(result.Metadata);
        }

        #endregion

        #region Route parsing tests

        [Fact]
        public void Read_ParsesEmptyRoute() {
            MemoryStream data = new MemoryStream(GpxTestData.gpx_route_empty);
            GpxReader target = new GpxReader(data, new GpxReaderSettings() {ReadMetadata = false});

            var result = target.Read() as GpxRoute;

            Assert.Equal(0, result.Points.Count);
        }

        [Fact]
        public void Read_ParsesSingleRoute() {
            MemoryStream data = new MemoryStream(GpxTestData.gpx_route_single_route);
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
            MemoryStream data = new MemoryStream(GpxTestData.gpx_route_with_metadata_and_extensions);
            GpxReader target = new GpxReader(data, new GpxReaderSettings() { ReadMetadata = false });

            var result = target.Read() as GpxRoute;

            Assert.Equal(3, result.Points.Count);
        }

        [Fact]
        public void Read_ParsesMultipleRoutes() {
            MemoryStream data = new MemoryStream(GpxTestData.gpx_route_multiple_routes);
            GpxReader target = new GpxReader(data, new GpxReaderSettings() { ReadMetadata = false });

            var result1 = target.Read() as GpxRoute;
            var result2 = target.Read() as GpxRoute;

            Assert.Equal(3, result1.Points.Count);
            Assert.Equal(2, result2.Points.Count);
        }

        [Fact]
        public void Read_ParsesRouteWithMetadata() {
            MemoryStream data = new MemoryStream(GpxTestData.gpx_route_with_metadata);
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
            MemoryStream data = new MemoryStream(GpxTestData.gpx_route_with_metadata);
            GpxReader target = new GpxReader(data, new GpxReaderSettings() { ReadMetadata = false });

            var result = target.Read() as GpxRoute;

            Assert.Null(result.Metadata);
        }

        #endregion

        #region Dispose() tests

        [Fact]
        public void Dispose_ClosesOutputStreamIfWritingToFiles() {
            string filename = "../../src/Tests.SpatialLite.Gps/Data/Gpx/gpx-real-file.gpx";

            var target = new GpxReader(filename, new GpxReaderSettings() { ReadMetadata = false });
            target.Dispose();

            FileStream testStream = null;
            Assert.DoesNotThrow(() => testStream = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite));
            testStream.Dispose();
        }

        [Fact]
        public void Dispose_ClosesOutputStreamIfWritingToStream() {
            var stream = new MemoryStream(GpxTestData.gpx_real_file);

            var target = new GpxReader(stream, new GpxReaderSettings() { ReadMetadata = false });
            target.Dispose();

            Assert.False(stream.CanRead);
        }

        #endregion

        #region Complex tests

        [Fact]
        public void Read_ReadsAllEntitiesFromRealGpxFile() {
            MemoryStream data = new MemoryStream(GpxTestData.gpx_real_file);
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
            Assert.Equal(1, parsed.Where(g => g.GeometryType == GpxGeometryType.Track).Count());
        }

        #endregion
    }
}
