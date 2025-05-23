using SpatialLite.Core.API;
using System;

namespace SpatialLite.Gps;

/// <summary>
/// Represents location on the earth surface with timestamp that defines time when the point was recorded 
/// </summary>
public interface IGpsPoint : IPoint
{
    /// <summary>
    /// Gets or sets time when the point was recorded.
    /// </summary>
    DateTime? Timestamp { get; set; }
}
