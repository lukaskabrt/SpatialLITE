using System;
using System.Collections.Generic;

using SpatialLite.Osm.IO;

namespace SpatialLite.Osm.Geometries;

/// <summary>
/// Represents in-memory OSM entities database that contains entities as IOsmGeometry objects.
/// </summary>
public class OsmGeometryDatabase : OsmDatabase<IOsmGeometry, Node, Way, Relation>
{

    /// <summary>
    /// Initializes a new instance of the OsmGeometryDatabase class that is empty.
    /// </summary>
    public OsmGeometryDatabase()
        : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the OsmGeometryDatabase class with specific entities.
    /// </summary>
    /// <param name="entities">Entities to add to the database.</param>
    public OsmGeometryDatabase(IEnumerable<IOsmGeometry> entities)
        : base(entities)
    {
    }

    /// <summary>
    /// Creates a new instaance of the OsmDatabase class and loads data from specific IOsmReader into it.
    /// </summary>
    /// <param name="reader">The IOsmReader to read data from.</param>
    /// <param name="ignoreReferentialErrors">A value indicatings whether Load function should skip geometries that reference miising geometries.</param>
    /// <returns>New instance of the OsmDatabase class with data loaded from specified reader.</returns>
    public static OsmGeometryDatabase Load(IOsmReader reader, bool ignoreReferentialErrors)
    {
        OsmGeometryDatabase db = new OsmGeometryDatabase();

        List<RelationInfo> relations = new List<RelationInfo>();

        IEntityInfo entityInfo = null;
        while ((entityInfo = reader.Read()) != null)
        {
            switch (entityInfo.EntityType)
            {
                case EntityType.Node: db.Nodes.Add(Node.FromNodeInfo(entityInfo as NodeInfo)); break;
                case EntityType.Way:
                    Way toAdd = Way.FromWayInfo(entityInfo as WayInfo, db, !ignoreReferentialErrors);
                    if (toAdd == null)
                    {
                        if (!ignoreReferentialErrors)
                        {
                            throw new ArgumentException(string.Format("Way (ID = {0}) references missing node.", entityInfo.ID));
                        }
                    }
                    else
                    {
                        db.Ways.Add(toAdd);
                    }

                    break;
                case EntityType.Relation:
                    RelationInfo ri = entityInfo as RelationInfo;
                    db.Relations.Add(new Relation(ri.ID) { Tags = ri.Tags, Metadata = ri.Metadata });
                    relations.Add(ri);
                    break;
            }
        }

        foreach (var relationInfo in relations)
        {
            Relation relation = db.Relations[relationInfo.ID];

            foreach (var memberInfo in relationInfo.Members)
            {
                RelationMember member = RelationMember.FromRelationMemberInfo(memberInfo, db, false);
                if (member == null)
                {
                    if (!ignoreReferentialErrors)
                    {
                        throw new ArgumentException(string.Format("Relation (ID = {0}) references missing OSM entity.", memberInfo.Reference));
                    }

                    db.Relations.Remove(relation);
                }
                else
                {
                    relation.Geometries.Add(member);
                }
            }
        }

        return db;
    }

    /// <summary>
    /// Saves entities from the database to the specific writer.
    /// </summary>
    /// <param name="writer">The IOsmWriter to save entities to.</param>
    public override void Save(IOsmWriter writer)
    {
        foreach (var entity in this)
        {
            writer.Write(entity);
        }
    }
}
