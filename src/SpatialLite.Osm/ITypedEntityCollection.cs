using System.Collections.Generic;

namespace SpatialLite.Osm {
    /// <summary>
    /// Defines methods and properties for collection of the OSM entities of single where entities can be accessed by their ID.
    /// </summary>
    /// <typeparam name="T">The type of the entitie in the collection.</typeparam>
    public interface ITypedEntityCollection<T> : ICollection<T> where T : IOsmEntity {
		/// <summary>
		/// Gets an entity with specific ID from the collection.
		/// </summary>
		/// <param name="id">The ID of the entity to get.</param>
		/// <returns>entity with the specific ID or null if such entity is not present in the collection.</returns>
		T this[int id] { get; }

		/// <summary>
		/// Removes an entity with the specific ID from the collection.
		/// </summary>
		/// <param name="id">The ID of the entity to remove from the collection.</param>
		/// <returns>true if entity was successfully removed from the ICollection; otherwise, false. This method also returns false if entity is not found in the original collection.</returns>
		bool Remove(int id);

		/// <summary>
		/// Determines whether the EntityICollection contains an entity with specific ID.
		/// </summary>
		/// <param name="id">The ID of the entity to locate in the EntityCollection.</param>
		/// <returns>true if entity is found in the collection, otherwise false.</returns>
		bool Contains(int id);
	}
}
