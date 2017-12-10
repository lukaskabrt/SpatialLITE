using System;

namespace SpatialLite.Core.API {
    /// <summary>
    /// Represents a location in the coordinate space.
    /// </summary>
    /// <remarks>
    /// A Coordinate may include a M value. The M value allows an application to associate some measure with the <c>Coordinate</c>. 
    /// </remarks>
    public struct Coordinate : IEquatable<Coordinate> {
		#region Public Static Fields

		/// <summary>
		/// Represents an empty coordinate.
		/// </summary>
		/// <remarks>
		/// The empty coordinate has all coordinates equal to NaN.
		/// </remarks>
		public static Coordinate Empty = new Coordinate(double.NaN, double.NaN, double.NaN, double.NaN);

		#endregion

		#region Private Fields

		private double _x;
		private double _y;
		private double _z;
		private double _m;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <c>Coordinate</c> struct with X, Y ordinates.
		/// </summary>
		/// <param name="x">X-coordinate value.</param>
		/// <param name="y">Y-coordinate value.</param>
		public Coordinate(double x, double y) {
			_x = x;
			_y = y;
			_z = double.NaN;
			_m = double.NaN;
		}

		/// <summary>
		/// Initializes a new instance of the <c>Coordinate</c> struct with X, Y, Z ordinates.
		/// </summary>
		/// <param name="x">X-coordinate value.</param>
		/// <param name="y">Y-coordinate value.</param>
		/// <param name="z">Z-coordinate value.</param>
		public Coordinate(double x, double y, double z) {
			_x = x;
			_y = y;
			_z = z;
			_m = double.NaN;
		}

		/// <summary>
		/// Initializes a new instance of the <c>Coordinate</c> struct with X, Y, Z ordinates and M value.
		/// </summary>
		/// <param name="x">X-coordinate value.</param>
		/// <param name="y">Y-coordinate value.</param>
		/// <param name="z">Z-coordinate value.</param>
		/// <param name="m">Measured value associated with the Coordinate.</param>
		public Coordinate(double x, double y, double z, double m) {
			_x = x;
			_y = y;
			_z = z;
			_m = m;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the X-coordinate
		/// </summary>
		public double X {
			get { return _x; }
            set { _x = value; }
		}

		/// <summary>
		/// Gets the Y-coordinate
		/// </summary>
		public double Y {
			get { return _y; }
            set { _y = value; }
		}

		/// <summary>
		/// Gets the Z-coordinate
		/// </summary>
		public double Z {
			get { return _z; }
            set { _z = value; }
		}

		/// <summary>
		/// Gets the M value
		/// </summary>
		public double M {
			get { return _m; }
            set { _m = value; }
		}

		/// <summary>
		/// Gets a value indicating whether this coordinate has assigned <see cref="Coordinate.Z"/> coordinate.
		/// </summary>
		public bool Is3D {
			get {
				return !double.IsNaN(this.Z);
			}
		}

		/// <summary>
		/// Gets a value indicating whether this coordinate has assigned <see cref="Coordinate.M"/> value.
		/// </summary>
		public bool IsMeasured {
			get {
				return !double.IsNaN(this.M);
			}
		}

		#endregion

		#region Operators

		/// <summary>
		/// Determiens whether specific Coordinates values are equal
		/// </summary>
		/// <param name="lhs">Coordinate to compare</param>
		/// <param name="rhs">Coordinate to compare</param>
		/// <returns>true if the specified <c>Coordinate</c> values are equal; otherwise, false.</returns>
		public static bool operator ==(Coordinate lhs, Coordinate rhs) {
			return lhs.Equals(rhs);
		}

		/// <summary>
		/// Determiens whether specific Coordinate values are not equal
		/// </summary>
		/// <param name="lhs">Coordinate to compare</param>
		/// <param name="rhs">Coordinate to compare</param>
		/// <returns>true if the specified <c>Coordinate</c> values are not equal; otherwise, false.</returns>
		public static bool operator !=(Coordinate lhs, Coordinate rhs) {
			return !(lhs == rhs);
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Returns a <c>string</c> that represents the current <c>Coordinate</c>.
		/// </summary>
		/// <returns>A <c>string</c> that represents the current <c>Coordinate</c></returns>
		public override string ToString() {
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, "[{0}; {1}; {2}, {3}]", this.X, this.Y, this.Z, this.M);
		}

		/// <summary>
		/// Determines whether specific <c>object</c> instance is equal to the current <c>Coordinate</c>.
		/// </summary>
		/// <param name="obj">The <c>object</c> to compare with the current <c>Coordinate</c></param>
		/// <returns>true if the specified  <c>object</c> is equal to the current <c>Coordinate</c>; otherwise, false.</returns>
		public override bool Equals(object obj) {
			Coordinate? other = obj as Coordinate?;
			if (other == null) {
				return false;
			}

			return this.Equals(other.Value);
		}

		/// <summary>
		/// Determines whether two <c>Coordinate</c> values are equal.
		/// </summary>
		/// <param name="other">The <c>Coordinate</c> to compare with the current <c>Coordinate</c></param>
		/// <returns>true if the specified  <c>Coordinate</c> is equal to the current <c>Coordinate</c>; otherwise, false.</returns>
		public bool Equals(Coordinate other) {
			return ((this.X == other.X) || (double.IsNaN(this.X) && double.IsNaN(other.X))) &&
				((this.Y == other.Y) || (double.IsNaN(this.Y) && double.IsNaN(other.Y))) &&
				((this.Z == other.Z) || (double.IsNaN(this.Z) && double.IsNaN(other.Z))) &&
				((this.M == other.M) || (double.IsNaN(this.M) && double.IsNaN(other.M)));
		}

		/// <summary>
		/// Serves as a hash function for the <c>Coordinate</c> structure.
		/// </summary>
		/// <returns>Hash code for current Coordinate value.</returns>
		public override int GetHashCode() {
			return _x.GetHashCode() + 17 * _y.GetHashCode() + 17 * _z.GetHashCode() + 17 * _m.GetHashCode();
		}

		/// <summary>
		/// Determines whether two <c>Coordinate</c> are equal in 2D space.
		/// </summary>
		/// <param name="other">The <c>Coordinate</c> to compare with the current <c>Coordinate</c></param>
		/// <returns>true if the specified  <c>Coordinate</c> is equal to the current <c>Coordinate</c> in 2D space otherwise, false.</returns>
		public bool Equals2D(Coordinate other) {
			return ((this.X == other.X) || (double.IsNaN(this.X) && double.IsNaN(other.X))) &&
				((this.Y == other.Y) || (double.IsNaN(this.Y) && double.IsNaN(other.Y)));
		}

		#endregion
	}
}
