using System.Linq;

using SpatialLite.Core.API;
using SpatialLite.Core.Algorithms;

namespace SpatialLite.Core
{
    /// <summary>
    /// Provides methods for measuring geometry objects.
    /// </summary>
    public class Measurements
    {

        private static readonly Measurements _euclidean2D;
        private static readonly Measurements _sphere2D;

        /// <summary>
        /// Initializes static members of the Measurements class.
        /// </summary>
        static Measurements()
        {
            _euclidean2D = new Measurements(new Euclidean2DCalculator());
            _sphere2D = new Measurements(new Sphere2DCalculator());
        }

        /// <summary>
        /// Initializes a new instance of the Measurements class that uses specific IDistanceCalculator.
        /// </summary>
        /// <param name="distanceCalculator">The IDistance calculator to be used.</param>
        public Measurements(IDimensionsCalculator distanceCalculator)
        {
            this.DimensionsCalculator = distanceCalculator;
        }

        /// <summary>
        /// Gets Measurments class instance that uses 2D Euclidean space.
        /// </summary>
        public static Measurements Euclidean2D
        {
            get
            {
                return _euclidean2D;
            }
        }

        /// <summary>
        /// Gets Measurements class instance that uses sphere approximation of the Earth and ignores Z coordinates.
        /// </summary>
        public static Measurements Sphere2D
        {
            get
            {
                return _sphere2D;
            }
        }

        /// <summary>
        /// Gets IDistanceCalculator object used by this Measurements to compute dimensions of geometries.
        /// </summary>
        public IDimensionsCalculator DimensionsCalculator { get; private set; }

        /// <summary>
        /// Computes distance between two points
        /// </summary>
        /// <param name="c1">The first point</param>
        /// <param name="c2">The second point</param>
        /// <returns>distance between two point</returns>
        public double ComputeDistance(Coordinate c1, Coordinate c2)
        {
            return this.DimensionsCalculator.CalculateDistance(c1, c2);
        }

        /// <summary>
        /// Computes distance of a point from LineString
        /// </summary>
        /// <param name="point">The point</param>
        /// <param name="linestring">The LineString</param>
        /// <returns>The distance of the point from the LineString</returns>
        public double ComputeDistance(IPoint point, ILineString linestring)
        {
            if (point.Position == Coordinate.Empty || linestring.Coordinates.Count == 0)
            {
                return double.NaN;
            }

            double minDistance = double.PositiveInfinity;

            for (int i = 1; i < linestring.Coordinates.Count; i++)
            {
                double distance = this.DimensionsCalculator.CalculateDistance(point.Position, linestring.Coordinates[i - 1], linestring.Coordinates[i], LineMode.LineSegment);
                if (distance < minDistance)
                {
                    minDistance = distance;
                }
            }

            return minDistance;
        }

        /// <summary>
        /// Computes distance of a point from MultiLineString
        /// </summary>
        /// <param name="point">The point</param>
        /// <param name="multilinestring">The MultiLineString</param>
        /// <returns>The distance of the point from the MultiLineString</returns>
        public double ComputeDistance(IPoint point, IMultiLineString multilinestring)
        {
            if (point.Position == Coordinate.Empty || multilinestring.Geometries.Count() == 0)
            {
                return double.NaN;
            }

            double minDistance = double.PositiveInfinity;
            foreach (var linestring in multilinestring.Geometries)
            {
                double distance = this.ComputeDistance(point, linestring);
                if (distance < minDistance)
                {
                    minDistance = distance;
                }
            }

            return minDistance;
        }

        /// <summary>
        /// Computes distance between two points
        /// </summary>
        /// <param name="p1">The first point</param>
        /// <param name="p2">The second point</param>
        /// <returns>distance between two point</returns>
        public double ComputeDistance(IPoint p1, IPoint p2)
        {
            if (p1.Position == Coordinate.Empty || p2.Position == Coordinate.Empty)
            {
                return double.NaN;
            }

            return this.DimensionsCalculator.CalculateDistance(p1.Position, p2.Position);
        }

        /// <summary>
        /// Compute length of the linestring
        /// </summary>
        /// <param name="line">The linestring to measure</param>
        /// <returns>The length of the Linestring</returns>
        public double ComputeLength(ILineString line)
        {
            double length = 0;
            for (int i = 1; i < line.Coordinates.Count; i++)
            {
                length += this.DimensionsCalculator.CalculateDistance(line.Coordinates[i - 1], line.Coordinates[i]);
            }

            return length;
        }

        /// <summary>
        /// Compute length of all linestrings in specific multilinestring
        /// </summary>
        /// <param name="multilinestring">The MultiLineString to measure</param>
        /// <returns>The length of the Linestring</returns>
        public double ComputeLength(IMultiLineString multilinestring)
        {
            double length = 0;

            foreach (var line in multilinestring.Geometries)
            {
                length += this.ComputeLength(line);
            }

            return length;
        }

        /// <summary>
        /// Computes area of the Polygon
        /// </summary>
        /// <param name="polygon">Polygon to be measured</param>
        /// <returns>The area of the Polygon</returns>
        public double ComputeArea(IPolygon polygon)
        {
            double area = this.DimensionsCalculator.CalculateArea(polygon.ExteriorRing);

            foreach (var interiorRing in polygon.InteriorRings)
            {
                area -= this.DimensionsCalculator.CalculateArea(interiorRing);
            }

            return area;
        }

        /// <summary>
        /// Computes area of the MultiPolygon
        /// </summary>
        /// <param name="multiPolygon">MultiPolygon to be measured</param>
        /// <returns>The area of the MultiPolygon</returns>
        public double ComputeArea(IMultiPolygon multiPolygon)
        {
            double area = 0;

            foreach (var polygon in multiPolygon.Geometries)
            {
                area += this.ComputeArea(polygon);
            }

            return area;
        }
    }
}
