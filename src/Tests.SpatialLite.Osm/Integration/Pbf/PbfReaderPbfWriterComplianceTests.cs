using System.Collections.Generic;
using System.IO;

using Xunit;

using SpatialLite.Osm;
using SpatialLite.Osm.IO;
using Tests.SpatialLite.Osm.Data;

namespace Tests.SpatialLite.Osm.Integration.Pbf;

public class PbfReaderPbfWriterComplianceTests
{
    private const int TestFileNodesCount = 129337;
    private const int TestFileWaysCount = 14461;
    private const int TestFileRelationsCount = 124;

    [Fact, Trait("Category", "Osm.Integration")]
    public void PbfReaderCanReadFileCreatedByPbfWriter_NoDenseNoCompression()
    {
        MemoryStream stream = new();

        using (PbfWriter writer = new(stream, new PbfWriterSettings() { WriteMetadata = true, UseDenseFormat = false, Compression = CompressionMode.None }))
        {
            foreach (var info in GetTestData())
            {
                writer.Write(info);
            }

            writer.Flush();

            stream.Seek(0, SeekOrigin.Begin);
            using (PbfReader reader = new(stream, new OsmReaderSettings() { ReadMetadata = true }))
            {
                TestReader(reader);
            }
        }
    }

    [Fact, Trait("Category", "Osm.Integration")]
    public void PbfReaderCanReadFileCreatedByPbfWriter_DenseNoCompression()
    {
        MemoryStream stream = new();

        using (PbfWriter writer = new(stream, new PbfWriterSettings() { WriteMetadata = true, UseDenseFormat = true, Compression = CompressionMode.None }))
        {
            foreach (var info in GetTestData())
            {
                writer.Write(info);
            }

            writer.Flush();

            stream.Seek(0, SeekOrigin.Begin);
            using (PbfReader reader = new(stream, new OsmReaderSettings() { ReadMetadata = true }))
            {
                TestReader(reader);
            }
        }
    }

    [Fact, Trait("Category", "Osm.Integration")]
    public void PbfReaderCanReadFileCreatedByPbfWriter_NoDenseDeflate()
    {
        MemoryStream stream = new();

        using (PbfWriter writer = new(stream, new PbfWriterSettings() { WriteMetadata = true, UseDenseFormat = false, Compression = CompressionMode.ZlibDeflate }))
        {
            foreach (var info in GetTestData())
            {
                writer.Write(info);
            }

            writer.Flush();

            stream.Seek(0, SeekOrigin.Begin);
            using (PbfReader reader = new(stream, new OsmReaderSettings() { ReadMetadata = true }))
            {
                TestReader(reader);
            }
        }
    }

    [Fact, Trait("Category", "Osm.Integration")]
    public void PbfReaderCanReadFileCreatedByPbfWriter_DenseDeflate()
    {
        MemoryStream stream = new();

        using (PbfWriter writer = new(stream, new PbfWriterSettings() { WriteMetadata = true, UseDenseFormat = true, Compression = CompressionMode.ZlibDeflate }))
        {
            foreach (var info in GetTestData())
            {
                writer.Write(info);
            }

            writer.Flush();

            stream.Seek(0, SeekOrigin.Begin);
            using (PbfReader reader = new(stream, new OsmReaderSettings() { ReadMetadata = true }))
            {
                TestReader(reader);
            }
        }
    }

    private void TestReader(IOsmReader reader)
    {
        IEntityInfo info = null;
        int nodesCount = 0, waysCount = 0, relationsCount = 0;
        while ((info = reader.Read()) != null)
        {
            switch (info.EntityType)
            {
                case EntityType.Node: nodesCount++; break;
                case EntityType.Way: waysCount++; break;
                case EntityType.Relation: relationsCount++; break;
            }
        }

        Assert.Equal(TestFileNodesCount, nodesCount);
        Assert.Equal(TestFileWaysCount, waysCount);
        Assert.Equal(TestFileRelationsCount, relationsCount);
    }

    private IEnumerable<IEntityInfo> GetTestData()
    {
        List<IEntityInfo> data = new();

        using (var stream = TestDataReader.OpenPbf("pbf-real-file.pbf"))
        {
            using (PbfReader reader = new(stream, new OsmReaderSettings() { ReadMetadata = true }))
            {
                IEntityInfo info = null;
                while ((info = reader.Read()) != null)
                {
                    data.Add(info);
                }
            }
        }

        return data;
    }
}
