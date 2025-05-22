using System.Collections.Generic;

using SpatialLite.Osm.IO;

namespace SpatialLite.Osm;

/// <summary>
/// Represents in-memory OSM entities database that contains entities as IEntityInfo objects.
/// </summary>
public class OsmEntityInfoDatabase : OsmDatabase<IEntityInfo, NodeInfo, WayInfo, RelationInfo>
{

    /// <summary>
    /// Initializes a new instance of the OsmEntityInfoDatabase class that is empty.
    /// </summary>
    public OsmEntityInfoDatabase()
        : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the OsmEntityInfoDatabase class with specific entities.
    /// </summary>
    /// <param name="entities">Entities to add to the database.</param>
    public OsmEntityInfoDatabase(IEnumerable<IEntityInfo> entities)
        : base(entities)
    {
    }

    /// <summary>
    /// Creates a new instaance of the OsmDatabase class and loads data from specific IOsmReader into it.
    /// </summary>
    /// <param name="reader">The IOsmReader to read data from.</param>
    /// <returns>New instance of the OsmDAtabase class with data loaded from specified reader.</returns>
    public static OsmEntityInfoDatabase Load(IOsmReader reader)
    {
        OsmEntityInfoDatabase db = new OsmEntityInfoDatabase();

        IEntityInfo entityInfo = null;
        while ((entityInfo = reader.Read()) != null)
        {
            switch (entityInfo.EntityType)
            {
                case EntityType.Node: db.Nodes.Add((NodeInfo)entityInfo); break;
                case EntityType.Way: db.Ways.Add((WayInfo)entityInfo); break;
                case EntityType.Relation: db.Relations.Add((RelationInfo)entityInfo); break;
            }
        }

        return db;
    }

    /// <summary>
    /// Saves entities from the database to specific writer.
    /// </summary>
    /// <param name="writer">IOsmWriter to save entities to.</param>
    public override void Save(IOsmWriter writer)
    {
        foreach (var entity in this)
        {
            writer.Write(entity);
        }
    }
}
