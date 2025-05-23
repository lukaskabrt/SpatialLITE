using SpatialLite.Osm;
using SpatialLite.Osm.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Tests.SpatialLite.Osm;

public class OsmDatabaseTests
{
    private readonly Node[] _nodeData;
    private readonly Way[] _wayData;
    private readonly Relation[] _relationData;
    private readonly IOsmGeometry[] _data;

    public OsmDatabaseTests()
    {
        _nodeData = new Node[3];
        _nodeData[0] = new Node(1);
        _nodeData[1] = new Node(2);
        _nodeData[2] = new Node(3);

        _wayData = new Way[2];
        _wayData[0] = new Way(1, _nodeData);
        _wayData[1] = new Way(2, _nodeData.Skip(1));

        _relationData = new Relation[2];
        _relationData[0] = new Relation(1, new RelationMember[] { new(_wayData[0], "way"), new(_nodeData[0], "node") });
        _relationData[1] = new Relation(2, new RelationMember[] { new(_relationData[0], "relation"), new(_nodeData[0], "node") });

        _data = _nodeData.Concat<IOsmGeometry>(_wayData).Concat<IOsmGeometry>(_relationData).ToArray();
    }

    [Fact]
    public void Constructor__CreatesEmptyDatabase()
    {
        OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new();

        Assert.Empty(target);
        Assert.Empty(target.Nodes);
        Assert.Empty(target.Ways);
        Assert.Empty(target.Relations);
    }

    [Fact]
    public void Constructor_IEnumerable_CreatesCollectionWithSpecifiedItems()
    {
        OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new(_data);

        for (int i = 0; i < _data.Length; i++)
        {
            Assert.Contains(_data[i], target);
        }
    }

