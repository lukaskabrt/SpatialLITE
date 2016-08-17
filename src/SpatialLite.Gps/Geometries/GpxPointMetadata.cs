namespace SpatialLite.Gps.Geometries {
    /// <summary>
    /// Contains additional information about Gpx point
    /// </summary>
    public class GpxPointMetadata : GpxMetadata {
        /// <summary>
        /// Gets or sets magnetic variation (in degrees) at the point
        /// </summary>
        public double? MagVar { get; set; }

        /// <summary>
        /// Gets or sets height (in meters) of geoid (mean sea level) above WGS84 earth ellipsoid. As defined in NMEA GGA message.
        /// </summary>
        public double? GeoidHeight { get; set; }

        /// <summary>
        /// Gets or sets text of GPS symbol name. 
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// Gets or sets type of the Gps fix. To signify "the fix info is unknown", set value to null.
        /// </summary>
        public GpsFix? Fix { get; set; }

        /// <summary>
        /// Gets or sets number of satellites used to calculate the GPX fix.
        /// </summary>
        public int? SatellitesCount { get; set; }

        /// <summary>
        /// Gets or sets horizontal dilution of precision.
        /// </summary>
        public double? Hdop { get; set; }

        /// <summary>
        /// Gets or sets vertical dilution of precision.
        /// </summary>
        public double? Vdop { get; set; }

        /// <summary>
        /// Gets or sets position dilution of precision.
        /// </summary>
        public double? Pdop { get; set; }

        /// <summary>
        /// Gets or sets age of DGPS data - a number of seconds since last DGPS update.
        /// </summary>
        public double? AgeOfDgpsData { get; set; }

        /// <summary>
        /// Gets or sets ID of DGPS station used in differential correction.
        /// </summary>
        public int? DgpsId { get; set; }
    }
}
