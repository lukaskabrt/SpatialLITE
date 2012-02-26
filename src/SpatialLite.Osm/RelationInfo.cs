using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SpatialLite.Core.Geometries;
using SpatialLite.Osm.Geometries;

namespace SpatialLite.Osm {
	/// <summary>
	/// Represents information about relation.
	/// </summary>
	public class RelationInfo : IEntityInfo {
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the RelationInfo class with specified ID, Tags, Member and optionaly EntityMetadata.
		/// </summary>
		/// <param name="id">The id of the relation.</param>
		/// <param name="tags">The collection of tags associated with the relation.</param>
		/// <param name="members">The members of the relation.</param>
		/// <param name="additionalInfo">The EntityMetadata structure with additinal properties. Default value is null.</param> 
		public RelationInfo(int id, TagsCollection tags, IList<RelationMemberInfo> members, EntityMetadata additionalInfo = null) {
			this.ID = id;
			this.Tags = tags;
			this.Members = members;
			this.Metadata = additionalInfo;
		}

		/// <summary>
		/// Initializes a new instance of the RelationInfo class with data from specific Relation.
		/// </summary>
		/// <param name="source">The Relation object to copy data from.</param>
		public RelationInfo(Relation source) {
			if (source == null) {
				throw new ArgumentNullException("Source relation cannot be null", "source");
			}

			this.ID = source.ID;
			this.Tags = source.Tags;
			this.Metadata = source.Metadata;

			this.Members = new List<RelationMemberInfo>(source.Geometries.Count);
			foreach (var member in source.Geometries) {
				this.Members.Add(new RelationMemberInfo(member));
			}
		}

		#endregion
		
		#region Public Properties

		/// <summary>
		/// Gets type of the object that is represented by this IOsmGeometryInfo.
		/// </summary>
		public EntityType EntityType { 
			get { 
				return EntityType.Relation; 
			}
		}

		/// <summary>
		/// Gets ID of the relation.
		/// </summary>
		public int ID { get; set; }

		/// <summary>
		/// Gets the collection of tags associated with the relation.
		/// </summary>
		public TagsCollection Tags { get; set; }

		/// <summary>
		/// Gets list of members of the relation.
		/// </summary>
		public IList<RelationMemberInfo> Members { get; set; }

		/// <summary>
		/// Gets additional information about this RelationInfo.
		/// </summary>
		public EntityMetadata Metadata { get; set; }

		#endregion
	}
}
