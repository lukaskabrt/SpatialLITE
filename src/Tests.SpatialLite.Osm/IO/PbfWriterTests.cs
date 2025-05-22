using SpatialLite.Osm;
using SpatialLite.Osm.Geometries;
using SpatialLite.Osm.IO;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Tests.SpatialLite.Osm.IO;

public class PbfWriterTests
{
    //resolution for default granularity
    private const double _resolution = 1E-07;

    private readonly EntityMetadata _details;
    private readonly NodeInfo _node, _nodeTags, _nodeProperties;
    private readonly WayInfo _way, _wayTags, _wayProperties, _wayWithoutNodes;
    private readonly RelationInfo _relationNode, _relationWay, _relationRelation, _relationNodeProperties, _relationTags;

    public PbfWriterTests()
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
    public void Constructor_FilenameSettings_SetsSettingsAndMakesThemReadOnly()
    {
        string filename = PathHelper.GetTempFilePath("pbfwriter-constructor-test.pbf");

        PbfWriterSettings settings = new();
        using (PbfWriter target = new(filename, settings))
        {
            Assert.Same(settings, target.Settings);
            Assert.True(settings.IsReadOnly);
        }
    }

    [Fact]
    public void Constructor_FilenameSettings_CreatesOutputFile()
    {
        string filename = PathHelper.GetTempFilePath("pbfwriter-constructor-creates-output-test.pbf");

        PbfWriterSettings settings = new();
        using (PbfWriter target = new(filename, settings))
        {
            ;
        }

        Assert.True(File.Exists(filename));
    }

    [Fact]
    public void Constructor_FilenameSettings_WritesOsmHeader()
    {
        string filename = PathHelper.GetTempFilePath("pbfwriter-constructor-writes-header-test.pbf");

        PbfWriterSettings settings = new();
        using (PbfWriter target = new(filename, settings))
        {
            ;
        }

        FileInfo fi = new(filename);
        Assert.True(fi.Length > 0);
    }

    [Fact]
    public void Constructor_StreamSettings_SetsSettingsAndMakeThemReadOnly()
    {
        PbfWriterSettings settings = new();
        using (PbfWriter target = new(new MemoryStream(), settings))
        {
            Assert.Same(settings, target.Settings);
            Assert.True(settings.IsReadOnly);
        }
    }

    [Fact]
    public void Constructor_StreamSettings_WritesOsmHeader()
    {
        MemoryStream stream = new();
        PbfWriterSettings settings = new();
        using (PbfWriter target = new(stream, settings))
        {
            ;
        }

        Assert.True(stream.ToArray().Length > 0);
    }

    [Fact]
    public void Dispose_ClosesOutputStreamIfWritingToFiles()
    {
        string filename = PathHelper.GetTempFilePath("pbfwriter-closes-output-filestream-test.pbf");

        PbfWriterSettings settings = new();
        PbfWriter target = new(filename, settings);
        target.Dispose();

        new FileStream(filename, FileMode.Open, FileAccess.ReadWrite);
    }

    [Fact]
    public void Write_ThrowsArgumentExceptionIfWriteMetadataIsTrueButEntityDoesntHaveMetadata()
    {
        using (PbfWriter target = new(new MemoryStream(), new PbfWriterSettings() { UseDenseFormat = true, Compression = CompressionMode.None, WriteMetadata = true }))
        {
            Assert.Throws<ArgumentException>(() => target.Write(_node));
        }
    }

    [Fact]
    public void Write_ThrowsArgumentNullExceptionIfMetadataContainsNullInsteadUsername()
    {
        _nodeProperties.Metadata.User = null;

        using (PbfWriter target = new(new MemoryStream(), new PbfWriterSettings() { UseDenseFormat = true, Compression = CompressionMode.None, WriteMetadata = true }))
        {
            Assert.Throws<ArgumentNullException>(() => target.Write(_nodeProperties));
        }
    }

    [Fact]
    public void Write_IEntityInfo_WritesNode()
    {
        PbfWriterSettings settings = new() { UseDenseFormat = false, Compression = CompressionMode.None, WriteMetadata = false };
        MemoryStream stream = new();

        using (PbfWriter target = new(stream, settings))
        {
            target.Write(_node);
        }

        TestPbfOutput(stream, _node);
    }

    [Fact]
    public void Write_IEntityInfo_WritesNodeWithTags()
    {
        PbfWriterSettings settings = new() { UseDenseFormat = false, Compression = CompressionMode.None, WriteMetadata = false };
        MemoryStream stream = new();

        using (PbfWriter target = new(stream, settings))
        {
            target.Write(_nodeTags);
        }

        TestPbfOutput(stream, _nodeTags);
    }

