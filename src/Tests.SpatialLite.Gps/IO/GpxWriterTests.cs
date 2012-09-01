using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

using SpatialLite.Gps.Geometries;
using SpatialLite.Gps.IO;
using System.IO;
using System.Xml.Linq;
using Tests.SpatialLite.Gps.Data;
using SpatialLite.Core.API;

namespace Tests.SpatialLite.Gps.IO {
    public class GpxWriterTests {
        GpxPoint _waypoint = new GpxPoint(-71.119277, 42.438878, 44.586548, new DateTime(2001, 11, 28, 21, 05, 28, DateTimeKind.Utc));
        GpxPoint _waypointWithMetadata = new GpxPoint(-71.119277, 42.438878, 44.586548, new DateTime(2001, 11, 28, 21, 05, 28, DateTimeKind.Utc));
        GpxPointMetadata _pointMetadata;

        GpxRoute _route = new GpxRoute(new GpxPoint[] {
		    new GpxPoint(new Coordinate(-76.638178825, 39.449270368), new DateTime(1970, 1, 1, 7, 10, 23, DateTimeKind.Utc)),
			new GpxPoint(new Coordinate(-76.638012528, 39.449130893), new DateTime(1970, 1, 1, 7, 10, 28, DateTimeKind.Utc)),
			new GpxPoint(new Coordinate(-76.637980342, 39.449098706), new DateTime(1970, 1, 1, 7, 10, 33, DateTimeKind.Utc))
		});
        GpxRoute _routeWithMetadata = new GpxRoute(new GpxPoint[] {
		    new GpxPoint(new Coordinate(-76.638178825, 39.449270368), new DateTime(1970, 1, 1, 7, 10, 23, DateTimeKind.Utc)),
			new GpxPoint(new Coordinate(-76.638012528, 39.449130893), new DateTime(1970, 1, 1, 7, 10, 28, DateTimeKind.Utc)),
			new GpxPoint(new Coordinate(-76.637980342, 39.449098706), new DateTime(1970, 1, 1, 7, 10, 33, DateTimeKind.Utc))
		});
        GpxTrackMetadata _routeMetadata;

        GpxTrackSegment _segment = new GpxTrackSegment(new GpxPoint[] {
		    new GpxPoint(new Coordinate(-76.638178825, 39.449270368), new DateTime(1970, 1, 1, 7, 10, 23, DateTimeKind.Utc)),
			new GpxPoint(new Coordinate(-76.638012528, 39.449130893), new DateTime(1970, 1, 1, 7, 10, 28, DateTimeKind.Utc)),
			new GpxPoint(new Coordinate(-76.637980342, 39.449098706), new DateTime(1970, 1, 1, 7, 10, 33, DateTimeKind.Utc))
		});
        GpxTrackMetadata _trackMetadata;
        GpxTrack _track;
        GpxTrack _trackWithMetadata;


        public GpxWriterTests() {
            _pointMetadata = new GpxPointMetadata();
            _pointMetadata.AgeOfDgpsData = 45;
            _pointMetadata.DgpsId = 124;
            _pointMetadata.Fix = GpsFix.Fix3D;
            _pointMetadata.GeoidHeight = 12.5;
            _pointMetadata.Hdop = 5.1;
            _pointMetadata.MagVar = 0.98;
            _pointMetadata.Pdop = 10.8;
            _pointMetadata.SatellitesCount = 8;
            _pointMetadata.Symbol = "WPT Symbol";
            _pointMetadata.Vdop = 8.1;

            _pointMetadata.Comment = "WPT Comment";
            _pointMetadata.Description = "WPT Description";
            _pointMetadata.Name = "WPT Name";
            _pointMetadata.Source = "WPT Source";
            _pointMetadata.Links.Add(new GpxLink(new Uri("http://www.topografix.com")) { Text = "Link text", Type = "plain/text" });
            _waypointWithMetadata.Metadata = _pointMetadata;

            _routeMetadata = new GpxTrackMetadata();
            _routeMetadata.Comment = "RTE Comment";
            _routeMetadata.Description = "RTE Description";
            _routeMetadata.Name = "RTE Name";
            _routeMetadata.Source = "RTE Source";
            _routeMetadata.Type = "RTE Type";
            _routeMetadata.Links.Add(new GpxLink(new Uri("http://www.topografix.com")) { Text = "Link text", Type = "plain/text" });
            _routeWithMetadata.Metadata = _routeMetadata;

            _trackMetadata = new GpxTrackMetadata();
            _trackMetadata.Comment = "TRK Comment";
            _trackMetadata.Description = "TRK Description";
            _trackMetadata.Name = "TRK Name";
            _trackMetadata.Source = "TRK Source";
            _trackMetadata.Type = "TRK Type";
            _trackMetadata.Links.Add(new GpxLink(new Uri("http://www.topografix.com")) { Text = "Link text", Type = "plain/text" });

            _track = new GpxTrack(new GpxTrackSegment[] { _segment });
            _trackWithMetadata = new GpxTrack(new GpxTrackSegment[] { _segment });
            _trackWithMetadata.Metadata = _trackMetadata;
        }

