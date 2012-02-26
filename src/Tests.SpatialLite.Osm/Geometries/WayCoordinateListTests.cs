using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xunit;

using SpatialLite.Core.API;
using SpatialLite.Osm.Geometries;

namespace Tests.SpatialLite.Osm.Geometries {
	public class WayCoordinateListTests {
		List<Node> _nodes = new List<Node>(new Node[] { 
			new Node(5, 1.1, 2.2),
			new Node(6, 10.1, -20.2),
			new Node(7, -30.1, 40.2) });

		#region Constructor(SourceList)

		[Fact]
		public void Constructor_Source_SetsSource() {
			WayCoordinateList target = new WayCoordinateList(_nodes);

			Assert.Same(_nodes, target.Source);
		}

		#endregion

		#region Indexer tests

		[Fact]
		public void Indexer_Get_ReturnsCoordinatesFromSourceList() {
			WayCoordinateList target = new WayCoordinateList(_nodes);

			for (int i = 0; i < _nodes.Count; i++) {
				Assert.Equal(_nodes[i].Position, target[i]);
			}
		}

		[Fact]
		public void Indexer_Set_ThrowsNotSupportedException() {
			WayCoordinateList target = new WayCoordinateList(_nodes);

			Assert.Throws<NotSupportedException>(() => target[0] = new Coordinate(10.1, 11.2));
		}

		#endregion

		#region Count property

		[Fact]
		public void Count_GetsNumberOfItemsInSourceCollection() {
			WayCoordinateList target = new WayCoordinateList(_nodes);

			Assert.Equal(_nodes.Count, target.Count);
		}

		#endregion
		
		#region Add(Coordinate) tests

		[Fact]
		public void Add_Coordinate_ThowsNotSupportedException() {
			WayCoordinateList target = new WayCoordinateList(_nodes);

			Assert.Throws<NotSupportedException>(() => target.Add(Coordinate.Empty));
		}

		#endregion

		#region Add(Coordinates) tests

		[Fact]
		public void Add_Coordinates_ThowsNotSupportedException() {
			WayCoordinateList target = new WayCoordinateList(_nodes);

			Assert.Throws<NotSupportedException>(() => target.Add(new Coordinate[] {Coordinate.Empty}));
		}

		#endregion

		#region Insert(Index, Coordinate) tests

		[Fact]
		public void Insert_Index_Coordinate_ThowsNotSupportedException() {
			WayCoordinateList target = new WayCoordinateList(_nodes);

			Assert.Throws<NotSupportedException>(() => target.Insert(0, Coordinate.Empty));
		}

		#endregion

		#region RemoveAt(Index) tests

		[Fact]
		public void RemoveAt_Index_ThowsNotSupportedException() {
			WayCoordinateList target = new WayCoordinateList(_nodes);

			Assert.Throws<NotSupportedException>(() => target.RemoveAt(0));
		}

		#endregion

		#region Clear() tests

		[Fact]
		public void Clear_ThowsNotSupportedException() {
			WayCoordinateList target = new WayCoordinateList(_nodes);

			Assert.Throws<NotSupportedException>(() => target.Clear());
		}

		#endregion
	}
}
