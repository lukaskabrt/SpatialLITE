namespace SpatialLite.Core.Api;

/// <summary>
/// Represents a point.
/// </summary>
public interface IPoint : IGeometry
{
    /// <summary>
    /// Gets the position of the point.
    /// </summary>
    Coordinate Position { get; }
}
