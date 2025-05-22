using System;
using System.Linq;
using System.IO;

using Xunit;

using SpatialLite.Osm.Geometries;
using System.Xml.Linq;
using SpatialLite.Osm.IO;
using SpatialLite.Osm;

namespace Tests.SpatialLite.Osm.IO;

public class OsmXmlWriterTests
{
    //resolution for default granularity
    private const double _resolution = 1E-07;

    private EntityMetadata _details;
    private NodeInfo _node, _nodeTags, _nodeProperties;
    private WayInfo _way, _wayTags, _wayProperties, _wayWithoutNodes;
    private RelationInfo _relationNode, _relationWay, _relationRelation, _relationNodeProperties, _relationTags;

    public OsmXmlWriterTests()
    {
        _details = new EntityMetadata()
        {
            Timestamp = new DateTime(2010, 11, 19, 22, 5, 56, DateTimeKind.Utc),
            Uid = 127998,
            User = "Luk@s",
            Visible = true,
            Version = 2,
            Changeset = 6410629
        };

        _node = new NodeInfo(1, 50.4, 16.2, new TagsCollection());
        _nodeTags = new NodeInfo(1, 50.4, 16.2, new TagsCollection(new Tag[] { new("name", "test"), new("name-2", "test-2") }));
        _nodeProperties = new NodeInfo(1, 50.4, 16.2, new TagsCollection(), _details);

        _way = new WayInfo(1, new TagsCollection(), new long[] { 10, 11, 12 });
        _wayTags = new WayInfo(1, new TagsCollection(new Tag[] { new("name", "test"), new("name-2", "test-2") }), new long[] { 10, 11, 12 });
        _wayProperties = new WayInfo(1, new TagsCollection(), new long[] { 10, 11, 12 }, _details);
        _wayWithoutNodes = new WayInfo(1, new TagsCollection(), new long[] { });

        _relationNode = new RelationInfo(1, new TagsCollection(), new RelationMemberInfo[] { new() { MemberType = EntityType.Node, Reference = 10, Role = "test" } });
        _relationWay = new RelationInfo(1, new TagsCollection(), new RelationMemberInfo[] { new() { MemberType = EntityType.Way, Reference = 10, Role = "test" } });
        _relationRelation = new RelationInfo(1, new TagsCollection(), new RelationMemberInfo[] { new() { MemberType = EntityType.Relation, Reference = 10, Role = "test" } });
        _relationTags = new RelationInfo(
            1,
            new TagsCollection(new Tag[] { new("name", "test"), new("name-2", "test-2") }),
            new RelationMemberInfo[] { new() { MemberType = EntityType.Node, Reference = 10, Role = "test" } });
        _relationNodeProperties = new RelationInfo(1, new TagsCollection(), new RelationMemberInfo[] { new() { MemberType = EntityType.Node, Reference = 10, Role = "test" } }, _details);
    }

    [Fact]
    public void Constructor_StreamSettings_SetsSettingsAndMakesThemReadOnly()
    {
        MemoryStream stream = new();
        OsmWriterSettings settings = new();
        using (OsmXmlWriter target = new(stream, settings))
        {
            Assert.Same(settings, target.Settings);
            Assert.True(target.Settings.IsReadOnly);
        }
    }

    [Fact]
    public void Constructor_PathSettings_SetsSettingsAndMakesThemReadOnly()
    {
        string path = PathHelper.GetTempFilePath("xmlwriter-constructor-test.osm");

        OsmWriterSettings settings = new();
        using (OsmXmlWriter target = new(path, settings))
        {
            Assert.Same(settings, target.Settings);
            Assert.True(target.Settings.IsReadOnly);
        }
    }

    [Fact]
    public void Constructor_PathSettings_CreatesOutputFile()
    {
        string filename = PathHelper.GetTempFilePath("osmwriter-constructor-creates-output-test.pbf");

        OsmWriterSettings settings = new();
        using (OsmXmlWriter target = new(filename, settings))
        {
            ;
        }

        Assert.True(File.Exists(filename));
    }

    [Fact]
    public void Dispose_ClosesOutputStreamIfWritingToFiles()
    {
        string path = PathHelper.GetTempFilePath("xmlwriter-closes-output-filestream-test.osm");

        OsmXmlWriter target = new(path, new OsmWriterSettings());
        target.Dispose();

        FileStream testStream = null;
        testStream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
        testStream.Dispose();
    }

