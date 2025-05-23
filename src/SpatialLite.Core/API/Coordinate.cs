using System;

namespace SpatialLite.Core.API {
    /// <summary>
    /// Represents a location in the coordinate space.
    /// </summary>
    public struct Coordinate : IEquatable<Coordinate> {

		/// <summary>
		/// Represents an empty coordinate.
		/// </summary>
		/// <remarks>
		/// The empty coordinate has all coordinates equal to NaN.
		/// </remarks>
		public static Coordinate Empty = new Coordinate(double.NaN, double.NaN);

		/// <summary>
		/// Initializes a new instance of the <c>Coordinate</c> struct with X, Y ordinates.
		/// </summary>
		/// <param name="x">X-coordinate value.</param>
		/// <param name="y">Y-coordinate value.</param>
		public Coordinate(double x, double y) {
			X = x;
			Y = y;
		}

        /// <summary>
        /// Gets the X-coordinate
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Gets the Y-coordinate
        /// </summary>
        public double Y { get; set; }

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

		/// <summary>
		/// Returns a <c>string</c> that represents the current <c>Coordinate</c>.
		/// </summary>
		/// <returns>A <c>string</c> that represents the current <c>Coordinate</c></returns>
		public override string ToString() {
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, "[{0}; {1}]", this.X, this.Y);
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
				((this.Y == other.Y) || (double.IsNaN(this.Y) && double.IsNaN(other.Y)));
		}

		/// <summary>
		/// Serves as a hash function for the <c>Coordinate</c> structure.
		/// </summary>
		/// <returns>Hash code for current Coordinate value.</returns>
		public override int GetHashCode() {
			return X.GetHashCode() + 7 * Y.GetHashCode();
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
	}
}
