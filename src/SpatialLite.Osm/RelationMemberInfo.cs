using SpatialLite.Osm.Geometries;

namespace SpatialLite.Osm {
    /// <summary>
    /// Represents information about relation member.
    /// </summary>
    public struct RelationMemberInfo {
		#region Public Fields

		/// <summary>
		/// The type of the member (node, way, relation)
		/// </summary>
		public EntityType MemberType;

		/// <summary>
		/// The role of the member in relation
		/// </summary>
		public string Role;

		/// <summary>
		/// The ID of the member entity
		/// </summary>
		public int Reference;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the RelationMemberInfo structure with values from specific RelationMember.
		/// </summary>
		/// <param name="source">The RelationMember object to copy valyes from.</param>
		public RelationMemberInfo(RelationMember source) {
			this.Reference = source.Member.ID;
			this.MemberType = source.MemberType;
			this.Role = source.Role;
		}

		#endregion
	}
}
