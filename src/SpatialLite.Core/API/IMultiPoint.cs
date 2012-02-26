using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpatialLite.Core.API {
	/// <summary>
	/// Defines properties and methods collections of points.
	/// </summary>
	public interface IMultiPoint : IGeometryCollection<IPoint> {
	}
}
