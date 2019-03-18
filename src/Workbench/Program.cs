using System;
using SpatialLite.Core.IO;
using SpatialLite.Osm;
using SpatialLite.Osm.Geometries;
using SpatialLite.Osm.IO;

namespace Workbench {
    class Program {
        static void Main(string[] args) {
            PrepareData();
        }

        private static void PrepareData() {
            using (var reader = new PbfReader("c:\\Temp\\azores-latest.osm.pbf", new OsmReaderSettings() { ReadMetadata = false })) {
                var db = OsmGeometryDatabase.Load(reader, true);

                using (var writer = new WktWriter("c:\\Temp\\azores.wkt", new WktWriterSettings())) {
                    foreach (var entity in db.Nodes) {
                        writer.Write(entity);
                    }

                    foreach (var entity in db.Ways) {
                        writer.Write(entity);
                    }
                }
            }
        }
    }
}
