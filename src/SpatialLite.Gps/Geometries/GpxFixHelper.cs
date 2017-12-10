namespace SpatialLite.Gps.Geometries {
    /// <summary>
    /// Contains helper function for parsing / formating GpxFix enum
    /// </summary>
    public static class GpxFixHelper {
        /// <summary>
        /// Converts string representation of the GpsFix to it's GpxFix enum equivalent.
        /// </summary>
        /// <param name="s">A string containing a value to convert</param>
        /// <returns>A GpxFix enum equivalent of the s parameter or null if s can not be converted to GpsFix enum</returns>
        /// <remarks>ParseGpsFix should be used instead Enum.Parse function becouse string representation of the GpsFix values do not equal to their names.</remarks>
        public static GpsFix? ParseGpsFix(string s) {
            switch (s) {
                case "none": return GpsFix.None;
                case "2d": return GpsFix.Fix2D;
                case "3d": return GpsFix.Fix3D;
                case "dgps": return GpsFix.Dgps;
                case "pps": return GpsFix.Pps;
                default: return null;
            }
        }

        /// <summary>
        /// Converts GpsFix enum value to it's string equivalent
        /// </summary>
        /// <param name="fix">A GpxFix value to convert</param>
        /// <returns>A string equivalent of the fix parameter</returns>
        /// <remarks>ConvertGpsFixToString shoul be used instead ToString() function becouse string representation of the GpsFix values if GPX schema do not equal to their names.</remarks>
        public static string GpsFixToString(GpsFix fix) {
            switch (fix) {
                case GpsFix.None: return "none";
                case GpsFix.Fix2D: return "2d";
                case GpsFix.Fix3D: return "3d";
                case GpsFix.Dgps: return "dgps";
                case GpsFix.Pps: return "pps";
                default: return null;
            }
        }
    }
}
