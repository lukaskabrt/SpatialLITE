using SpatialLite.Core.Api;
using System.Collections.Generic;
using System.Linq;

namespace SpatialLite.Core.Geometries
{
    /// <summary>
    /// Represents generic collection of geometry objects.
    /// </summary>
    /// <typeparam name="T">The type of objects in the collection</typeparam>
    public class GeometryCollection<T> : Geometry, IGeometryCollection<T> where T : IGeometry
    {
        private readonly List<T> _geometries;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryCollection{T}"/> class that is empty.
        /// </summary>
        public GeometryCollection()
            : base()
        {
            _geometries = new List<T>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryCollection{T}"/> class and fills it with specified geometries.
        /// </summary>
        /// <param name="geometries">Geometry objects to be added to the collection</param>
        public GeometryCollection(IEnumerable<T> geometries)
            : base()
        {
            _geometries = new List<T>(geometries);
        }

        /// <summary>
        /// Gets the list of IGeometry objects in this collection
        /// </summary>
        public List<T> Geometries => _geometries;

        /// <summary>
        /// Gets collection of geometry objects from this <see cref="GeometryCollection{T}"/> as the collection of <see cref="IGeometry"/> objects.
        /// </summary>
        IEnumerable<T> IGeometryCollection<T>.Geometries => _geometries;

        /// <summary>
        /// Gets collection of all <see cref="Coordinate"/> of this IGeometry object
        /// </summary>
        /// <returns>the collection of all <see cref="Coordinate"/> of this object</returns>
        public override IReadOnlyList<Coordinate> GetCoordinates()
        {
            return this.Geometries.SelectMany(o => o.GetCoordinates()).ToList();
        }
    }
}
