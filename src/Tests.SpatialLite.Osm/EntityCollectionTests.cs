using Moq;
using SpatialLite.Osm;
using SpatialLite.Osm.Geometries;
using System;
using System.Linq;
using Xunit;

namespace Tests.SpatialLite.Osm;

public class EntityCollectionTests
{
    private readonly IOsmGeometry[] _data;

    public EntityCollectionTests()
    {
        _data = new IOsmGeometry[3];
        _data[0] = new Node(1);
        _data[1] = new Node(2);
        _data[2] = new Node(3);
    }

    [Fact]
    public void Constructor__CreatesEmptyCollection()
    {
        EntityCollection<IOsmGeometry> target = new();

        Assert.Empty(target);
    }

    [Fact]
    public void Constructor_IEnumerable_CreatesCollectionWithSpecifiedItems()
    {
        EntityCollection<IOsmGeometry> target = new(_data);

        for (int i = 0; i < _data.Length; i++)
        {
            Assert.Contains(_data[i], target);
        }
    }

    [Fact]
    public void Count_ReturnsNumberOfElements()
    {
        EntityCollection<IOsmGeometry> target = new(_data);
        Mock<IOsmGeometry> entityM = new();

        Assert.Equal(_data.Length, target.Count);
    }

    [Fact]
    public void Contains_IOsmGeometry_ReturnsFalseForEmptyCollection()
    {
        EntityCollection<IOsmGeometry> target = new();

        Assert.DoesNotContain(_data[0], target);
    }

    [Fact]
    public void Contains_IOsmGeometry_ReturnsFalseForNull()
    {
        EntityCollection<IOsmGeometry> target = new();

        Assert.DoesNotContain(null, target);
    }

    [Fact]
    public void Contains_IOsmGeometry_ReturnsFalseIfCollectionDoesNotContainEntity()
    {
        EntityCollection<IOsmGeometry> target = new(_data.Skip(1));

        Assert.DoesNotContain(_data[0], target);
    }

    [Fact]
    public void Contains_IOsmGeometry_ReturnsTrueIfCollectionContainsEntity()
    {
        EntityCollection<IOsmGeometry> target = new(_data);

        Assert.Contains(_data[0], target);
    }

    [Fact]
    public void Contains_ID_ReturnsFalseForEmptyCollection()
    {
        EntityCollection<IOsmGeometry> target = new();

        Assert.False(target.Contains(_data[0].ID));
    }

    [Fact]
    public void Contains_ID_ReturnsFalseIfCollectionDoesNotContainEntity()
    {
        EntityCollection<IOsmGeometry> target = new(_data.Skip(1));

        Assert.False(target.Contains(_data[0].ID));
    }

    [Fact]
    public void Contains_ID_ReturnsTrueIfCollectionContainsEntity()
    {
        EntityCollection<IOsmGeometry> target = new(_data);

        Assert.True(target.Contains(_data[0].ID));
    }

    [Fact]
    public void Clear_RemovesAllItemsFromCollection()
    {
        EntityCollection<IOsmGeometry> target = new(_data);
        target.Clear();

        Assert.Empty(target);
    }

    [Fact]
    public void Add_AddsEntityToCollection()
    {
        EntityCollection<IOsmGeometry> target = new();
        target.Add(_data[0]);

        Assert.Contains(_data[0], target);
    }

    [Fact]
    public void Add_ThrowsArgumentNullExceptionIfItemIsNull()
    {
        EntityCollection<IOsmGeometry> target = new();

        Assert.Throws<ArgumentNullException>(() => target.Add(null));
    }

    [Fact]
    public void Add_ThrowsExceptionWhenAddingDuplicateID()
    {
        EntityCollection<IOsmGeometry> target = new(_data);

        Assert.Throws<ArgumentException>(() => target.Add(_data[0]));
    }

    [Fact]
    public void IsReadOnly_ReturnsFalse()
    {
        EntityCollection<IOsmGeometry> target = new();

        Assert.False(target.IsReadOnly);
    }

    [Fact]
    public void Remove_IOsmGeometry_ReturnsFalseAndDoesntModifyCollectionIfItemIsNotPresent()
    {
        EntityCollection<IOsmGeometry> target = new(_data.Skip(1));

        bool callResult = target.Remove(_data[0]);

        Assert.False(callResult);
        Assert.Contains(_data[1], target);
        Assert.Contains(_data[2], target);
    }

    [Fact]
    public void Remove_IOsmGeometry_ReturnsFalseIfItemIsNull()
    {
        EntityCollection<IOsmGeometry> target = new(_data);

        bool callResult = target.Remove(null);

        Assert.False(callResult);
    }

    [Fact]
    public void Remove_IOsmGeometry_ReturnsTrueAndRemovesItemFromCollection()
    {
        EntityCollection<IOsmGeometry> target = new(_data);

        bool callResult = target.Remove(_data[0]);

        Assert.True(callResult);
        Assert.DoesNotContain(_data[0], target);
        Assert.Contains(_data[1], target);
        Assert.Contains(_data[2], target);
    }

    [Fact]
    public void Remove_ID_ReturnsFalseAndDoesntModifyCollectionIfItemIsNotPresent()
    {
        EntityCollection<IOsmGeometry> target = new(_data.Skip(1));

        bool callResult = target.Remove(_data[0].ID);

        Assert.False(callResult);
        Assert.Contains(_data[1], target);
        Assert.Contains(_data[2], target);
    }

    [Fact]
    public void Remove_ID_ReturnsTrueAndRemovesItemFromCollection()
    {
        EntityCollection<IOsmGeometry> target = new(_data);

        bool callResult = target.Remove(_data[0].ID);

        Assert.True(callResult);
        Assert.DoesNotContain(_data[0], target);
        Assert.Contains(_data[1], target);
        Assert.Contains(_data[2], target);
    }

    [Fact]
    public void Item_ReturnsNullIfIDIsNotpResentInCollection()
    {
        EntityCollection<IOsmGeometry> target = new(_data);

        Assert.Null(target[1000]);
    }

    [Fact]
    public void Item_ReturnsEntityWitSpecificID()
    {
        EntityCollection<IOsmGeometry> target = new(_data);
        IOsmGeometry entity = target[1];

        Assert.Equal(1, entity.ID);
    }
}
