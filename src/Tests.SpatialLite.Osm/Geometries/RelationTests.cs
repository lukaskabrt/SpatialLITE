using Moq;
using SpatialLite.Osm;
using SpatialLite.Osm.Geometries;
using System;
using System.Collections.Generic;
using Xunit;

namespace Tests.SpatialLite.Osm.Geometries;

public class RelationTests
{
    private readonly RelationInfo _relationEmptyInfo = new(100, new TagsCollection(), new List<RelationMemberInfo>(), new EntityMetadata());
    private readonly RelationInfo _relationInfo = new(100, new TagsCollection(), new RelationMemberInfo[] { new() { Reference = 1, MemberType = EntityType.Node } }, new EntityMetadata());

    private readonly IEntityCollection<IOsmGeometry> _nodesEntityCollection;

    public RelationTests()
    {
        Mock<IEntityCollection<IOsmGeometry>> _nodesCollectionM = new();
        _nodesCollectionM.SetupGet(c => c[1, EntityType.Node]).Returns(new Node(1, 1.1, 2.2));
        _nodesCollectionM.Setup(c => c.Contains(1, EntityType.Node)).Returns(true);

        _nodesEntityCollection = _nodesCollectionM.Object;
    }

    [Fact]
    public void Constructor_int_CreatesNewRelationAndInitializesProperties()
    {
        int id = 11;
        Relation target = new(id);

        Assert.Equal(id, target.ID);
        Assert.Empty(target.Tags);
        Assert.Empty(target.Geometries);
    }

    [Fact]
    public void Constructor_int_IEnumerable_CreatesRelationWithMembersAndInitializesProperties()
    {
        int id = 11;
        var members = new RelationMember[] { new(new Node(1)), new(new Node(2)), new(new Node(3)) };
        Relation target = new(id, members);

        Assert.Equal(id, target.ID);
        Assert.Equal(members.Length, target.Geometries.Count);
        for (int i = 0; i < members.Length; i++)
        {
            Assert.Same(members[i], target.Geometries[i]);
        }

        Assert.Empty(target.Tags);
    }

    [Fact]
    public void Constructor_int_tags_CreatesRelationAndIntializesProperties()
    {
        int id = 11;
        var members = new RelationMember[] { new(new Node(1)), new(new Node(2)), new(new Node(3)) };
        TagsCollection tags = new();
        Relation target = new(id, members, tags);

        Assert.Equal(id, target.ID);
        Assert.Equal(members.Length, target.Geometries.Count);
        for (int i = 0; i < members.Length; i++)
        {
            Assert.Same(members[i], target.Geometries[i]);
        }

        Assert.Same(tags, target.Tags);
    }

    [Fact]
    public void FromRelationInfo_SetsRelationProperties()
    {
        Relation target = Relation.FromRelationInfo(_relationEmptyInfo, _nodesEntityCollection, true);

        Assert.Equal(_relationEmptyInfo.ID, target.ID);
        Assert.Same(_relationEmptyInfo.Tags, target.Tags);
        Assert.Same(_relationEmptyInfo.Metadata, target.Metadata);
    }

    [Fact]
    public void FromRelationInfo_SetsRelationMembers()
    {
        Relation target = Relation.FromRelationInfo(_relationInfo, _nodesEntityCollection, true);

        Assert.Single(target.Geometries);
        Assert.Equal(_relationInfo.Members[0].Reference, target.Geometries[0].Member.ID);
    }

    [Fact]
    public void FromRelationInfo_ThrowExceptionIfCollectionDoesntContainReferencedEntityAndThrowOnMissingIsTrue()
    {
        _relationInfo.Members[0] = new RelationMemberInfo() { Reference = 10000, MemberType = EntityType.Node };

        Assert.Throws<ArgumentException>(() => Relation.FromRelationInfo(_relationInfo, _nodesEntityCollection, true));
    }

    [Fact]
    public void FromRelationInfo_ReturnsNullIfCollectionDoesntContainReferencedEntityAndThrowOnMissingIsFalse()
    {
        _relationInfo.Members[0] = new RelationMemberInfo() { Reference = 10000, MemberType = EntityType.Node };

        Assert.Null(Relation.FromRelationInfo(_relationInfo, _nodesEntityCollection, false));
    }

    [Fact]
    public void EntityType_Returns_Relation()
    {
        Relation target = new(100);

        Assert.Equal(EntityType.Relation, target.EntityType);
    }

}
