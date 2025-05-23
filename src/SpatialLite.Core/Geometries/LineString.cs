using System.Collections.Generic;
using System.Linq;
using SpatialLite.Core.API;

namespace SpatialLite.Core.Geometries {
    /// <summary>
    /// Represents a curve with linear interpolation between consecutive vertices.  
    /// </summary>
    public class LineString : Geometry, ILineString {

		private CoordinateList _coordinates;

		/// <summary>
		/// Initializes a new instance of the <c>LineString</c> class that is empty and has assigned WSG84 coordinate reference system.
		/// </summary>
		public LineString()
			: base() {
			_coordinates = new CoordinateList();
		}

		/// <summary>
		/// Initializes a new instance of the <c>LineString</c> class with specified coordinates and WSG84 coordinate reference system.
		/// </summary>
		/// <param name="coords">The collection of coordinates to be copied to the new LineString.</param>
		public LineString(IEnumerable<Coordinate> coords)
			: base() {
			_coordinates = new CoordinateList(coords);
		}

		/// <summary>
		/// Gets the list of çoordinates that define this LisneString
		/// </summary>
		public virtual ICoordinateList Coordinates {
			get { return _coordinates; }
		}

		/// <summary>
		/// Gets the list of çoordinates that define this LisneString
		/// </summary>
		ICoordinateList ILineString.Coordinates {
			get { return this.Coordinates; }
		}

		/// <summary>
		/// Gets the first coordinate of the <c>ILineString</c> object.
		/// </summary>
		public Coordinate Start {
			get {
				if (_coordinates.Count == 0) {
					return Coordinate.Empty;
				}

				return _coordinates[0];
			}
		}

		/// <summary>
		/// Gets the last coordinate of the <c>ILineString</c> object.
		/// </summary>
		public Coordinate End {
			get {
				if (_coordinates.Count == 0) {
					return Coordinate.Empty;
				}

				return _coordinates[_coordinates.Count - 1];
			}
		}

        /// <summary>
        /// Gets a value indicating whether this <c>LineString</c> is closed.
        /// </summary>
        /// <remarks>
        /// The LineStringBase is closed if <see cref="Start"/> and <see cref="End"/> are identical.
        /// </remarks>
        public virtual bool IsClosed {
			get {
				if (_coordinates.Count == 0) {
					return false;
				}
				else {
					return this.Start.Equals(this.End);
				}
			}
		}

		/// <summary>
		/// Computes envelope of the <c>IGeometry</c> object. The envelope is defined as a minimal bounding box for a geometry.
		/// </summary>
		/// <returns>
		/// Returns an <see cref="Envelope"/> object that specifies the minimal bounding box of the <c>IGeometry</c> object.
		/// </returns>
		public override Envelope GetEnvelope() {
			return new Envelope(_coordinates);
		}

        /// <summary>
        /// Gets collection of all <see cref="Coordinate"/> of this IGeometry object
        /// </summary>
        /// <returns>the collection of all <see cref="Coordinate"/> of this object</returns>
        public override IEnumerable<Coordinate> GetCoordinates() {
            return this.Coordinates;
        }
	}
}