        #region Constructor(Stream, Settings) tests

        [Fact]
        public void Constructor_StreamSettings_SetsSettingsAndMarkSettingsAsReadOnly() {
            var stream = new MemoryStream();
            var settings = new GpxWriterSettings();
            using (var target = new GpxWriter(stream, settings)) {
                Assert.Same(settings, target.Settings);
                Assert.True(target.Settings.IsReadOnly);
            }
        }

        [Fact]
        public void Constructor_StreamSettings_CreatesGpxFileWithRootElement() {
            string generatorName = "SpatialLite";
            var stream = new MemoryStream();

            using (GpxWriter target = new GpxWriter(stream, new GpxWriterSettings() { WriteMetadata = false, GeneratorName = generatorName })) {
            }

            XDocument written = XDocument.Load(new MemoryStream(stream.ToArray()));
            XDocument expected = XDocument.Load(new MemoryStream(GpxTestData.gpx_empty_file));

            Assert.True(XDocumentExtensions.DeepEqualsWithNormalization(written, expected, null));
        }

        #endregion

        #region Constructor(Path, Settings) tests

        [Fact]
        public void Constructor_PathSettings_SetsSettingsAndMakesThemReadOnly() {
            string path = "TestFiles\\gpxwriter-constructor-test.gpx";
            var settings = new GpxWriterSettings();
            using (var target = new GpxWriter(path, settings)) {
                Assert.Same(settings, target.Settings);
                Assert.True(target.Settings.IsReadOnly);
            }
        }

        [Fact]
        public void Constructor_PathSettings_CreatesOutputFile() {
            string filename = "TestFiles\\gpxwriter-constructor-creates-output-test.gpx";
            File.Delete(filename);

            var settings = new GpxWriterSettings();
            using (var target = new GpxWriter(filename, settings)) {
                ;
            }

            Assert.True(File.Exists(filename));
        }

        [Fact]
        public void Constructor_PathSettings_CreatesGpxFileWithRootElement() {
            string path = "TestFiles\\gpx-writer-constructor-test.gpx";
            File.Delete(path);
            string generatorName = "SpatialLite";

            using (GpxWriter target = new GpxWriter(path, new GpxWriterSettings() { WriteMetadata = false, GeneratorName = generatorName })) {
            }

            XDocument written = XDocument.Load(path);
            XDocument expected = XDocument.Load(new MemoryStream(GpxTestData.gpx_empty_file));

            Assert.True(XDocumentExtensions.DeepEqualsWithNormalization(written, expected, null));
        }

        #endregion

        #region Write(waypoint) tests

        [Fact]
        public void Write_WritesWaypointWithoutMetadataIfMetadataIsNull() {
            MemoryStream stream = new MemoryStream();

            using (GpxWriter target = new GpxWriter(stream, new GpxWriterSettings() { WriteMetadata = false })) {
                target.Write(_waypoint);
            }

            XDocument written = XDocument.Load(new MemoryStream(stream.ToArray()));
            XDocument expected = XDocument.Load(new MemoryStream(GpxTestData.gpx_waypoint_simple));

            Assert.True(XDocumentExtensions.DeepEqualsWithNormalization(written, expected, null));
        }

