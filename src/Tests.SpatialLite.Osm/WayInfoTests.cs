using SpatialLite.Osm;
using SpatialLite.Osm.Geometries;
using System;
using System.Collections.Generic;
using Xunit;

namespace Tests.SpatialLite.Osm;

public class WayInfoTests
{

    [Fact]
    public void Constructor_PropertiesWithoutEntityDetails_SetsProperties()
    {
        int id = 45;
        TagsCollection tags = new();
        List<long> nodes = new();

        WayInfo target = new(id, tags, nodes);

        Assert.Equal(EntityType.Way, target.EntityType);
        Assert.Equal(id, target.ID);
        Assert.Same(tags, target.Tags);
        Assert.Same(nodes, target.Nodes);
        Assert.Null(target.Metadata);
    }

    [Fact]
    public void Constructor_Properties_SetsProperties()
    {
        int id = 45;
        TagsCollection tags = new();
        List<long> nodes = new();
        EntityMetadata details = new();

        WayInfo target = new(id, tags, nodes, details);

        Assert.Equal(EntityType.Way, target.EntityType);
        Assert.Equal(id, target.ID);
        Assert.Same(tags, target.Tags);
        Assert.Same(nodes, target.Nodes);
        Assert.Same(details, target.Metadata);
    }

    [Fact]
    public void Constructor_Way_SetsProperties()
    {
        Way way = new(10, new Node[0], new TagsCollection()) { Metadata = new EntityMetadata() };

        WayInfo target = new(way);

        Assert.Equal(way.ID, target.ID);
        Assert.Same(way.Tags, target.Tags);
        Assert.Same(way.Metadata, target.Metadata);
    }

    [Fact]
    public void Constructor_Way_SetsNodesReferences()
    {
        Way way = new(10, new Node[] { new(1), new(2), new(3) }, new TagsCollection()) { Metadata = new EntityMetadata() };

        WayInfo target = new(way);

        Assert.Equal(way.Nodes.Count, target.Nodes.Count);
        for (int i = 0; i < way.Nodes.Count; i++)
        {
            Assert.Equal(way.Nodes[i].ID, target.Nodes[i]);
        }
    }

    [Fact]
    public void Constructor_Way_ThrowsExceptionIfWayIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new WayInfo(null));
    }
}
