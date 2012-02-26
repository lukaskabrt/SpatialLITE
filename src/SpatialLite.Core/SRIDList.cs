using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpatialLite.Core {
	/// <summary>
	/// Contains list of SRIDs for common coordinate reference systems.
	/// </summary>
	public static class SRIDList {
		/// <summary>
		/// Unspecified coordinate reference system.
		/// </summary>
		public static int Unspecified = 0;
		
		/// <summary>
		/// WSG84 coordinate reference system.
		/// </summary>
		public static int WSG84 = 4326;
	}
}
