using System;
using System.Collections.Generic;

using Xunit;

using SpatialLite.Gps.IO;
using SpatialLite.Gps;
using SpatialLite.Gps.Geometries;
using System.IO;
using System.Xml.Linq;
using Moq;
using Tests.SpatialLite.Gps.Data;

namespace Tests.SpatialLite.Gps {
    public class GpxDocumentTests {
        [Fact]
        public void Constructor_CreatesEmptyDocument() {
            var target = new GpxDocument();

            Assert.Empty(target.Waypoints);
            Assert.Empty(target.Routes);
            Assert.Empty(target.Tracks);
        }

        [Fact]
        public void Constructor_WaypointsRoutesTracks_CreatesDocumentWithGpxEntities() {
            IEnumerable<GpxPoint> waypoints = new[] { new GpxPoint() };
            IEnumerable<GpxRoute> routes = new[] { new GpxRoute() };
            IEnumerable<GpxTrack> tracks = new[] { new GpxTrack() };

            var target = new GpxDocument(waypoints, routes, tracks);

            Assert.Equal(waypoints, target.Waypoints);
            Assert.Equal(routes, target.Routes);
            Assert.Equal(tracks, target.Tracks);
        }

        [Fact]
        public void Constructor_WaypointsRoutesTracks_ThrowsArgumentNullExceptionIfWaypointsIsNull() {
            IEnumerable<GpxRoute> routes = new[] { new GpxRoute() };
            IEnumerable<GpxTrack> tracks = new[] { new GpxTrack() };

            Assert.Throws<ArgumentNullException>(() => new GpxDocument(null, routes, tracks));
        }

        [Fact]
        public void Constructor_WaypointsRoutesTracks_ThrowsArgumentNullExceptionIfRoutesIsNull() {
            IEnumerable<GpxPoint> waypoints = new[] { new GpxPoint() };
            IEnumerable<GpxTrack> tracks = new[] { new GpxTrack() };

            Assert.Throws<ArgumentNullException>(() => new GpxDocument(waypoints, null, tracks));
        }

        [Fact]
        public void Constructor_WaypointsRoutesTracks_ThrowsArgumentNullExceptionIfTracksIsNull() {
            IEnumerable<GpxPoint> waypoints = new[] { new GpxPoint() };
            IEnumerable<GpxRoute> routes = new[] { new GpxRoute() };

            Assert.Throws<ArgumentNullException>(() => new GpxDocument(waypoints, routes, null));
        }

        [Fact]
        public void Load_IGpxReader_ThrowsExceptionIfReaderIsNull() {
            IGpxReader reader = null;

            Assert.Throws<ArgumentNullException>(() => GpxDocument.Load(reader));
        }

        [Fact]
        public void Load_IGpxReader_LoadsEntitiesFromReader() {
            using (var reader = new GpxReader(TestDataReader.Open("gpx-real-file.gpx"), new GpxReaderSettings() { ReadMetadata = true })) {
                var target = GpxDocument.Load(reader);

                Assert.Equal(3, target.Waypoints.Count);
                Assert.Equal(2, target.Routes.Count);
                Assert.Single(target.Tracks);
            }
        }

        [Fact]
        public void Load_string_ThrowsExceptionIfPathIsNull() {
            string path = null;

            Assert.Throws<ArgumentNullException>(() => GpxDocument.Load(path));
        }

        [Fact]
        public void Load_string_ThrowsExceptionIfFileDoesntExists() {
            string path = "non-existing-file.gpx";

            Assert.Throws<FileNotFoundException>(() => GpxDocument.Load(path));
        }

        [Fact]
        public void Load_LoadsGpxEntitiesFromFile() {
            string path = "../../../Data/Gpx/gpx-real-file.gpx";

            var target = GpxDocument.Load(path);

            Assert.Equal(3, target.Waypoints.Count);
            Assert.Equal(2, target.Routes.Count);
            Assert.Single(target.Tracks);
        }

        [Fact]
        public void Save_IGpxWriter_ThrowsExceptionIfWriterIsNull() {
            IGpxWriter writer = null;

            var target = new GpxDocument();
            Assert.Throws<ArgumentNullException>(() => target.Save(writer));
        }

        [Fact]
        public void Save_IGpxWriter_WritesDataToWriter() {
            var waypoint = new GpxPoint();
            var route = new GpxRoute();
            var track = new GpxTrack();

            Mock<IGpxWriter> writerM = new Mock<IGpxWriter>();
            writerM.Setup(w => w.Write(waypoint)).Verifiable();
            writerM.Setup(w => w.Write(route)).Verifiable();
            writerM.Setup(w => w.Write(track)).Verifiable();

            var target = new GpxDocument(new[] { waypoint }, new[] { route }, new[] { track });
            target.Save(writerM.Object);

            writerM.Verify(w => w.Write(waypoint), Times.Once());
            writerM.Verify(w => w.Write(route), Times.Once());
            writerM.Verify(w => w.Write(track), Times.Once());
        }

        [Fact]
        public void Save_ThrowsExceptionIfPathIsNull() {
            string path = null;
            var target = new GpxDocument();

            Assert.Throws<ArgumentNullException>(() => target.Save(path));
        }

        [Fact]
        public void Save_SavesDataToFile() {
            string path = PathHelper.GetTempFilePath("gpxdocument-save-test.gpx");

            var target = GpxDocument.Load(PathHelper.RealGpxFilePath);
            target.Save(path);

            var original = XDocument.Load(PathHelper.RealGpxFilePath);
            var saved = XDocument.Load(path);

            Assert.True(XDocumentExtensions.DeepEqualsWithNormalization(original, saved));
        }
    }
}
