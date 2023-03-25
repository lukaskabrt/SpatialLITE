using SpatialLite.Core.Api;
using System.Collections.Generic;
using System.Linq;

namespace SpatialLite.Core.Geometries
{
    /// <summary>
    /// Represents a polygon, which may include holes.
    /// </summary>
    public class Polygon : Geometry, IPolygon
    {
        /// <summary>
        /// Initializes a new instance of the <c>Polygon</c> class in WSG84 coordinate reference system that without ExteriorRing and no InteriorRings.
        /// </summary>
        public Polygon()
            : base()
        {
            this.ExteriorRing = new CoordinateSequence();
            this.InteriorRings = new List<ICoordinateSequence>(0);
        }

        /// <summary>
        /// Initializes a new instance of the <c>Polygon</c> class with the given exterior boundary in WSG84 coordinate reference system.
        /// </summary>
        /// <param name="exteriorRing">The exterior boundary of the polygon.</param>
        public Polygon(ICoordinateSequence exteriorRing)
        {
            this.ExteriorRing = exteriorRing;
            this.InteriorRings = new List<ICoordinateSequence>(0);
        }

        /// <summary>
        /// Initializes a new instance of the <c>Polygon</c> class with the given exterior boundary in WSG84 coordinate reference system.
        /// </summary>
        /// <param name="exteriorRing">The exterior boundary of the polygon.</param>
        public Polygon(ICoordinateSequence exteriorRing, IReadOnlyList<ICoordinateSequence> interiorRings)
        {
            this.ExteriorRing = exteriorRing;
            this.InteriorRings = interiorRings;
        }

        /// <summary>
        /// Gets or sets the exterior boundary of the polygon.
        /// </summary>
        public ICoordinateSequence ExteriorRing { get; set; }

        /// <summary>
        /// Gets the exterior boundary of the polygon.
        /// </summary>
        ICoordinateSequence IPolygon.ExteriorRing => this.ExteriorRing;

        /// <summary>
        /// Gets the list of holes in the polygon.
        /// </summary>
        public IReadOnlyList<ICoordinateSequence> InteriorRings { get; private set; }

        /// <summary>
        /// Gets the list of holes in the polygon.
        /// </summary>
        IEnumerable<ICoordinateSequence> IPolygon.InteriorRings => this.InteriorRings;

        /// <summary>
        /// Computes envelope of the <c>Polygon</c> object. The envelope is defined as a minimal bounding box for a geometry.
        /// </summary>
        /// <returns>
        /// Returns an <see cref="Envelope2D"/> object that specifies the minimal bounding box of the <c>Polygon</c> object.
        /// </returns>
        public override Envelope2D GetEnvelope() => this.ExteriorRing.Count == 0 ? Envelope2D.Empty : new Envelope2D(this.ExteriorRing);

        /// <summary>
        /// Gets collection of all <see cref="Coordinate"/> of this IGeometry object
        /// </summary>
        /// <returns>the collection of all <see cref="Coordinate"/> of this object</returns>
        public override IReadOnlyList<Coordinate> GetCoordinates()
        {
            return this.ExteriorRing.Concat(this.InteriorRings.SelectMany(o => o)).ToList();
        }
    }
}
