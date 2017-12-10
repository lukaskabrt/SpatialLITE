namespace SpatialLite.Gps.Geometries {
    /// <summary>
    /// Defines type of the GPS fix
    /// </summary>
    public enum GpsFix {
        /// <summary>
        /// No fix
        /// </summary>
        None,

        /// <summary>
        /// Position only
        /// </summary>
        Fix2D,

        /// <summary>
        /// Position and elevation
        /// </summary>
        Fix3D,

        /// <summary>
        /// Differential GPS
        /// </summary>
        Dgps,

        /// <summary>
        /// Military signal used
        /// </summary>
        Pps
    }
}
