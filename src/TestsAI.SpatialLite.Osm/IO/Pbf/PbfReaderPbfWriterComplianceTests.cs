using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Xunit;

using SpatialLite.Osm;
using SpatialLite.Osm.IO;
using SpatialLite.Osm.IO.Pbf;
using SpatialLite.Osm.Geometries;

namespace TestsAI.SpatialLite.Osm.IO.Pbf {
	public class PbfReaderPbfWriterComplianceTests {
		private const int TestFileNodesCount = 129337;
		private const int TestFileWaysCount = 14461;
		private const int TestFileRelationsCount = 124;

		#region PbfReader-PbfWriter compliance tests

		[Fact]
		public void PbfReaderCanReadFileCreatedByPbfWriter_NoDenseNoCompression() {
			MemoryStream stream = new MemoryStream();

			using (PbfWriter writer = new PbfWriter(stream, new PbfWriterSettings() { WriteMetadata = true, UseDenseFormat = false, Compression = CompressionMode.None })) {
				foreach (var info in this.GetTestData()) {
					writer.Write(info);
				}

				writer.Flush();

				stream.Seek(0, SeekOrigin.Begin);
				using (PbfReader reader = new PbfReader(stream, new OsmReaderSettings() { ReadMetadata = true })) {
					this.TestReader(reader);
				}
			}
		}

		[Fact]
		public void PbfReaderCanReadFileCreatedByPbfWriter_DenseNoCompression() {
			MemoryStream stream = new MemoryStream();

			using (PbfWriter writer = new PbfWriter(stream, new PbfWriterSettings() { WriteMetadata = true, UseDenseFormat = true, Compression = CompressionMode.None })) {
				foreach (var info in this.GetTestData()) {
					writer.Write(info);
				}

				writer.Flush();

				stream.Seek(0, SeekOrigin.Begin);
				using (PbfReader reader = new PbfReader(stream, new OsmReaderSettings() { ReadMetadata = true })) {
					this.TestReader(reader);
				}
			}
		}

		[Fact]
		public void PbfReaderCanReadFileCreatedByPbfWriter_NoDenseDeflate() {
			MemoryStream stream = new MemoryStream();

			using (PbfWriter writer = new PbfWriter(stream, new PbfWriterSettings() { WriteMetadata = true, UseDenseFormat = false, Compression = CompressionMode.ZlibDeflate })) {
				foreach (var info in this.GetTestData()) {
					writer.Write(info);
				}

				writer.Flush();

				stream.Seek(0, SeekOrigin.Begin);
				using (PbfReader reader = new PbfReader(stream, new OsmReaderSettings() { ReadMetadata = true })) {
					this.TestReader(reader);
				}
			}
		}

		[Fact]
		public void PbfReaderCanReadFileCreatedByPbfWriter_DenseDeflate() {
			MemoryStream stream = new MemoryStream();

			using (PbfWriter writer = new PbfWriter(stream, new PbfWriterSettings() { WriteMetadata = true, UseDenseFormat = true, Compression = CompressionMode.ZlibDeflate })) {
				foreach (var info in this.GetTestData()) {
					writer.Write(info);
				}

				writer.Flush();

				stream.Seek(0, SeekOrigin.Begin);
				using (PbfReader reader = new PbfReader(stream, new OsmReaderSettings() { ReadMetadata = true })) {
					this.TestReader(reader);
				}
			}
		}

		#endregion

		#region Helper functions

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

			using (MemoryStream stream = new MemoryStream(Data.IOTestData.pbf_real_test_file_1)) {
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
