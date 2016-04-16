using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SpatialLite.Core.API;

namespace SpatialLite.Core.Geometries {
	/// <summary>
	/// Represetns the base class for all geometry object.
	/// </summary>
	public abstract class Geometry : IGeometry {
		#region Constructors

		/// <summary>
		/// Initializes Geometry class that uses the WSG84 coordinate reference system.
		/// </summary>
		public Geometry() {
			this.Srid = SRIDList.WSG84;
		}

		/// <summary>
		/// Initializes Geometry class with the specific coordinate reference system
		/// </summary>
		/// <param name="srid">The Srid of the coordinate reference system used by this Geometry.</param>
		public Geometry(int srid) {
			this.Srid = srid;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the Srid of the coordinate reference system used by this Geometry.
		/// </summary>
		public int Srid { get; private set; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="Geometry"/> has Z-coordinates.
		/// </summary>
		public abstract bool Is3D { get; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="Geometry"/> has M values.
		/// </summary>
		public abstract bool IsMeasured { get; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Computes envelope of the <c>IGeometry</c> object. The envelope is defined as a minimal bounding box for a geometry.
		/// </summary>
		/// <returns>
		/// Returns an <see cref="Envelope"/> object that specifies the minimal bounding box of the <c>Geometry</c> object.
		/// </returns>
		public abstract Envelope GetEnvelope();

		/// <summary>
		/// Returns  the  closure  of  the  combinatorial  boundary  of  this  geometric  object
		/// </summary>
		/// <returns> the  closure  of  the  combinatorial  boundary  of  this  geometric  object</returns>
		public abstract IGeometry GetBoundary();

        /// <summary>
        /// Gets collection of all <see cref="Coordinate"/> of this IGeometry object
        /// </summary>
        /// <returns>the collection of all <see cref="Coordinate"/> of this object</returns>
        public abstract IEnumerable<Coordinate> GetCoordinates();

        /// <summary>
        /// Applies the specific filter on this geometry
        /// </summary>
        /// <param name="filter">The filter to apply</param>
        public abstract void Apply(ICoordinateFilter filter);
		
        #endregion
	}
}
