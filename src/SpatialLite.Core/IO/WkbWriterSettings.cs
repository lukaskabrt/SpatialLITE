using System;

namespace SpatialLite.Core.IO {
    /// <summary>
    /// Contains settings that determine behaviour of the WkbWriter
    /// </summary>
    public class WkbWriterSettings {
		#region Private Filds

		private BinaryEncoding _encoding;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the WkbWriterSettings class with default values.
		/// </summary>
		public WkbWriterSettings()
			: base() {
				this.Encoding = BinaryEncoding.LittleEndian;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets a encoding that <c>WkbWriter</c> will use for writing geometries.
		/// </summary>
		/// <remarks>
		/// BigEnndian encoding is not supported in current version of the <see cref="WkbWriter"/> class.
		/// </remarks>
		public BinaryEncoding Encoding {
			get {
				return _encoding;
			}

			set {
				if (this.IsReadOnly) {
					throw new InvalidOperationException("Cannot change the 'Encoding' property. The WkbWriterSettings instance is read-only");
				}

				_encoding = value;
			}
		}

		/// <summary>
		/// Gets a value indicating whether properties of this OsmWriterSettings instance can be changed.
		/// </summary>
		protected internal bool IsReadOnly { get; internal set; }

		#endregion
	}
}
