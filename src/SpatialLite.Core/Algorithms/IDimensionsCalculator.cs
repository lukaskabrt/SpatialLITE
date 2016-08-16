using SpatialLite.Core.API;

namespace SpatialLite.Core.Algorithms {
    /// <summary>
    /// Defines methods that a class providing distance and area calculations capabilities must implement
    /// </summary>
    public interface IDimensionsCalculator {
		/// <summary>
		/// Calculates distance between two points
		/// </summary>
		/// <param name="c1">The first point</param>
		/// <param name="c2">The second point</param>
		/// <returns>distance between two point</returns>
		double CalculateDistance(Coordinate c1, Coordinate c2);

		/// <summary>
		/// Calculates distance between a point and a line AB
		/// </summary>
		/// <param name="c">The coordinate to compute the distance for.</param>
		/// <param name="a">One point of the line.</param>
		/// <param name="b">Another point of the line.</param>
		/// <param name="mode">LineDistanceMode value that specifies whether AB should be treated as infinite line or as line segment</param>
		/// <returns> The distance from c to line AB.</returns>
		double CalculateDistance(Coordinate c, Coordinate a, Coordinate b, LineMode mode);

		/// <summary>
		/// Calculates area of the polygon specified by given vertices
		/// </summary>
		/// <param name="vertices">The vertices of the polygon</param>
		/// <returns>The area of the polygon</returns>
		double CalculateArea(ICoordinateList vertices);
	}
}
