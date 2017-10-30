using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Xunit;

using SpatialLite.Osm.IO;
using SpatialLite.Osm.IO.Pbf;
using SpatialLite.Osm.Geometries;
using SpatialLite.Osm;
using Tests.SpatialLite.Osm.Data;

namespace Tests.SpatialLite.Osm.IO {
	public class PbfWriterTests {
		//resolution for default granularity
		private const double _resolution = 1E-07;

		private EntityMetadata _details;
		private NodeInfo _node, _nodeTags, _nodeProperties;
		private WayInfo _way, _wayTags, _wayProperties, _wayWithoutNodes;
		private RelationInfo _relationNode, _relationWay, _relationRelation, _relationNodeProperties, _relationTags;

		public PbfWriterTests() {
			_details = new EntityMetadata() {
				Timestamp = new DateTime(2010, 11, 19, 22, 5, 56, DateTimeKind.Utc),
				Uid = 127998,
				User = "Luk@s",
				Visible = true,
				Version = 2,
				Changeset = 6410629
			};

			_node = new NodeInfo(1, 50.4, 16.2, new TagsCollection());
			_nodeTags = new NodeInfo(1, 50.4, 16.2, new TagsCollection(new Tag[] { new Tag("name", "test"), new Tag("name-2", "test-2") }));
			_nodeProperties = new NodeInfo(1, 50.4, 16.2, new TagsCollection(), _details);

			_way = new WayInfo(1, new TagsCollection(), new long[] { 10, 11, 12 });
			_wayTags = new WayInfo(1, new TagsCollection(new Tag[] { new Tag("name", "test"), new Tag("name-2", "test-2") }), new long[] { 10, 11, 12 });
			_wayProperties = new WayInfo(1, new TagsCollection(), new long[] { 10, 11, 12 }, _details);
			_wayWithoutNodes = new WayInfo(1, new TagsCollection(), new long[] { });

			_relationNode = new RelationInfo(1, new TagsCollection(), new RelationMemberInfo[] { new RelationMemberInfo() { MemberType = EntityType.Node, Reference = 10, Role = "test" } });
			_relationWay = new RelationInfo(1, new TagsCollection(), new RelationMemberInfo[] { new RelationMemberInfo() { MemberType = EntityType.Way, Reference = 10, Role = "test" } });
			_relationRelation = new RelationInfo(1, new TagsCollection(), new RelationMemberInfo[] { new RelationMemberInfo() { MemberType = EntityType.Relation, Reference = 10, Role = "test" } });
			_relationTags = new RelationInfo(
				1,
				new TagsCollection(new Tag[] { new Tag("name", "test"), new Tag("name-2", "test-2") }),
				new RelationMemberInfo[] { new RelationMemberInfo() { MemberType = EntityType.Node, Reference = 10, Role = "test" } });
			_relationNodeProperties = new RelationInfo(1, new TagsCollection(), new RelationMemberInfo[] { new RelationMemberInfo() { MemberType = EntityType.Node, Reference = 10, Role = "test" } }, _details);

            if (!Directory.Exists("TestFiles")) {
                Directory.CreateDirectory("TestFiles");
            }
        }

        #region Constructor(Filename, Settings) tests

        [Fact]
		public void Constructor_FilenameSettings_SetsSettingsAndMakesThemReadOnly() {
			string filename = "TestFiles\\pbfwriter-constructor-test.pbf";
			File.Delete(filename);

			PbfWriterSettings settings = new PbfWriterSettings();
			using (PbfWriter target = new PbfWriter(filename, settings)) {
				Assert.Same(settings, target.Settings);
				Assert.True(settings.IsReadOnly);
			}
		}

		[Fact]
		public void Constructor_FilenameSettings_CreatesOutputFile() {
			string filename = "TestFiles\\pbfwriter-constructor-creates-output-test.pbf";
			File.Delete(filename);

			PbfWriterSettings settings = new PbfWriterSettings();
			using (PbfWriter target = new PbfWriter(filename, settings)) {
				;
			}

			Assert.True(File.Exists(filename));
		}

		[Fact]
		public void Constructor_FilenameSettings_WritesOsmHeader() {
			string filename = "TestFiles\\pbfwriter-constructor-writes-header-test.pbf";
			File.Delete(filename);

			PbfWriterSettings settings = new PbfWriterSettings();
			using (PbfWriter target = new PbfWriter(filename, settings)) {
				;
			}

			FileInfo fi = new FileInfo(filename);
			Assert.True(fi.Length > 0);
		}

