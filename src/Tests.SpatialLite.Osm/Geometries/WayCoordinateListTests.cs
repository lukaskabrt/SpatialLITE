using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

using SpatialLite.Core.API;
using SpatialLite.Osm.Geometries;

namespace Tests.SpatialLite.Osm.Geometries
{
    public class WayCoordinateListTests
    {
        List<Node> _nodes = new List<Node>(new Node[] {
            new Node(5, 1.1, 2.2),
            new Node(6, 10.1, -20.2),
            new Node(7, -30.1, 40.2) });


        [Fact]
        public void Constructor_Source_SetsSource()
        {
            WayCoordinateList target = new WayCoordinateList(_nodes);

            Assert.Same(_nodes, target.Source);
        }

        [Fact]
        public void Indexer_Get_ReturnsCoordinatesFromSourceList()
        {
            WayCoordinateList target = new WayCoordinateList(_nodes);

            for (int i = 0; i < _nodes.Count; i++)
            {
                Assert.Equal(_nodes[i].Position, target[i]);
            }
        }

        [Fact]
        public void Indexer_Set_ThrowsNotSupportedException()
        {
            WayCoordinateList target = new WayCoordinateList(_nodes);

            Assert.Throws<NotSupportedException>(() => target[0] = new Coordinate(10.1, 11.2));
        }

        [Fact]
        public void Count_GetsNumberOfItemsInSourceCollection()
        {
            WayCoordinateList target = new WayCoordinateList(_nodes);

            Assert.Equal(_nodes.Count, target.Count);
        }

        [Fact]
        public void Add_Coordinate_ThowsNotSupportedException()
        {
            WayCoordinateList target = new WayCoordinateList(_nodes);

            Assert.Throws<NotSupportedException>(() => target.Add(Coordinate.Empty));
        }

        [Fact]
        public void Add_Coordinates_ThowsNotSupportedException()
        {
            WayCoordinateList target = new WayCoordinateList(_nodes);

            Assert.Throws<NotSupportedException>(() => target.Add(new Coordinate[] { Coordinate.Empty }));
        }

        [Fact]
        public void Insert_Index_Coordinate_ThowsNotSupportedException()
        {
            WayCoordinateList target = new WayCoordinateList(_nodes);

            Assert.Throws<NotSupportedException>(() => target.Insert(0, Coordinate.Empty));
        }

        [Fact]
        public void RemoveAt_Index_ThowsNotSupportedException()
        {
            WayCoordinateList target = new WayCoordinateList(_nodes);

            Assert.Throws<NotSupportedException>(() => target.RemoveAt(0));
        }

        [Fact]
        public void Clear_ThowsNotSupportedException()
        {
            WayCoordinateList target = new WayCoordinateList(_nodes);

            Assert.Throws<NotSupportedException>(() => target.Clear());
        }
    }
}
