using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

using SpatialLite.Gps.IO;
using SpatialLite.Gps;
using SpatialLite.Gps.Geometries;
using System.IO;
using System.Xml.Linq;

namespace Tests.SpatialLite.Gps {
    public class GpxDocumentTests {
        #region Constructors tests

        #region Constructor() tests

        [Fact]
        public void Constructor_CreatesEmptyDocument() {
            var target = new GpxDocument();

            Assert.Empty(target.Waypoints);
            Assert.Empty(target.Routes);
            Assert.Empty(target.Tracks);
        }

        #endregion

        #region Constructor(Waypoints, Routes, Tracks) tests

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

        #endregion

        #endregion

        #region static Load(string) tests

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
            string path = "../../src/Tests.SpatialLite.Gps/Data/Gpx/gpx-real-file.gpx";

            var target = GpxDocument.Load(path);


            Assert.Equal(3, target.Waypoints.Count);
            Assert.Equal(2, target.Routes.Count);
            Assert.Equal(1, target.Tracks.Count);
        }

        #endregion

        #region Save(string) tests

        [Fact]
        public void Save_ThrowsExceptionIfPathIsNull() {
            string path = null;
            var target = new GpxDocument();

            Assert.Throws<ArgumentNullException>(() => target.Save(path));
        }

        [Fact]
        public void Save_SavesDataToFile() {
            string path = "TestFiles\\gpxdocument-save-test.gpx";
            File.Delete(path);

            var target = GpxDocument.Load("../../src/Tests.SpatialLite.Gps/Data/Gpx/gpx-real-file.gpx");
            target.Save(path);

            var original = XDocument.Load("../../src/Tests.SpatialLite.Gps/Data/Gpx/gpx-real-file.gpx");
            var saved = XDocument.Load(path);

            Assert.True(XDocumentExtensions.DeepEqualsWithNormalization(original, saved, null));
        }

        #endregion
    }
}
