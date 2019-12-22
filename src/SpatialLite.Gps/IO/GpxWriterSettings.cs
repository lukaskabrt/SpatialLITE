using System;

namespace SpatialLite.Gps.IO {
    /// <summary>
    /// Contains settings that determine behaviour of the GpxWriter.
    /// </summary>
    public class GpxWriterSettings {

		bool _writeMetadata = true;
        string _generatorName;

		/// <summary>
		/// Initializes a new instance of the GpxWriterSettings class with default values.
		/// </summary>
        public GpxWriterSettings() {
		}

		/// <summary>
		/// Gets or sets a value indicating whether GpxWriter should write entity metadata.
		/// </summary>
		public bool WriteMetadata {
			get {
				return _writeMetadata;
			}
			set {
				if (this.IsReadOnly) {
                    throw new InvalidOperationException("Cannot change the 'WriteMetadata' property - GpxWriterSettings is read-only.");
				}

				_writeMetadata = value;
			}
		}

		/// <summary>
		/// Gets or sets the name of the program that will be save to the output file.
		/// </summary>
		public string GeneratorName {
			get {
				return _generatorName;
			}
			set {
				if (this.IsReadOnly) {
                    throw new InvalidOperationException("Cannot change the 'GeneratorName' property - GpxWriterSettings is read-only.");
				}

                _generatorName = value;
			}
		}

		/// <summary>
        /// Gets or sets value indicating whether properties of the current GpxWriterSettings instance can be changed.
		/// </summary>
		public bool IsReadOnly { get; internal set; }
    }
}