    [Fact]
    public void Write_IEntityInfo_WritesNodeWithMetadata()
    {
        PbfWriterSettings settings = new() { UseDenseFormat = false, Compression = CompressionMode.None, WriteMetadata = true };
        MemoryStream stream = new();

        using (PbfWriter target = new(stream, settings))
        {
            target.Write(_nodeProperties);
        }

        TestPbfOutput(stream, _nodeProperties);
    }

    [Fact]
    public void Write_IEntityInfo_DoesntWritesNodeMetadataIfWriteMetasdataSettingsIsFalse()
    {
        PbfWriterSettings settings = new() { UseDenseFormat = false, Compression = CompressionMode.None, WriteMetadata = false };
        MemoryStream stream = new();

        using (PbfWriter target = new(stream, settings))
        {
            target.Write(_nodeProperties);
        }

        TestPbfOutput(stream, _node);
    }

    [Fact]
    public void Write_IEntityInfo_WritesNode_Dense()
    {
        PbfWriterSettings settings = new() { UseDenseFormat = true, Compression = CompressionMode.None, WriteMetadata = false };
        MemoryStream stream = new();

        using (PbfWriter target = new(stream, settings))
        {
            target.Write(_node);
        }

        TestPbfOutput(stream, _node);
    }

    [Fact]
    public void Write_IEntityInfo_WritesNodeWithTags_Dense()
    {
        PbfWriterSettings settings = new() { UseDenseFormat = true, Compression = CompressionMode.None, WriteMetadata = false };
        MemoryStream stream = new();

        using (PbfWriter target = new(stream, settings))
        {
            target.Write(_nodeTags);
        }

        TestPbfOutput(stream, _nodeTags);
    }

    [Fact]
    public void Write_IEntityInfo_WritesNodeWithMetadata_Dense()
    {
        PbfWriterSettings settings = new() { UseDenseFormat = true, Compression = CompressionMode.None, WriteMetadata = true };
        MemoryStream stream = new();

        using (PbfWriter target = new(stream, settings))
        {
            target.Write(_nodeProperties);
        }

        TestPbfOutput(stream, _nodeProperties);
    }

    [Fact]
    public void Write_IEntityInfo_DoesntWritesNodeMetadataIfWriteMetasdataSettingsIsFalse_Dense()
    {
        PbfWriterSettings settings = new() { UseDenseFormat = true, Compression = CompressionMode.None, WriteMetadata = false };
        MemoryStream stream = new();

        using (PbfWriter target = new(stream, settings))
        {
            target.Write(_nodeProperties);
        }

        TestPbfOutput(stream, _node);
    }

    [Fact]
    public void Write_IEntityInfo_WritesWay()
    {
        PbfWriterSettings settings = new() { UseDenseFormat = false, Compression = CompressionMode.None, WriteMetadata = false };
        MemoryStream stream = new();

        using (PbfWriter target = new(stream, settings))
        {
            target.Write(_way);
        }

        TestPbfOutput(stream, _way);
    }

    [Fact]
    public void Write_IEntityInfo_WritesWayWithTags()
    {
        PbfWriterSettings settings = new() { UseDenseFormat = false, Compression = CompressionMode.None, WriteMetadata = false };
        MemoryStream stream = new();

        using (PbfWriter target = new(stream, settings))
        {
            target.Write(_wayTags);
        }

        TestPbfOutput(stream, _wayTags);
    }

    [Fact]
    public void Write_IEntityInfo_WritesWayWithMetadata()
    {
        PbfWriterSettings settings = new() { UseDenseFormat = false, Compression = CompressionMode.None, WriteMetadata = true };
        MemoryStream stream = new();

        using (PbfWriter target = new(stream, settings))
        {
            target.Write(_wayProperties);
        }

        TestPbfOutput(stream, _wayProperties);
    }

    [Fact]
    public void Write_IEntityInfo_DoesntWriteWayMetadataIfWriteMetadataSettingsIsFalse()
    {
        PbfWriterSettings settings = new() { UseDenseFormat = false, Compression = CompressionMode.None, WriteMetadata = false };
        MemoryStream stream = new();

        using (PbfWriter target = new(stream, settings))
        {
            target.Write(_wayProperties);
        }

        TestPbfOutput(stream, _way);
    }

    [Fact]
    public void Write_IEntityInfo_WritesRelationWithNode()
    {
        PbfWriterSettings settings = new() { UseDenseFormat = false, Compression = CompressionMode.None, WriteMetadata = false };
        MemoryStream stream = new();

        using (PbfWriter target = new(stream, settings))
        {
            target.Write(_relationNode);
        }

        TestPbfOutput(stream, _relationNode);
    }

    [Fact]
    public void Write_IEntityInfo_WritesRelationWithWay()
    {
        PbfWriterSettings settings = new() { UseDenseFormat = false, Compression = CompressionMode.None, WriteMetadata = false };
        MemoryStream stream = new();

        using (PbfWriter target = new(stream, settings))
        {
            target.Write(_relationWay);
        }

        TestPbfOutput(stream, _relationWay);
    }

