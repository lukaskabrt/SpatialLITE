using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Xunit;

using SpatialLite.Osm;
using SpatialLite.Osm.Geometries;
using SpatialLite.Osm.IO;
using SpatialLite.Osm.IO.Pbf;
using Tests.SpatialLite.Osm.Data;

namespace Tests.SpatialLite.Osm.IO {
	public class PbfReaderTests {
		//resolution for default granularity
		private const double _resolution = 1E-07;

		private EntityMetadata _details;
		private NodeInfo _node, _nodeTags, _nodeProperties;
		private WayInfo _way, _wayTags, _wayProperties, _wayWithoutNodes;
		private RelationInfo _relationNode, _relationWay, _relationRelation, _relationProperties, _relationTags;

		public PbfReaderTests() {
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

			_way = new WayInfo(1, new TagsCollection(), new int[] { 10, 11, 12 });
			_wayTags = new WayInfo(1, new TagsCollection(new Tag[] { new Tag("name", "test"), new Tag("name-2", "test-2") }), new int[] { 10, 11, 12 });
			_wayProperties = new WayInfo(1, new TagsCollection(), new int[] { 10, 11, 12 }, _details);
			_wayWithoutNodes = new WayInfo(1, new TagsCollection(), new int[] { });

			_relationNode = new RelationInfo(1, new TagsCollection(), new RelationMemberInfo[] { new RelationMemberInfo() { MemberType = EntityType.Node, Reference = 10, Role = "test" } });
			_relationWay = new RelationInfo(1, new TagsCollection(), new RelationMemberInfo[] { new RelationMemberInfo() { MemberType = EntityType.Way, Reference = 10, Role = "test" } });
			_relationRelation = new RelationInfo(1, new TagsCollection(), new RelationMemberInfo[] { new RelationMemberInfo() { MemberType = EntityType.Relation, Reference = 10, Role = "test" } });
			_relationTags = new RelationInfo(
				1,
				new TagsCollection(new Tag[] { new Tag("name", "test"), new Tag("name-2", "test-2") }),
				new RelationMemberInfo[] { new RelationMemberInfo() { MemberType = EntityType.Node, Reference = 10, Role = "test" } });
			_relationProperties = new RelationInfo(1, new TagsCollection(), new RelationMemberInfo[] { new RelationMemberInfo() { MemberType = EntityType.Node, Reference = 10, Role = "test" } }, _details);
		}

		#region Constructor(Stream, Settings) tests

		[Fact]
		public void Constructor_StreamSettings_ThrowsExceptionIfStreamDoesntContainOSMHeaderBeforeOSMData() {
			MemoryStream dataStream = new MemoryStream(PbfTestData.pbf_without_osm_header);
			Assert.Throws<InvalidDataException>(() => new PbfReader(dataStream, new OsmReaderSettings() {ReadMetadata = false}));
		}

		[Fact]
		public void Constructor_StreamSettings_ThrowsExceptionIfOSMHeaderDefinedUnsupportedRequiredFeature() {
			MemoryStream dataStream = new MemoryStream(PbfTestData.pbf_unsupported_required_feature);
			Assert.Throws<NotSupportedException>(() => new PbfReader(dataStream, new OsmReaderSettings() {ReadMetadata = false}));
		}

		[Fact]
		public void Constructor_StreamSettings_SetsSettingsAndMakesThemIsReadOnly() {
			MemoryStream dataStream = new MemoryStream(PbfTestData.pbf_n_node);
			OsmReaderSettings settings = new OsmReaderSettings();
			
			using (PbfReader target = new PbfReader(dataStream, settings)) {
				Assert.Same(settings, target.Settings);
				Assert.True(settings.IsReadOnly);
			}
		}

		#endregion
		
		#region Constructor(Path, Settings) tests

		[Fact]
		public void Constructor_StringSettings_ThrowsExceptionIfFileDoesnotExist() {
			Assert.Throws<FileNotFoundException>(delegate { new PbfReader("non-existing-file.pbf", new OsmReaderSettings() {ReadMetadata = false}); });
		}

		[Fact]
		public void Constructor_StringSettings_ThrowsExceptionIfFileDoesntContainOSMHeaderBeforeOSMData() {
			string filename = "..\\..\\src\\Tests.SpatialLite.Osm\\Data\\Pbf\\pbf-without-osm-header.pbf";
			Assert.Throws<InvalidDataException>(() => new PbfReader(filename, new OsmReaderSettings() {ReadMetadata = false}));
		}

		[Fact]
		public void Constructor_StringSettings_ThrowsExceptionIfOSMHeaderDefinedUnsupportedRequiredFeature() {
			string filename = "..\\..\\src\\Tests.SpatialLite.Osm\\Data\\Pbf\\pbf-unsupported-required-feature.pbf";
			Assert.Throws<NotSupportedException>(() => new PbfReader(filename, new OsmReaderSettings() {ReadMetadata = false}));
		}

