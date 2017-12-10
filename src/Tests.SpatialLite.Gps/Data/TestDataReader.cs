using System.IO;
using System.Reflection;

namespace Tests.SpatialLite.Gps.Data {
    public static class TestDataReader {
        public static Stream Open(string name) {
            var assembly = typeof(TestDataReader).GetTypeInfo().Assembly;
            return assembly.GetManifestResourceStream("Tests.SpatialLite.Gps.Data.Gpx." + name);
        }

        public static byte[] Read(string name) {
            var assembly = typeof(TestDataReader).GetTypeInfo().Assembly;

            var stream = new MemoryStream();
            assembly.GetManifestResourceStream("Tests.SpatialLite.Gps.Data.Gpx." + name).CopyTo(stream);

            return stream.ToArray();
        }
    }
}
