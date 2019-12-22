using System.Collections.Generic;

namespace SpatialLite.Core.API {
    /// <summary>
    /// Defines common properties and methods for all geometry objects.
    /// </summary>
    public interface IGeometry {
		/// <summary>
		/// Gets a value indicating whether the <c>IGeometry</c> object has Z coordinates.
		/// </summary>
		bool Is3D { get; }

		/// <summary>
		/// Gets a value indicating whether the <c>IGeometry</c> object has M values.
		/// </summary>
		bool IsMeasured { get; }

		/// <summary>
		/// Computes envelope of the <c>IGeometry</c> object. The envelope is defined as a minimal bounding box for a geometry.
		/// </summary>
		/// <returns>
		/// Returns an <see cref="Envelope"/> object that specifies the minimal bounding box of the <c>IGeometry</c> object.
		/// </returns>
		Envelope GetEnvelope();

        /// <summary>
        /// Gets collection of all <see cref="Coordinate"/> of this IGeometry object
        /// </summary>
        /// <returns>the collection of all <see cref="Coordinate"/> of this object</returns>
        IEnumerable<Coordinate> GetCoordinates();
	}
}
