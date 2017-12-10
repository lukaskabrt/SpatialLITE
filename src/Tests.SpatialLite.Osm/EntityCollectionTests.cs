using System;
using System.Linq;

using Xunit;
using Moq;

using SpatialLite.Osm;
using SpatialLite.Osm.Geometries;

namespace Tests.SpatialLite.Osm {
    public class EntityCollectionTests {
		IOsmGeometry[] _data;

		public EntityCollectionTests() {
			_data = new IOsmGeometry[3];
			_data[0] = new Node(1);
			_data[1] = new Node(2);
			_data[2] = new Node(3);
		}

		#region Constructor() tests

		[Fact]
		public void Constructor__CreatesEmptyCollection() {
			EntityCollection<IOsmGeometry> target = new EntityCollection<IOsmGeometry>();

			Assert.Empty(target);
		}

		#endregion

		#region Constructor(IEnumerable<T>) tests

		[Fact]
		public void Constructor_IEnumerable_CreatesCollectionWithSpecifiedItems() {
			EntityCollection<IOsmGeometry> target = new EntityCollection<IOsmGeometry>(_data);

			for (int i = 0; i < _data.Length; i++) {
				Assert.Contains(_data[i], target);
			}
		}

		#endregion

		#region Count tests

		[Fact]
		public void Count_ReturnsNumberOfElements() {
			EntityCollection<IOsmGeometry> target = new EntityCollection<IOsmGeometry>(_data);
			Mock<IOsmGeometry> entityM = new Mock<IOsmGeometry>();

			Assert.Equal(_data.Length, target.Count);
		}

		#endregion

		#region Contains(T) tests

		[Fact]
		public void Contains_IOsmGeometry_ReturnsFalseForEmptyCollection() {
			EntityCollection<IOsmGeometry> target = new EntityCollection<IOsmGeometry>();

			Assert.False(target.Contains(_data[0]));
		}

		[Fact]
		public void Contains_IOsmGeometry_ReturnsFalseForNull() {
			EntityCollection<IOsmGeometry> target = new EntityCollection<IOsmGeometry>();

			Assert.False(target.Contains(null));
		}

		[Fact]
		public void Contains_IOsmGeometry_ReturnsFalseIfCollectionDoesNotContainEntity() {
			EntityCollection<IOsmGeometry> target = new EntityCollection<IOsmGeometry>(_data.Skip(1));

			Assert.False(target.Contains(_data[0]));
		}

		[Fact]
		public void Contains_IOsmGeometry_ReturnsTrueIfCollectionContainsEntity() {
			EntityCollection<IOsmGeometry> target = new EntityCollection<IOsmGeometry>(_data);

			Assert.True(target.Contains(_data[0]));
		}

		#endregion

		#region Contains(ID) tests

		[Fact]
		public void Contains_ID_ReturnsFalseForEmptyCollection() {
			EntityCollection<IOsmGeometry> target = new EntityCollection<IOsmGeometry>();

			Assert.False(target.Contains(_data[0].ID));
		}

		[Fact]
		public void Contains_ID_ReturnsFalseIfCollectionDoesNotContainEntity() {
			EntityCollection<IOsmGeometry> target = new EntityCollection<IOsmGeometry>(_data.Skip(1));

			Assert.False(target.Contains(_data[0].ID));
		}

		[Fact]
		public void Contains_ID_ReturnsTrueIfCollectionContainsEntity() {
			EntityCollection<IOsmGeometry> target = new EntityCollection<IOsmGeometry>(_data);

			Assert.True(target.Contains(_data[0].ID));
		}

		#endregion

		#region Clear() tests

		[Fact]
		public void Clear_RemovesAllItemsFromCollection() {
			EntityCollection<IOsmGeometry> target = new EntityCollection<IOsmGeometry>(_data);
			target.Clear();

			Assert.Empty(target);
		}

		#endregion

		#region Add(T) tests

		[Fact]
		public void Add_AddsEntityToCollection() {
			EntityCollection<IOsmGeometry> target = new EntityCollection<IOsmGeometry>();
			target.Add(_data[0]);

			Assert.Contains(_data[0], target);
		}

		[Fact]
		public void Add_ThrowsArgumentNullExceptionIfItemIsNull() {
			EntityCollection<IOsmGeometry> target = new EntityCollection<IOsmGeometry>();

			Assert.Throws<ArgumentNullException>(() => target.Add(null));
		}

		[Fact]
		public void Add_ThrowsExceptionWhenAddingDuplicateID() {
			EntityCollection<IOsmGeometry> target = new EntityCollection<IOsmGeometry>(_data);

			Assert.Throws<ArgumentException>(() => target.Add(_data[0]));
		}

		#endregion

		#region IsReadOnly tests

		[Fact]
		public void IsReadOnly_ReturnsFalse() {
			EntityCollection<IOsmGeometry> target = new EntityCollection<IOsmGeometry>();

			Assert.False(target.IsReadOnly);
		}

		#endregion

		#region Remove(T) tests

		[Fact]
		public void Remove_IOsmGeometry_ReturnsFalseAndDoesntModifyCollectionIfItemIsNotPresent() {
			EntityCollection<IOsmGeometry> target = new EntityCollection<IOsmGeometry>(_data.Skip(1));

			bool callResult = target.Remove(_data[0]);

			Assert.False(callResult);
			Assert.Contains(_data[1], target);
			Assert.Contains(_data[2], target);
		}

		[Fact]
		public void Remove_IOsmGeometry_ReturnsFalseIfItemIsNull() {
			EntityCollection<IOsmGeometry> target = new EntityCollection<IOsmGeometry>(_data);
			
			bool callResult = target.Remove(null);

			Assert.False(callResult);
		}

		[Fact]
		public void Remove_IOsmGeometry_ReturnsTrueAndRemovesItemFromCollection() {
			EntityCollection<IOsmGeometry> target = new EntityCollection<IOsmGeometry>(_data);

			bool callResult = target.Remove(_data[0]);

			Assert.True(callResult);
			Assert.DoesNotContain(_data[0], target);
			Assert.Contains(_data[1], target);
			Assert.Contains(_data[2], target);
		}

		#endregion

		#region Remove(ID) tests

		[Fact]
		public void Remove_ID_ReturnsFalseAndDoesntModifyCollectionIfItemIsNotPresent() {
			EntityCollection<IOsmGeometry> target = new EntityCollection<IOsmGeometry>(_data.Skip(1));

			bool callResult = target.Remove(_data[0].ID);

			Assert.False(callResult);
			Assert.Contains(_data[1], target);
			Assert.Contains(_data[2], target);
		}

		[Fact]
		public void Remove_ID_ReturnsTrueAndRemovesItemFromCollection() {
			EntityCollection<IOsmGeometry> target = new EntityCollection<IOsmGeometry>(_data);

			bool callResult = target.Remove(_data[0].ID);

			Assert.True(callResult);
			Assert.DoesNotContain(_data[0], target);
			Assert.Contains(_data[1], target);
			Assert.Contains(_data[2], target);
		}

		#endregion

		#region Item[ID] tests

		[Fact]
		public void Item_ReturnsNullIfIDIsNotpResentInCollection() {
			EntityCollection<IOsmGeometry> target = new EntityCollection<IOsmGeometry>(_data);

			Assert.Null(target[1000]);
		}

		[Fact]
		public void Item_ReturnsEntityWitSpecificID() {
			EntityCollection<IOsmGeometry> target = new EntityCollection<IOsmGeometry>(_data);
			IOsmGeometry entity = target[1];

			Assert.Equal(1, entity.ID);
		}

		#endregion
	}
}
