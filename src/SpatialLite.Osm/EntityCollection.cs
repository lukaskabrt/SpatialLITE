using System;
using System.Collections.Generic;

namespace SpatialLite.Osm {
	/// <summary>
	/// Represents collection of the OSM entities that optimized for access speed.
	/// </summary>
	/// <typeparam name="T">The type of the entities in the collection.</typeparam>
	public class EntityCollection<T> : ITypedEntityCollection<T> where T : IOsmEntity {
		#region Private Fields

		private Dictionary<long, T> _storage = null;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the EntityCollection class that is empty.
		/// </summary>
		public EntityCollection() {
			_storage = new Dictionary<long, T>();
		}

		/// <summary>
		/// Initializes a new instance of the EntityCollection class with specified entities.
		/// </summary>
		/// <param name="entities">Collection of entities to be populated into this EntityCollection.</param>
		public EntityCollection(IEnumerable<T> entities) {
			_storage = new Dictionary<long, T>();

			foreach (var entity in entities) {
				_storage.Add(entity.ID, entity);
			}
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the number of entities in the collection.
		/// </summary>
		public int Count {
			get {
				return _storage.Count;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the collection is read-only. 
		/// </summary>
		public bool IsReadOnly {
			get {
				return false;
			}
		}

		/// <summary>
		/// Gets an entity with specific ID from the collection.
		/// </summary>
		/// <param name="id">The ID of the entity to get.</param>
		/// <returns>entity with the specific ID or null if such entity is not present in the collection.</returns>
		public T this[long id] {
			get {
				if (_storage.ContainsKey(id)) {
					return _storage[id];
				}

				return default(T);
			}
		}

		/// <summary>
		/// Adds specific entity to the collection.
		/// </summary>
		/// <param name="entity">The entity to add to the collection.</param>
		public void Add(T entity) {
			if (entity == null) {
				throw new ArgumentNullException("entity", "Cannot add null to EntityCollection");
			}

			if (_storage.ContainsKey(entity.ID)) {
				throw new ArgumentException("An entity with the same ID has already been added.");
			}

			_storage.Add(entity.ID, entity);
		}

		/// <summary>
		/// Removes all entities form the collection.
		/// </summary>
		public void Clear() {
			_storage.Clear();
		}

		/// <summary>
		/// Determines whether the EntityICollection contains a specific entity.
		/// </summary>
		/// <param name="item">The entity to locate in the EntityCollection</param>
		/// <returns>true if entity is found in the collection, otherwise false.</returns>
		public bool Contains(T item) {
			if (item == null) {
				return false;
			}

			return _storage.ContainsKey(item.ID);
		}

		/// <summary>
		/// Determines whether the EntityICollection contains an entity with specific ID.
		/// </summary>
		/// <param name="id">The ID of the entity to locate in the EntityCollection.</param>
		/// <returns>true if entity is found in the collection, otherwise false.</returns>
		public bool Contains(long id) {
			return _storage.ContainsKey(id);
		}

		/// <summary>
		/// Copies the entire EntityCollection to a compatible one-dimensional Array, starting at the specified index of the target array.
		/// </summary>
		/// <param name="array">The one-dimensional Array that is the destination of the elements copied from EntityCollection.</param>
		/// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
		public void CopyTo(T[] array, int arrayIndex) {
			_storage.Values.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Removes the specific entity from the collection.
		/// </summary>
		/// <param name="entity">The entity to remove from the collection.</param>
		/// <returns>true if entity was successfully removed from the ICollection; otherwise, false. This method also returns false if entity is not found in the original collection.</returns>
		public bool Remove(T entity) {
			if (entity == null) {
				return false;
			}

			return _storage.Remove(entity.ID);
		}

		/// <summary>
		/// Removes an entity with the specific ID from the collection.
		/// </summary>
		/// <param name="id">The ID of the entity to remove from the collection.</param>
		/// <returns>true if entity was successfully removed from the ICollection; otherwise, false. This method also returns false if entity is not found in the original collection.</returns>
		public bool Remove(long id) {
			return _storage.Remove(id);
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>A IEnumerator&lt;T&gt; that can be used to iterate through the collection.</returns>
		public IEnumerator<T> GetEnumerator() {
			return _storage.Values.GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>A IEnumerator that can be used to iterate through the collection.</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}

		#endregion
	}
}
