using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SpatialLite.Osm;
using SpatialLite.Osm.Geometries;

namespace SpatialLite.Osm.IO {
	/// <summary>
	/// Defines functions and properties for classes that can write OSM entities to various destinations.
	/// </summary>
	public interface IOsmWriter : IDisposable {
		/// <summary>
		/// Writes entity to the destination.
		/// </summary>
		/// <param name="entity">The entity to write.</param>
		void Write(IOsmGeometry entity);

		/// <summary>
		/// Writes entity info object to the target.
		/// </summary>
		/// <param name="info">The entity info object to wrirte.</param>
		void Write(IEntityInfo info);

		/// <summary>
		/// Clears internal buffers and causes any buffered data to be written to the unerlaying storage.
		/// </summary>
		void Flush();
	}
}
