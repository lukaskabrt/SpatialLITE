using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Xunit;
using Moq;

using SpatialLite.Osm;
using SpatialLite.Osm.IO;
using SpatialLite.Osm.IO.Xml;
using SpatialLite.Osm.Geometries;

using Tests.SpatialLite.Osm.Data;

namespace Tests.SpatialLite.Osm {
	public class OsmDatabaseTests {
		Node[] _nodeData;
		Way[] _wayData;
		Relation[] _relationData;
		IOsmGeometry[] _data;

		public OsmDatabaseTests() {
			_nodeData = new Node[3];
			_nodeData[0] = new Node(1);
			_nodeData[1] = new Node(2);
			_nodeData[2] = new Node(3);

			_wayData = new Way[2];
			_wayData[0] = new Way(10, _nodeData);
			_wayData[1] = new Way(11, _nodeData.Skip(1));

			_relationData = new Relation[2];
			_relationData[0] = new Relation(100, new RelationMember[] { new RelationMember(_wayData[0], "way"), new RelationMember(_nodeData[0], "node") });
			_relationData[1] = new Relation(101, new RelationMember[] { new RelationMember(_relationData[0], "relation"), new RelationMember(_nodeData[0], "node") });

			_data = _nodeData.Concat<IOsmGeometry>(_wayData).Concat<IOsmGeometry>(_relationData).ToArray();
		}

		#region Constructor() tests

		[Fact]
		public void Constructor__CreatesEmptyDatabase() {
			OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new OsmDatabase<IOsmGeometry, Node, Way, Relation>();

			Assert.Empty(target);
			Assert.Empty(target.Nodes);
			Assert.Empty(target.Ways);
			Assert.Empty(target.Relations);
		}

		#endregion

		#region Constructor(IEnumerable<T>) tests

		[Fact]
		public void Constructor_IEnumerable_CreatesCollectionWithSpecifiedItems() {
			OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new OsmDatabase<IOsmGeometry, Node, Way, Relation>(_data);

			for (int i = 0; i < _data.Length; i++) {
				Assert.Contains(_data[i], target);
			}
		}

		[Fact]
		public void Constructor_IEnumerable_AddEnittiesToCorrextCollections() {
			OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new OsmDatabase<IOsmGeometry, Node, Way, Relation>(_data);

			for (int i = 0; i < _nodeData.Length; i++) {
				Assert.Contains(_nodeData[i], target.Nodes);
			}

			for (int i = 0; i < _wayData.Length; i++) {
				Assert.Contains(_wayData[i], target.Ways);
			}

			for (int i = 0; i < _relationData.Length; i++) {
				Assert.Contains(_relationData[i], target.Relations);
			}
		}

		#endregion


		#region Count property tests

		[Fact]
		public void Count_ReturnsNumberOfAllEntities() {
			OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new OsmDatabase<IOsmGeometry, Node, Way, Relation>(_data);

			Assert.Equal(_data.Length, target.Count);
		}

		#endregion

		#region IsReadOnly property tests

		[Fact]
		public void IsReadOnly_ReturnsFalse() {
			OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new OsmDatabase<IOsmGeometry, Node, Way, Relation>(_data);

			Assert.False(target.IsReadOnly);
		}

		#endregion

		#region Item[ID] tests

		[Fact]
		public void Item_ReturnsNullIfIDIsNotPresentInCollection() {
			OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new OsmDatabase<IOsmGeometry, Node, Way, Relation>(_data);

			Assert.Null(target[10000]);
		}

		[Fact]
		public void Item_ReturnsNodeWithSpecificID() {
			OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new OsmDatabase<IOsmGeometry, Node, Way, Relation>(_data);
			IOsmGeometry entity = target[_nodeData[0].ID];

			Assert.Same(_nodeData[0], entity);
		}

		[Fact]
		public void Item_ReturnsWayWithSpecificID() {
			OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new OsmDatabase<IOsmGeometry, Node, Way, Relation>(_data);
			IOsmGeometry entity = target[_wayData[0].ID];

			Assert.Same(_wayData[0], entity);
		}

