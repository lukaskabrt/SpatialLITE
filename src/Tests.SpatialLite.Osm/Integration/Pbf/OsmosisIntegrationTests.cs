﻿using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using Xunit;

using SpatialLite.Osm;
using SpatialLite.Osm.IO;
using Tests.SpatialLite.Osm.Data;

namespace Tests.SpatialLite.Osm.Integration.Pbf {
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

        #region PbfReader tests

        [Fact, Trait("Category", "Osm.Integration")]
		public void PbfReaderReadsFilesCreatedByOsmosis_NoDenseNoCompression() {
			string pbfFile = Path.GetFullPath("..\\..\\..\\TestFiles\\pbfreader-osmosis-compatibility-test-osmosis-real-file.pbf");
			if (File.Exists(pbfFile)) {
				File.Delete(pbfFile);
			}

			string osmosisArguments = string.Format("--read-pbf file={0} --write-pbf file={1} usedense=false compress=none", Path.GetFullPath(TestFilePath), pbfFile);
			this.CallOsmosis(osmosisArguments);

			using (PbfReader reader = new PbfReader(pbfFile, new OsmReaderSettings() { ReadMetadata = true })) {
				this.TestReader(reader);
			}
		}

        [Fact, Trait("Category", "Osm.Integration")]
        public void PbfReaderReadsFilesCreatedByOsmosis_DenseNoCompression() {
			string pbfFile = Path.GetFullPath("..\\..\\..\\TestFiles\\pbfreader-osmosis-compatibility-test-osmosis-real-file-d.pbf");
			if (File.Exists(pbfFile)) {
				File.Delete(pbfFile);
			}

			string osmosisArguments = string.Format("--read-pbf file={0} --write-pbf file={1} usedense=true compress=none", Path.GetFullPath(TestFilePath), pbfFile);
			this.CallOsmosis(osmosisArguments);

			using (PbfReader reader = new PbfReader(pbfFile, new OsmReaderSettings() { ReadMetadata = true })) {
				this.TestReader(reader);
			}
		}

        [Fact, Trait("Category", "Osm.Integration")]
        public void PbfReaderReadsFilesCreatedByOsmosis_NoDenseDeflateCompression() {
			string pbfFile = Path.GetFullPath("..\\..\\..\\TestFiles\\pbfreader-osmosis-compatibility-test-osmosis-real-file-c.pbf");
			if (File.Exists(pbfFile)) {
				File.Delete(pbfFile);
			}

			string osmosisArguments = string.Format("--read-pbf file={0} --write-pbf file={1} usedense=false compress=deflate", Path.GetFullPath(TestFilePath), pbfFile);
			this.CallOsmosis(osmosisArguments);

			using (PbfReader reader = new PbfReader(pbfFile, new OsmReaderSettings() { ReadMetadata = true })) {
				this.TestReader(reader);
			}
		}

        [Fact, Trait("Category", "Osm.Integration")]
        public void PbfReaderReadsFilesCreatedByOsmosis_DenseDeflate() {
			string pbfFile = Path.GetFullPath("..\\..\\..\\TestFiles\\pbfreader-osmosis-compatibility-test-osmosis-real-file-dc.pbf");
			if (File.Exists(pbfFile)) {
				File.Delete(pbfFile);
			}

			string osmosisArguments = string.Format("--read-pbf file={0} --write-pbf file={1} usedense=true compress=deflate", Path.GetFullPath(TestFilePath), pbfFile);
			this.CallOsmosis(osmosisArguments);

			using (PbfReader reader = new PbfReader(pbfFile, new OsmReaderSettings() { ReadMetadata = true })) {
				this.TestReader(reader);
			}
		}

        #endregion

        #region PbfWriter tests

        [Fact, Trait("Category", "Osm.Integration")]
        public void PbfWriterWritesFilesCompatibleWithOsmosis_NoDenseNoCompression() {
			string pbfFile = Path.GetFullPath("..\\..\\..\\TestFiles\\pbfwriter-osmosis-compatibility-test-pbfwriter-real-file.pbf");
			if (File.Exists(pbfFile)) {
				File.Delete(pbfFile);
			}

			using (PbfWriter writer = new PbfWriter(pbfFile, new PbfWriterSettings() { WriteMetadata = true, Compression = CompressionMode.None, UseDenseFormat = false })) {
				foreach (var entityInfo in this.GetTestData()) {
					writer.Write(entityInfo);
				}
			}

			string osmosisXmlFile = Path.GetFullPath("..\\..\\..\\TestFiles\\pbfwriter-osmosis-compatibility-test-test-file.osm");
			if (File.Exists(osmosisXmlFile)) {
				File.Delete(osmosisXmlFile);
			}

			string osmosisArguments = string.Format("--read-pbf file={0} --write-xml file={1}", pbfFile, osmosisXmlFile);
			this.CallOsmosis(osmosisArguments);

			Assert.True(File.Exists(osmosisXmlFile));
			Assert.True(new FileInfo(osmosisXmlFile).Length > 0);
		}

