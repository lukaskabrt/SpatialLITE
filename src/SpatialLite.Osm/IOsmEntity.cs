namespace SpatialLite.Osm {
    /// <summary>
    /// Defines properties for all OSM entities.
    /// </summary>
    public interface IOsmEntity {
		#region Public Properties

		/// <summary>
		/// Gets or sets ID of the object.
		/// </summary>
		int ID { get; set; }

		/// <summary>
		/// Gets or sets the collection of tags associated with the IOsmGeometry.
		/// </summary>
		TagsCollection Tags { get; set; }

		/// <summary>
		/// Gets or sets detailed information about OSM entity.
		/// </summary>
		EntityMetadata Metadata { get; set; }

		/// <summary>
		/// Gets type of the entity.
		/// </summary>
		EntityType EntityType { get; }

		#endregion
	}
}