		#endregion

		#region Constructor(Stream, Settings) tests

		[Fact]
		public void Constructor_StreamSettings_SetsSettingsAndMakeThemReadOnly() {
			PbfWriterSettings settings = new PbfWriterSettings();
			using (PbfWriter target = new PbfWriter(new MemoryStream(), settings)) {
				Assert.Same(settings, target.Settings);
				Assert.True(settings.IsReadOnly);
			}
		}

		[Fact]
		public void Constructor_StreamSettings_WritesOsmHeader() {
			MemoryStream stream = new MemoryStream();
			PbfWriterSettings settings = new PbfWriterSettings();
			using (PbfWriter target = new PbfWriter(stream, settings)) {
				;
			}

			Assert.True(stream.GetBuffer().Length > 0);
		}

		#endregion

		#region Dispose() tests

		[Fact]
		public void Dispose_ClosesOutputStreamIfWritingToFiles() {
			string filename = "TestFiles\\pbfwriter-closes-output-filestream-test.pbf";
			File.Delete(filename);

			PbfWriterSettings settings = new PbfWriterSettings();
			PbfWriter target = new PbfWriter(filename, settings);
			target.Dispose();

			new FileStream(filename, FileMode.Open, FileAccess.ReadWrite);
		}

		[Fact]
		public void Dispose_ClosesOutputStreamIfWritingToStream() {
			PbfWriterSettings settings = new PbfWriterSettings();
			MemoryStream stream = new MemoryStream();

			PbfWriter target = new PbfWriter(stream, settings);
			target.Dispose();

			Assert.False(stream.CanRead);
		}

		#endregion

		#region Write(IEntityInfo) tests

		[Fact]
		public void Write_ThrowsArgumentExceptionIfWriteMetadataIsTrueButEntityDoesntHaveMetadata() {
			using (PbfWriter target = new PbfWriter(new MemoryStream(), new PbfWriterSettings() { UseDenseFormat = true, Compression = CompressionMode.None, WriteMetadata = true })) {
				Assert.Throws<ArgumentException>(() => target.Write(_node));
			}
		}

		[Fact]
		public void Write_ThrowsArgumentNullExceptionIfMetadataContainsNullInsteadUsername() {
			_nodeProperties.Metadata.User = null;

			using (PbfWriter target = new PbfWriter(new MemoryStream(), new PbfWriterSettings() { UseDenseFormat = true, Compression = CompressionMode.None, WriteMetadata = true })) {
				Assert.Throws<ArgumentNullException>(() => target.Write(_nodeProperties));
			}
		}

		#endregion

		#region Write(NodeInfo) tests

		[Fact]
		public void Write_IEntityInfo_WritesNode() {
			PbfWriterSettings settings = new PbfWriterSettings() { UseDenseFormat = false, Compression = CompressionMode.None, WriteMetadata = false };
			MemoryStream stream = new MemoryStream();

			using (PbfWriter target = new PbfWriter(stream, settings)) {
				target.Write(_node);
			}

			this.TestPbfOutput(stream, _node);
		}

		[Fact]
		public void Write_IEntityInfo_WritesNodeWithTags() {
			PbfWriterSettings settings = new PbfWriterSettings() { UseDenseFormat = false, Compression = CompressionMode.None, WriteMetadata = false };
			MemoryStream stream = new MemoryStream();

			using (PbfWriter target = new PbfWriter(stream, settings)) {
				target.Write(_nodeTags);
			}

			this.TestPbfOutput(stream, _nodeTags);
		}

		[Fact]
		public void Write_IEntityInfo_WritesNodeWithMetadata() {
			PbfWriterSettings settings = new PbfWriterSettings() { UseDenseFormat = false, Compression = CompressionMode.None, WriteMetadata = true };
			MemoryStream stream = new MemoryStream();

			using (PbfWriter target = new PbfWriter(stream, settings)) {
				target.Write(_nodeProperties);
			}

			this.TestPbfOutput(stream, _nodeProperties);
		}


		[Fact]
		public void Write_IEntityInfo_DoesntWritesNodeMetadataIfWriteMetasdataSettingsIsFalse() {
			PbfWriterSettings settings = new PbfWriterSettings() { UseDenseFormat = false, Compression = CompressionMode.None, WriteMetadata = false };
			MemoryStream stream = new MemoryStream();

			using (PbfWriter target = new PbfWriter(stream, settings)) {
				target.Write(_nodeProperties);
			}

			this.TestPbfOutput(stream, _node);
		}

