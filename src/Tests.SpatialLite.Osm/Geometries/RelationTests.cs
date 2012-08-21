using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xunit;
using Moq;

using SpatialLite.Osm.Geometries;
using SpatialLite.Osm;

namespace Tests.SpatialLite.Osm.Geometries {
	public class RelationTests {
		RelationInfo _relationEmptyInfo = new RelationInfo(100, new TagsCollection(), new List<RelationMemberInfo>(), new EntityMetadata());
		RelationInfo _relationInfo = new RelationInfo(100, new TagsCollection(), new RelationMemberInfo[] { new RelationMemberInfo() { Reference = 1, MemberType = EntityType.Node } }, new EntityMetadata());

		IEntityCollection<IOsmGeometry> _nodesEntityCollection;

		public RelationTests() {
			Mock<IEntityCollection<IOsmGeometry>> _nodesCollectionM = new Mock<IEntityCollection<IOsmGeometry>>();
			_nodesCollectionM.SetupGet(c => c[1, EntityType.Node]).Returns(new Node(1, 1.1, 2.2));
            _nodesCollectionM.Setup(c => c.Contains(1, EntityType.Node)).Returns(true);

			_nodesEntityCollection = _nodesCollectionM.Object;
		}

		#region Constructor(ID) tests

		[Fact]
		public void Constructor_int_CreatesNewRelationAndInitializesProperties() {
			int id = 11;
			Relation target = new Relation(id);

			Assert.Equal(id, target.ID);
			Assert.Empty(target.Tags);
			Assert.Empty(target.Geometries);
		}

		#endregion

		#region Constructor(ID, Members) tests

		[Fact]
		public void Constructor_int_IEnumerable_CreatesRelationWithMembersAndInitializesProperties() {
			int id = 11;
			var members = new RelationMember[] { new RelationMember(new Node(1)), new RelationMember(new Node(2)), new RelationMember(new Node(3)) };
			Relation target = new Relation(id, members);

			Assert.Equal(id, target.ID);
			Assert.Equal(members.Length, target.Geometries.Count);
			for (int i = 0; i < members.Length; i++) {
				Assert.Same(members[i], target.Geometries[i]);
			}

			Assert.Empty(target.Tags);
		}

		#endregion

		#region Constructor(ID, Members, Tags) tests

		[Fact]
		public void Constructor_int_tags_CreatesRelationAndIntializesProperties() {
			int id = 11;
			var members = new RelationMember[] { new RelationMember(new Node(1)), new RelationMember(new Node(2)), new RelationMember(new Node(3)) };
			TagsCollection tags = new TagsCollection();
			Relation target = new Relation(id, members, tags);

			Assert.Equal(id, target.ID);
			Assert.Equal(members.Length, target.Geometries.Count);
			for (int i = 0; i < members.Length; i++) {
				Assert.Same(members[i], target.Geometries[i]);
			}

			Assert.Same(tags, target.Tags);
		}

		#endregion

		#region  FromRelationInfo(RelationInfo, IEntityCollection<IOsmGeometry>, ThrowOnMissing) tests

		[Fact]
		public void FromRelationInfo_SetsRelationProperties() {
			Relation target = Relation.FromRelationInfo(_relationEmptyInfo, _nodesEntityCollection ,true);

			Assert.Equal(_relationEmptyInfo.ID, target.ID);
			Assert.Same(_relationEmptyInfo.Tags, target.Tags);
			Assert.Same(_relationEmptyInfo.Metadata, target.Metadata);
		}

		[Fact]
		public void FromRelationInfo_SetsRelationMembers() {
			Relation target = Relation.FromRelationInfo(_relationInfo, _nodesEntityCollection, true);

			Assert.Equal(target.Geometries.Count, 1);
			Assert.Equal(_relationInfo.Members[0].Reference, target.Geometries[0].Member.ID);
		}

		[Fact]
		public void FromRelationInfo_ThrowExceptionIfCollectionDoesntContainReferencedEntityAndThrowOnMissingIsTrue() {
			_relationInfo.Members[0] = new RelationMemberInfo() { Reference = 10000, MemberType = EntityType.Node };

			Assert.Throws<ArgumentException>(() => Relation.FromRelationInfo(_relationInfo, _nodesEntityCollection, true));
		}

		[Fact]
		public void FromRelationInfo_ReturnsNullIfCollectionDoesntContainReferencedEntityAndThrowOnMissingIsFalse() {
			_relationInfo.Members[0] = new RelationMemberInfo() { Reference = 10000, MemberType = EntityType.Node };

			Assert.Null(Relation.FromRelationInfo(_relationInfo, _nodesEntityCollection, false));
		}

		#endregion

		#region EntityType property tests

		[Fact]
		public void EntityType_Returns_Relation() {
			Relation target = new Relation(100);

			Assert.Equal(EntityType.Relation, target.EntityType);
		}

		#endregion
	}
}
