using SpatialLite.Osm.Geometries;
using System;

namespace SpatialLite.Osm;

/// <summary>
/// Represents information about node.
/// </summary>
public class NodeInfo : IEntityInfo
{

    /// <summary>
    /// Initializes a new instance of the NodeInfo class with specified ID, latitude, longitude, collection of tags and optionally EntityMetadata.
    /// </summary>
    /// <param name="id">The id of the node.</param>
    /// <param name="latitude">The latitude of the node.</param>
    /// <param name="longitude">The longitude of the node.</param>
    /// <param name="tags">The collection of thag associated with the node.</param>
    /// <param name="additionalInfo">The EntityMetadata structure with additinal properties. Default value is null.</param>
    public NodeInfo(long id, double latitude, double longitude, TagsCollection tags, EntityMetadata additionalInfo = null)
    {
        ID = id;
        Latitude = latitude;
        Longitude = longitude;
        Tags = tags;
        Metadata = additionalInfo;
    }

    /// <summary>
    /// Initializes a new instance of the NodeInfo class with data from Node.
    /// </summary>
    /// <param name="node">The Node obejct to take data from.</param>
    public NodeInfo(Node node)
    {
        if (node == null)
        {
            throw new ArgumentNullException(nameof(node), "Node parameter cannot be null");
        }

        ID = node.ID;
        Longitude = node.Position.X;
        Latitude = node.Position.Y;
        Tags = node.Tags;
        Metadata = node.Metadata;
    }

    /// <summary>
    /// Gets type of the object that is represented by this IOsmGeometryInfo.
    /// </summary>
    public EntityType EntityType
    {
        get
        {
            return EntityType.Node;
        }
    }

    /// <summary>
    /// Gets ID of the object.
    /// </summary>
    public long ID { get; set; }

    /// <summary>
    /// Gets the collection of tags associated with this node.
    /// </summary>
    public TagsCollection Tags { get; set; }

    /// <summary>
    /// Gets the latitude of the node.
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// Gets the longitude of the node.
    /// </summary>
    public double Longitude { get; set; }

    /// <summary>
    /// Gets or sets metadata of this Node.
    /// </summary>
    public EntityMetadata Metadata { get; set; }
}
