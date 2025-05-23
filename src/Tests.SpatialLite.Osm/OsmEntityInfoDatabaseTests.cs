using Moq;
using SpatialLite.Osm;
using SpatialLite.Osm.IO;
using System.Collections.Generic;
using System.Linq;
using Tests.SpatialLite.Osm.Data;
using Xunit;

namespace Tests.SpatialLite.Osm;

public class OsmEntityInfoDatabaseTests
{
    private readonly NodeInfo[] _nodeData;
    private readonly WayInfo[] _wayData;
    private readonly RelationInfo[] _relationData;
    private readonly IEntityInfo[] _data;

    public OsmEntityInfoDatabaseTests()
    {
        _nodeData = new NodeInfo[3];
        _nodeData[0] = new NodeInfo(1, 10.1, 11.1, new TagsCollection());
        _nodeData[1] = new NodeInfo(2, 10.2, 11.2, new TagsCollection());
        _nodeData[2] = new NodeInfo(3, 10.3, 11.3, new TagsCollection());

        _wayData = new WayInfo[2];
        _wayData[0] = new WayInfo(10, new TagsCollection(), _nodeData.Select(n => n.ID).ToArray());
        _wayData[1] = new WayInfo(11, new TagsCollection(), _nodeData.Select(n => n.ID).Skip(1).ToArray());

        _relationData = new RelationInfo[2];
        _relationData[0] = new RelationInfo(100, new TagsCollection(), new RelationMemberInfo[] { new() { Reference = 10, Role = "way" }, new() { Reference = 1, Role = "node" } });
        _relationData[1] = new RelationInfo(101, new TagsCollection(), new RelationMemberInfo[] { new() { Reference = 101, Role = "relation" }, new() { Reference = 1, Role = "node" } });

        _data = _nodeData.Concat<IEntityInfo>(_wayData).Concat<IEntityInfo>(_relationData).ToArray();
    }

    [Fact]
    public void Constructor__CreatesEmptyDatabase()
    {
        OsmEntityInfoDatabase target = new();

        Assert.Empty(target);
        Assert.Empty(target.Nodes);
        Assert.Empty(target.Ways);
        Assert.Empty(target.Relations);
    }

    [Fact]
    public void Constructor_IEnumerable_CreatesCollectionWithSpecifiedItems()
    {
        OsmEntityInfoDatabase target = new(_data);

        for (int i = 0; i < _data.Length; i++)
        {
            Assert.Contains(_data[i], target);
        }
    }

    [Fact]
    public void Constructor_IEnumerable_AddEnittiesToCorrextCollections()
    {
        OsmEntityInfoDatabase target = new(_data);

        for (int i = 0; i < _nodeData.Length; i++)
        {
            Assert.Contains(_nodeData[i], target.Nodes);
        }

        for (int i = 0; i < _wayData.Length; i++)
        {
            Assert.Contains(_wayData[i], target.Ways);
        }

        for (int i = 0; i < _relationData.Length; i++)
        {
            Assert.Contains(_relationData[i], target.Relations);
        }
    }

    [Fact]
    public void Load_IOsmReader_LoadsNodes()
    {
        IOsmReader reader = new OsmXmlReader(TestDataReader.OpenOsmDB("osm-nodes.osm"), new OsmXmlReaderSettings() { ReadMetadata = true });
        OsmEntityInfoDatabase target = OsmEntityInfoDatabase.Load(reader);

        Assert.Equal(3, target.Nodes.Count);
        Assert.True(target.Nodes.Contains(1));
        Assert.True(target.Nodes.Contains(2));
        Assert.True(target.Nodes.Contains(3));
    }

    [Fact]
    public void Load_IOsmReader_LoadsWay()
    {
        IOsmReader reader = new OsmXmlReader(TestDataReader.OpenOsmDB("osm-way.osm"), new OsmXmlReaderSettings() { ReadMetadata = true });
        OsmEntityInfoDatabase target = OsmEntityInfoDatabase.Load(reader);

        Assert.Equal(3, target.Nodes.Count);
        Assert.True(target.Nodes.Contains(1));
        Assert.True(target.Nodes.Contains(2));
        Assert.True(target.Nodes.Contains(3));

        Assert.Single(target.Ways);
        Assert.True(target.Ways.Contains(10));
    }

    [Fact]
    public void Load_IOsmReader_LoadsRelation()
    {
        IOsmReader reader = new OsmXmlReader(TestDataReader.OpenOsmDB("osm-relation.osm"), new OsmXmlReaderSettings() { ReadMetadata = true });
        OsmEntityInfoDatabase target = OsmEntityInfoDatabase.Load(reader);

        Assert.Single(target.Nodes);
        Assert.True(target.Nodes.Contains(1));

        Assert.Single(target.Relations);
        Assert.True(target.Relations.Contains(100));
    }

    [Fact]
    public void Save_CallsIOsmWriterWriteForAllEntities()
    {
        List<IEntityInfo> written = new();
        Mock<IOsmWriter> writerM = new();

        writerM.Setup(w => w.Write(It.IsAny<IEntityInfo>())).Callback<IEntityInfo>(written.Add);

        OsmEntityInfoDatabase target = new(_data);
        target.Save(writerM.Object);

        Assert.Equal(target.Count, written.Count);
        foreach (var entity in target)
        {
            Assert.Contains(entity, written);
        }
    }
}
