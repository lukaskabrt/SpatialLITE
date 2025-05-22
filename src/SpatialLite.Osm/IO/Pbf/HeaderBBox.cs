using ProtoBuf;

namespace SpatialLite.Osm.IO.Pbf
{
    /// <summary>
    /// Represents rectangular envelope of data.
    /// </summary>
    [ProtoContract(Name = "HeaderBBox")]
    internal class HeaderBBox
    {

        /// <summary>
        /// Gets or sets Bootom boundary of the BBox.
        /// </summary>
        [ProtoMember(4, Name = "bottom", IsRequired = true, DataFormat = DataFormat.ZigZag)]
        public long Bottom { get; set; }

        /// <summary>
        /// Gets or sets Left boundary of the BBox.
        /// </summary>
        [ProtoMember(1, Name = "left", IsRequired = true, DataFormat = DataFormat.ZigZag)]
        public long Left { get; set; }

        /// <summary>
        /// Gets or sets Right boundary of the BBox.
        /// </summary>
        [ProtoMember(2, Name = "right", IsRequired = true, DataFormat = DataFormat.ZigZag)]
        public long Right { get; set; }

        /// <summary>
        /// Gets or sets Top boundary of the BBox.
        /// </summary>
        [ProtoMember(3, Name = "top", IsRequired = true, DataFormat = DataFormat.ZigZag)]
        public long Top { get; set; }

    }
}
