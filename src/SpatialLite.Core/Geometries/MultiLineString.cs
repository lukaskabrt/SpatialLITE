using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SpatialLite.Core.API;

namespace SpatialLite.Core.Geometries {
	/// <summary>
	/// Represents a collection of LineStrings
	/// </summary>
	public class MultiLineString : GeometryCollection<LineString>, IMultiLineString {
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the MultiLineString class that is empty and has assigned WSG84 coordinate reference system.
		/// </summary>
		public MultiLineString()
			: base() {
		}

		/// <summary>
		/// Initializes a new instance of the MultiLineString class that is empty and has assigned specified coordinate reference system.
		/// </summary>
		/// <param name="srid">The <c>SRID</c> of the coordinate reference system.</param>
		public MultiLineString(int srid)
			: base(srid) {
		}

		/// <summary>
		/// Initializes a new instance of the MultiLineString class with specified LineStrings
		/// </summary>
		/// <param name="linestrings">The collection of LineString to be copied to the new MultiLineString.</param>
		public MultiLineString(IEnumerable<LineString> linestrings)
			: base(linestrings) {
		}

		/// <summary>
		/// Initializes a new instance of the MultiLineString class with specified LineStrings and coordinate reference system
		/// </summary>
		/// <param name="srid">The <c>SRID</c> of the coordinate reference system</param> 
		/// <param name="linestrings">The collection of LineString to be copied to the new MultiLineString.</param>
		public MultiLineString(int srid, IEnumerable<LineString> linestrings)
			: base(srid, linestrings) {
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets collection of geometry obejcts from this MultiLineString as the collection of IMultiLineString objects.
		/// </summary>
		IEnumerable<ILineString> IGeometryCollection<ILineString>.Geometries {
			get { return base.Geometries; }
		}

		#endregion
	}
}
