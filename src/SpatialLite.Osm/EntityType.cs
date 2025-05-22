namespace SpatialLite.Osm
{
    /// <summary>
    /// Defines possible type of object that IOsmGeometryInfo can represent.
    /// </summary>
    public enum EntityType
    {
        /// <summary>
        /// Unknown type of the entity.
        /// </summary>
        Unknown,
        /// <summary>
        /// Node
        /// </summary>
        Node,
        /// <summary>
        /// Way
        /// </summary>
        Way,
        /// <summary>
        /// Relation
        /// </summary>
        Relation
    }
}
