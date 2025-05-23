using System.IO;

namespace Tests.SpatialLite.Gps;

internal static class PathHelper
{
    private const string TempDirectoryName = "Temp";

    private static readonly string RealGpxFilePathValue = Path.GetFullPath(Path.Combine("Data", "Gpx", "gpx-real-file.gpx"));
    public static string RealGpxFilePath => RealGpxFilePathValue;

    public static string GetTempFilePath(string filename)
    {
        if (!Directory.Exists(TempDirectoryName))
        {
            Directory.CreateDirectory(TempDirectoryName);
        }

        string pbfFile = Path.GetFullPath(Path.Combine(TempDirectoryName, filename));
        if (File.Exists(pbfFile))
        {
            File.Delete(pbfFile);
        }

        return pbfFile;
    }
}
