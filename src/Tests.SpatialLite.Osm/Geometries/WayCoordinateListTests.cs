using SpatialLite.Core.API;
using SpatialLite.Osm.Geometries;
using System;
using System.Collections.Generic;
using Xunit;

namespace Tests.SpatialLite.Osm.Geometries;

public class WayCoordinateListTests
{
    private readonly List<Node> _nodes = new(new Node[] {
        new(5, 1.1, 2.2),
        new(6, 10.1, -20.2),
        new(7, -30.1, 40.2) });


    [Fact]
    public void Constructor_Source_SetsSource()
    {
        WayCoordinateList target = new(_nodes);

        Assert.Same(_nodes, target.Source);
    }

    [Fact]
    public void Indexer_Get_ReturnsCoordinatesFromSourceList()
    {
        WayCoordinateList target = new(_nodes);

        for (int i = 0; i < _nodes.Count; i++)
        {
            Assert.Equal(_nodes[i].Position, target[i]);
        }
    }

    [Fact]
    public void Indexer_Set_ThrowsNotSupportedException()
    {
        WayCoordinateList target = new(_nodes);

        Assert.Throws<NotSupportedException>(() => target[0] = new Coordinate(10.1, 11.2));
    }

    [Fact]
    public void Count_GetsNumberOfItemsInSourceCollection()
    {
        WayCoordinateList target = new(_nodes);

        Assert.Equal(_nodes.Count, target.Count);
    }

    [Fact]
    public void Add_Coordinate_ThowsNotSupportedException()
    {
        WayCoordinateList target = new(_nodes);

        Assert.Throws<NotSupportedException>(() => target.Add(Coordinate.Empty));
    }

    [Fact]
    public void Add_Coordinates_ThowsNotSupportedException()
    {
        WayCoordinateList target = new(_nodes);

        Assert.Throws<NotSupportedException>(() => target.Add(new Coordinate[] { Coordinate.Empty }));
    }

    [Fact]
    public void Insert_Index_Coordinate_ThowsNotSupportedException()
    {
        WayCoordinateList target = new(_nodes);

        Assert.Throws<NotSupportedException>(() => target.Insert(0, Coordinate.Empty));
    }

    [Fact]
    public void RemoveAt_Index_ThowsNotSupportedException()
    {
        WayCoordinateList target = new(_nodes);

        Assert.Throws<NotSupportedException>(() => target.RemoveAt(0));
    }

    [Fact]
    public void Clear_ThowsNotSupportedException()
    {
        WayCoordinateList target = new(_nodes);

        Assert.Throws<NotSupportedException>(target.Clear);
    }
}
