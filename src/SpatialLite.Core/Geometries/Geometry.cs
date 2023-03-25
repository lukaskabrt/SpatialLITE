using System.Collections.Generic;
using System.Linq;
using SpatialLite.Core.Api;

namespace SpatialLite.Core.Geometries {
    /// <summary>
    /// Represents the base class for all geometry object.
    /// </summary>
    public abstract class Geometry : IGeometry {

        /// <summary>
        /// Gets a value indicating whether this <see cref="Geometry"/> has Z-coordinates.
        /// </summary>
        public virtual bool Is3D => this.GetCoordinates().Any(c => c.Is3D);

        /// <summary>
        /// Gets a value indicating whether this <see cref="Geometry"/> has M values.
        /// </summary>
        public virtual bool IsMeasured => this.GetCoordinates().Any(c => c.IsMeasured);

        /// <summary>
        /// Computes envelope of the <c>IGeometry</c> object. The envelope is defined as a minimal bounding box for a geometry.
        /// </summary>
        /// <returns>
        /// Returns an <see cref="Envelope2D"/> object that specifies the minimal bounding box of the <c>Geometry</c> object.
        /// </returns>
        public virtual Envelope2D GetEnvelope()
        {
            return new Envelope2D(this.GetCoordinates());
        }

        /// <summary>
        /// Gets collection of all <see cref="Coordinate"/> of this IGeometry object
        /// </summary>
        /// <returns>the collection of all <see cref="Coordinate"/> of this object</returns>
        public abstract IReadOnlyList<Coordinate> GetCoordinates();
    }
}
