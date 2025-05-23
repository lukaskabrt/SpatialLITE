using SpatialLite.Core.API;
using SpatialLite.Core.Geometries;
using System.Collections.Generic;

namespace SpatialLite.Gps.Geometries;

/// <summary>
/// Represents base class for Gps tracks
/// </summary>
/// <typeparam name="T">The type of the Gps points</typeparam>
public class GpsTrackBase<T> : LineString where T : IGpsPoint
{
    private readonly ICoordinateList _coordinatesAdapter;

    /// <summary>
    /// Creates a new instance of the GpsTrackBase class
    /// </summary>
    public GpsTrackBase() : this(new T[] { })
    {
    }

    /// <summary>
    /// Creates a new instance of GpsTrackBase with given points
    /// </summary>
    /// <param name="points">The points of the track</param>
    public GpsTrackBase(IEnumerable<T> points)
    {
        Points = new List<T>(points);
        _coordinatesAdapter = new ReadOnlyCoordinateList<T>(Points);
    }

    /// <summary>
    /// Gets the collection of track points of this track
    /// </summary>
    public List<T> Points { get; private set; }

    /// <summary>
    /// Gets the list of çoordinates of this track.
    /// </summary>
    public override ICoordinateList Coordinates
    {
        get
        {
            return _coordinatesAdapter;
        }
    }
}
