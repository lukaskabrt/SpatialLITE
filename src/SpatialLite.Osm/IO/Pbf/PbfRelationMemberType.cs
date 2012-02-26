using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpatialLite.Osm.IO.Pbf {
	/// <summary>
	/// Defines possible types of the relation member.
	/// </summary>
	public enum PbfRelationMemberType {
		/// <summary>
		/// Relation member is Node.
		/// </summary>
		Node,
		/// <summary>
		/// Relation member is Way.
		/// </summary>
		Way,
		/// <summary>
		/// Relation member is Relation.
		/// </summary>
		Relation
	}
}