        [Fact]
        public void Write_WritesWaypointWithoutMetadataIfWriteMetadataIsFalse() {
            MemoryStream stream = new MemoryStream();

            using (GpxWriter target = new GpxWriter(stream, new GpxWriterSettings() { WriteMetadata = false })) {
                target.Write(_waypointWithMetadata);
            }

            XDocument written = XDocument.Load(new MemoryStream(stream.ToArray()));
            XDocument expected = XDocument.Load(new MemoryStream(GpxTestData.gpx_waypoint_simple));

            Assert.True(XDocumentExtensions.DeepEqualsWithNormalization(written, expected, null));
        }

        [Fact]
        public void Write_WritesWaypointWithMetadata() {
            MemoryStream stream = new MemoryStream();

            using (GpxWriter target = new GpxWriter(stream, new GpxWriterSettings() { WriteMetadata = true })) {
                target.Write(_waypointWithMetadata);
            }

            XDocument written = XDocument.Load(new MemoryStream(stream.ToArray()));
            XDocument expected = XDocument.Load(new MemoryStream(GpxTestData.gpx_waypoint_with_metadata));

            Assert.True(XDocumentExtensions.DeepEqualsWithNormalization(written, expected, null));
        }

        [Fact]
        public void Write_WritesWaypointWithoutUnnecessaryElements() {
            _waypointWithMetadata.Metadata.SatellitesCount = null;
            _waypointWithMetadata.Metadata.Name = null;
            MemoryStream stream = new MemoryStream();

            using (GpxWriter target = new GpxWriter(stream, new GpxWriterSettings() { WriteMetadata = true })) {
                target.Write(_waypointWithMetadata);
            }

            XDocument written = XDocument.Load(new MemoryStream(stream.ToArray()));
            XDocument expected = XDocument.Load(new MemoryStream(GpxTestData.gpx_waypoint_with_metadata_selection));

            Assert.True(XDocumentExtensions.DeepEqualsWithNormalization(written, expected, null));
        }

        #endregion

        #region Write(route) tests

        [Fact]
        public void Write_WritesRouteWith3Points() {
            MemoryStream stream = new MemoryStream();

            using (GpxWriter target = new GpxWriter(stream, new GpxWriterSettings() { WriteMetadata = false })) {
                target.Write(_route);
            }

            XDocument written = XDocument.Load(new MemoryStream(stream.ToArray()));
            XDocument expected = XDocument.Load(new MemoryStream(GpxTestData.gpx_route_single_route));

            Assert.True(XDocumentExtensions.DeepEqualsWithNormalization(written, expected, null));
        }

        [Fact]
        public void Write_WritesRouteWithMetadata() {
            MemoryStream stream = new MemoryStream();

            using (GpxWriter target = new GpxWriter(stream, new GpxWriterSettings() { WriteMetadata = true })) {
                target.Write(_routeWithMetadata);
            }

            XDocument written = XDocument.Load(new MemoryStream(stream.ToArray()));
            XDocument expected = XDocument.Load(new MemoryStream(GpxTestData.gpx_route_with_metadata));

            Assert.True(XDocumentExtensions.DeepEqualsWithNormalization(written, expected, null));
        }

        [Fact]
        public void Write_WritesRouteWithoutMetadataIfWriteMetadataIsFalse() {
            MemoryStream stream = new MemoryStream();

            using (GpxWriter target = new GpxWriter(stream, new GpxWriterSettings() { WriteMetadata = false })) {
                target.Write(_routeWithMetadata);
            }

            XDocument written = XDocument.Load(new MemoryStream(stream.ToArray()));
            XDocument expected = XDocument.Load(new MemoryStream(GpxTestData.gpx_route_single_route));

            Assert.True(XDocumentExtensions.DeepEqualsWithNormalization(written, expected, null));
        }

