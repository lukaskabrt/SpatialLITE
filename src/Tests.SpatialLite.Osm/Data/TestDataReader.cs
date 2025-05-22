using System.IO;
using System.Reflection;

namespace Tests.SpatialLite.Osm.Data
{
    public static class TestDataReader
    {
        public static Stream OpenXml(string name)
        {
            var assembly = typeof(TestDataReader).GetTypeInfo().Assembly;
            return assembly.GetManifestResourceStream("Tests.SpatialLite.Osm.Data.Xml." + name);
        }

        public static Stream OpenPbf(string name)
        {
            var assembly = typeof(TestDataReader).GetTypeInfo().Assembly;
            return assembly.GetManifestResourceStream("Tests.SpatialLite.Osm.Data.Pbf." + name);
        }

        public static Stream OpenOsmDB(string name)
        {
            var assembly = typeof(TestDataReader).GetTypeInfo().Assembly;
            return assembly.GetManifestResourceStream("Tests.SpatialLite.Osm.Data.OsmDatabase." + name);
        }
    }
}