        [Fact, Trait("Category", "Osm.Integration")]
        public void PbfWriterWritesFilesCompatibleWithOsmosis_NoDenseDeflate() {
			string pbfFile = Path.GetFullPath("..\\..\\..\\TestFiles\\pbfwriter-osmosis-compatibility-test-pbfwriter-real-file-c.pbf");
			if (File.Exists(pbfFile)) {
				File.Delete(pbfFile);
			}

			using (PbfWriter writer = new PbfWriter(pbfFile, new PbfWriterSettings() { WriteMetadata = true, Compression = CompressionMode.ZlibDeflate, UseDenseFormat = false })) {
				foreach (var entityInfo in this.GetTestData()) {
					writer.Write(entityInfo);
				}
			}

			string osmosisXmlFile = Path.GetFullPath("..\\..\\..\\TestFiles\\pbfwriter-osmosis-compatibility-test-test-file.osm");
			if (File.Exists(osmosisXmlFile)) {
				File.Delete(osmosisXmlFile);
			}

			string osmosisArguments = string.Format("--read-pbf file={0} --write-xml file={1}", pbfFile, osmosisXmlFile);
			this.CallOsmosis(osmosisArguments);

			Assert.True(File.Exists(osmosisXmlFile));
			Assert.True(new FileInfo(osmosisXmlFile).Length > 0);
		}

        [Fact, Trait("Category", "Osm.Integration")]
        public void PbfWriterWritesFilesCompatibleWithOsmosis_DenseNoCompression() {
			string pbfFile = Path.GetFullPath("..\\..\\..\\TestFiles\\pbfwriter-osmosis-compatibility-test-pbfwriter-real-file-d.pbf");
			if (File.Exists(pbfFile)) {
				File.Delete(pbfFile);
			}

			using (PbfWriter writer = new PbfWriter(pbfFile, new PbfWriterSettings() { WriteMetadata = true, Compression = CompressionMode.None, UseDenseFormat = true })) {
				foreach (var entityInfo in this.GetTestData()) {
					writer.Write(entityInfo);
				}
			}

			string osmosisXmlFile = Path.GetFullPath("..\\..\\..\\TestFiles\\pbfwriter-osmosis-compatibility-test-test-file.osm");
			if (File.Exists(osmosisXmlFile)) {
				File.Delete(osmosisXmlFile);
			}

			string osmosisArguments = string.Format("--read-pbf file={0} --write-xml file={1}", pbfFile, osmosisXmlFile);
			this.CallOsmosis(osmosisArguments);

			Assert.True(File.Exists(osmosisXmlFile));
			Assert.True(new FileInfo(osmosisXmlFile).Length > 0);
		}

        [Fact, Trait("Category", "Osm.Integration")]
        public void PbfWriterWritesFilesCompatibleWithOsmosis_DenseDeflate() {
			string pbfFile = Path.GetFullPath("..\\..\\..\\TestFiles\\pbfwriter-osmosis-compatibility-test-pbfwriter-real-file-dc.pbf");
			if (File.Exists(pbfFile)) {
				File.Delete(pbfFile);
			}

			using (PbfWriter writer = new PbfWriter(pbfFile, new PbfWriterSettings() { WriteMetadata = true, Compression = CompressionMode.ZlibDeflate, UseDenseFormat = true })) {
				foreach (var entityInfo in this.GetTestData()) {
					writer.Write(entityInfo);
				}
			}

			string osmosisXmlFile = Path.GetFullPath("..\\..\\..\\TestFiles\\pbfwriter-osmosis-compatibility-test-test-file.osm");
			if (File.Exists(osmosisXmlFile)) {
				File.Delete(osmosisXmlFile);
			}

			string osmosisArguments = string.Format("--read-pbf file={0} --write-xml file={1}", pbfFile, osmosisXmlFile);
			this.CallOsmosis(osmosisArguments);

			Assert.True(File.Exists(osmosisXmlFile));
			Assert.True(new FileInfo(osmosisXmlFile).Length > 0);
		}

		#endregion

		#region Helper functions

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

		#endregion
	}
}
