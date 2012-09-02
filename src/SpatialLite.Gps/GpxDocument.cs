using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialLite.Gps.Geometries;
using SpatialLite.Gps.IO;

namespace SpatialLite.Gps {
    /// <summary>
    /// Represents an in-memory GPX document with it's waypoints, routes and tracks 
    /// </summary>
    public class GpxDocument {

        #region Constructors

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

        #endregion

        #region Public properties

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

        #endregion

        #region Public Methods

        /// <summary>
        /// Saves content of the GpxDocument to file.
        /// </summary>
        /// <param name="path">Path to the file.</param>
        public void Save(string path) {
            using (GpxWriter writer = new GpxWriter(path, new GpxWriterSettings() {WriteMetadata = true})) {
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
        }

        #endregion

        /// <summary>
        /// Loads Gpx data from a file.
        /// </summary>
        /// <param name="path">Path to the GPX file.</param>
        /// <returns>GpxDocument instance with data from GPX file</returns>
        public static GpxDocument Load(string path) {
            GpxDocument result = new GpxDocument();

            using (GpxReader reader = new GpxReader(path, new GpxReaderSettings() { ReadMetadata = true })) {
                IGpxGeometry geometry = null;
                while ((geometry = reader.Read()) != null) {
                    switch (geometry.GeometryType) {
                        case GpxGeometryType.Waypoint: result.Waypoints.Add((GpxPoint)geometry); break;
                        case GpxGeometryType.Route: result.Routes.Add((GpxRoute)geometry); break;
                        case GpxGeometryType.Track: result.Tracks.Add((GpxTrack)geometry); break;
                    }
                }
            }

            return result;
        }
    }
}