    [Fact]
    public void Write_ThrowsArgumentExceptionIfWriteMetadataIsTrueButEntityDoesntHaveMetadata()
    {
        MemoryStream stream = new();

        using (OsmXmlWriter target = new(stream, new OsmWriterSettings() { WriteMetadata = true }))
        {
            Assert.Throws<ArgumentException>(() => target.Write(_node));
        }
    }

    [Fact]
    public void Write_DoesNotThrowsExceptionIfMetadataContainsNullInsteadUsername()
    {
        MemoryStream stream = new();
        _nodeProperties.Metadata.User = null;

        using (OsmXmlWriter target = new(stream, new OsmWriterSettings() { WriteMetadata = true }))
        {
            target.Write(_nodeProperties);
        }
    }

    [Fact]
    public void Write_IEntityInfo_WritesNode()
    {
        MemoryStream stream = new();

        using (OsmXmlWriter target = new(stream, new OsmWriterSettings() { WriteMetadata = false }))
        {
            target.Write(_node);
        }

        TestXmlOutput(stream, _node, false);
    }

    [Fact]
    public void Write_IEntityInfo_WritesNodeWithTags()
    {
        MemoryStream stream = new();

        using (OsmXmlWriter target = new(stream, new OsmWriterSettings() { WriteMetadata = false }))
        {
            target.Write(_nodeTags);
        }

        TestXmlOutput(stream, _nodeTags, false);
    }

    [Fact]
    public void Write_IEntityInfo_WritesNodeWithMetadata()
    {
        MemoryStream stream = new();

        using (OsmXmlWriter target = new(stream, new OsmWriterSettings() { WriteMetadata = true }))
        {
            target.Write(_nodeProperties);
        }

        TestXmlOutput(stream, _nodeProperties, true);
    }

    [Fact]
    public void Write_IEntityInfo_DoesntWriteNodeMetadataIfWriteMedataIsFalse()
    {
        MemoryStream stream = new();

        using (OsmXmlWriter target = new(stream, new OsmWriterSettings() { WriteMetadata = false }))
        {
            target.Write(_nodeProperties);
        }

        stream = new MemoryStream(stream.ToArray());

        using (TextReader reader = new StreamReader(stream))
        {
            string line = null;
            while ((line = reader.ReadLine()) != null)
            {
                Assert.DoesNotContain("timestamp", line);
            }
        }
    }

    [Fact]
    public void Write_IEntityInfo_WritesWay()
    {
        MemoryStream stream = new();

        using (OsmXmlWriter target = new(stream, new OsmWriterSettings() { WriteMetadata = false }))
        {
            target.Write(_way);
        }

        TestXmlOutput(stream, _way, false);
    }

    [Fact]
    public void Write_IEntityInfo_WritesWayWithTags()
    {
        MemoryStream stream = new();

        using (OsmXmlWriter target = new(stream, new OsmWriterSettings() { WriteMetadata = false }))
        {
            target.Write(_wayTags);
        }

        TestXmlOutput(stream, _wayTags, false);
    }

    [Fact]
    public void Write_IEntityInfo_WritesWayWithMetadata()
    {
        MemoryStream stream = new();

        using (OsmXmlWriter target = new(stream, new OsmWriterSettings() { WriteMetadata = true }))
        {
            target.Write(_wayProperties);
        }

        TestXmlOutput(stream, _wayProperties, true);
    }

    [Fact]
    public void Write_IEntityInfo_DoesntWriteWayMetadataIfWriteMedataIsFalse()
    {
        MemoryStream stream = new();

        using (OsmXmlWriter target = new(stream, new OsmWriterSettings() { WriteMetadata = false }))
        {
            target.Write(_wayProperties);
        }

        stream = new MemoryStream(stream.ToArray());

        using (TextReader reader = new StreamReader(stream))
        {
            string line = null;
            while ((line = reader.ReadLine()) != null)
            {
                Assert.DoesNotContain("timestamp", line);
            }
        }
    }

    [Fact]
    public void Write_IEntityInfo_WritesRelationWithNode()
    {
        MemoryStream stream = new();

        using (OsmXmlWriter target = new(stream, new OsmWriterSettings() { WriteMetadata = false }))
        {
            target.Write(_relationNode);
        }

        TestXmlOutput(stream, _relationNode, false);
    }

