using System.Collections.Generic;

namespace SpatialLite.Core.Api;

/// <summary>
/// Represents a geometry object.
/// </summary>
public interface IGeometry
{
    /// <summary>
    /// Gets a value indicating whether the <see cref="IGeometry"/> object has Z coordinates.
    /// </summary>
    bool Is3D { get; }

    /// <summary>
    /// Gets a value indicating whether the <c>IGeometry</c> object has M values.
    /// </summary>
    bool IsMeasured { get; }

    /// <summary>
    /// Computes envelope of the <see cref="IGeometry"/> object. The envelope is defined as a minimal bounding box for a geometry.
    /// </summary>
    /// <returns>
    /// Returns an <see cref="Envelope2D"/> object that specifies the minimal bounding box of the <see cref="IGeometry"/> object.
    /// </returns>
    Envelope2D GetEnvelope();

    /// <summary>
    /// Gets collection of all <see cref="Coordinate"/> of this <see cref="IGeometry"/> object.
    /// </summary>
    /// <returns>The collection of all <see cref="Coordinate"/> of this object</returns>
    IReadOnlyList<Coordinate> GetCoordinates();
}
