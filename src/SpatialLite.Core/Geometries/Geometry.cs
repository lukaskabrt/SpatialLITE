using System.Collections.Generic;

using SpatialLite.Core.API;

namespace SpatialLite.Core.Geometries {
    /// <summary>
    /// Represetns the base class for all geometry object.
    /// </summary>
    public abstract class Geometry : IGeometry {

		/// <summary>
		/// Computes envelope of the <c>IGeometry</c> object. The envelope is defined as a minimal bounding box for a geometry.
		/// </summary>
		/// <returns>
		/// Returns an <see cref="Envelope"/> object that specifies the minimal bounding box of the <c>Geometry</c> object.
		/// </returns>
		public abstract Envelope GetEnvelope();

        /// <summary>
        /// Gets collection of all <see cref="Coordinate"/> of this IGeometry object
        /// </summary>
        /// <returns>the collection of all <see cref="Coordinate"/> of this object</returns>
        public abstract IEnumerable<Coordinate> GetCoordinates();
	}
}
