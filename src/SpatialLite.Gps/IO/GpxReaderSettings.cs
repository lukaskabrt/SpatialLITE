using System;

namespace SpatialLite.Gps.IO {
    /// <summary>
    /// Contains settings that determine behaviour of GpxReader.
    /// </summary>
    public class GpxReaderSettings {

		bool _readMetadata = true;

		/// <summary>
		/// Initializes a new instance of the GpxReaderSettings class with default values.
		/// </summary>
		public GpxReaderSettings() {
			this.ReadMetadata = true;
		}

		/// <summary>
		/// Gets a value indicating whether GpxReader should read and parse metadata.
		/// </summary>
		public bool ReadMetadata {
			get {
				return _readMetadata;
			}
			set {
				if (this.IsReadOnly) {
					throw new InvalidOperationException("Cannot change the 'ReadMetadata' property GpxReaderSettings is read-only.");
				}

				_readMetadata = value;
			}
		}

		/// <summary>
		/// Gets or sets value indicating whether properties of the current GpxReaderSettings instance can be changed.
		/// </summary>
		protected internal bool IsReadOnly { get; internal set; }
    }
}
