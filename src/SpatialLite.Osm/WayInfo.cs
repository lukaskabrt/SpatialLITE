using System;
using System.Collections.Generic;

using SpatialLite.Osm.Geometries;

namespace SpatialLite.Osm;

/// <summary>
/// Represents information about way.
/// </summary>
/// <remarks>
/// Nodes are represented with their id's only.
/// </remarks>
public class WayInfo : IEntityInfo
{

    /// <summary>
    /// Initializes a new instance of the WayInfo class with specified ID, Nodes, collection of tags and optionaly EntityMetadata.
    /// </summary>
    /// <param name="id">The Id of the way.</param>
    /// <param name="tags">The collection of tags associated with the way.</param>
    /// <param name="nodes">The nodes of the way.</param>
    /// <param name="additionalInfo">The EntityMetadata structure with additinal properties. Default value is null.</param>
    public WayInfo(long id, TagsCollection tags, IList<long> nodes, EntityMetadata additionalInfo = null)
    {
        this.ID = id;
        this.Tags = tags;
        this.Nodes = nodes;
        this.Metadata = additionalInfo;
    }

    /// <summary>
    /// Initializes a new instance of the WayInfo class with data from specific Way
    /// </summary>
    /// <param name="way">The way to get data from</param>
		public WayInfo(Way way)
    {
        if (way == null)
        {
            throw new ArgumentNullException(nameof(way), "Way parameter cannot be null");
        }

        this.ID = way.ID;
        this.Tags = way.Tags;
        this.Metadata = way.Metadata;

        this.Nodes = new List<long>(way.Nodes.Count);
        foreach (var node in way.Nodes)
        {
            this.Nodes.Add(node.ID);
        }
    }

    /// <summary>
    /// Gets type of the object that is represented by this WayInfo.
    /// </summary>
    public EntityType EntityType
    {
        get
        {
            return EntityType.Way;
        }
    }

    /// <summary>
    /// Gets ID of the object.
    /// </summary>
    public long ID { get; set; }

    /// <summary>
    /// Gets the collection of tags associated with this WayInfo.
    /// </summary>
    public TagsCollection Tags { get; set; }

    /// <summary>
    /// Gets the list of id's of this way nodes.
    /// </summary>
    public IList<long> Nodes { get; private set; }

    /// <summary>
    /// Gets additional information about this IOsmGeometryInfo.
    /// </summary>
    public EntityMetadata Metadata { get; set; }
}
