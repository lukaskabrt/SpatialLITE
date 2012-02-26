using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpatialLite.Core.IO {
	/// <summary>
	/// Specifies settings that determine behaviour of the WkbWriter.
	/// </summary>
	public class WktWriterSettings {
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the WkbWriterSettings class with default values.
		/// </summary>
		public WktWriterSettings()
			: base() {
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets a value indicating whether properties of this OsmWriterSettings instance can be changed.
		/// </summary>
		protected internal bool IsReadOnly { get; internal set; }

		#endregion
	}
}
