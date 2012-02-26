using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpatialLite.Core.API {
	/// <summary>
	/// Defines properties and methodss for Collection of LineStrings.
	/// </summary>
	public interface IMultiLineString : IGeometryCollection<ILineString> {
	}
}