    [Fact]
    public void Write_IEntityInfo_WritesRelationWithRelation()
    {
        PbfWriterSettings settings = new() { UseDenseFormat = false, Compression = CompressionMode.None, WriteMetadata = false };
        MemoryStream stream = new();

        using (PbfWriter target = new(stream, settings))
        {
            target.Write(_relationRelation);
        }

        TestPbfOutput(stream, _relationRelation);
    }

    [Fact]
    public void Write_IEntityInfo_WritesRelationWithTags()
    {
        PbfWriterSettings settings = new() { UseDenseFormat = false, Compression = CompressionMode.None, WriteMetadata = false };
        MemoryStream stream = new();

        using (PbfWriter target = new(stream, settings))
        {
            target.Write(_relationTags);
        }

        TestPbfOutput(stream, _relationTags);
    }

    [Fact]
    public void Write_IEntityInfo_WritesRelationWithMetadata()
    {
        PbfWriterSettings settings = new() { UseDenseFormat = false, Compression = CompressionMode.None, WriteMetadata = true };
        MemoryStream stream = new();

        using (PbfWriter target = new(stream, settings))
        {
            target.Write(_relationNodeProperties);
        }

        TestPbfOutput(stream, _relationNodeProperties);
    }

    [Fact]
    public void Write_IEntityInfo_DoesntWritesRelationMetadataIfWriteMetasdataSettingsIsFalse()
    {
        PbfWriterSettings settings = new() { UseDenseFormat = false, Compression = CompressionMode.None, WriteMetadata = false };
        MemoryStream stream = new();

        using (PbfWriter target = new(stream, settings))
        {
            target.Write(_relationNodeProperties);
        }

        TestPbfOutput(stream, _relationNode);
    }

    [Fact]
    public void Write_IOsmGeometry_WritesNode()
    {
        Node node = new(1, 11.1, 12.1);
        PbfWriterSettings settings = new() { UseDenseFormat = false, Compression = CompressionMode.None, WriteMetadata = false };
        MemoryStream stream = new();

        using (PbfWriter target = new(stream, settings))
        {
            target.Write(node);
        }

        TestPbfOutput(stream, new NodeInfo(node));
    }

    [Fact]
    public void Write_IOsmGeometry_WritesWay()
    {
        Way way = new(10, new Node[] { new(1), new(2), new(3) });
        PbfWriterSettings settings = new() { UseDenseFormat = false, Compression = CompressionMode.None, WriteMetadata = false };
        MemoryStream stream = new();

        using (PbfWriter target = new(stream, settings))
        {
            target.Write(way);
        }

        TestPbfOutput(stream, new WayInfo(way));
    }

    [Fact]
    public void Write_IOsmGeometry_WritesRelation()
    {
        Relation relation = new(100, new RelationMember[] { new(new Node(1), "test-role") });

        PbfWriterSettings settings = new() { UseDenseFormat = false, Compression = CompressionMode.None, WriteMetadata = false };
        MemoryStream stream = new();

        using (PbfWriter target = new(stream, settings))
        {
            target.Write(relation);
        }

        TestPbfOutput(stream, new RelationInfo(relation));
    }

    [Fact]
    public void Write_IOsmGeometry_ThrowsExceptionIfEntityIsNull()
    {
        PbfWriterSettings settings = new() { UseDenseFormat = false, Compression = CompressionMode.None, WriteMetadata = false };
        MemoryStream stream = new();

        using (PbfWriter target = new(stream, settings))
        {
            IOsmGeometry entity = null;
            Assert.Throws<ArgumentNullException>(() => target.Write(entity));
        }
    }

    [Fact]
    public void Flush_ForcesWriterToWriteDataToUnderalyingStorage()
    {
        MemoryStream stream = new();

        PbfWriter target = new(stream, new PbfWriterSettings() { UseDenseFormat = false, Compression = CompressionMode.None, WriteMetadata = false });

        //1000 nodes should fit into tokens
        for (int i = 0; i < 1000; i++)
        {
            NodeInfo node = new(i, 45.87, -126.5, new TagsCollection());
            target.Write(node);
        }
        int minimalExpectedLengthIncrease = 1000 * 8;

        long originalStreamLength = stream.Length;
        target.Flush();

        Assert.True(stream.Length > originalStreamLength + minimalExpectedLengthIncrease);
    }

    private void TestPbfOutput(MemoryStream pbfStream, IEntityInfo expected)
    {
        if (pbfStream.CanSeek)
        {
            pbfStream.Seek(0, SeekOrigin.Begin);
        }
        else
        {
            pbfStream = new MemoryStream(pbfStream.ToArray());
        }

        PbfReader reader = new(pbfStream, new OsmReaderSettings() { ReadMetadata = true });
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
}
