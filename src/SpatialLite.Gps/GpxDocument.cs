using System;
using System.Collections.Generic;
using SpatialLite.Gps.Geometries;
using SpatialLite.Gps.IO;

namespace SpatialLite.Gps {
    /// <summary>
    /// Represents an in-memory GPX document with it's waypoints, routes and tracks 
    /// </summary>
    public class GpxDocument {
        /// <summary>
        /// Initializes a new instance of the GpxDocument class that is empty
        /// </summary>
        public GpxDocument() {
            this.Waypoints = new List<GpxPoint>();
            this.Routes = new List<GpxRoute>();
            this.Tracks = new List<GpxTrack>();
        }

        /// <summary>
        /// Initializes a new instance of the GpxDocument class with given GPX entities
        /// </summary>
        /// <param name="waypoints">A collection of waypoints to add to the document.</param>
        /// <param name="routes">A collection of routes to add to the document.</param>
        /// <param name="tracks">A collection of tracks to add to the document.</param>
        public GpxDocument(IEnumerable<GpxPoint> waypoints, IEnumerable<GpxRoute> routes, IEnumerable<GpxTrack> tracks) {
            this.Waypoints = new List<GpxPoint>(waypoints);
            this.Routes = new List<GpxRoute>(routes);
            this.Tracks = new List<GpxTrack>(tracks);
        }

        /// <summary>
        /// Gets collection of waypoints from the document.
        /// </summary>
        public List<GpxPoint> Waypoints { get; private set; }

        /// <summary>
        /// Gets collection of routes from the document.
        /// </summary>
        public List<GpxRoute> Routes { get;private  set; }

        /// <summary>
        /// Gets collection of tracks from the document.
        /// </summary>
        public List<GpxTrack> Tracks { get; private set; }

        /// <summary>
        /// Saves content of the GpxDocument to file.
        /// </summary>
        /// <param name="path">Path to the file.</param>
        public void Save(string path) {
            using (GpxWriter writer = new GpxWriter(path, new GpxWriterSettings() {WriteMetadata = true})) {
                this.Save(writer);
            }
        }

        /// <summary>
        /// Saves content of the GpxDocument using suplied writer.
        /// </summary>
        /// <param name="writer">GpxWriter to be used</param>
        public void Save(IGpxWriter writer) {
            if (writer == null) {
                throw new ArgumentNullException("writer");
            }

            foreach (var waypoint in this.Waypoints) {
                writer.Write(waypoint);
            }

            foreach (var route in this.Routes) {
                writer.Write(route);
            }

            foreach (var track in this.Tracks) {
                writer.Write(track);
            }
        }

        /// <summary>
        /// Loads Gpx data from reader to this instance of the GpxDocument class
        /// </summary>
        /// <param name="reader">The reader to read data from</param>
        private void LoadFromReader(IGpxReader reader) {
            IGpxGeometry geometry = null;
            while ((geometry = reader.Read()) != null) {
                switch (geometry.GeometryType) {
                    case GpxGeometryType.Waypoint: this.Waypoints.Add((GpxPoint)geometry); break;
                    case GpxGeometryType.Route: this.Routes.Add((GpxRoute)geometry); break;
                    case GpxGeometryType.Track: this.Tracks.Add((GpxTrack)geometry); break;
                }
            }
        }

        /// <summary>
        /// Loads Gpx data from a file.
        /// </summary>
        /// <param name="path">Path to the GPX file.</param>
        /// <returns>GpxDocument instance with data from GPX file</returns>
        public static GpxDocument Load(string path) {
            GpxDocument result = new GpxDocument();

            using (GpxReader reader = new GpxReader(path, new GpxReaderSettings() { ReadMetadata = true })) {
                result.LoadFromReader(reader);
            }

            return result;
        }

        /// <summary>
        /// Loads Gpx data from IGpxReader
        /// </summary>
        /// <param name="reader">The reader to read data from</param>
        /// <returns>GpxDocument instance with data from GpxReader</returns>
        public static GpxDocument Load(IGpxReader reader) {
            if (reader == null) {
                throw new ArgumentNullException("reader");
            }

            GpxDocument result = new GpxDocument();
            result.LoadFromReader(reader);
            return result;
        }
    }
}
