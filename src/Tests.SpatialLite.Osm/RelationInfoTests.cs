using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xunit;

using SpatialLite.Osm.Geometries;
using SpatialLite.Osm;

namespace Tests.SpatialLite.Osm {
	public class RelationMemberTests {
		#region Constructor(ID, Tags, Members, Metadata) tests

		[Fact]
		public void Constructor_PropertiesWithoutEntityDetails_SetsProperties() {
			int id = 45;
			TagsCollection tags = new TagsCollection();
			List<RelationMemberInfo> members = new List<RelationMemberInfo>();

			RelationInfo target = new RelationInfo(id, tags, members);

			Assert.Equal(EntityType.Relation, target.EntityType);
			Assert.Equal(id, target.ID);
			Assert.Same(tags, target.Tags);
			Assert.Same(members, target.Members);
			Assert.Null(target.Metadata);
		}

		[Fact]
		public void Constructor_Properties_SetsProperties() {
			int id = 45;
			TagsCollection tags = new TagsCollection();
			List<RelationMemberInfo> members = new List<RelationMemberInfo>();
			EntityMetadata details = new EntityMetadata();

			RelationInfo target = new RelationInfo(id, tags, members, details);

			Assert.Equal(EntityType.Relation, target.EntityType);
			Assert.Equal(id, target.ID);
			Assert.Same(tags, target.Tags);
			Assert.Same(members, target.Members);
			Assert.Same(details, target.Metadata);
		}

		#endregion

		#region Constructor(Relation) tests

		[Fact]
		public void Constructor_Relation_SetsProperties() {
			Relation relation = new Relation(100, new RelationMember[0], new TagsCollection()) { Metadata = new EntityMetadata() };

			RelationInfo target = new RelationInfo(relation);

			Assert.Equal(relation.ID, target.ID);
			Assert.Same(relation.Tags, target.Tags);
			Assert.Same(relation.Metadata, target.Metadata);
		}

		[Fact]
		public void Constructor_Relation_SetsRelationMembers() {
			Relation relation = new Relation(100, new RelationMember[] { new RelationMember(new Node(1)), new RelationMember(new Node(2)) }, new TagsCollection()) { Metadata = new EntityMetadata() };

			RelationInfo target = new RelationInfo(relation);

			Assert.Equal(relation.Geometries.Count, target.Members.Count);
			for (int i = 0; i < relation.Geometries.Count; i++) {
				Assert.Equal(relation.Geometries[i].Member.ID, target.Members[i].Reference);
			}
		}

		[Fact]
		public void Constructor_Relation_ThrowExceptionIfRelationIsNull() {
			Assert.Throws<ArgumentNullException>(() => new RelationInfo(null));
		}

		#endregion
	}
}