		[Fact]
		public void Item_ReturnsRelationWithSpecificID() {
			OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new OsmDatabase<IOsmGeometry, Node, Way, Relation>(_data);
			IOsmGeometry entity = target[_relationData[0].ID];

			Assert.Same(_relationData[0], entity);
		}

		#endregion


		#region Add(IOsmGeometry) tests

		[Fact]
		public void Add_AddsNodeToCollection() {
			OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new OsmDatabase<IOsmGeometry, Node, Way, Relation>(_nodeData.Skip(1));
			target.Add(_nodeData[0]);

			Assert.Contains(_nodeData[0], target);
		}


		[Fact]
		public void Add_AddsWayToCollection() {
			OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new OsmDatabase<IOsmGeometry, Node, Way, Relation>(_wayData.Skip(1));
			target.Add(_wayData[0]);

			Assert.Contains(_wayData[0], target);
		}


		[Fact]
		public void Add_AddsRelationToCollection() {
			OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new OsmDatabase<IOsmGeometry, Node, Way, Relation>(_relationData.Skip(1));
			target.Add(_relationData[0]);

			Assert.Contains(_relationData[0], target);
		}

		[Fact]
		public void Add_ThrowsArgumentNullExceptionIfItemIsNull() {
			OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new OsmDatabase<IOsmGeometry, Node, Way, Relation>();

			Assert.Throws<ArgumentNullException>(() => target.Add(null));
		}

		[Fact]
		public void Add_ThrowsExceptionWhenAddingDuplicateID() {
			OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new OsmDatabase<IOsmGeometry, Node, Way, Relation>(_data);

			Assert.Throws<ArgumentException>(() => target.Add(_data[0]));
		}

		#endregion
		
		#region Clear() tests

		[Fact]
		public void Clear_RemovesAllItemsFromCollection() {
			OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new OsmDatabase<IOsmGeometry, Node, Way, Relation>(_data);
			target.Clear();

			Assert.Empty(target);
			Assert.Empty(target.Nodes);
			Assert.Empty(target.Ways);
			Assert.Empty(target.Relations);
		}

		#endregion

		#region Contains(IOsmGeometry) tests

		[Fact]
		public void Contains_IOsmGeometry_ReturnsFalseForNull() {
			OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new OsmDatabase<IOsmGeometry, Node, Way, Relation>(_data);

			Assert.False(target.Contains(null));
		}

		[Fact]
		public void Contains_IOsmGeometry_ReturnsFalseIfCollectionDoesNotContainNode() {
			OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new OsmDatabase<IOsmGeometry, Node, Way, Relation>(_data);

			Assert.False(target.Contains(new Node(10000)));
		}

		[Fact]
		public void Contains_IOsmGeometry_ReturnsTrueIfCollectionContainsNode() {
			OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new OsmDatabase<IOsmGeometry, Node, Way, Relation>(_data);

			Assert.True(target.Contains(_nodeData[0]));
		}

		[Fact]
		public void Contains_IOsmGeometry_ReturnsFalseIfCollectionDoesNotContainWay() {
			OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new OsmDatabase<IOsmGeometry, Node, Way, Relation>(_data);

			Assert.False(target.Contains(new Way(10000)));
		}

		[Fact]
		public void Contains_IOsmGeometry_ReturnsTrueIfCollectionContainsWay() {
			OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new OsmDatabase<IOsmGeometry, Node, Way, Relation>(_data);

			Assert.True(target.Contains(_wayData[0]));
		}

		[Fact]
		public void Contains_IOsmGeometry_ReturnsFalseIfCollectionDoesNotContainRelation() {
			OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new OsmDatabase<IOsmGeometry, Node, Way, Relation>(_data);

			Assert.False(target.Contains(new Relation(10000)));
		}

		[Fact]
		public void Contains_IOsmGeometry_ReturnsTrueIfCollectionContainsRelation() {
			OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new OsmDatabase<IOsmGeometry, Node, Way, Relation>(_data);

			Assert.True(target.Contains(_relationData[0]));
		}

