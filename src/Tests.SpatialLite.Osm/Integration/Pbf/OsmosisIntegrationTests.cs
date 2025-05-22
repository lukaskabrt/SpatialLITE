using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using Xunit;

using SpatialLite.Osm;
using SpatialLite.Osm.IO;
using Tests.SpatialLite.Osm.Data;

namespace Tests.SpatialLite.Osm.Integration.Pbf;

public class OsmosisIntegrationTests
{
    private const int TestFileNodesCount = 129337;
    private const int TestFileWaysCount = 14461;
    private const int TestFileRelationsCount = 124;

    [Fact(Skip = "Test requires Windows-specific osmosis.bat file which is not available in Linux CI environment"), Trait("Category", "Osm.Integration")]
    public void PbfReaderReadsFilesCreatedByOsmosis_NoDenseNoCompression()
    {
        string pbfFile = PathHelper.GetTempFilePath("pbfreader-osmosis-compatibility-test-osmosis-real-file.pbf");

        string osmosisArguments = string.Format("--read-pbf file={0} --write-pbf file={1} usedense=false compress=none", PathHelper.RealPbfFilePath, pbfFile);
        this.CallOsmosis(osmosisArguments);

        using (PbfReader reader = new PbfReader(pbfFile, new OsmReaderSettings() { ReadMetadata = true }))
        {
            this.TestReader(reader);
        }
    }

    [Fact(Skip = "Test requires Windows-specific osmosis.bat file which is not available in Linux CI environment"), Trait("Category", "Osm.Integration")]
    public void PbfReaderReadsFilesCreatedByOsmosis_DenseNoCompression()
    {
        var pbfFile = PathHelper.GetTempFilePath("pbfreader-osmosis-compatibility-test-osmosis-real-file-d.pbf");

        string osmosisArguments = string.Format("--read-pbf file={0} --write-pbf file={1} usedense=true compress=none", PathHelper.RealPbfFilePath, pbfFile);
        this.CallOsmosis(osmosisArguments);

        using (PbfReader reader = new PbfReader(pbfFile, new OsmReaderSettings() { ReadMetadata = true }))
        {
            this.TestReader(reader);
        }
    }

    [Fact(Skip = "Test requires Windows-specific osmosis.bat file which is not available in Linux CI environment"), Trait("Category", "Osm.Integration")]
    public void PbfReaderReadsFilesCreatedByOsmosis_NoDenseDeflateCompression()
    {
        string pbfFile = PathHelper.GetTempFilePath("pbfreader-osmosis-compatibility-test-osmosis-real-file-c.pbf");

        string osmosisArguments = string.Format("--read-pbf file={0} --write-pbf file={1} usedense=false compress=deflate", PathHelper.RealPbfFilePath, pbfFile);
        this.CallOsmosis(osmosisArguments);

        using (PbfReader reader = new PbfReader(pbfFile, new OsmReaderSettings() { ReadMetadata = true }))
        {
            this.TestReader(reader);
        }
    }

    [Fact(Skip = "Test requires Windows-specific osmosis.bat file which is not available in Linux CI environment"), Trait("Category", "Osm.Integration")]
    public void PbfReaderReadsFilesCreatedByOsmosis_DenseDeflate()
    {
        string pbfFile = PathHelper.GetTempFilePath("pbfreader-osmosis-compatibility-test-osmosis-real-file-dc.pbf");

        string osmosisArguments = string.Format("--read-pbf file={0} --write-pbf file={1} usedense=true compress=deflate", PathHelper.RealPbfFilePath, pbfFile);
        this.CallOsmosis(osmosisArguments);

        using (PbfReader reader = new PbfReader(pbfFile, new OsmReaderSettings() { ReadMetadata = true }))
        {
            this.TestReader(reader);
        }
    }

    [Fact(Skip = "Test requires Windows-specific osmosis.bat file which is not available in Linux CI environment"), Trait("Category", "Osm.Integration")]
    public void PbfWriterWritesFilesCompatibleWithOsmosis_NoDenseNoCompression()
    {
        string pbfFile = PathHelper.GetTempFilePath("pbfwriter-osmosis-compatibility-test-pbfwriter-real-file.pbf");

        using (PbfWriter writer = new PbfWriter(pbfFile, new PbfWriterSettings() { WriteMetadata = true, Compression = CompressionMode.None, UseDenseFormat = false }))
        {
            foreach (var entityInfo in this.GetTestData())
            {
                writer.Write(entityInfo);
            }
        }

        string osmosisXmlFile = PathHelper.GetTempFilePath("pbfwriter-osmosis-compatibility-test-test-file.osm");
        string osmosisArguments = string.Format("--read-pbf file={0} --write-xml file={1}", pbfFile, osmosisXmlFile);
        this.CallOsmosis(osmosisArguments);

        Assert.True(File.Exists(osmosisXmlFile));
        Assert.True(new FileInfo(osmosisXmlFile).Length > 0);
    }