    [Fact]
    public void Write_IEntityInfo_WritesRelationWithWay()
    {
        MemoryStream stream = new();

        using (OsmXmlWriter target = new(stream, new OsmWriterSettings() { WriteMetadata = false }))
        {
            target.Write(_relationWay);
        }

        TestXmlOutput(stream, _relationWay, false);
    }

    [Fact]
    public void Write_IEntityInfo_WritesRelationWithRelation()
    {
        MemoryStream stream = new();

        using (OsmXmlWriter target = new(stream, new OsmWriterSettings() { WriteMetadata = false }))
        {
            target.Write(_relationRelation);
        }

        TestXmlOutput(stream, _relationRelation, false);
    }

    [Fact]
    public void Write_IEntityInfo_WritesRelationWithTags()
    {
        MemoryStream stream = new();

        using (OsmXmlWriter target = new(stream, new OsmWriterSettings() { WriteMetadata = false }))
        {
            target.Write(_relationTags);
        }

        TestXmlOutput(stream, _relationTags, false);
    }

    [Fact]
    public void Write_IEntityInfo_WritesRelationWithMetadata()
    {
        MemoryStream stream = new();

        using (OsmXmlWriter target = new(stream, new OsmWriterSettings() { WriteMetadata = true }))
        {
            target.Write(_relationNodeProperties);
        }

        TestXmlOutput(stream, _relationNodeProperties, true);
    }

    [Fact]
    public void Write_IEntityInfo_DoesntWriteRelationMetadataIfWriteMedataIsFalse()
    {
        MemoryStream stream = new();

        using (OsmXmlWriter target = new(stream, new OsmWriterSettings() { WriteMetadata = false }))
        {
            target.Write(_relationNodeProperties);
        }

        stream = new MemoryStream(stream.ToArray());

        using (TextReader reader = new StreamReader(stream))
        {
            string line = null;
            while ((line = reader.ReadLine()) != null)
            {
                Assert.DoesNotContain("timestamp", line);
            }
        }
    }

    [Fact]
    public void Write_IOsmGeometry_WritesNode()
    {
        Node node = new(1, 11.1, 12.1);

        MemoryStream stream = new();

        using (OsmXmlWriter target = new(stream, new OsmWriterSettings() { WriteMetadata = false }))
        {
            target.Write(node);
        }

        TestXmlOutput(stream, new NodeInfo(node), false);
    }

    [Fact]
    public void Write_IOsmGeometry_WritesWay()
    {
        Way way = new(10, new Node[] { new(1), new(2), new(3) });

        MemoryStream stream = new();

        using (OsmXmlWriter target = new(stream, new OsmWriterSettings() { WriteMetadata = false }))
        {
            target.Write(way);
        }

        TestXmlOutput(stream, new WayInfo(way), false);
    }

    [Fact]
    public void Write_IOsmGeometry_WritesRelation()
    {
        Relation relation = new(100, new RelationMember[] { new(new Node(1), "test-role") });

        MemoryStream stream = new();

        using (OsmXmlWriter target = new(stream, new OsmWriterSettings() { WriteMetadata = false }))
        {
            target.Write(relation);
        }

        TestXmlOutput(stream, new RelationInfo(relation), false);
    }

    [Fact]
    public void Write_IOsmGeometry_ThrowsExceptionIfEntityIsNull()
    {
        MemoryStream stream = new();

        using (OsmXmlWriter target = new(stream, new OsmWriterSettings() { WriteMetadata = false }))
        {
            IOsmGeometry entity = null;
            Assert.Throws<ArgumentNullException>(() => target.Write(entity));
        }
    }

    private void TestXmlOutput(MemoryStream xmlStream, IEntityInfo expected, bool readMetadata)
    {
        if (xmlStream.CanSeek)
        {
            xmlStream.Seek(0, SeekOrigin.Begin);
        }
        else
        {
            xmlStream = new MemoryStream(xmlStream.ToArray());
        }

        OsmXmlReader reader = new(xmlStream, new OsmXmlReaderSettings() { ReadMetadata = readMetadata });
        IEntityInfo read = reader.Read();

        switch (expected.EntityType)
        {
            case EntityType.Node: CompareNodes(expected as NodeInfo, read as NodeInfo); break;
            case EntityType.Way: CompareWays(expected as WayInfo, read as WayInfo); break;
            case EntityType.Relation: CompareRelation(expected as RelationInfo, read as RelationInfo); break;
        }
    }

