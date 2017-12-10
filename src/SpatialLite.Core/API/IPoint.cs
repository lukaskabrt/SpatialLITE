namespace SpatialLite.Core.API {
    /// <summary>
    /// Defines properties and methods for points.
    /// </summary>
    public interface IPoint : IGeometry {
		/// <summary>
		/// Gets position of the point.
		/// </summary>
		Coordinate Position { get; }
	}
}
