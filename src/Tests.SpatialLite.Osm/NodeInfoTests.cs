using SpatialLite.Osm;
using SpatialLite.Osm.Geometries;
using System;
using Xunit;

namespace Tests.SpatialLite.Osm;

public class NodeInfoTests
{

    [Fact]
    public void Constructor_PropertiesWithoutEntityDetails_SetsProperties()
    {
        int id = 15;
        double latitude = 15.4;
        double longitude = -23.7;
        TagsCollection tags = new TagsCollection();

        NodeInfo target = new NodeInfo(id, latitude, longitude, tags);

        Assert.Equal(EntityType.Node, target.EntityType);
        Assert.Equal(id, target.ID);
        Assert.Equal(latitude, target.Latitude);
        Assert.Equal(longitude, target.Longitude);
        Assert.Same(tags, target.Tags);
        Assert.Null(target.Metadata);
    }

    [Fact]
    public void Constructor_Properties_SetsProperties()
    {
        int id = 15;
        double latitude = 15.4;
        double longitude = -23.7;
        TagsCollection tags = new TagsCollection();
        EntityMetadata details = new EntityMetadata();

        NodeInfo target = new NodeInfo(id, latitude, longitude, tags, details);

        Assert.Equal(EntityType.Node, target.EntityType);
        Assert.Equal(id, target.ID);
        Assert.Equal(latitude, target.Latitude);
        Assert.Equal(longitude, target.Longitude);
        Assert.Same(tags, target.Tags);
        Assert.Same(details, target.Metadata);
    }

    [Fact]
    public void Constructor_Node_SetsProperties()
    {
        Node node = new Node(1, 10.1, 12.1, new TagsCollection()) { Metadata = new EntityMetadata() };

        NodeInfo target = new NodeInfo(node);

        Assert.Equal(node.ID, target.ID);
        Assert.Equal(node.Position.X, target.Longitude);
        Assert.Equal(node.Position.Y, target.Latitude);
        Assert.Same(node.Tags, target.Tags);
        Assert.Same(node.Metadata, target.Metadata);
    }

    [Fact]
    public void Constructor_Node_ThrowsExceptionIfNodeIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new NodeInfo(null));
    }
}