		#endregion

		#region Write(NodeInfo) tests - dense format

		[Fact]
		public void Write_IEntityInfo_WritesNode_Dense() {
			PbfWriterSettings settings = new PbfWriterSettings() { UseDenseFormat = true, Compression = CompressionMode.None, WriteMetadata = false };
			MemoryStream stream = new MemoryStream();

			using (PbfWriter target = new PbfWriter(stream, settings)) {
				target.Write(_node);
			}

			this.TestPbfOutput(stream, _node);
		}

		[Fact]
		public void Write_IEntityInfo_WritesNodeWithTags_Dense() {
			PbfWriterSettings settings = new PbfWriterSettings() { UseDenseFormat = true, Compression = CompressionMode.None, WriteMetadata = false };
			MemoryStream stream = new MemoryStream();

			using (PbfWriter target = new PbfWriter(stream, settings)) {
				target.Write(_nodeTags);
			}

			this.TestPbfOutput(stream, _nodeTags);
		}

		[Fact]
		public void Write_IEntityInfo_WritesNodeWithMetadata_Dense() {
			PbfWriterSettings settings = new PbfWriterSettings() { UseDenseFormat = true, Compression = CompressionMode.None, WriteMetadata = true };
			MemoryStream stream = new MemoryStream();

			using (PbfWriter target = new PbfWriter(stream, settings)) {
				target.Write(_nodeProperties);
			}

			this.TestPbfOutput(stream, _nodeProperties);
		}

		[Fact]
		public void Write_IEntityInfo_DoesntWritesNodeMetadataIfWriteMetasdataSettingsIsFalse_Dense() {
			PbfWriterSettings settings = new PbfWriterSettings() { UseDenseFormat = true, Compression = CompressionMode.None, WriteMetadata = false };
			MemoryStream stream = new MemoryStream();

			using (PbfWriter target = new PbfWriter(stream, settings)) {
				target.Write(_nodeProperties);
			}

			this.TestPbfOutput(stream, _node);
		}

		#endregion

		#region Write(WayInfo) tests

		[Fact]
		public void Write_IEntityInfo_WritesWay() {
			PbfWriterSettings settings = new PbfWriterSettings() { UseDenseFormat = false, Compression = CompressionMode.None, WriteMetadata = false };
			MemoryStream stream = new MemoryStream();

			using (PbfWriter target = new PbfWriter(stream, settings)) {
				target.Write(_way);
			}

			this.TestPbfOutput(stream, _way);
		}

		[Fact]
		public void Write_IEntityInfo_WritesWayWithTags() {
			PbfWriterSettings settings = new PbfWriterSettings() { UseDenseFormat = false, Compression = CompressionMode.None, WriteMetadata = false };
			MemoryStream stream = new MemoryStream();

			using (PbfWriter target = new PbfWriter(stream, settings)) {
				target.Write(_wayTags);
			}

			this.TestPbfOutput(stream, _wayTags);
		}

		[Fact]
		public void Write_IEntityInfo_WritesWayWithMetadata() {
			PbfWriterSettings settings = new PbfWriterSettings() { UseDenseFormat = false, Compression = CompressionMode.None, WriteMetadata = true };
			MemoryStream stream = new MemoryStream();

			using (PbfWriter target = new PbfWriter(stream, settings)) {
				target.Write(_wayProperties);
			}

			this.TestPbfOutput(stream, _wayProperties);
		}

		[Fact]
		public void Write_IEntityInfo_DoesntWriteWayMetadataIfWriteMetadataSettingsIsFalse() {
			PbfWriterSettings settings = new PbfWriterSettings() { UseDenseFormat = false, Compression = CompressionMode.None, WriteMetadata = false };
			MemoryStream stream = new MemoryStream();

			using (PbfWriter target = new PbfWriter(stream, settings)) {
				target.Write(_wayProperties);
			}

			this.TestPbfOutput(stream, _way);
		}

		#endregion

		#region Write(RelationInfo) tests

