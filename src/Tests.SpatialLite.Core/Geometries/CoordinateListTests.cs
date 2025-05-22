using SpatialLite.Core.API;
using SpatialLite.Core.Geometries;
using System.Linq;
using Xunit;

namespace Tests.SpatialLite.Core.Geometries;

public class CoordinateListTests
{
    private Coordinate _coordinate = new(-10, -20, -200);
    private readonly Coordinate[] _coordinates = new Coordinate[] {
            new(12,10,100),
            new(22,20,200),
            new(32,30,300)
    };

    [Fact]
    public void Constructor__CreatesEmptyList()
    {
        CoordinateList target = new();

        Assert.Empty(target);
    }

    [Fact]
    public void Constructor_IEnumerable_CreatesListWithSpecifiedItems()
    {
        CoordinateList target = new(_coordinates);

        for (int i = 0; i < _coordinates.Length; i++)
        {
            Assert.Equal(_coordinates[i], target[i]);
        }
    }

    [Fact]
    public void Indexer_GetsAndSetsValues()
    {
        CoordinateList target = new(_coordinates);

        Assert.Equal(_coordinates[1], target[1]);

        target[1] = _coordinate;
        Assert.Equal(_coordinate, target[1]);
    }

    [Fact]
    public void Count_Returns0ForEmptyList()
    {
        CoordinateList target = new();

        Assert.Equal(0, target.Count);
    }

    [Fact]
    public void Count_ReturnsNumberOfCoordinates()
    {
        CoordinateList target = new(_coordinates);

        Assert.Equal(_coordinates.Length, target.Count);
    }

    [Fact]
    public void Add_AppendsCoordinateToTheEndOfList()
    {
        CoordinateList target = new(_coordinates);
        target.Add(_coordinate);

        Assert.Equal(_coordinate, target.Last());
    }

    [Fact]
    public void Add_IncresesCount()
    {
        CoordinateList target = new(_coordinates);
        target.Add(_coordinate);

        Assert.Equal(_coordinates.Length + 1, target.Count);
    }

    [Fact]
    public void Add_AppendsCollectionOfCoordinatesToTheEndOfList()
    {
        CoordinateList target = new(new Coordinate[] { _coordinate });
        target.Add(_coordinates);

        for (int i = 0; i < _coordinates.Length; i++)
        {
            Assert.Equal(_coordinates[i], target[i + 1]);
        }
    }

    [Fact]
    public void Add_IncresesCount2()
    {
        CoordinateList target = new(new Coordinate[] { _coordinate });
        target.Add(_coordinates);

        Assert.Equal(_coordinates.Length + 1, target.Count);
    }

    [Fact]
    public void Insert_InsertsCoordinateToSpecifiedIndex()
    {
        int index = 1;
        CoordinateList target = new(_coordinates);
        target.Insert(index, _coordinate);

        Assert.Equal(_coordinate, target[index]);
    }

    [Fact]
    public void Insert_IncresesCount()
    {
        int index = 1;
        CoordinateList target = new(_coordinates);
        target.Insert(index, _coordinate);

        Assert.Equal(_coordinates.Length + 1, target.Count);
    }

    [Fact]
    public void Insert_AppendsCoordinateToListIfIndexEqulasCount()
    {
        CoordinateList target = new(_coordinates);
        target.Insert(target.Count, _coordinate);

        Assert.Equal(_coordinate, target.Last());
        Assert.Equal(_coordinates.Length + 1, target.Count);
    }

    [Fact]
    public void RemoveAt_RemovesCoordinateAtIndex()
    {
        CoordinateList target = new(_coordinates);
        target.RemoveAt(1);

        Assert.Equal(_coordinates[0], target[0]);
        Assert.Equal(_coordinates[2], target[1]);
    }

    [Fact]
    public void RemoveAt_DecreasesCount()
    {
        CoordinateList target = new(_coordinates);
        target.RemoveAt(1);

        Assert.Equal(_coordinates.Length - 1, target.Count);
    }

    [Fact]
    public void Clear_RemovesAllCoordinatesFromList()
    {
        CoordinateList target = new(_coordinates);
        target.Clear();

        Assert.Empty(target.ToArray());
        Assert.Equal(0, target.Count);
    }
}
