using System.Collections.Generic;

namespace SpatialLite.Osm {
    /// <summary>
    /// Defines methods and properties for collection of the OSM entities where entities can be accessed by their ID and type.
    /// </summary>
    /// <typeparam name="T">The type of the entitie in the collection.</typeparam>
    public interface IEntityCollection<T> : ICollection<T> where T : IOsmEntity {
        /// <summary>
        /// Gets an entity with specific ID from the collection.
        /// </summary>
        /// <param name="id">The ID of the entity to get.</param>
        /// <param name="type">The type of the entity to get.</param>
        /// <returns>entity with the specific ID or null if such entity is not present in the collection.</returns>
        T this[int id, EntityType type] { get; }

        /// <summary>
        /// Removes an entity with the specific ID from the collection.
        /// </summary>
        /// <param name="id">The ID of the entity to remove from the collection.</param>
        /// <param name="type">The type of the entity to remove from the collection.</param>
        /// <returns>true if entity was successfully removed from the ICollection; otherwise, false. This method also returns false if entity is not found in the original collection.</returns>
        bool Remove(int id, EntityType type);

        /// <summary>
        /// Determines whether the EntityICollection contains an entity with specific ID.
        /// </summary>
        /// <param name="id">The ID of the entity to locate in the EntityCollection.</param>
        /// <param name="type">The type of the entity to locate in the EntityCollection</param>
        /// <returns>true if entity is found in the collection, otherwise false.</returns>
        bool Contains(int id, EntityType type);
    }
}