		[Fact]
		public void Constructor_StringSettings_SetsSettingsAndMakesThemIsReadOnly() {
			string filename = "..\\..\\src\\Tests.SpatialLite.Osm\\Data\\Pbf\\pbf-n-node.pbf";
			OsmReaderSettings settings = new OsmReaderSettings();

			using (PbfReader target = new PbfReader(filename, settings)) {
				Assert.Same(settings, target.Settings);
				Assert.True(settings.IsReadOnly);
			}
		}

		#endregion

		#region Read() tests

		[Fact]
		public void Read_ReturnsNullIfAllEntitiesHaveBeenRead() {
			PbfReader target = new PbfReader(new MemoryStream(PbfTestData.pbf_n_node), new OsmReaderSettings() {ReadMetadata = false});
			//read only entity
			IEntityInfo read = target.Read();

			// should return null
			read = target.Read();
			Assert.Null(read);
		}

		[Fact]
		public void Read_ThrowInvalidDataExceptionIfHeaderBlockSizeExceedesAllowdValue() {
			Assert.Throws<InvalidDataException>(() => new PbfReader(new MemoryStream(PbfTestData.pbf_too_large_header_block), new OsmReaderSettings() {ReadMetadata = false}));
		}

		[Fact]
		public void Read_ThrowInvalidDataExceptionIfOsmDataBlockSizeExceedesAllowdValue() {
			using (PbfReader reader = new PbfReader(new MemoryStream(PbfTestData.pbf_too_large_data_block), new OsmReaderSettings() {ReadMetadata = false})) {
				Assert.Throws<InvalidDataException>(() => reader.Read());			
			}
		}

		#endregion

		#region Read() tests - nodes, dense, no compresion

		[Fact]
		public void Read_ReadsNode_DenseNoCompression() {
			PbfReader target = new PbfReader(new MemoryStream(PbfTestData.pbf_nd_node), new OsmReaderSettings() {ReadMetadata = false});
			NodeInfo readNode = target.Read() as NodeInfo;

			this.CompareNodes(_node, readNode);
		}

		[Fact]
		public void Read_ReadsNodeWithTags_DenseNoCompression() {
			PbfReader target = new PbfReader(new MemoryStream(PbfTestData.pbf_nd_node_tags), new OsmReaderSettings() {ReadMetadata = false});
			NodeInfo readNode = target.Read() as NodeInfo;

			this.CompareNodes(_nodeTags, readNode);
		}

		[Fact]
		public void Read_ReadsNodeWithMetadata_DenseNoCompression() {
			PbfReader target = new PbfReader(new MemoryStream(PbfTestData.pbf_nd_node_all_properties), new OsmReaderSettings() {ReadMetadata = true});
			NodeInfo readNode = target.Read() as NodeInfo;

			this.CompareNodes(_nodeProperties, readNode);
		}

		[Fact]
		public void Read_SkipsNodeMetadataIfProcessMetadataIsFalse_DenseNoCompression() {
			PbfReader target = new PbfReader(new MemoryStream(PbfTestData.pbf_nd_node_all_properties), new OsmReaderSettings() {ReadMetadata = false});
			NodeInfo readNode = target.Read() as NodeInfo;

			Assert.Null(readNode.Metadata);
		}

		#endregion
		
		#region Read() tests - nodes, no compresion

		[Fact]
		public void Read_ReadsNode_NoCompression() {
			PbfReader target = new PbfReader(new MemoryStream(PbfTestData.pbf_n_node), new OsmReaderSettings() {ReadMetadata = false});
			NodeInfo readNode = target.Read() as NodeInfo;

			this.CompareNodes(_node, readNode);
		}

		[Fact]
		public void Read_ReadsNodeWithTags_NoCompression() {
			PbfReader target = new PbfReader(new MemoryStream(PbfTestData.pbf_n_node_tags), new OsmReaderSettings() {ReadMetadata = false});
			NodeInfo readNode = target.Read() as NodeInfo;

			this.CompareNodes(_nodeTags, readNode);
		}

		[Fact]
		public void Read_ReadsNodeWithMetadata_NoCompression() {
			PbfReader target = new PbfReader(new MemoryStream(PbfTestData.pbf_n_node_all_properties), new OsmReaderSettings() {ReadMetadata = true});
			NodeInfo readNode = target.Read() as NodeInfo;

			this.CompareNodes(_nodeProperties, readNode);
		}

		[Fact]
		public void Read_SkipsNodeMetadataIfProcessMetadataIsFalse_NoCompression() {
			PbfReader target = new PbfReader(new MemoryStream(PbfTestData.pbf_n_node_all_properties), new OsmReaderSettings() {ReadMetadata = false});
			NodeInfo readNode = target.Read() as NodeInfo;

			Assert.Null(readNode.Metadata);
		}

		#endregion

		#region Read() tests - ways, no compresion

		[Fact]
		public void Read_ReadsWay_NoCompresion() {
			PbfReader target = new PbfReader(new MemoryStream(PbfTestData.pbf_n_way), new OsmReaderSettings() {ReadMetadata = false});
			WayInfo readWay = target.Read() as WayInfo;

			this.CompareWays(_way, readWay);
		}

