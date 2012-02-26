using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpatialLite.Core.API {
	/// <summary>
	/// Defines properties and methods for Geometry collection.
	/// </summary>
	/// <typeparam name="T">The type of the objects included in the collection.</typeparam>
	public interface IGeometryCollection<out T> : IGeometry where T : IGeometry {
		/// <summary>
		/// Gets the collection of geometry objects from the IGeometryCollection.
		/// </summary>
		IEnumerable<T> Geometries { get; }
	}
}
