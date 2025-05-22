using System;
using System.Collections.Generic;
using System.Text;
using SpatialLite.Osm;
using SpatialLite.Osm.Geometries;
using SpatialLite.Osm.IO;
using Tests.SpatialLite.Osm.Data;
using Xunit;

namespace Tests.SpatialLite.Osm.Integration.Xml;

public class OsmDatabaseTests
{
    [Fact, Trait("Category", "Osm.Integration")]
    public void OsmEntityInfoDatabase_LoadesRealFile()
    {
        OsmXmlReader reader = new OsmXmlReader(TestDataReader.OpenXml("osm-real-file.osm"), new OsmXmlReaderSettings() { ReadMetadata = true });
        OsmEntityInfoDatabase target = OsmEntityInfoDatabase.Load(reader);

        Assert.Equal(6688, target.Nodes.Count);
        Assert.Equal(740, target.Ways.Count);
        Assert.Equal(75, target.Relations.Count);
    }

    [Fact, Trait("Category", "Osm.Integration")]
    public void OsmGeometryDatabase_LoadesRealFile()
    {
        OsmXmlReader reader = new OsmXmlReader(TestDataReader.OpenXml("osm-real-file.osm"), new OsmXmlReaderSettings() { ReadMetadata = true });
        OsmGeometryDatabase target = OsmGeometryDatabase.Load(reader, true);

        Assert.Equal(6688, target.Nodes.Count);
        Assert.Equal(740, target.Ways.Count);
        Assert.Equal(75, target.Relations.Count);
    }
}
