using System;

namespace SpatialLite.Osm;

/// <summary>
/// Contains additional information about OSM entities such as Author, Version, Timesptam of the last change and others.
/// </summary>
public class EntityMetadata
{

    /// <summary>
    /// Gets or sets version of the object.
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Gets or sets date and time when the entity was modified.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets if of the user who made last modification to the entity.
    /// </summary>
    public int Uid { get; set; }

    /// <summary>
    /// Gets or sets username of the person who made last modification to the entity.
    /// </summary>
    public string User { get; set; }

    /// <summary>
    /// Gets or sets changeset id.
    /// </summary>
    public int Changeset { get; set; }

    /// <summary>
    /// Gets or sets visibility of this item.
    /// </summary>
    public bool Visible { get; set; }
}