        [Fact]
        public void Write_WritesRouteWithoutUnnecessaryElements() {
            _routeWithMetadata.Metadata.Source = null;
            MemoryStream stream = new MemoryStream();

            using (GpxWriter target = new GpxWriter(stream, new GpxWriterSettings() { WriteMetadata = true })) {
                target.Write(_routeWithMetadata);
            }

            XDocument written = XDocument.Load(new MemoryStream(stream.ToArray()));
            XDocument expected = XDocument.Load(new MemoryStream(GpxTestData.gpx_route_with_metadata_selection));

            Assert.True(XDocumentExtensions.DeepEqualsWithNormalization(written, expected, null));
        }

        #endregion

        #region Write(track) tests

        [Fact]
        public void Write_WritesTrack() {
            MemoryStream stream = new MemoryStream();

            using (GpxWriter target = new GpxWriter(stream, new GpxWriterSettings() { WriteMetadata = false })) {
                target.Write(_track);
            }

            XDocument written = XDocument.Load(new MemoryStream(stream.ToArray()));
            XDocument expected = XDocument.Load(new MemoryStream(GpxTestData.gpx_track_single_track_segment));

            Assert.True(XDocumentExtensions.DeepEqualsWithNormalization(written, expected, null));
        }

        [Fact]
        public void Write_WritesTrackWithMetadata() {
            MemoryStream stream = new MemoryStream();

            using (GpxWriter target = new GpxWriter(stream, new GpxWriterSettings() { WriteMetadata = true })) {
                target.Write(_trackWithMetadata);
            }

            XDocument written = XDocument.Load(new MemoryStream(stream.ToArray()));
            XDocument expected = XDocument.Load(new MemoryStream(GpxTestData.gpx_track_with_metadata));

            Assert.True(XDocumentExtensions.DeepEqualsWithNormalization(written, expected, null));
        }

        [Fact]
        public void Write_DoesntWriteTrackMetadataIfWriteMetadataIsFalse() {
            MemoryStream stream = new MemoryStream();

            using (GpxWriter target = new GpxWriter(stream, new GpxWriterSettings() { WriteMetadata = false })) {
                target.Write(_trackWithMetadata);
            }

            XDocument written = XDocument.Load(new MemoryStream(stream.ToArray()));
            XDocument expected = XDocument.Load(new MemoryStream(GpxTestData.gpx_track_single_track_segment));

            Assert.True(XDocumentExtensions.DeepEqualsWithNormalization(written, expected, null));
        }

        [Fact]
        public void Write_TrackWithEntityDetailsButNullValues_WritesTrackWithoutUnnecessaryElements() {
            MemoryStream stream = new MemoryStream();
            _trackWithMetadata.Metadata.Source = null;

            using (GpxWriter target = new GpxWriter(stream, new GpxWriterSettings() { WriteMetadata = true })) {
                target.Write(_trackWithMetadata);
            }

            XDocument written = XDocument.Load(new MemoryStream(stream.ToArray()));
            XDocument expected = XDocument.Load(new MemoryStream(GpxTestData.gpx_track_with_metadata_selection));

            Assert.True(XDocumentExtensions.DeepEqualsWithNormalization(written, expected, null));
        }

        #endregion

        #region Dispose() tests

        [Fact]
        public void Dispose_ClosesOutputStreamIfWritingToFiles() {
            string path = "TestFiles\\gpxwriter-closes-output-filestream-test.osm";
            File.Delete(path);

            var target = new GpxWriter(path, new GpxWriterSettings());
            target.Dispose();

            FileStream testStream = null;
            Assert.DoesNotThrow(() => testStream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite));
            testStream.Dispose();
        }

        [Fact]
        public void Dispose_ClosesOutputStreamIfWritingToStream() {
            MemoryStream stream = new MemoryStream();

            var target = new GpxWriter(stream, new GpxWriterSettings());
            target.Dispose();

            Assert.False(stream.CanRead);
        }

        #endregion
    }
}
