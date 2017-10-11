using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SpatialLite.Core.Geometries;

namespace SpatialLite.Osm.Geometries {
	/// <summary>
	/// Represents OSM relation.
	/// </summary>
	public class Relation : GeometryCollection<RelationMember>, IOsmGeometry {
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the Relation class with the specified ID.
		/// </summary>
		/// <param name="id">The ID of the Relation.</param>
		public Relation(long id)
			: this(id, new RelationMember[] { }, new TagsCollection()) {
		}

		/// <summary>
		/// Initializes a new instance of the Relation class with the specified ID and Members.
		/// </summary>
		/// <param name="id">The ID of the Relation.</param>
		/// <param name="members">The memberes of the relation.</param>
		public Relation(long id, IEnumerable<RelationMember> members)
			: this(id, members, new TagsCollection()) {
		}

		/// <summary>
		/// Initializes a new instance of the Relation class with the specified ID, Members and tags.
		/// </summary>
		/// <param name="id">The ID of the Relation.</param>
		/// <param name="members">The memberes of the relation.</param>
		/// <param name="tags">The collectoin of tags associated with the relation.</param>
		public Relation(long id, IEnumerable<RelationMember> members, TagsCollection tags)
			: base(members) {
			this.ID = id;
			this.Tags = tags;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets ID of the Relation.
		/// </summary>
		public long ID { get; set; }

		/// <summary>
		/// Gets or sets the collection of tags associated with the Relation.
		/// </summary>
		public TagsCollection Tags { get; set; }

		/// <summary>
		/// Gets or sets metadata of the Relation.
		/// </summary>
		public EntityMetadata Metadata { get; set; }

		/// <summary>
		/// Gets type of this entity.
		/// </summary>
		public EntityType EntityType {
			get { return EntityType.Relation; }
		}

		#endregion

		#region Public Static Methods

		/// <summary>
		/// Creates a new instance of the Relation class with data from RelationInfo object.
		/// </summary>
		/// <param name="info">The RelationInfo object that contains data about relation.</param>
		/// <param name="entities">The collection of entities that can be referenced by the relation.</param>
		/// <param name="throwOnMissing">bool value indicating whether references to the missing entities should cause exception.</param>
		/// <returns>The Relation object created from RelationInfo or null if referenced entity is missing.</returns>
        public static Relation FromRelationInfo(RelationInfo info, IEntityCollection<IOsmGeometry> entities, bool throwOnMissing) {
			Relation result = new Relation(info.ID) { Tags = info.Tags, Metadata = info.Metadata };

			result.Geometries.Capacity = info.Members.Count;
			foreach (var memberInfo in info.Members) {
				RelationMember member = RelationMember.FromRelationMemberInfo(memberInfo, entities, throwOnMissing);
				if (member == null) {
					return null;
				}

				result.Geometries.Add(member);
			}

			return result;
		}

		#endregion
	}
}