		[Fact]
		public void Write_IEntityInfo_WritesRelationWithNode() {
			PbfWriterSettings settings = new PbfWriterSettings() { UseDenseFormat = false, Compression = CompressionMode.None, WriteMetadata = false };
			MemoryStream stream = new MemoryStream();

			using (PbfWriter target = new PbfWriter(stream, settings)) {
				target.Write(_relationNode);
			}

			this.TestPbfOutput(stream, _relationNode);
		}

		[Fact]
		public void Write_IEntityInfo_WritesRelationWithWay() {
			PbfWriterSettings settings = new PbfWriterSettings() { UseDenseFormat = false, Compression = CompressionMode.None, WriteMetadata = false };
			MemoryStream stream = new MemoryStream();

			using (PbfWriter target = new PbfWriter(stream, settings)) {
				target.Write(_relationWay);
			}

			this.TestPbfOutput(stream, _relationWay);
		}

		[Fact]
		public void Write_IEntityInfo_WritesRelationWithRelation() {
			PbfWriterSettings settings = new PbfWriterSettings() { UseDenseFormat = false, Compression = CompressionMode.None, WriteMetadata = false };
			MemoryStream stream = new MemoryStream();

			using (PbfWriter target = new PbfWriter(stream, settings)) {
				target.Write(_relationRelation);
			}

			this.TestPbfOutput(stream, _relationRelation);
		}

		[Fact]
		public void Write_IEntityInfo_WritesRelationWithTags() {
			PbfWriterSettings settings = new PbfWriterSettings() { UseDenseFormat = false, Compression = CompressionMode.None, WriteMetadata = false };
			MemoryStream stream = new MemoryStream();

			using (PbfWriter target = new PbfWriter(stream, settings)) {
				target.Write(_relationTags);
			}

			this.TestPbfOutput(stream, _relationTags);
		}

		[Fact]
		public void Write_IEntityInfo_WritesRelationWithMetadata() {
			PbfWriterSettings settings = new PbfWriterSettings() { UseDenseFormat = false, Compression = CompressionMode.None, WriteMetadata = true };
			MemoryStream stream = new MemoryStream();

			using (PbfWriter target = new PbfWriter(stream, settings)) {
				target.Write(_relationNodeProperties);
			}

			this.TestPbfOutput(stream, _relationNodeProperties);
		}

		[Fact]
		public void Write_IEntityInfo_DoesntWritesRelationMetadataIfWriteMetasdataSettingsIsFalse() {
			PbfWriterSettings settings = new PbfWriterSettings() { UseDenseFormat = false, Compression = CompressionMode.None, WriteMetadata = false };
			MemoryStream stream = new MemoryStream();

			using (PbfWriter target = new PbfWriter(stream, settings)) {
				target.Write(_relationNodeProperties);
			}

			this.TestPbfOutput(stream, _relationNode);
		}

		#endregion

		#region Write(IOsmGeometry)

		[Fact]
		public void Write_IOsmGeometry_WritesNode() {
			Node node = new Node(1, 11.1, 12.1);
			PbfWriterSettings settings = new PbfWriterSettings() { UseDenseFormat = false, Compression = CompressionMode.None, WriteMetadata = false };
			MemoryStream stream = new MemoryStream();

			using (PbfWriter target = new PbfWriter(stream, settings)) {
				target.Write(node);
			}

			this.TestPbfOutput(stream, new NodeInfo(node));
		}

		[Fact]
		public void Write_IOsmGeometry_WritesWay() {
			Way way = new Way(10, new Node[] { new Node(1), new Node(2), new Node(3) });
			PbfWriterSettings settings = new PbfWriterSettings() { UseDenseFormat = false, Compression = CompressionMode.None, WriteMetadata = false };
			MemoryStream stream = new MemoryStream();

			using (PbfWriter target = new PbfWriter(stream, settings)) {
				target.Write(way);
			}

			this.TestPbfOutput(stream, new WayInfo(way));
		}

		[Fact]
		public void Write_IOsmGeometry_WritesRelation() {
			Relation relation = new Relation(100, new RelationMember[] { new RelationMember(new Node(1), "test-role") });

			PbfWriterSettings settings = new PbfWriterSettings() { UseDenseFormat = false, Compression = CompressionMode.None, WriteMetadata = false };
			MemoryStream stream = new MemoryStream();

			using (PbfWriter target = new PbfWriter(stream, settings)) {
				target.Write(relation);
			}

			this.TestPbfOutput(stream, new RelationInfo(relation));
		}

