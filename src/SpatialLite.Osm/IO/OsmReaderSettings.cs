using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpatialLite.Osm.IO {
	/// <summary>
	/// Contains settings that determine behaviour of OsmReaders.
	/// </summary>
	public class OsmReaderSettings {
		#region Private Fields

		bool _readMetadata = true;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the OsmReaderSettings class with default values.
		/// </summary>
		public OsmReaderSettings() {
			this.ReadMetadata = true;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets a value indicating whether OsmReader should read and parse entity metadata.
		/// </summary>
		public bool ReadMetadata {
			get {
				return _readMetadata;
			}
			set {
				if (this.IsReadOnly) {
					throw new InvalidOperationException("Cannot change the 'ReadMetadata' property OsmReaderSettings is read-only.");
				}

				_readMetadata = value;
			}
		}

		/// <summary>
		/// Gets or sets value indicating whether properties of the current OsmReaderSettings instance can be changed.
		/// </summary>
		protected internal bool IsReadOnly { get; internal set; }

		#endregion
	}
}
