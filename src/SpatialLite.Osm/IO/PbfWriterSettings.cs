using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpatialLite.Osm.IO {
	/// <summary>
	///  Contains settings that determine behaviour of the PbfWriter.
	/// </summary>
	public class PbfWriterSettings : OsmWriterSettings {
		#region Private Fields

		private bool _useDenseFormat;
		private CompressionMode _compression;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the PbfWriterSettings class with default values.
		/// </summary>
		public PbfWriterSettings()
			: base() {
			this.UseDenseFormat = true;
			this.Compression = CompressionMode.ZlibDeflate;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets a value indicating whether PbfWriter should use dense format for serializing nodes.
		/// </summary>
		public bool UseDenseFormat {
			get {
				return _useDenseFormat;
			}
			set {
				if (this.IsReadOnly) {
					throw new InvalidOperationException("Cannot change the 'UseDenseFromat' property - PbfReaderSettings is read-only");
				}

				_useDenseFormat = value;
			}
		}

		/// <summary>
		/// Gets or sets a compression to be used by PbfWriter.
		/// </summary>
		public CompressionMode Compression {
			get {
				return _compression;
			}
			set {
				if (this.IsReadOnly) {
					throw new InvalidOperationException("Cannot change the 'Compression' property - PbfReaderSettings is read-only");
				}

				_compression = value;
			}
		}

		#endregion
	}
}
