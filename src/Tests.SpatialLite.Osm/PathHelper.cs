using System.IO;

namespace Tests.SpatialLite.Osm
{
    static class PathHelper
    {
        private const string TempDirectoryName = "Temp";

        private static string _osmosisPath = Path.GetFullPath(Path.Combine("Utils", "Osmosis", "bin", "osmosis.bat"));
        public static string OsmosisPath => _osmosisPath;

        private static string _realXmlFilePath = Path.GetFullPath(Path.Combine("Data", "Xml", "osm-real-file.osm"));
        public static string RealXmlFilePath => _realXmlFilePath;


        private static string _realPbfFilePath = Path.GetFullPath(Path.Combine("Data", "Pbf", "pbf-real-file.pbf"));
        public static string RealPbfFilePath => _realPbfFilePath;

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
}
