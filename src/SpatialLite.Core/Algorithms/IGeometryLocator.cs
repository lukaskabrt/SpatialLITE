using SpatialLite.Core.API;

namespace SpatialLite.Core.Algorithms {
    /// <summary>
    /// Defines methods that a class providing relative geometry position computation capabilities must implement.
    /// </summary>
    public interface  IGeometryLocator {
		/// <summary>
		/// Determines whether specific coordinate is on line defined by two points.
		/// </summary>
		/// <param name="c">The coordinate to be tested.</param>
		/// <param name="a">The first point of the line.</param>
		/// <param name="b">The second point of the line.</param>
		/// <param name="mode">LineMode value that specifies whether AB should be treated as infinite line or as line segment.</param>
		/// <returns>true if coordinate C is on line AB, otherwise false.</returns>
		bool IsOnLine(Coordinate c, Coordinate a, Coordinate b, LineMode mode);

		/// <summary>
		/// Determines whether specific coordinate is on the given polyline.
		/// </summary>
		/// <param name="c">The coordinate to be tested.</param>
		/// <param name="line">The polyline.</param>
		/// <returns>true if coordinate C is on the line, otherwise false.</returns>
		bool IsOnLine(Coordinate c, ICoordinateList line);

		/// <summary>
		/// Determines whether specific point is in ring.
		/// </summary>
		/// <param name="c">The coordinate to be tested.</param>
		/// <param name="ring">The ring to locate point in.</param>
		/// <returns>True if point lies inside ring, otherwise false.</returns>
		bool IsInRing(Coordinate c, ICoordinateList ring);

		/// <summary>
		/// Determines whether two lines or line segments defined by two points each intersects.
		/// </summary>
		/// <param name="a1">The first point of the first line.</param>
		/// <param name="b1">The second point of the first line.</param>
		/// <param name="line1Mode">LineMode value that specifies whether A1B1 should be treated as infinite line or as line segment.</param>
		/// <param name="a2">The first point of the second line.</param>
		/// <param name="b2">The second point of the second line.</param>
		/// <param name="line2Mode">LineMode value that specifies whether A2B2 should be treated as infinite line or as line segment.</param>
		/// <returns>true if A1B1 intersects A2B2, otherwise returns false.</returns>
		bool Intersects(Coordinate a1, Coordinate b1, LineMode line1Mode, Coordinate a2, Coordinate b2, LineMode line2Mode);

		/// <summary>
		/// Determines whether two polylines defined by series of coordinates intersects.
		/// </summary>
		/// <param name="line1">The first polyline to test.</param>
		/// <param name="line2">The second polyline to test.</param>
		/// <returns>true if polylines intersets, otherwise false.</returns>
		bool Intersects(ICoordinateList line1, ICoordinateList line2);
	}
}
