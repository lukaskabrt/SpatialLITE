using ProtoBuf;
using System.Collections.Generic;

namespace SpatialLite.Osm.IO.Pbf;

/// <summary>
/// Represents container for PBF data transfer objects for OSM entities.
/// </summary>
[ProtoContract(Name = "PrimitiveGroup")]
internal class PrimitiveGroup
{

    /// <summary>
    /// Gets or sets collection of nodes.
    /// </summary>
    [ProtoMember(1, Name = "nodes")]
    public List<PbfNode> Nodes { get; set; }

    /// <summary>
    /// Gets or sets collection of nodes serialized in dense format.
    /// </summary>
    [ProtoMember(2, IsRequired = false, Name = "dense")]
    public PbfDenseNodes DenseNodes { get; set; }

    /// <summary>
    /// Gets or sets collection of way.
    /// </summary>
    [ProtoMember(3, Name = "ways")]
    public List<PbfWay> Ways { get; set; }

    /// <summary>
    /// Gets or sets collection of relations.
    /// </summary>
    [ProtoMember(4, Name = "relations")]
    public List<PbfRelation> Relations { get; set; }

    /// <summary>
    /// Gets or sets collection of changesets.
    /// </summary>
    [ProtoMember(5, Name = "changesets")]
    public List<PbfChangeset> Changesets { get; set; }
}
