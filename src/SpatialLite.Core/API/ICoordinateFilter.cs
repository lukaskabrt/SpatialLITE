namespace SpatialLite.Core.API {
    /// <summary>
    /// Specifies a filter that can modify coordinates
    /// </summary>
    public interface ICoordinateFilter {
        /// <summary>
        /// Applies a filter function to the specific <see cref="Coordinate"/>
        /// </summary>
        /// <param name="coordinate">the Coordinate to apply the filter function to</param>
        void Filter(ref Coordinate coordinate);
    }
}