    [Fact(Skip = "Test requires Windows-specific osmosis.bat file which is not available in Linux CI environment"), Trait("Category", "Osm.Integration")]
    public void PbfWriterWritesFilesCompatibleWithOsmosis_NoDenseDeflate()
    {
        string pbfFile = PathHelper.GetTempFilePath("pbfwriter-osmosis-compatibility-test-pbfwriter-real-file-c.pbf");

        using (PbfWriter writer = new PbfWriter(pbfFile, new PbfWriterSettings() { WriteMetadata = true, Compression = CompressionMode.ZlibDeflate, UseDenseFormat = false }))
        {
            foreach (var entityInfo in this.GetTestData())
            {
                writer.Write(entityInfo);
            }
        }

        string osmosisXmlFile = PathHelper.GetTempFilePath("pbfwriter-osmosis-compatibility-test-test-file.osm");
        string osmosisArguments = string.Format("--read-pbf file={0} --write-xml file={1}", pbfFile, osmosisXmlFile);
        this.CallOsmosis(osmosisArguments);

        Assert.True(File.Exists(osmosisXmlFile));
        Assert.True(new FileInfo(osmosisXmlFile).Length > 0);
    }

    [Fact(Skip = "Test requires Windows-specific osmosis.bat file which is not available in Linux CI environment"), Trait("Category", "Osm.Integration")]
    public void PbfWriterWritesFilesCompatibleWithOsmosis_DenseNoCompression()
    {
        string pbfFile = PathHelper.GetTempFilePath("pbfwriter-osmosis-compatibility-test-pbfwriter-real-file-d.pbf");

        using (PbfWriter writer = new PbfWriter(pbfFile, new PbfWriterSettings() { WriteMetadata = true, Compression = CompressionMode.None, UseDenseFormat = true }))
        {
            foreach (var entityInfo in this.GetTestData())
            {
                writer.Write(entityInfo);
            }
        }

        string osmosisXmlFile = PathHelper.GetTempFilePath("pbfwriter-osmosis-compatibility-test-test-file.osm");
        string osmosisArguments = string.Format("--read-pbf file={0} --write-xml file={1}", pbfFile, osmosisXmlFile);
        this.CallOsmosis(osmosisArguments);

        Assert.True(File.Exists(osmosisXmlFile));
        Assert.True(new FileInfo(osmosisXmlFile).Length > 0);
    }

    [Fact(Skip = "Test requires Windows-specific osmosis.bat file which is not available in Linux CI environment"), Trait("Category", "Osm.Integration")]
    public void PbfWriterWritesFilesCompatibleWithOsmosis_DenseDeflate()
    {
        string pbfFile = PathHelper.GetTempFilePath("pbfwriter-osmosis-compatibility-test-pbfwriter-real-file-dc.pbf");

        using (PbfWriter writer = new PbfWriter(pbfFile, new PbfWriterSettings() { WriteMetadata = true, Compression = CompressionMode.ZlibDeflate, UseDenseFormat = true }))
        {
            foreach (var entityInfo in this.GetTestData())
            {
                writer.Write(entityInfo);
            }
        }

        string osmosisXmlFile = PathHelper.GetTempFilePath("pbfwriter-osmosis-compatibility-test-test-file.osm");
        string osmosisArguments = string.Format("--read-pbf file={0} --write-xml file={1}", pbfFile, osmosisXmlFile);
        this.CallOsmosis(osmosisArguments);

        Assert.True(File.Exists(osmosisXmlFile));
        Assert.True(new FileInfo(osmosisXmlFile).Length > 0);
    }

    private void CallOsmosis(string arguments)
    {
        ProcessStartInfo osmosisInfo = new ProcessStartInfo(PathHelper.OsmosisPath);
        osmosisInfo.Arguments = arguments;

        Process osmosis = Process.Start(osmosisInfo);
        osmosis.WaitForExit();

        Assert.Equal(0, osmosis.ExitCode);
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
        List<IEntityInfo> data = new List<IEntityInfo>();

        using (var stream = TestDataReader.OpenPbf("pbf-real-file.pbf"))
        {
            using (PbfReader reader = new PbfReader(stream, new OsmReaderSettings() { ReadMetadata = true }))
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
