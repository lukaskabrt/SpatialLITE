using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ProtoBuf;

namespace SpatialLite.Osm.IO.Pbf {
	/// <summary>
	/// Represetns data transfer object used by PBF serializer for changesets.
	/// </summary>
	[Serializable, ProtoContract(Name = "ChangeSet")]
	public class PbfChangeset {
		#region Public Properties

		/// <summary>
		/// Gets or sets id of the changeset.
		/// </summary>
		[ProtoMember(1, IsRequired = true, Name = "id")]
		public long ID { get; set; }

		#endregion
	}
}
