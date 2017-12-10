using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xunit;

using SpatialLite.Core.API;
using SpatialLite.Core.Geometries;

namespace Tests.SpatialLite.Core.Geometries {
	public class CoordinateListTests {
		#region Test data

		Coordinate _coordinate = new Coordinate(-10, -20, -200);

		Coordinate[] _coordinates = new Coordinate[] {
				new Coordinate(12,10,100),
				new Coordinate(22,20,200),
				new Coordinate(32,30,300)
		};

		#endregion

		#region Constructors tests

		#region Constructor() tests

		[Fact]
		public void Constructor__CreatesEmptyList() {
			CoordinateList target = new CoordinateList();

			Assert.Empty(target);
		}

		#endregion

		#region Constructor(IEnumerable<Coordinate>) tests

		[Fact]
		public void Constructor_IEnumerable_CreatesListWithSpecifiedItems() {
			CoordinateList target = new CoordinateList(_coordinates);

			for (int i = 0; i < _coordinates.Length; i++) {
				Assert.Equal(_coordinates[i], target[i]);
			}
		}

		#endregion

		#endregion

		#region Indexer tests

		[Fact]
		public void Indexer_GetsAndSetsValues() {
			CoordinateList target = new CoordinateList(_coordinates);

			Assert.Equal(_coordinates[1], target[1]);

			target[1] = _coordinate;
			Assert.Equal(_coordinate, target[1]);
		}

		#endregion

		#region Count property tests

		[Fact]
		public void Count_Returns0ForEmptyList() {
			CoordinateList target = new CoordinateList();

			Assert.Equal(0, target.Count);
		}

		[Fact]
		public void Count_ReturnsNumberOfCoordinates() {
			CoordinateList target = new CoordinateList(_coordinates);

			Assert.Equal(_coordinates.Length, target.Count);
		}

		#endregion

		#region Add(Coordinate) tests

		[Fact]
		public void Add_AppendsCoordinateToTheEndOfList() {
			CoordinateList target = new CoordinateList(_coordinates);
			target.Add(_coordinate);

			Assert.Equal(_coordinate, target.Last());
		}

		[Fact]
		public void Add_IncresesCount() {
			CoordinateList target = new CoordinateList(_coordinates);
			target.Add(_coordinate);

			Assert.Equal(_coordinates.Length + 1, target.Count);
		}

		#endregion

		#region Add(IEnumerable<Coordinate>) tests

		[Fact]
		public void Add_AppendsCollectionOfCoordinatesToTheEndOfList() {
			CoordinateList target = new CoordinateList(new Coordinate[] {_coordinate});
			target.Add(_coordinates);

			for (int i = 0; i < _coordinates.Length; i++) {
				Assert.Equal(_coordinates[i], target[i + 1]);
			}
		}

		[Fact]
		public void Add_IncresesCount2() {
			CoordinateList target = new CoordinateList(new Coordinate[] { _coordinate });
			target.Add(_coordinates);

			Assert.Equal(_coordinates.Length + 1, target.Count);
		}

		#endregion

		#region Insert tests

		[Fact]
		public void Insert_InsertsCoordinateToSpecifiedIndex() {
			int index = 1;
			CoordinateList target = new CoordinateList(_coordinates);
			target.Insert(index, _coordinate);

			Assert.Equal(_coordinate, target[index]);
		}

		[Fact]
		public void Insert_IncresesCount() {
			int index = 1;
			CoordinateList target = new CoordinateList(_coordinates);
			target.Insert(index, _coordinate);

			Assert.Equal(_coordinates.Length + 1, target.Count);
		}

		[Fact]
		public void Insert_AppendsCoordinateToListIfIndexEqulasCount() {
			CoordinateList target = new CoordinateList(_coordinates);
			target.Insert(target.Count, _coordinate);

			Assert.Equal(_coordinate, target.Last());
			Assert.Equal(_coordinates.Length + 1, target.Count);
		}

		#endregion

		#region RemoveAt(index) tests

		[Fact]
		public void RemoveAt_RemovesCoordinateAtIndex() {
			CoordinateList target = new CoordinateList(_coordinates);
			target.RemoveAt(1);

			Assert.Equal(_coordinates[0], target[0]);
			Assert.Equal(_coordinates[2], target[1]);
		}


		[Fact]
		public void RemoveAt_DecreasesCount() {
			CoordinateList target = new CoordinateList(_coordinates);
			target.RemoveAt(1);

			Assert.Equal(_coordinates.Length - 1, target.Count);
		}

		#endregion

		#region Clear() tests

		[Fact]
		public void Clear_RemovesAllCoordinatesFromList() {
			CoordinateList target = new CoordinateList(_coordinates);
			target.Clear();

			Assert.Empty(target.ToArray());
			Assert.Equal(0, target.Count);
		}

		#endregion
	}
}
