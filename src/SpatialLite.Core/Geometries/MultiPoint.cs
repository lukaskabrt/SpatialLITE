using System.Collections.Generic;

using SpatialLite.Core.API;

namespace SpatialLite.Core.Geometries {
    /// <summary>
    /// Represents a collection of Points
    /// </summary>
    public class MultiPoint : GeometryCollection<Point>, IMultiPoint {

		/// <summary>
		/// Initializes a new instance of the MultiPoint class that is empty and has assigned WSG84 coordinate reference system.
		/// </summary>
		public MultiPoint()
			: base() {
		}

		/// <summary>
		/// Initializes a new instance of the MultiPoint class that is empty and has assigned specified coordinate reference system.
		/// </summary>
		/// <param name="srid">The <c>SRID</c> of the coordinate reference system.</param>
		public MultiPoint(int srid)
			: base(srid) {
		}

		/// <summary>
		/// Initializes a new instance of the MultiPoint class with specified points.
		/// </summary>
		/// <param name="points">The collection of points to be copied to the new MultiPoint.</param>
		public MultiPoint(IEnumerable<Point> points)
			: base(points) {
		}

		/// <summary>
		/// Initializes a new instance of the MultiPoint class with specified points and coordinate reference system.
		/// </summary>
		/// <param name="srid">The <c>SRID</c> of the coordinate reference system</param>		
		/// <param name="points">The collection of points to be copied to the new MultiPoint.</param>
		public MultiPoint(int srid, IEnumerable<Point> points)
			: base(srid, points) {
		}

		/// <summary>
		/// Gets collection of points from this Multipoint as the collection of IPoint objects.
		/// </summary>
		IEnumerable<IPoint> IGeometryCollection<IPoint>.Geometries {
			get { return base.Geometries; }
		}

	}
}