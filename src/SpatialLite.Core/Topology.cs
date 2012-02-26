using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SpatialLite.Core.Algorithms;
using SpatialLite.Core.API;

namespace SpatialLite.Core {
	/// <summary>
	/// Provides methods for basic topology analysis of geometry objects and relationships among them.
	/// </summary>
	public class Topology {
		#region Private Static Fields
	
		private static Topology _euclidean2D;

		#endregion

		#region Static Constructors

		/// <summary>
		/// Initializes static members of the Topology class.
		/// </summary>
		static Topology() {
			_euclidean2D = new Topology(new Euclidean2DLocator());
		}

		#endregion

		#region Public Static Properties

		/// <summary>
		/// Gets Topology class instance that uses Euclidean2DLocator to determine relationships among geometries.
		/// </summary>
		public static Topology Euclidean2D {
			get {
				return _euclidean2D;
			}
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the Topology class with specific geometry locator.
		/// </summary>
		/// <param name="geometryLocator">The IGeometryLocator object to use.</param>
		public Topology(IGeometryLocator geometryLocator) {
			if (geometryLocator == null) {
				throw new ArgumentNullException("geometryLocator", "GeometryLocator can't be null");
			}

			this.GeometryLocator = geometryLocator;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets IGeometryLocator that this Topology class instance uses to determine relationships among geometries.
		/// </summary>
		public IGeometryLocator GeometryLocator { get; private set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Determines whether specific Coordinate is in the given polygon.
		/// </summary>
		/// <param name="c">The coordinate to chec.k</param>
		/// <param name="polygon">The polygon to check coordinate against.</param>
		/// <param name="includeBoundaries">bool values that specifies whether points on boundaries of the polygon should be considered as inside.</param>
		/// <returns>true if coordinate lies inside outer polygon boundary and outside polygon's inner boundaries (holes), otherwise returns false.</returns>
		public bool IsInside(Coordinate c, IPolygon polygon, bool includeBoundaries) {
			bool insideExterior = this.GeometryLocator.IsInRing(c, polygon.ExteriorRing);

			if (insideExterior == false) {
				if (includeBoundaries) {
					return this.GeometryLocator.IsOnLine(c, polygon.ExteriorRing);
				}
				else {
					return false;
				}
			}
			else {
				foreach (var ring in polygon.InteriorRings) {
					if (this.GeometryLocator.IsInRing(c, ring)) {
						return false;
					}
					else if (includeBoundaries && this.GeometryLocator.IsOnLine(c, ring)) {
						return true;
					}
				}
			}

			return true;
		}

		/// <summary>
		/// Determines whether specific Coordinate is in the given multipolygon.
		/// </summary>
		/// <param name="c">The coordinate to check.</param>
		/// <param name="multipolygon">The multipolygon to check coordinate against.</param>
		/// <param name="includeBoundaries">bool values that specifies whether points on boundaries of the polygon should be considered as inside.</param>
		/// <returns>true if coordinate lies inside any polygon of the given multipolygon, otherwise false.</returns>
		public bool IsInside(Coordinate c, IMultiPolygon multipolygon, bool includeBoundaries) {
			foreach (var polygon in multipolygon.Geometries) {
				if (this.IsInside(c, polygon, includeBoundaries)) {
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Determines whether specific Coordinate is on the given polyline.
		/// </summary>
		/// <param name="c">The coordinate to check.</param>
		/// <param name="line">The line to check coordinate against.</param>
		/// <returns>true if coordinate lies on the polyline, otherwise false.</returns>
		public bool IsOnLine(Coordinate c, ILineString line) {
			return this.GeometryLocator.IsOnLine(c, line.Coordinates);
		}

		/// <summary>
		/// Determines whether specific Coordinate in on the given multiline.
		/// </summary>
		/// <param name="c">The coordinate to check.</param>
		/// <param name="multiline">The multiline to check coordinate against.</param>
		/// <returns>true if coordinate lies on any line of the given multiline, otherwise false.</returns>
		public bool IsOnLine(Coordinate c, IMultiLineString multiline) {
			foreach (var line in multiline.Geometries) {
				if (this.IsOnLine(c, line)) {
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Determines whether two polylines defined by series of coordinates intersects.
		/// </summary>
		/// <param name="line1">The first polyline to test.</param>
		/// <param name="line2">The second polyline to test.</param>
		/// <returns>true if polylines intersets, otherwise false.</returns>
		public bool Intersects(ILineString line1, ILineString line2) {
			return this.GeometryLocator.Intersects(line1.Coordinates, line2.Coordinates);
		}

		#endregion
	}
}
