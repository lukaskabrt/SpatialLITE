using System.Collections.Generic;

using ProtoBuf;

namespace SpatialLite.Osm.IO.Pbf {
    /// <summary>
    /// Represetns data transfer object used by PBF serializer for relations.
    /// </summary>
    [ProtoContract(Name = "Relation")]
	internal class PbfRelation {

		private IList<long> _memberIds = null;
		private IList<int> _rolesIndexes = null;
		private IList<PbfRelationMemberType> _types = null;

		/// <summary>
		/// Initializes a new instance of the PbfRelation class with internal fields initialized to default capacity.
		/// </summary>
		public PbfRelation() {
			_memberIds = new List<long>();
			_rolesIndexes = new List<int>();
			_types = new List<PbfRelationMemberType>();
		}

		/// <summary>
		/// Initializes a new instance of the PbfRelation class with internal fields initialized to specified capacity.
		/// </summary>
		/// <param name="capacity">The desired capacity of internal fields.</param>
		public PbfRelation(int capacity) {
			_memberIds = new List<long>(capacity);
			_rolesIndexes = new List<int>(capacity);
			_types = new List<PbfRelationMemberType>(capacity);
		}

		/// <summary>
		/// Gets or sets ID of the relation.
		/// </summary>
		[ProtoMember(1, Name = "id", IsRequired = true)]
		public long ID { get; set; }

		/// <summary>
		/// Gets or sets relation metadata.
		/// </summary>
		[ProtoMember(4, Name = "info", IsRequired = false)]
		public PbfMetadata Metadata { get; set; }

		/// <summary>
		/// Gets or sets indexes of tag's keys in string table.
		/// </summary>
		[ProtoMember(2, Name = "keys", Options = MemberSerializationOptions.Packed)]
		public IList<uint> Keys { get; set; }

		/// <summary>
		/// Gets or sets indexes of tag's values in string table.
		/// </summary>
		[ProtoMember(3, Name = "vals", Options = MemberSerializationOptions.Packed)]
		public IList<uint> Values { get; set; }

		/// <summary>
		/// Gets or sets IDs of the relation members. This propeerty is delta encoded.
		/// </summary>
		[ProtoMember(9, Name = "memids", Options = MemberSerializationOptions.Packed, DataFormat = DataFormat.ZigZag)]
		public IList<long> MemberIds {
			get { return _memberIds; }
			set { _memberIds = value; }
		}

		/// <summary>
		/// Gets or sets index of the role in string table for appropriate members.
		/// </summary>
		[ProtoMember(8, Name = "roles_sid", Options = MemberSerializationOptions.Packed)]
		public IList<int> RolesIndexes {
			get { return _rolesIndexes; }
			set { _rolesIndexes = value; }
		}

		/// <summary>
		/// Gets or sets type of the relation members.
		/// </summary>
		[ProtoMember(10, Name = "types", Options = MemberSerializationOptions.Packed)]
		public IList<PbfRelationMemberType> Types {
			get { return _types; }
			set { _types = value; }
		}
	}
}
