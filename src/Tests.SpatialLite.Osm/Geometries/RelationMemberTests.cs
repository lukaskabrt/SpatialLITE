using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xunit;
using Moq;

using SpatialLite.Osm.Geometries;
using SpatialLite.Core.API;
using SpatialLite.Osm;

namespace Tests.SpatialLite.Osm.Geometries {
	public class RelationMemberTests {
		IEntityCollection<IOsmGeometry> _nodesEntityCollection;

		public RelationMemberTests() {
			Mock<IEntityCollection<IOsmGeometry>> _nodesCollectionM = new Mock<IEntityCollection<IOsmGeometry>>();
			_nodesCollectionM.SetupGet(c => c[1, EntityType.Node]).Returns(new Node(1, 1.1, 2.2));
			_nodesCollectionM.Setup(c => c.Contains(1, EntityType.Node)).Returns(true);
			_nodesEntityCollection = _nodesCollectionM.Object;
		}

		#region Constructor(Member) tests

		[Fact]
		public void Constructor_Member_CreatesNewRelationMembeAndSetsMember() {
			Node member = new Node(11);
			RelationMember target = new RelationMember(member);

			Assert.Same(member, target.Member);
			Assert.True(string.IsNullOrEmpty(target.Role));
		}

		[Fact]
		public void Constructor_Member_ThrowsExceptionIfMemberIsNull() {
			Assert.Throws<ArgumentNullException>(() => new RelationMember(null));
		}

		#endregion

		#region Constructor(Member, Role) tests

		[Fact]
		public void Constructor_Member_Role_CreatesRelationMemberAndSetsMemberAndRole() {
			Node member = new Node(11);
			string role = "role";
			RelationMember target = new RelationMember(member, role);

			Assert.Same(member, target.Member);
			Assert.Equal(role, target.Role);
		}

		[Fact]
		public void Constructor_Member_Role_ThrowsExceptionIfMemberIsNull() {
			Assert.Throws<ArgumentNullException>(() => new RelationMember(null));
		}

		#endregion

		#region FromRelationMemberInfo(RelationMemberInfo, IEntityCollection<IOsmGeometry>, ThrowOnMissing) tests

		[Fact]
		public void FromRelationMemberInfo_ThrowExceptionIfTypeIsUnknown() {
			RelationMemberInfo info = new RelationMemberInfo() { Reference = 1, MemberType = EntityType.Unknown, Role = "role" };

			Assert.Throws<ArgumentException>(() => RelationMember.FromRelationMemberInfo(info, _nodesEntityCollection, true));
		}

		[Fact]
		public void FromRelationMemberInfo_CreatesRelationMember() {
			RelationMemberInfo info = new RelationMemberInfo() { Reference = 1, MemberType = EntityType.Node, Role = "role" };
			RelationMember target = RelationMember.FromRelationMemberInfo(info, _nodesEntityCollection, true);

			Assert.Equal(info.Reference, target.Member.ID);
			Assert.Equal(info.Role, target.Role);
			Assert.Equal(info.MemberType, target.MemberType);
		}

		[Fact]
		public void FromRelationMemberInfo_ThrowExceptionIfReferencedEntityIsNotAvailable() {
			RelationMemberInfo info = new RelationMemberInfo() { Reference = 10000, MemberType = EntityType.Node, Role = "role" };

			Assert.Throws<ArgumentException>(() => RelationMember.FromRelationMemberInfo(info, _nodesEntityCollection, true));
		}

		[Fact]
		public void FromRelationMemberInfo_ReturnsNullIfReferencedEntityIsNotAvailableAndThrowOnMissingIsFalse() {
			RelationMemberInfo info = new RelationMemberInfo() { Reference = 10000, MemberType = EntityType.Node, Role = "role" };

			Assert.Null(RelationMember.FromRelationMemberInfo(info, _nodesEntityCollection, false));
		}

		[Fact]
		public void FromRelationMemberInfo_ThrowExceptionIfReferencedEntityTypeDoesntMatchMemberType() {
			RelationMemberInfo info = new RelationMemberInfo() { Reference = 1, MemberType = EntityType.Way, Role = "role" };

			Assert.Throws<ArgumentException>(() => RelationMember.FromRelationMemberInfo(info, _nodesEntityCollection, true));
		}

		#endregion

		#region Is3D tests

		[Fact]
		public void Is3D_GetsTrueFor3DMember() {
			Mock<Node> memberM = new Mock<Node>(11);
			memberM.SetupGet(property => property.Is3D).Returns(true);

			RelationMember target = new RelationMember(memberM.Object);

			Assert.Equal(memberM.Object.Is3D, target.Is3D);
		}

		[Fact]
		public void Is3D_GetsFalseFor2DMember() {
			Mock<Node> memberM = new Mock<Node>(11);
			memberM.SetupGet(property => property.Is3D).Returns(false);

			RelationMember target = new RelationMember(memberM.Object);

			Assert.Equal(memberM.Object.Is3D, target.Is3D);
		}

		#endregion

		#region IsMeasured tests

		[Fact]
		public void IsMeasured_GetsTrueForMeasuredMember() {
			Mock<Node> memberM = new Mock<Node>(11);
			memberM.SetupGet(property => property.IsMeasured).Returns(true);

			RelationMember target = new RelationMember(memberM.Object);

			Assert.Equal(memberM.Object.IsMeasured, target.IsMeasured);
		}

		[Fact]
		public void IsMeasured_GetFalseForNonMeasuredMember() {
			Mock<Node> memberM = new Mock<Node>(11);
			memberM.SetupGet(property => property.IsMeasured).Returns(false);

			RelationMember target = new RelationMember(memberM.Object);

			Assert.Equal(memberM.Object.IsMeasured, target.IsMeasured);
		}

		#endregion

		#region MemberType Tests

		[Fact]
		public void MemberType_ReturnsCorrectValueForNode() {
			RelationMember target = new RelationMember(new Node(11));

			Assert.Equal(EntityType.Node, target.MemberType);
		}

		[Fact]
		public void MemberType_ReturnsCorrectValueForWay() {
			RelationMember target = new RelationMember(new Way(11));

			Assert.Equal(EntityType.Way, target.MemberType);
		}

		[Fact]
		public void MemberType_ReturnsCorrectValueForRelation() {
			RelationMember target = new RelationMember(new Relation(11));

			Assert.Equal(EntityType.Relation, target.MemberType);
		}

		#endregion

		#region GetEnvelope Tests

		[Fact]
		public void GetEnvelopeReturnsMembersEnvelope() {
			Envelope expectedEnvelope = new Envelope(new Coordinate(1.1, 2.2));
			Mock<Way> member = new Mock<Way>(11);
			member.Setup(function => function.GetEnvelope()).Returns(expectedEnvelope);

			RelationMember target = new RelationMember(member.Object);

			Assert.Same(expectedEnvelope, target.GetEnvelope());
		}

		#endregion

		#region GetBoundary() tests

		//TODO implement test for GetBoundary()

		#endregion	
	}
}
