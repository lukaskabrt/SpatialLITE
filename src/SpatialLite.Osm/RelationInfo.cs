using SpatialLite.Osm.Geometries;
using System;
using System.Collections.Generic;

namespace SpatialLite.Osm;

/// <summary>
/// Represents information about relation.
/// </summary>
public class RelationInfo : IEntityInfo
{

    /// <summary>
    /// Initializes a new instance of the RelationInfo class with specified ID, Tags, Member and optionally EntityMetadata.
    /// </summary>
    /// <param name="id">The id of the relation.</param>
    /// <param name="tags">The collection of tags associated with the relation.</param>
    /// <param name="members">The members of the relation.</param>
    /// <param name="additionalInfo">The EntityMetadata structure with additinal properties. Default value is null.</param> 
    public RelationInfo(long id, TagsCollection tags, IList<RelationMemberInfo> members, EntityMetadata additionalInfo = null)
    {
        ID = id;
        Tags = tags;
        Members = members;
        Metadata = additionalInfo;
    }

    /// <summary>
    /// Initializes a new instance of the RelationInfo class with data from specific Relation.
    /// </summary>
    /// <param name="source">The Relation object to copy data from.</param>
    public RelationInfo(Relation source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source), "Source relation cannot be null");
        }

        ID = source.ID;
        Tags = source.Tags;
        Metadata = source.Metadata;

        Members = new List<RelationMemberInfo>(source.Geometries.Count);
        foreach (var member in source.Geometries)
        {
            Members.Add(new RelationMemberInfo(member));
        }
    }

    /// <summary>
    /// Gets type of the object that is represented by this IOsmGeometryInfo.
    /// </summary>
    public EntityType EntityType
    {
        get
        {
            return EntityType.Relation;
        }
    }

    /// <summary>
    /// Gets ID of the relation.
    /// </summary>
    public long ID { get; set; }

    /// <summary>
    /// Gets the collection of tags associated with the relation.
    /// </summary>
    public TagsCollection Tags { get; set; }

    /// <summary>
    /// Gets list of members of the relation.
    /// </summary>
    public IList<RelationMemberInfo> Members { get; set; }

    /// <summary>
    /// Gets additional information about this RelationInfo.
    /// </summary>
    public EntityMetadata Metadata { get; set; }
}
