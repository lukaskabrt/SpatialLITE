using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using Xunit;

using SpatialLite.Osm;
using SpatialLite.Osm.IO;
using Tests.SpatialLite.Osm.Data;

namespace Tests.SpatialLite.Osm.Integration.Xml {
    public class OsmosisIntegrationTests {
        public const string OsmosisPath = "..\\..\\..\\..\\..\\lib\\Osmosis\\bin\\osmosis.bat";

        private const string TestFilePath = "..\\..\\..\\Data\\Pbf\\pbf-real-file.pbf";
        private const int TestFileNodesCount = 129337;
        private const int TestFileWaysCount = 14461;
        private const int TestFileRelationsCount = 124;

        public OsmosisIntegrationTests() {
            if (!Directory.Exists("TestFiles")) {
                Directory.CreateDirectory("TestFiles");
            }
        }

        #region OsmXmlReader Tests

        [Fact, Trait("Category", "Osm.Integration")]
        public void XmlOsmReaderReadsFilesCreatedByOsmosis() {
            string xmlFile = Path.GetFullPath("..\\..\\..\\TestFiles\\xmlreader-osmosis-compatibility-test-osmosis-real-file.osm");
            string osmosisArguments = string.Format("--read-pbf file={0} --write-xml file={1}", Path.GetFullPath(TestFilePath), xmlFile);
            this.CallOsmosis(osmosisArguments);

            using (OsmXmlReader reader = new OsmXmlReader(xmlFile, new OsmXmlReaderSettings() { ReadMetadata = true, StrictMode = false })) {
                this.TestReader(reader);
            }
        }

        #endregion

        #region OsmXmlWriter Tests

        [Fact, Trait("Category", "Osm.Integration")]
        public void XmlOsmWriterWritesFilesCompatibleWithOsmosis() {
            string xmlFile = Path.GetFullPath("..\\..\\..\\TestFiles\\xmlwriter-osmosis-compatibility-test-xmlwriter-real-file.osm");
            if (File.Exists(xmlFile)) {
                File.Delete(xmlFile);
            }

            using (OsmXmlWriter writer = new OsmXmlWriter(xmlFile, new OsmWriterSettings() { WriteMetadata = true })) {
                foreach (var entityInfo in this.GetTestData()) {
                    writer.Write(entityInfo);
                }
            }

            string osmosisXmlFile = Path.GetFullPath("TestFiles\\xmlwriter-osmosis-compatibility-test-test-file.osm");
            if (File.Exists(osmosisXmlFile)) {
                File.Delete(osmosisXmlFile);
            }

            string osmosisArguments = string.Format("--read-xml file={0} --write-xml file={1}", xmlFile, osmosisXmlFile);
            this.CallOsmosis(osmosisArguments);

            Assert.True(File.Exists(osmosisXmlFile));
            Assert.True(new FileInfo(osmosisXmlFile).Length > 0);
        }

        #endregion

        #region Helper functions

        private IEnumerable<IEntityInfo> GetTestData() {
            List<IEntityInfo> data = new List<IEntityInfo>();

            using (var stream = TestDataReader.OpenPbf("pbf-real-file.pbf")) {
                using (PbfReader reader = new PbfReader(stream, new OsmReaderSettings() { ReadMetadata = true })) {
                    IEntityInfo info = null;
                    while ((info = reader.Read()) != null) {
                        data.Add(info);
                    }
                }
            }

            return data;
        }

        private void CallOsmosis(string arguments) {
            ProcessStartInfo osmosisInfo = new ProcessStartInfo(Path.GetFullPath(OsmosisPath));
            osmosisInfo.Arguments = arguments;

            Process osmosis = Process.Start(osmosisInfo);
            osmosis.WaitForExit();

            Assert.Equal(0, osmosis.ExitCode);
        }

        private void TestReader(IOsmReader reader) {
            IEntityInfo info = null;
            int nodesCount = 0, waysCount = 0, relationsCount = 0;
            while ((info = reader.Read()) != null) {
                switch (info.EntityType) {
                    case EntityType.Node: nodesCount++; break;
                    case EntityType.Way: waysCount++; break;
                    case EntityType.Relation: relationsCount++; break;
                }
            }

            Assert.Equal(TestFileNodesCount, nodesCount);
            Assert.Equal(TestFileWaysCount, waysCount);
            Assert.Equal(TestFileRelationsCount, relationsCount);
        }

        #endregion
    }
}