		[Fact]
		public void Read_ReadsWayWithTags_NoCompresion() {
			PbfReader target = new PbfReader(new MemoryStream(PbfTestData.pbf_n_way_tags), new OsmReaderSettings() {ReadMetadata = false});
			WayInfo readWay = target.Read() as WayInfo;

			this.CompareWays(_wayTags, readWay);
		}

		[Fact]
		public void Read_ReadsWayWithMetadata_NoCompresion() {
			PbfReader target = new PbfReader(new MemoryStream(PbfTestData.pbf_n_way_all_properties), new OsmReaderSettings() {ReadMetadata = true});
			WayInfo readWay = target.Read() as WayInfo;

			this.CompareWays(_wayProperties, readWay);
		}

		[Fact]
		public void Read_SkipsWayMetadataIfProcessMetadataIsFalse_NoCompresion() {
			PbfReader target = new PbfReader(new MemoryStream(PbfTestData.pbf_n_way_all_properties), new OsmReaderSettings() {ReadMetadata = false});
			WayInfo readWay = target.Read() as WayInfo;

			Assert.Null(readWay.Metadata);
		}

		[Fact]
		public void Read_ReadsWayWithoutNodes_NoCompresion() {
			PbfReader target = new PbfReader(new MemoryStream(PbfTestData.pbf_n_way_without_nodes), new OsmReaderSettings() {ReadMetadata = false});
			WayInfo readWay = target.Read() as WayInfo;

			this.CompareWays(_wayWithoutNodes, readWay);
		}

		#endregion

		#region Read() tests - relations, no compresion

		[Fact]
		public void Read_ReadsRelationWithNode_NoCompresion() {
			PbfReader target = new PbfReader(new MemoryStream(PbfTestData.pbf_n_relation_node), new OsmReaderSettings() {ReadMetadata = false});
			RelationInfo readRelation = target.Read() as RelationInfo;

			this.CompareRelation(_relationNode, readRelation);
		}

		[Fact]
		public void Read_ReadsRelationWithWay_NoCompresion() {
			PbfReader target = new PbfReader(new MemoryStream(PbfTestData.pbf_n_relation_way), new OsmReaderSettings() {ReadMetadata = false});
			RelationInfo readRelation = target.Read() as RelationInfo;

			this.CompareRelation(_relationWay, readRelation);
		}

		[Fact]
		public void Read_ReadsRelationWithRelation_NoCompresion() {
			PbfReader target = new PbfReader(new MemoryStream(PbfTestData.pbf_n_relation_relation), new OsmReaderSettings() {ReadMetadata = false});
			RelationInfo readRelation = target.Read() as RelationInfo;

			this.CompareRelation(_relationRelation, readRelation);
		}

		[Fact]
		public void Read_ReadsRelationWithTags_NoCompresion() {
			PbfReader target = new PbfReader(new MemoryStream(PbfTestData.pbf_n_relation_tags), new OsmReaderSettings() {ReadMetadata = false});
			RelationInfo readRelation = target.Read() as RelationInfo;

			this.CompareRelation(_relationTags, readRelation);
		}

		[Fact]
		public void Read_ReadsRelationWithAllProperties_NoCompresion() {
			PbfReader target = new PbfReader(new MemoryStream(PbfTestData.pbf_n_relation_all_properties), new OsmReaderSettings() {ReadMetadata = true});
			RelationInfo readRelation = target.Read() as RelationInfo;

			this.CompareRelation(_relationProperties, readRelation);
		}

		[Fact]
		public void Read_SkipsRelationMetadataIfProcessMetadataIsFalse_NoCompresion() {
			PbfReader target = new PbfReader(new MemoryStream(PbfTestData.pbf_n_relation_all_properties), new OsmReaderSettings() {ReadMetadata = false});
			RelationInfo readRelation = target.Read() as RelationInfo;

			Assert.Null(readRelation.Metadata);
		}

		#endregion

		#region Dispose() tests

		[Fact]
		public void Dispose_ClosesOutputStreamIfWritingToFiles() {
			string filename = "..\\..\\src\\Tests.SpatialLite.Osm\\Data\\Pbf\\pbf-n-node.pbf";
			OsmReaderSettings settings = new OsmReaderSettings() {ReadMetadata = true};

			PbfReader target = new PbfReader(filename, settings);
			target.Dispose();

			FileStream testStream = null;
			Assert.DoesNotThrow(() => testStream = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite));
			testStream.Dispose();
		}

		[Fact]
		public void Dispose_ClosesOutputStreamIfWritingToStream() {
			MemoryStream stream = new MemoryStream(PbfTestData.pbf_n_node);
			OsmReaderSettings settings = new OsmReaderSettings() {ReadMetadata = true};

			PbfReader target = new PbfReader(stream, settings);
			target.Dispose();

			Assert.False(stream.CanRead);
		}

		#endregion

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