		#endregion

		#region Contains(ID) tests

		[Fact]
		public void Contains_ID_ReturnsFalseIfCollectionDoesNotContainNodeID() {
			OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new OsmDatabase<IOsmGeometry, Node, Way, Relation>(_data);

			Assert.False(target.Contains(10000));
		}

		[Fact]
		public void Contains_ID_ReturnsTrueIfCollectionContainsNodeID() {
			OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new OsmDatabase<IOsmGeometry, Node, Way, Relation>(_data);

			Assert.True(target.Contains(_nodeData[0].ID));
		}

		[Fact]
		public void Contains_ID_ReturnsFalseIfCollectionDoesNotContainWayID() {
			OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new OsmDatabase<IOsmGeometry, Node, Way, Relation>(_data);

			Assert.False(target.Contains(10000));
		}

		[Fact]
		public void Contains_ID_ReturnsTrueIfCollectionContainsWayID() {
			OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new OsmDatabase<IOsmGeometry, Node, Way, Relation>(_data);

			Assert.True(target.Contains(_wayData[0].ID));
		}

		[Fact]
		public void Contains_ID_ReturnsFalseIfCollectionDoesNotContainRelationID() {
			OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new OsmDatabase<IOsmGeometry, Node, Way, Relation>(_data);

			Assert.False(target.Contains(10000));
		}

		[Fact]
		public void Contains_ID_ReturnsTrueIfCollectionContainsRelationID() {
			OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new OsmDatabase<IOsmGeometry, Node, Way, Relation>(_data);

			Assert.True(target.Contains(_relationData[0].ID));
		}

		#endregion

		#region Remove(IOsmGeometry) tests

		[Fact]
		public void Remove_IOsmGeometry_ReturnsFalseIfItemIsNull() {
			OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new OsmDatabase<IOsmGeometry, Node, Way, Relation>(_data);

			bool callResult = target.Remove(null);

			Assert.False(callResult);
		}

		[Fact]
		public void Remove_IOsmGeometry_ReturnsFalseAndDoesntModifyCollectionIfNodeIsNotPresent() {
			OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new OsmDatabase<IOsmGeometry, Node, Way, Relation>(_nodeData.Skip(1));

			bool callResult = target.Remove(_nodeData[0]);

			Assert.False(callResult);
			Assert.Contains(_nodeData[1], target);
			Assert.Contains(_nodeData[2], target);
		}

		[Fact]
		public void Remove_IOsmGeometry_ReturnsTrueAndRemovesNodeFromCollection() {
			OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new OsmDatabase<IOsmGeometry, Node, Way, Relation>(_nodeData);

			bool callResult = target.Remove(_data[0]);

			Assert.True(callResult);
			Assert.DoesNotContain(_nodeData[0], target);
			Assert.Contains(_nodeData[1], target);
			Assert.Contains(_nodeData[2], target);
		}

		[Fact]
		public void Remove_IOsmGeometry_ReturnsFalseAndDoesntModifyCollectionIfWayIsNotPresent() {
			OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new OsmDatabase<IOsmGeometry, Node, Way, Relation>(_wayData.Skip(1));

			bool callResult = target.Remove(_wayData[0]);

			Assert.False(callResult);
			Assert.Contains(_wayData[1], target);
		}

		[Fact]
		public void Remove_IOsmGeometry_ReturnsTrueAndRemovesWayFromCollection() {
			OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new OsmDatabase<IOsmGeometry, Node, Way, Relation>(_wayData);

			bool callResult = target.Remove(_wayData[0]);

			Assert.True(callResult);
			Assert.DoesNotContain(_wayData[0], target);
			Assert.Contains(_wayData[1], target);
		}

		[Fact]
		public void Remove_IOsmGeometry_ReturnsFalseAndDoesntModifyCollectionIfRelationIsNotPresent() {
			OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new OsmDatabase<IOsmGeometry, Node, Way, Relation>(_relationData.Skip(1));

			bool callResult = target.Remove(_relationData[0]);

			Assert.False(callResult);
			Assert.Contains(_relationData[1], target);
		}

