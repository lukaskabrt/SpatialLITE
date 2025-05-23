using System.IO;
using System.Reflection;

namespace Tests.SpatialLite.Core.Data;

public static class TestDataReader
{
    public static Stream Open(string name)
    {
        var assembly = typeof(TestDataReader).GetTypeInfo().Assembly;
        return assembly.GetManifestResourceStream("Tests.SpatialLite.Core.Data.IO." + name);
    }

    public static byte[] Read(string name)
    {
        var assembly = typeof(TestDataReader).GetTypeInfo().Assembly;

        var stream = new MemoryStream();
        assembly.GetManifestResourceStream("Tests.SpatialLite.Core.Data.IO." + name).CopyTo(stream);

        return stream.ToArray();
    }
}
