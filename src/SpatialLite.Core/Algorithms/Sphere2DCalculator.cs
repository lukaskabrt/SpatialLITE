using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SpatialLite.Core.API;

namespace SpatialLite.Core.Algorithms {
	/// <summary>
	/// Provides methods for calculating distance and area on the surface of a sphere.
	/// </summary>
	/// <remarks>
	/// <para>
	/// All calulations ignore <see cref="Coordinate.Z"/> coordinate.
	/// </para>
	/// All coordinate are expected to be a long/lat pairs in degrees.
	/// </remarks>
	public class Sphere2DCalculator : IDimensionsCalculator {
		#region Public Constants

		/// <summary>
		/// The radius of spheric approximation of the Earth.
		/// </summary>
		public const double EarthRadius = 6371010;

		#endregion

		#region Public Constructors()

		/// <summary>
		/// Initializes a new instance of the Sphere2DCalculator that uses sphere approximation of the Earth.
		/// </summary>
		public Sphere2DCalculator() {
			this.Radius = Sphere2DCalculator.EarthRadius;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets radius of the sphere that Sphere2DCalculator uses for calculations.
		/// </summary>
		public double Radius { get; private set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Calculates distance between 2 <see cref="Coordinate"/> objects using great circle path.
		/// </summary>
		/// <param name="c1">The first coordinate.</param>
		/// <param name="c2">The second coordinate.</param>
		/// <returns>The distance of the points in units of the <see cref="Sphere2DCalculator.Radius"/> property.</returns>
		public double CalculateDistance(Coordinate c1, Coordinate c2) {
			double dLon = (c2.X - c1.X) * Math.PI / 180;
			double dLat = (c2.Y - c1.Y) * Math.PI / 180;

			double sinLat = Math.Sin(dLat / 2);
			double sinLon = Math.Sin(dLon / 2);

			double a = sinLat * sinLat + Math.Cos(c1.Y * Math.PI / 180) * Math.Cos(c2.Y * Math.PI / 180) * sinLon * sinLon;

			//length or arc in radians
			double c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));

			return c * this.Radius;
		}

		/// <summary>
		/// Calculates distance between a point and a great circle path connecting points A and B.
		/// </summary>
		/// <param name="c">The coordinate to compute the distance for.</param>
		/// <param name="a">The first point of the great circle.</param>
		/// <param name="b">The second point of the great circle.</param>
		/// <param name="mode">LineMode value that specifies whether great circle should be treated as whole circle or as arc segment between points A and B.</param>
		/// <returns> The distance from c to great circle connecting points AB in units of the <see cref="Sphere2DCalculator.Radius"/> property.</returns>
		public double CalculateDistance(Coordinate c, Coordinate a, Coordinate b, LineMode mode) {
			double bearingAB = this.CalculateBearing(a, b);
			double bearingAC = this.CalculateBearing(a, c);

			double distAC = this.CalculateDistance(a, c);

			//Sign can be used to determine if point is left/right of the great circle
			double distCircleC = Math.Abs(Math.Asin(Math.Sin(distAC / Sphere2DCalculator.EarthRadius) * Math.Sin(bearingAC - bearingAB)) * Sphere2DCalculator.EarthRadius);

			if (mode == LineMode.Line) {
				return distCircleC;
			}
			else {
				double bearingBA = this.CalculateBearing(b, a);
				double bearingBC = this.CalculateBearing(b, c);

				if (Math.Abs(bearingAC - bearingAB) > Math.PI / 2) {
					return distAC;
				}
				else if (Math.Abs(bearingBC - bearingBA) > Math.PI / 2) {
					return this.CalculateDistance(b, c);
				}
				else {
					return distCircleC;
				}
			}
		}

		/// <summary>
		/// Calculates area of the polygon on the surface of a sphere specified by given vertices.
		/// </summary>
		/// <param name="vertices">The vertices of the polygon.</param>
		/// <returns>The area of the polygon in squared units of the <see cref="Sphere2DCalculator.Radius"/> property.</returns>
		/// <remarks>Polygon is expected to be simple.</remarks>
		public double CalculateArea(ICoordinateList vertices) {
			if (vertices.Count < 3) {
				throw new ArgumentException("List must contain at least 3 vertices.", "vertices");
			}

			double area = 0;
			int maxIndex = vertices.Count - 1;
			if (vertices[0] != vertices[maxIndex]) {
				maxIndex++;
			}

			for (int i = 0; i <= maxIndex; i++) {
				area += (this.ToRadians(vertices[(i + 1) % maxIndex].X) - this.ToRadians(vertices[(i - 1) % maxIndex].X)) * Math.Sin(this.ToRadians(vertices[i % maxIndex].Y));
			}

			return Math.Abs(area * Sphere2DCalculator.EarthRadius * Sphere2DCalculator.EarthRadius);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Calculates initial course when moving from point C1 to point C2 on great circle path.
		/// </summary>
		/// <param name="c1">The first point.</param>
		/// <param name="c2">The second point</param>
		/// <returns>The bearing in radians.</returns>
		/// <remarks>The bearing changes as traveling along great circle.</remarks>
		private double CalculateBearing(Coordinate c1, Coordinate c2) {
			double dLon = (c2.X - c1.X) * Math.PI / 180;

			double y = Math.Sin(dLon) * Math.Cos(c2.Y * Math.PI / 180);
			double x = Math.Cos(c1.Y * Math.PI / 180) * Math.Sin(c2.Y * Math.PI / 180) -
							Math.Sin(c1.Y * Math.PI / 180) * Math.Cos(c2.Y * Math.PI / 180) * Math.Cos(c2.X * Math.PI / 180);

			double bearing = Math.Atan2(y, x);

			return bearing;
		}

		/// <summary>
		/// Converts angle in degrees to radians
		/// </summary>
		/// <param name="degrees">Angle in degrees</param>
		/// <returns>angle in radians</returns>
		private double ToRadians(double degrees) {
			return degrees * Math.PI / 180;
		}

		#endregion
	}
}
