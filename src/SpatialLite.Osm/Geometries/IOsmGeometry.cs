using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SpatialLite.Core.API;
using SpatialLite.Osm;

namespace SpatialLite.Osm.Geometries {
	/// <summary>
	/// Represents OSM entity that implements IGeometry interface.
	/// </summary>
	public interface IOsmGeometry : IOsmEntity, IGeometry {
	}
}
