using System.Collections.Generic;

using SpatialLite.Core.API;

namespace SpatialLite.Core.Geometries {
    /// <summary>
    /// Represents a collection of Polygons
    /// </summary>
    public class MultiPolygon : GeometryCollection<Polygon>, IMultiPolygon {

		/// <summary>
		/// Initializes a new instance of the MultiPolygon class that is empty and has assigned WSG84 coordinate reference system.
		/// </summary>
		public MultiPolygon()
			: base() {
		}

		/// <summary>
		/// Initializes a new instance of the MultiPolygon class that is empty and has assigned specified coordinate reference system.
		/// </summary>
		/// <param name="srid">The <c>SRID</c> of the coordinate reference system.</param>
		public MultiPolygon(int srid)
			: base(srid) {
		}

		/// <summary>
		/// Initializes a new instance of the MultiPolygon class with specified Polygons
		/// </summary>
		/// <param name="polygons">The collection of Polygons to be copied to the new MultiPolygon.</param>
		public MultiPolygon(IEnumerable<Polygon> polygons)
			: base(polygons) {
		}

		/// <summary>
		/// Initializes a new instance of the MultiPolygon class with specified Polygons and coordinate reference system.
		/// </summary>
		/// <param name="srid">The <c>SRID</c> of the coordinate reference system.</param>
		/// <param name="polygons">The collection of Polygons to be copied to the new MultiPolygon.</param>
		public MultiPolygon(int srid, IEnumerable<Polygon> polygons)
			: base(srid, polygons) {
		}

		/// <summary>
		/// Gets collection of polygons from this MultiPolygon as the collection of IPolygon objects.
		/// </summary>
		IEnumerable<IPolygon> IGeometryCollection<IPolygon>.Geometries {
			get { return base.Geometries; }
		}
	}
}
