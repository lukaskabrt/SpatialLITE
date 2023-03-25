using System.Collections.Generic;

namespace SpatialLite.Core.Api;

/// <summary>
/// Represents a geometry collection.
/// </summary>
/// <typeparam name="T">The type of the objects included in the collection.</typeparam>
public interface IGeometryCollection<out T> : IGeometry where T : IGeometry
{
    /// <summary>
    /// Gets the collection of geometry objects from the <see cref="IGeometryCollection{T}"/>.
    /// </summary>
    IEnumerable<T> Geometries { get; }
}
