namespace SpatialLite.Gps.IO {
    /// <summary>
    /// Defines functions and properties for classes that can read GPX entities from a source.
    /// </summary>
    public interface IGpxReader {
        /// <summary>
        /// Parses next element of the GPX file
        /// </summary>
        IGpxGeometry Read();
    }
}
