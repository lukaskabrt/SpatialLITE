﻿namespace SpatialLite.Osm.IO
{
    /// <summary>
    /// Defines compressions that can be used in the PBF format.
    /// </summary>
    public enum CompressionMode
    {
        /// <summary>
        /// No compresion is used.
        /// </summary>
        None,

        /// <summary>
        /// Zlib compression.
        /// </summary>
        ZlibDeflate
    }
}
