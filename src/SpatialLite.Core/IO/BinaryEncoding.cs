namespace SpatialLite.Core.IO {
    /// <summary>
    /// Specifies byte ordering in multibyte values.
    /// </summary>
    public enum BinaryEncoding : byte {
		/// <summary>
		/// Most significant byte first.
		/// </summary>
		BigEndian = 0,
		/// <summary>
		/// Least significant byte first.
		/// </summary>
		LittleEndian = 1
	}
}
