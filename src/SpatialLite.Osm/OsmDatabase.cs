using System;
using System.Collections.Generic;
using SpatialLite.Osm.IO;

namespace SpatialLite.Osm {
    /// <summary>
    /// Represents in-memory database of OSM entities.
    /// </summary>
    /// <typeparam name="T">Generic type that relresents all nodes, ways and relations</typeparam>
    /// <typeparam name="N">The type of Nodes</typeparam>
    /// <typeparam name="W">The type of Ways</typeparam>
    /// <typeparam name="R">The type of Relations</typeparam>
    public class OsmDatabase<T, N, W, R> : IEntityCollection<T> where T : IOsmEntity where N : T where W : T where R : T {
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the OsmDatabase class that is empty.
		/// </summary>
		internal OsmDatabase() {
			this.Nodes = new EntityCollection<N>();
			this.Ways = new EntityCollection<W>();
			this.Relations = new EntityCollection<R>();
		}

		/// <summary>
		/// Initializes a new instance of the OsmDatabase class with specific entities.
		/// </summary>
		/// <param name="entities">Entities to add to the database.</param>
		internal OsmDatabase(IEnumerable<T> entities)
			: this() {
			foreach (var entity in entities) {
				this.Add(entity);
			}
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets collection of nodes in the database.
		/// </summary>
		public ITypedEntityCollection<N> Nodes { get; private set; }

		/// <summary>
		/// Gets collection of ways in the database.
		/// </summary>
		public ITypedEntityCollection<W> Ways { get; private set; }

		/// <summary>
		/// Gets collection of relations in the database.
		/// </summary>
		public ITypedEntityCollection<R> Relations { get; private set; }

		/// <summary>
		/// Gets the number of entities in the collection.
		/// </summary>
		public int Count {
			get {
				return this.Nodes.Count + this.Ways.Count + this.Relations.Count;
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
        /// <param name="type">The type of the entity to get.</param>
		/// <returns>entity with the specific ID or null if such entity is not present in the collection.</returns>
		public T this[int id, EntityType type] {
			get {
                if (type == EntityType.Node && this.Nodes.Contains(id)) {
					return (T)this.Nodes[id];
				}

				if (type == EntityType.Way && this.Ways.Contains(id)) {
					return (T)this.Ways[id];
				}

				if (type == EntityType.Relation && this.Relations.Contains(id)) {
					return (T)this.Relations[id];
				}

				return default(T);
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// When overriden in derived class, saves entities from the database to specific writer.
		/// </summary>
		/// <param name="writer">IOsmWriter to save entities to.</param>
		public virtual void Save(IOsmWriter writer) {
		}

		/// <summary>
		/// Removes an entity with the specific ID from the collection.
		/// </summary>
		/// <param name="id">The ID of the entity to remove from the collection.</param>
        /// <param name="type">The type of the entity to remove from the collection.</param>
		/// <returns>true if entity was successfully removed from the ICollection; otherwise, false. This method also returns false if entity is not found in the original collection.</returns>
		public bool Remove(int id, EntityType type) {
			if (this.Nodes.Contains(id)) {
				return this.Nodes.Remove(id);
			}

			if (this.Ways.Contains(id)) {
				return this.Ways.Remove(id);
			}

			if (this.Relations.Contains(id)) {
				return this.Relations.Remove(id);
			}

			return false;
		}

		/// <summary>
		/// Determines whether the EntityICollection contains an entity with specific ID.
		/// </summary>
		/// <param name="id">The ID of the entity to locate in the EntityCollection.</param>
        /// <param name="type">The type of the entity to locate in the EntityCollection</param>
		/// <returns>true if entity is found in the collection, otherwise false.</returns>
		public bool Contains(int id, EntityType type) {
            switch (type) {
                case EntityType.Node: return this.Nodes.Contains(id);
                case EntityType.Way: return this.Ways.Contains(id);
                case EntityType.Relation: return this.Relations.Contains(id);
            }

            throw new NotImplementedException();
		}

		/// <summary>
		/// Adds specified entity to the collection.
		/// </summary>
		/// <param name="entity">The entity to add to the collection.</param>
		public void Add(T entity) {
			if (entity == null) {
				throw new ArgumentNullException("entity", "Cannot add null to EntityCollection");
			}

			if (this.Contains(entity.ID, entity.EntityType)) {
				throw new ArgumentException("An entity with the same ID has already been added.");
			}

			switch (entity.EntityType) {
				case EntityType.Node: this.Nodes.Add((N)entity); break;
				case EntityType.Way: this.Ways.Add((W)entity); break;
				case EntityType.Relation: this.Relations.Add((R)entity); break;
			}
		}

		/// <summary>
		/// Removes all entities form the collection.
		/// </summary>
		public void Clear() {
			this.Nodes.Clear();
			this.Ways.Clear();
			this.Relations.Clear();
		}

		/// <summary>
		/// Determines whether the EntityICollection contains a specific entity.
		/// </summary>
		/// <param name="item">The entity to locate in the EntityCollection.</param>
		/// <returns>true if entity is found in the collection, otherwise false.</returns>
		public bool Contains(T item) {
			if (item == null) {
				return false;
			}

			return this.Contains(item.ID, item.EntityType);
		}

		/// <summary>
		/// Copies the entire EntityCollection to a compatible one-dimensional Array, starting at the specified index of the target array.
		/// </summary>
		/// <param name="array">The one-dimensional Array that is the destination of the elements copied from EntityCollection.</param>
		/// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
		public void CopyTo(T[] array, int arrayIndex) {
			foreach (var entity in this) {
				array[arrayIndex++] = entity;
			}
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

			return this.Remove(entity.ID, entity.EntityType);
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>A IEnumerator&lt;T&gt; that can be used to iterate through the collection.</returns>
		public IEnumerator<T> GetEnumerator() {
			foreach (var node in this.Nodes) {
				yield return node;
			}

			foreach (var way in this.Ways) {
				yield return way;
			}

			foreach (var relation in this.Relations) {
				yield return relation;
			}
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
