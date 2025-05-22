using System.Collections.Generic;

using ProtoBuf;

namespace SpatialLite.Osm.IO.Pbf
{
    /// <summary>
    /// Represetns data transfer object used by PBF serializer for nodes saved in dense format.
    /// </summary>
    [ProtoContract(Name = "DenseNodes")]
    internal class PbfDenseNodes
    {

        private IList<long> _id = null;
        private IList<long> _latitude = null;
        private IList<long> _longitude = null;
        private IList<int> _keysVals = null;

        /// <summary>
        /// Initializes a new instance of the DenseNodes class with internal fields initialized to default capacity.
        /// </summary>
        public PbfDenseNodes()
        {
            _id = new List<long>();
            _latitude = new List<long>();
            _longitude = new List<long>();
            _keysVals = new List<int>();
        }

        /// <summary>
        /// Initializes a new instance of the DenseInfo class with internal fields initialized to specified capacity.
        /// </summary>
        /// <param name="capacity">The desired capacity of internal fields.</param>
        public PbfDenseNodes(int capacity)
        {
            _id = new List<long>(capacity);
            _latitude = new List<long>(capacity);
            _longitude = new List<long>(capacity);
            _keysVals = new List<int>(capacity);
        }

        /// <summary>
        /// Gets or sets ids of the nodes. This property is delta encoded.
        /// </summary>
        [ProtoMember(1, Name = "id", IsRequired = true, DataFormat = DataFormat.ZigZag, Options = MemberSerializationOptions.Packed)]
        public IList<long> Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// Gets or sets latitudes of the nodes as number of granularity steps from the LatOffset. This property is delta encoded.
        /// </summary>
        /// <example>
        /// double nodeLat = 1E-09 * (block.LatOffset + (block.Granularity * Latitude));
        /// </example>
        [ProtoMember(8, Name = "lat", IsRequired = true, DataFormat = DataFormat.ZigZag, Options = MemberSerializationOptions.Packed)]
        public IList<long> Latitude
        {
            get { return _latitude; }
            set { _latitude = value; }
        }

        /// <summary>
        /// Gets or sets longitude of the nodes as number of granularity steps from the LonOffset. This property is delta encoded.
        /// </summary>
        /// <example>
        /// double nodeLon = 1E-09 * (block.LonOffset + (block.Granularity * Longitude));
        /// </example>
        [ProtoMember(9, Name = "lon", IsRequired = true, DataFormat = DataFormat.ZigZag, Options = MemberSerializationOptions.Packed)]
        public IList<long> Longitude
        {
            get { return _longitude; }
            set { _longitude = value; }
        }

        /// <summary>
        /// Gets or sets entities metadata encoded in the DenseInfo object
        /// </summary>
        [ProtoMember(5, Name = "denseinfo", IsRequired = false)]
        public PbfDenseMetadata DenseInfo { get; set; }

        /// <summary>
        /// Gets or sets tags for nodes.
        /// </summary>
        /// <remarks>
        /// Tags are saved as (KeyIndex, ValueIndex) pairs. Tags for concesutive nodes are seprated by 0.
        /// </remarks>
        [ProtoMember(10, Name = "keys_vals", Options = MemberSerializationOptions.Packed)]
        public IList<int> KeysVals
        {
            get { return _keysVals; }
            set { _keysVals = value; }
        }
    }
}
