using System;

namespace SpatialLite.Osm.IO
{
    /// <summary>
    /// Defines functions and properties for classes that can read OSM entities from various sources.
    /// </summary>
    public interface IOsmReader : IDisposable
    {
        /// <summary>
        /// Reads the next Osm entity from a source.
        /// </summary>
        /// <returns>IEntityInfo object with information about entity, or null if no more entities are available.</returns>
        IEntityInfo Read();
    }
}