    private void CompareNodes(NodeInfo expected, NodeInfo actual)
    {
        Assert.Equal(expected.ID, actual.ID);
        Assert.InRange(actual.Longitude, expected.Longitude - _resolution, expected.Longitude + _resolution);
        Assert.InRange(actual.Latitude, expected.Latitude - _resolution, expected.Latitude + _resolution);

        CompareTags(expected.Tags, actual.Tags);
        CompareEntityDetails(expected.Metadata, actual.Metadata);
    }

    private void CompareWays(WayInfo expected, WayInfo actual)
    {
        Assert.Equal(expected.ID, actual.ID);
        Assert.Equal(expected.Nodes.Count, actual.Nodes.Count);
        for (int i = 0; i < expected.Nodes.Count; i++)
        {
            Assert.Equal(expected.Nodes[i], actual.Nodes[i]);
        }

        CompareTags(expected.Tags, actual.Tags);
        CompareEntityDetails(expected.Metadata, actual.Metadata);
    }

    private void CompareRelation(RelationInfo expected, RelationInfo actual)
    {
        Assert.Equal(expected.ID, actual.ID);
        Assert.Equal(expected.Members.Count, actual.Members.Count);
        for (int i = 0; i < expected.Members.Count; i++)
        {
            Assert.Equal(expected.Members[i], actual.Members[i]);
        }

        CompareTags(expected.Tags, actual.Tags);
        CompareEntityDetails(expected.Metadata, actual.Metadata);
    }

    private void CompareTags(TagsCollection expected, TagsCollection actual)
    {
        if (expected == null && actual == null)
        {
            return;
        }

        Assert.Equal(expected.Count, actual.Count);
        Assert.True(expected.All(tag => actual.Contains(tag)));
    }

    private void CompareEntityDetails(EntityMetadata expected, EntityMetadata actual)
    {
        if (expected == null && actual == null)
        {
            return;
        }

        Assert.NotNull(actual);

        Assert.Equal(expected.Timestamp, actual.Timestamp);
        Assert.Equal(expected.Uid, actual.Uid);
        Assert.Equal(expected.User, actual.User);
        Assert.Equal(expected.Visible, actual.Visible);
        Assert.Equal(expected.Version, actual.Version);
        Assert.Equal(expected.Changeset, actual.Changeset);
    }

    private void CheckNode(XElement element)
    {
        Assert.Equal("15", element.Attribute("id").Value);
        Assert.Equal("46.8", element.Attribute("lon").Value);
        Assert.Equal("-15.6", element.Attribute("lat").Value);

        XElement tagE = element.Elements("tag").Single();
        Assert.Equal("source", tagE.Attribute("k").Value);
        Assert.Equal("survey", tagE.Attribute("v").Value);
    }

    private void CheckWay(XElement element)
    {
        Assert.Equal("25", element.Attribute("id").Value);

        var nodesElement = element.Elements("nd");
        Assert.Equal(3, nodesElement.Count());
        Assert.Equal("11", nodesElement.ToList()[0].Attribute("ref").Value);
        Assert.Equal("12", nodesElement.ToList()[1].Attribute("ref").Value);
        Assert.Equal("13", nodesElement.ToList()[2].Attribute("ref").Value);

        XElement tagE = element.Elements("tag").Single();
        Assert.Equal("source", tagE.Attribute("k").Value);
        Assert.Equal("survey", tagE.Attribute("v").Value);
    }

    private void CheckRelation(XElement element)
    {
        Assert.Equal("25", element.Attribute("id").Value);

        XElement tagE = element.Elements("tag").Single();
        Assert.Equal("source", tagE.Attribute("k").Value);
        Assert.Equal("survey", tagE.Attribute("v").Value);
    }

    private void CheckOsmDetails(EntityMetadata details, XElement element)
    {
        if (details == null)
        {
            Assert.Null(element.Attribute("version"));
            Assert.Null(element.Attribute("changeset"));
            Assert.Null(element.Attribute("uid"));
            Assert.Null(element.Attribute("user"));
            Assert.Null(element.Attribute("visible"));
            Assert.Null(element.Attribute("timestamp"));
        }
        else
        {
            Assert.Equal("2", element.Attribute("version").Value);
            Assert.Equal("123", element.Attribute("changeset").Value);
            Assert.Equal("4587", element.Attribute("uid").Value);
            Assert.Equal("username", element.Attribute("user").Value);
            Assert.Equal("true", element.Attribute("visible").Value);
            Assert.Equal("2011-01-20T14:00:04Z", element.Attribute("timestamp").Value);
        }
    }
}
