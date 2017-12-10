using System.Collections.Generic;

using SpatialLite.Core.API;

namespace SpatialLite.Core.Geometries {
    /// <summary>
    /// Represents a location in the coordinate space.
    /// </summary>
    public class Point : Geometry, IPoint {
		#region Private Fields

		private Coordinate _position = Coordinate.Empty;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <c>Point</c> class that is empty and has assigned the WSG84 coordinate reference system.
		/// </summary>
		public Point()
			: base() {
		}

		/// <summary>
		/// Initializes a new instance of the <c>Point</c> class with specified X and Y coordinates in WSG84 coordinate reference system.
		/// </summary>
		/// <param name="x">The X-coordinate of the <c>Point</c></param>
		/// <param name="y">The Y-coordinate of the <c>Point</c></param>
		public Point(double x, double y)
			: base() {
			_position = new Coordinate(x, y);
		}

	/// <summary>
		/// Initializes a new instance of the <c>Point</c> class with specified X, Y and Z coordinates in WSG84 coordinate reference system.
		/// </summary>
		/// <param name="x">The X-coordinate of the <c>Point</c></param>
		/// <param name="y">The Y-coordinate of the <c>Point</c></param>
		/// <param name="z">The Z-coordinate of the <c>Point</c></param>
		public Point(double x, double y, double z)
			: base() {
			_position = new Coordinate(x, y, z);
		}

			/// <summary>
		/// Initializes a new instance of the <c>Point</c> class with specified X, Y, Z coordinates and M value in WSG84 coordinate reference system.
		/// </summary>
		/// <param name="x">The X-coordinate of the <c>Point</c></param>
		/// <param name="y">The Y-coordinate of the <c>Point</c></param>
		/// <param name="z">The Z-coordinate of the <c>Point</c></param>
		/// <param name="m">The measured value of the <c>Point</c></param>
		public Point(double x, double y, double z, double m)
			: base() {
			_position = new Coordinate(x, y, z, m);
		}

		/// <summary>
		/// Initializes a new instance of the <c>Point</c> class with specific <c>Position</c> in WSG84 coordinate reference system.
		/// </summary>
		/// <param name="position">The position of this <c>Point</c></param>
		public Point(Coordinate position)
			: base() {
			_position = position;
		}

		/// <summary>
		/// Initializes a new instance of the <c>Point</c> class with specified <c>Position</c> and coordinate reference system.
		/// </summary>
		/// <param name="srid">The SRID of coordinate reference system.</param>
		/// <param name="position">The position of the <c>Point</c>.</param>
		public Point(int srid, Coordinate position)
			: base(srid) {
			_position = position;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets position of this <c>Point</c>
		/// </summary>
		public Coordinate Position {
			get {
				return _position;
			}

			set {
				_position = value;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the this <c>Point</c> has Z-coordinate set.
		/// </summary>
		public override bool Is3D {
			get { return _position.Is3D; }
		}

		/// <summary>
		/// Gets a value indicating whether the this <c>Point</c> has M value set.
		/// </summary>
		public override bool IsMeasured {
			get { return _position.IsMeasured; }
		}

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns Envelope, that covers this Point.
        /// </summary>
        /// <returns>Envelope, that covers this Point.</returns>
        public override Envelope GetEnvelope() {
			return new Envelope(this.Position);
		}

		/// <summary>
		/// Returns  the  closure  of  the  combinatorial  boundary  of  this  geometric  object.
		/// </summary>
		/// <returns> the  closure  of  the  combinatorial  boundary  of  this  point.</returns>
		/// <remarks>Boundary of the point is always empty.</remarks>
		public override IGeometry GetBoundary() {
			return new GeometryCollection<Geometry>(this.Srid);
		}

        /// <summary>
        /// Applies the specific filter on this geometry
        /// </summary>
        /// <param name="filter">The filter to apply</param>
        public override void Apply(ICoordinateFilter filter) {
            filter.Filter(ref _position);
        }

        /// <summary>
        /// Gets collection of all <see cref="Coordinate"/> of this IGeometry object
        /// </summary>
        /// <returns>the collection of all <see cref="Coordinate"/> of this object</returns>
        public override IEnumerable<Coordinate> GetCoordinates() {
            yield return this.Position;
        }

        #endregion
    }
}
