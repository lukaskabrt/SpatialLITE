using SpatialLite.Gps.Geometries;

namespace SpatialLite.Gps.IO;

/// <summary>
/// Defines functions and properties for classes that can writes GPX entities to a destination.
/// </summary>
public interface IGpxWriter
{
    /// <summary>
    /// Writes GpxWaypoint
    /// </summary>
    /// <param name="waypoint">The waypoint to write.</param>
    void Write(GpxPoint waypoint);

    /// <summary>
    /// Writes GpxRoute
    /// </summary>
    /// <param name="route">The route to write</param>
    void Write(GpxRoute route);

    /// <summary>
    /// Writes GpxTrack
    /// </summary>
    /// <param name="track">The track to write</param>
    void Write(GpxTrack track);
}