		[Fact]
		public void Write_IOsmGeometry_ThrowsExceptionIfEntityIsNull() {
			PbfWriterSettings settings = new PbfWriterSettings() { UseDenseFormat = false, Compression = CompressionMode.None, WriteMetadata = false };
			MemoryStream stream = new MemoryStream();

			using (PbfWriter target = new PbfWriter(stream, settings)) {
				IOsmGeometry entity = null;
				Assert.Throws<ArgumentNullException>(() => target.Write(entity));
			}
		}

		#endregion

		#region Flush() tests

		[Fact]
		public void Flush_ForcesWriterToWriteDataToUnderalyingStorage() {
			MemoryStream stream = new MemoryStream();

			PbfWriter target = new PbfWriter(stream, new PbfWriterSettings() { UseDenseFormat = false, Compression = CompressionMode.None, WriteMetadata = false });

			//1000 nodes should fit into tokens
			for (int i = 0; i < 1000; i++) {
				NodeInfo node = new NodeInfo(i, 45.87, -126.5, new TagsCollection());
				target.Write(node);
			}
			int minimalExpectedLengthIncrease = 1000 * 8;

			long originalStreamLength = stream.Length;
			target.Flush();

			Assert.True(stream.Length > originalStreamLength + minimalExpectedLengthIncrease);
		}

		#endregion

		private void TestPbfOutput(MemoryStream pbfStream, IEntityInfo expected) {
			if (pbfStream.CanSeek) {
				pbfStream.Seek(0, SeekOrigin.Begin);
			}
			else {
				pbfStream = new MemoryStream(pbfStream.GetBuffer());
			}

			PbfReader reader = new PbfReader(pbfStream, new OsmReaderSettings() { ReadMetadata = true });
			IEntityInfo read = reader.Read();

			switch (expected.EntityType) {
				case EntityType.Node: this.CompareNodes(expected as NodeInfo, read as NodeInfo); break;
				case EntityType.Way: this.CompareWays(expected as WayInfo, read as WayInfo); break;
				case EntityType.Relation: this.CompareRelation(expected as RelationInfo, read as RelationInfo); break;
			}
		}

		#region Compare*(Expected, actual)

		private void CompareNodes(NodeInfo expected, NodeInfo actual) {
			Assert.Equal(expected.ID, actual.ID);
			Assert.InRange(actual.Longitude, expected.Longitude - _resolution, expected.Longitude + _resolution);
			Assert.InRange(actual.Latitude, expected.Latitude - _resolution, expected.Latitude + _resolution);

			this.CompareTags(expected.Tags, actual.Tags);
			this.CompareEntityDetails(expected.Metadata, actual.Metadata);
		}

		private void CompareWays(WayInfo expected, WayInfo actual) {
			Assert.Equal(expected.ID, actual.ID);
			Assert.Equal(expected.Nodes.Count, actual.Nodes.Count);
			for (int i = 0; i < expected.Nodes.Count; i++) {
				Assert.Equal(expected.Nodes[i], actual.Nodes[i]);
			}

			this.CompareTags(expected.Tags, actual.Tags);
			this.CompareEntityDetails(expected.Metadata, actual.Metadata);
		}

		private void CompareRelation(RelationInfo expected, RelationInfo actual) {
			Assert.Equal(expected.ID, actual.ID);
			Assert.Equal(expected.Members.Count, actual.Members.Count);
			for (int i = 0; i < expected.Members.Count; i++) {
				Assert.Equal(expected.Members[i], actual.Members[i]);
			}

			this.CompareTags(expected.Tags, actual.Tags);
			this.CompareEntityDetails(expected.Metadata, actual.Metadata);
		}

		private void CompareTags(TagsCollection expected, TagsCollection actual) {
			if (expected == null && actual == null) {
				return;
			}

			Assert.Equal(expected.Count, actual.Count);
			Assert.True(expected.All(tag => actual.Contains(tag)));
		}

		private void CompareEntityDetails(EntityMetadata expected, EntityMetadata actual) {
			if (expected == null && actual == null) {
				return;
			}

			Assert.NotNull(actual);

			Assert.Equal(expected.Timestamp, actual.Timestamp);
			Assert.Equal(expected.Uid, actual.Uid);
			Assert.Equal(expected.User, actual.User);
			Assert.Equal(expected.Visible, actual.Visible);
			Assert.Equal(expected.Version, actual.Version);
			Assert.Equal(expected.Changeset, actual.Changeset);
		}

		#endregion
	}
}