    [Fact]
    public void Constructor_IEnumerable_AddEnittiesToCorrectCollections()
    {
        OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new(_data);

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
    public void Count_ReturnsNumberOfAllEntities()
    {
        OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new(_data);

        Assert.Equal(_data.Length, target.Count);
    }

    [Fact]
    public void IsReadOnly_ReturnsFalse()
    {
        OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new(_data);

        Assert.False(target.IsReadOnly);
    }

    [Fact]
    public void Item_ReturnsNullIfIDIsNotPresentInCollection()
    {
        OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new(_data);

        Assert.Null(target[10000, EntityType.Node]);
    }

    [Fact]
    public void Item_ReturnsNodeWithSpecificID()
    {
        OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new(_data);
        IOsmGeometry entity = target[_nodeData[0].ID, EntityType.Node];

        Assert.Same(_nodeData[0], entity);
    }

    [Fact]
    public void Item_ReturnsWayWithSpecificID()
    {
        OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new(_data);
        IOsmGeometry entity = target[_wayData[0].ID, EntityType.Way];

        Assert.Same(_wayData[0], entity);
    }

    [Fact]
    public void Item_ReturnsRelationWithSpecificID()
    {
        OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new(_data);
        IOsmGeometry entity = target[_relationData[0].ID, EntityType.Relation];

        Assert.Same(_relationData[0], entity);
    }

    [Fact]
    public void Add_AddsNodeToCollection()
    {
        OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new(_nodeData.Skip(1));
        target.Add(_nodeData[0]);

        Assert.Contains(_nodeData[0], target);
    }

    [Fact]
    public void Add_AddsWayToCollection()
    {
        OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new(_wayData.Skip(1));
        target.Add(_wayData[0]);

        Assert.Contains(_wayData[0], target);
    }

    [Fact]
    public void Add_AddsRelationToCollection()
    {
        OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new(_relationData.Skip(1));
        target.Add(_relationData[0]);

        Assert.Contains(_relationData[0], target);
    }

    [Fact]
    public void Add_ThrowsArgumentNullExceptionIfItemIsNull()
    {
        OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new();

        Assert.Throws<ArgumentNullException>(() => target.Add(null));
    }

    [Fact]
    public void Add_ThrowsExceptionWhenAddingDuplicateID()
    {
        OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new(_data);

        Assert.Throws<ArgumentException>(() => target.Add(_data[0]));
    }

    [Fact]
    public void Clear_RemovesAllItemsFromCollection()
    {
        OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new(_data);
        target.Clear();

        Assert.Empty(target);
        Assert.Empty(target.Nodes);
        Assert.Empty(target.Ways);
        Assert.Empty(target.Relations);
    }

    [Fact]
    public void Contains_IOsmGeometry_ReturnsFalseForNull()
    {
        OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new(_data);

        Assert.DoesNotContain(null, target);
    }

    [Fact]
    public void Contains_IOsmGeometry_ReturnsFalseIfCollectionDoesNotContainNode()
    {
        OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new(_data);

        Assert.DoesNotContain(new Node(10000), target);
    }

    [Fact]
    public void Contains_IOsmGeometry_ReturnsTrueIfCollectionContainsNode()
    {
        OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new(_data);

        Assert.Contains(_nodeData[0], target);
    }

    [Fact]
    public void Contains_IOsmGeometry_ReturnsFalseIfCollectionDoesNotContainWay()
    {
        OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new(_data);

        Assert.DoesNotContain(new Way(10000), target);
    }

    [Fact]
    public void Contains_IOsmGeometry_ReturnsTrueIfCollectionContainsWay()
    {
        OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new(_data);

        Assert.Contains(_wayData[0], target);
    }

    [Fact]
    public void Contains_IOsmGeometry_ReturnsFalseIfCollectionDoesNotContainRelation()
    {
        OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new(_data);

        Assert.DoesNotContain(new Relation(10000), target);
    }

    [Fact]
    public void Contains_IOsmGeometry_ReturnsTrueIfCollectionContainsRelation()
    {
        OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new(_data);

        Assert.Contains(_relationData[0], target);
    }

    [Fact]
    public void Contains_ID_ReturnsFalseIfCollectionDoesNotContainNodeID()
    {
        OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new(_data);

        Assert.False(target.Contains(10000, EntityType.Node));
    }

    [Fact]
    public void Contains_ID_ReturnsTrueIfCollectionContainsNodeID()
    {
        OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new(_data);

        Assert.True(target.Contains(_nodeData[0].ID, EntityType.Node));
    }

    [Fact]
    public void Contains_ID_ReturnsFalseIfCollectionDoesNotContainWayID()
    {
        OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new(_data);

        Assert.False(target.Contains(10000, EntityType.Way));
    }

    [Fact]
    public void Contains_ID_ReturnsTrueIfCollectionContainsWayID()
    {
        OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new(_data);

        Assert.True(target.Contains(_wayData[0].ID, EntityType.Way));
    }

    [Fact]
    public void Contains_ID_ReturnsFalseIfCollectionDoesNotContainRelationID()
    {
        OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new(_data);

        Assert.False(target.Contains(10000, EntityType.Relation));
    }

    [Fact]
    public void Contains_ID_ReturnsTrueIfCollectionContainsRelationID()
    {
        OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new(_data);

        Assert.True(target.Contains(_relationData[0].ID, EntityType.Relation));
    }

    [Fact]
    public void Remove_IOsmGeometry_ReturnsFalseIfItemIsNull()
    {
        OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new(_data);

        bool callResult = target.Remove(null);

        Assert.False(callResult);
    }

    [Fact]
    public void Remove_IOsmGeometry_ReturnsFalseAndDoesntModifyCollectionIfNodeIsNotPresent()
    {
        OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new(_nodeData.Skip(1));

        bool callResult = target.Remove(_nodeData[0]);

        Assert.False(callResult);
        Assert.Contains(_nodeData[1], target);
        Assert.Contains(_nodeData[2], target);
    }

    [Fact]
    public void Remove_IOsmGeometry_ReturnsTrueAndRemovesNodeFromCollection()
    {
        OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new(_nodeData);

        bool callResult = target.Remove(_data[0]);

        Assert.True(callResult);
        Assert.DoesNotContain(_nodeData[0], target);
        Assert.Contains(_nodeData[1], target);
        Assert.Contains(_nodeData[2], target);
    }

    [Fact]
    public void Remove_IOsmGeometry_ReturnsFalseAndDoesntModifyCollectionIfWayIsNotPresent()
    {
        OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new(_wayData.Skip(1));

        bool callResult = target.Remove(_wayData[0]);

        Assert.False(callResult);
        Assert.Contains(_wayData[1], target);
    }

    [Fact]
    public void Remove_IOsmGeometry_ReturnsTrueAndRemovesWayFromCollection()
    {
        OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new(_wayData);

        bool callResult = target.Remove(_wayData[0]);

        Assert.True(callResult);
        Assert.DoesNotContain(_wayData[0], target);
        Assert.Contains(_wayData[1], target);
    }

    [Fact]
    public void Remove_IOsmGeometry_ReturnsFalseAndDoesntModifyCollectionIfRelationIsNotPresent()
    {
        OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new(_relationData.Skip(1));

        bool callResult = target.Remove(_relationData[0]);

        Assert.False(callResult);
        Assert.Contains(_relationData[1], target);
    }

    [Fact]
    public void Remove_IOsmGeometry_ReturnsTrueAndRemovesRelationFromCollection()
    {
        OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new(_relationData);

        bool callResult = target.Remove(_relationData[0]);

        Assert.True(callResult);
        Assert.DoesNotContain(_relationData[0], target);
        Assert.Contains(_relationData[1], target);
    }

    [Fact]
    public void Remove_ID_ReturnsFalseAndDoesntModifyCollectionIfNodeIsNotPresent()
    {
        OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new(_nodeData.Skip(1));

        bool callResult = target.Remove(_nodeData[0].ID, EntityType.Node);

        Assert.False(callResult);
        Assert.Contains(_nodeData[1], target);
        Assert.Contains(_nodeData[2], target);
    }

    [Fact]
    public void Remove_ID_ReturnsTrueAndRemovesNodeFromCollection()
    {
        OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new(_nodeData);

        bool callResult = target.Remove(_data[0].ID, EntityType.Node);

        Assert.True(callResult);
        Assert.DoesNotContain(_nodeData[0], target);
        Assert.Contains(_nodeData[1], target);
        Assert.Contains(_nodeData[2], target);
    }

    [Fact]
    public void Remove_ID_ReturnsFalseAndDoesntModifyCollectionIfWayIsNotPresent()
    {
        OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new(_wayData.Skip(1));

        bool callResult = target.Remove(_wayData[0].ID, EntityType.Way);

        Assert.False(callResult);
        Assert.Contains(_wayData[1], target);
    }

    [Fact]
    public void Remove_ID_ReturnsTrueAndRemovesWayFromCollection()
    {
        OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new(_wayData);

        bool callResult = target.Remove(_wayData[0].ID, EntityType.Way);

        Assert.True(callResult);
        Assert.DoesNotContain(_wayData[0], target);
        Assert.Contains(_wayData[1], target);
    }

    [Fact]
    public void Remove_ID_ReturnsFalseAndDoesntModifyCollectionIfRelationIsNotPresent()
    {
        OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new(_relationData.Skip(1));

        bool callResult = target.Remove(_relationData[0].ID, EntityType.Relation);

        Assert.False(callResult);
        Assert.Contains(_relationData[1], target);
    }

    [Fact]
    public void Remove_ID_ReturnsTrueAndRemovesRelationFromCollection()
    {
        OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new(_relationData);

        bool callResult = target.Remove(_relationData[0].ID, EntityType.Relation);

        Assert.True(callResult);
        Assert.DoesNotContain(_relationData[0], target);
        Assert.Contains(_relationData[1], target);
    }

    [Fact]
    public void GetEnumerator_ReturnsEnumeratorThatEnumeratesAllEntities()
    {
        OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new(_data);

        IEnumerable<IOsmGeometry> result = target;

        Assert.Equal(_data.Length, target.Count());
        foreach (var entity in target)
        {
            Assert.Contains(entity, _data);
        }
    }

    [Fact]
    public void CopyTo_CopiesEntitiesToArray()
    {
        OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new(_data);
        IOsmGeometry[] array = new IOsmGeometry[_data.Length];

        target.CopyTo(array, 0);

        for (int i = 0; i < _data.Length; i++)
        {
            Assert.Same(_data[i], array[i]);
        }
    }
}
