using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SpatialLite.Core.API;

namespace SpatialLite.Core.Algorithms {
	/// <summary>
	/// Provides methods for determining relative position of geometries in Euclidean 2D space.
	/// </summary>
	public class Euclidean2DLocator : IGeometryLocator {
		#region Public methods

		/// <summary>
		/// Determines whether specific coordinate is on the line defined by two points.
		/// </summary>
		/// <param name="c">The coordinate to be tested</param>
		/// <param name="a">The first point of the line.</param>
		/// <param name="b">The second point of the line.</param>
		/// <param name="mode">LineMode value that specifies whether AB should be treated as infinite line or as line segment</param>
		/// <returns>true if coordinate C is on line AB, otherwise false</returns>
		public bool IsOnLine(Coordinate c, Coordinate a, Coordinate b, LineMode mode) {
			/*
					Express line in analytic form
							paramA * x  +  paramB * y  +  paramC  =  0
			*/

			double paramA = b.Y - a.Y;
			double paramB = a.X - b.X;
			double paramC = b.X * a.Y - a.X * b.Y;

			/*
					induct coordinate C into equation of the line - if equation
							paramA * c.X + paramB * c.Y + paramC = 0
					is valid, point lies on the line
			*/

			double cLineResult = paramA * c.X + paramB * c.Y + paramC;

			if (cLineResult != 0.0) {
				return false;
			}

			if (mode == LineMode.Line) {
				return true;
			}

			// compute distance of the C along vector AB
			double r;
			double dx = Math.Abs(b.X - a.X);
			double dy = Math.Abs(b.Y - a.Y);

			if (dx > dy) {
				r = (c.X - a.X) / (b.X - a.X);
			}
			else {
				r = (c.Y - a.Y) / (b.Y - a.Y);
			}

			if (r < 0.0 || r > 1.0) {
				return false;
			}
			else {
				return true;
			}
		}

		/// <summary>
		/// Determines whether specific coordinate is on the given polyline
		/// </summary>
		/// <param name="c">The coordinate to be tested</param>
		/// <param name="line">The polyline</param>
		/// <returns>true if coordinate C is on the line, otherwise false</returns>
		public bool IsOnLine(Coordinate c, ICoordinateList line) {
			for (int i = 1; i < line.Count; i++) {
				if (this.IsOnLine(c, line[i - 1], line[i], LineMode.LineSegment)) {
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Determines if specific point is in ring
		/// </summary>
		/// <param name="c">The coordinate to be tested</param>
		/// <param name="ring">The ring to locate point in</param>
		/// <returns>True if point lies inside ring, otherwise false</returns>
		public bool IsInRing(Coordinate c, ICoordinateList ring) {
			if (ring.Count < 3) {
				throw new ArgumentException("Ring must contain at least 3 points", "ring");
			}

			if (ring[0].Equals2D(ring[ring.Count - 1]) == false) {
				throw new ArgumentException("Ring does not have the same endpoints", "ring");
			}

			// determine if point is in ring using ray-tracing algorithm
			// ray is casted from c in the direction of positive x-axis
			int crossings = 0;

			for (int i = 0; i < ring.Count; i++) {
				Coordinate p1 = ring[i];
				Coordinate p2 = ring[(i + 1) % ring.Count];

				double y1 = p1.Y - c.Y;
				double y2 = p2.Y - c.Y;

				// check there can be crossing - c lies between P1 and P2 on y-axis
				if (((y1 > 0) && (y2 <= 0)) || ((y2 > 0) && (y1 <= 0))) {
					double x1 = p1.X - c.X;
					double x2 = p2.X - c.X;

					// compute intersection
					double intersection = (x1 * y2 - x2 * y1) / (y2 - y1);

					if (intersection > 0) {
						crossings++;
					}
				}
			}

			// p is inside if an odd number of crossings
			if ((crossings % 2) == 1) {
				return true;
			}
			else {
				return false;
			}
		}

		/// <summary>
		/// Determines whether two lines or line segments intersects
		/// </summary>
		/// <param name="a1">The first point of the first line</param>
		/// <param name="b1">The second point of the first line</param>
		/// <param name="line1Mode">LineMode value that specifies whether A1B1 should be treated as infinite line or as line segment</param>
		/// <param name="a2">The first point of the second line</param>
		/// <param name="b2">The second point of the second line</param>
		/// <param name="line2Mode">LineMode value that specifies whether A2B2 should be treated as infinite line or as line segment</param>
		/// <returns>true if A1B1 intersects A2B2, otherwise returns false</returns>
		public bool Intersects(Coordinate a1, Coordinate b1, LineMode line1Mode, Coordinate a2, Coordinate b2, LineMode line2Mode) {
			double paramA1 = b1.Y - a1.Y;
			double paramB1 = a1.X - b1.X;
			double paramC1 = paramA1 * a1.X + paramB1 * a1.Y;

			double paramA2 = b2.Y - a2.Y;
			double paramB2 = a2.X - b2.X;
			double paramC2 = paramA2 * a2.X + paramB2 * a2.Y;

			double det = paramA1 * paramB2 - paramA2 * paramB1;
			if (det == 0) {
				return false;
			}
			else {
				double x = (paramB2 * paramC1 - paramB1 * paramC2) / det;
				double y = (paramA1 * paramC2 - paramA2 * paramC1) / det;

				if (line1Mode == LineMode.LineSegment) {
					if (x < Math.Min(a1.X, b1.X) || x > Math.Max(a1.X, b1.X) || y < Math.Min(a1.Y, b1.Y) || y > Math.Max(a1.Y, b1.Y)) {
						return false;
					}
				}

				if (line2Mode == LineMode.LineSegment) {
					if (x < Math.Min(a2.X, b2.X) || x > Math.Max(a2.X, b2.X) || y < Math.Min(a2.Y, b2.Y) || y > Math.Max(a2.Y, b2.Y)) {
						return false;
					}
				}

				return true;
			}
		}

		/// <summary>
		/// Determines whether two polylines defined by series of coordinates intersects
		/// </summary>
		/// <param name="line1">The first polyline to test</param>
		/// <param name="line2">The second polyline to test</param>
		/// <returns>true if polylines intersets, otherwise false</returns>
		public bool Intersects(ICoordinateList line1, ICoordinateList line2) {
			//TODO implement more efficient algorithm

			for (int i = 1; i < line1.Count; i++) {
				for (int ii = 1; ii < line2.Count; ii++) {
					if (this.Intersects(line1[i - 1], line1[i], LineMode.LineSegment, line2[ii - 1], line2[ii], LineMode.LineSegment)) {
						return true;
					}
				}
			}

			return false;
		}

		#endregion
	}
}