		[Fact]
		public void Remove_IOsmGeometry_ReturnsTrueAndRemovesRelationFromCollection() {
			OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new OsmDatabase<IOsmGeometry, Node, Way, Relation>(_relationData);

			bool callResult = target.Remove(_relationData[0]);

			Assert.True(callResult);
			Assert.DoesNotContain(_relationData[0], target);
			Assert.Contains(_relationData[1], target);
		}

		#endregion

		#region Remove(ID) tests

		[Fact]
		public void Remove_ID_ReturnsFalseAndDoesntModifyCollectionIfNodeIsNotPresent() {
			OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new OsmDatabase<IOsmGeometry, Node, Way, Relation>(_nodeData.Skip(1));

			bool callResult = target.Remove(_nodeData[0].ID);

			Assert.False(callResult);
			Assert.Contains(_nodeData[1], target);
			Assert.Contains(_nodeData[2], target);
		}

		[Fact]
		public void Remove_ID_ReturnsTrueAndRemovesNodeFromCollection() {
			OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new OsmDatabase<IOsmGeometry, Node, Way, Relation>(_nodeData);

			bool callResult = target.Remove(_data[0].ID);

			Assert.True(callResult);
			Assert.DoesNotContain(_nodeData[0], target);
			Assert.Contains(_nodeData[1], target);
			Assert.Contains(_nodeData[2], target);
		}

		[Fact]
		public void Remove_ID_ReturnsFalseAndDoesntModifyCollectionIfWayIsNotPresent() {
			OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new OsmDatabase<IOsmGeometry, Node, Way, Relation>(_wayData.Skip(1));

			bool callResult = target.Remove(_wayData[0].ID);

			Assert.False(callResult);
			Assert.Contains(_wayData[1], target);
		}

		[Fact]
		public void Remove_ID_ReturnsTrueAndRemovesWayFromCollection() {
			OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new OsmDatabase<IOsmGeometry, Node, Way, Relation>(_wayData);

			bool callResult = target.Remove(_wayData[0].ID);

			Assert.True(callResult);
			Assert.DoesNotContain(_wayData[0], target);
			Assert.Contains(_wayData[1], target);
		}

		[Fact]
		public void Remove_ID_ReturnsFalseAndDoesntModifyCollectionIfRelationIsNotPresent() {
			OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new OsmDatabase<IOsmGeometry, Node, Way, Relation>(_relationData.Skip(1));

			bool callResult = target.Remove(_relationData[0].ID);

			Assert.False(callResult);
			Assert.Contains(_relationData[1], target);
		}

		[Fact]
		public void Remove_ID_ReturnsTrueAndRemovesRelationFromCollection() {
			OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new OsmDatabase<IOsmGeometry, Node, Way, Relation>(_relationData);

			bool callResult = target.Remove(_relationData[0].ID);

			Assert.True(callResult);
			Assert.DoesNotContain(_relationData[0], target);
			Assert.Contains(_relationData[1], target);
		}

		#endregion

		#region IEnumerable<IOsmGeometry>.GetEnumerator() tests

		[Fact]
		public void GetEnumerator_ReturnsEnumeratorThatEnumeratesAllEntities() {
			OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new OsmDatabase<IOsmGeometry, Node, Way, Relation>(_data);

			IEnumerable<IOsmGeometry> result = target;

			Assert.Equal(_data.Length, target.Count());
			foreach (var entity in target) {
				Assert.Contains(entity, _data);
			}
		}

		#endregion

		#region CopyTo(Array, ArrayIndex) tests

		[Fact]
		public void CopyTo_CopiesEntitiesToArray() {
			OsmDatabase<IOsmGeometry, Node, Way, Relation> target = new OsmDatabase<IOsmGeometry, Node, Way, Relation>(_data);
			IOsmGeometry[] array = new IOsmGeometry[_data.Length];

			target.CopyTo(array, 0);

			for (int i = 0; i < _data.Length; i++) {
				Assert.Same(_data[i], array[i]);
			}
		}

		#endregion
	}
}
