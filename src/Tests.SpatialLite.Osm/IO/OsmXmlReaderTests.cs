using System;
using System.Linq;
using System.IO;
using System.Xml;

using Xunit;
using SpatialLite.Osm.IO;
using SpatialLite.Osm;
using Tests.SpatialLite.Osm.Data;

namespace Tests.SpatialLite.Osm.IO {
    public class OsmXmlReaderTests {
		private EntityMetadata _details;
		private NodeInfo _node, _nodeTags, _nodeProperties;
		private WayInfo _way, _wayTags, _wayProperties, _wayWithoutNodes;
		private RelationInfo _relationNode, _relationWay, _relationRelation, _relationTags, _relationProperties, _relationWithoutMembers;

		public OsmXmlReaderTests() {
			_details = new EntityMetadata() {
				Timestamp = new DateTime(2010, 11, 19, 22, 5, 56, DateTimeKind.Utc),
				Uid = 127998,
				User = "Luk@s",
				Visible = true,
				Version = 2,
				Changeset = 6410629
			};

			_node = new NodeInfo(1, 50.4, 16.2, new TagsCollection());
			_nodeTags = new NodeInfo(2, 50.4, 16.2, new TagsCollection(new Tag[] { new Tag("name", "test"), new Tag("name-2", "test-2") }));
			_nodeProperties = new NodeInfo(3, 50.4, 16.2, new TagsCollection(), _details);

			_way = new WayInfo(1, new TagsCollection(), new int[] { 10, 11, 12 });
			_wayTags = new WayInfo(2, new TagsCollection(new Tag[] { new Tag("name", "test"), new Tag("name-2", "test-2") }), new int[] { 10, 11, 12 });
			_wayProperties = new WayInfo(1, new TagsCollection(), new int[] { 10, 11, 12 }, _details);
			_wayWithoutNodes = new WayInfo(1, new TagsCollection(), new int[] { });

			_relationNode = new RelationInfo(1, new TagsCollection(), new RelationMemberInfo[] { new RelationMemberInfo() { MemberType = EntityType.Node, Reference = 10, Role = "test" } });
			_relationWay = new RelationInfo(1, new TagsCollection(), new RelationMemberInfo[] { new RelationMemberInfo() { MemberType = EntityType.Way, Reference = 10, Role = "test" } });
			_relationRelation = new RelationInfo(1, new TagsCollection(), new RelationMemberInfo[] { new RelationMemberInfo() { MemberType = EntityType.Relation, Reference = 10, Role = "test" } });
			_relationTags = new RelationInfo(
				2,
				new TagsCollection(new Tag[] { new Tag("name", "test"), new Tag("name-2", "test-2") }),
				new RelationMemberInfo[] { new RelationMemberInfo() { MemberType = EntityType.Node, Reference = 10, Role = "test" } });
			_relationProperties = new RelationInfo(1, new TagsCollection(), new RelationMemberInfo[] { new RelationMemberInfo() { MemberType = EntityType.Node, Reference = 10, Role = "test" } }, _details);
			_relationWithoutMembers = new RelationInfo(1, new TagsCollection(), new RelationMemberInfo[] { });
		}

		#region Constructor(Path, Settings)

		[Fact]
		public void Constructor_StringSettings_ThrowsExceptionIfFileDoesnotExist() {
			Assert.Throws<FileNotFoundException>(delegate { new OsmXmlReader("non-existing-file.osm", new OsmXmlReaderSettings() { ReadMetadata = false }); });
		}

		[Fact]
		public void Constructor_StringSettings_SetsSettingsAndMakesItReadOnly() {
			string path = "../../src/Tests.SpatialLite.Osm/Data/Xml/osm-real-file.osm";
			OsmXmlReaderSettings settings = new OsmXmlReaderSettings() { ReadMetadata = false };
			using (OsmXmlReader target = new OsmXmlReader(path, settings)) {
				Assert.Same(settings, target.Settings);
				Assert.True(settings.IsReadOnly);
			}
		}

		#endregion

		#region Constructor(Stream, Settings) tests

		[Fact]
		public void Constructor_StreamSettings_SetsSettingsAndMakesItReadOnly() {
			OsmXmlReaderSettings settings = new OsmXmlReaderSettings() { ReadMetadata = false };
			using (OsmXmlReader target = new OsmXmlReader(TestDataReader.OpenXml("osm-simple-node.osm"), settings)) {
				Assert.Same(settings, target.Settings);
				Assert.True(settings.IsReadOnly);
			}
		}

		#endregion

		#region Read() tests

		[Fact]
		public void Read_SkipsUnknownElements() {
			OsmXmlReader target = new OsmXmlReader(TestDataReader.OpenXml("osm-unknown-inner-element.osm"), new OsmXmlReaderSettings() { ReadMetadata = false });
			IEntityInfo result = target.Read();

			Assert.NotNull(result as NodeInfo);
		}

		//Tested only on Nodes - code for parsing Tags is shared among functions parsing Node, Way and Relation
		[Fact]
		public void Read_ThrowsExceptionIfTagHasNotKey() {
			OsmXmlReader target = new OsmXmlReader(TestDataReader.OpenXml("osm-node-tag-without-key.osm"), new OsmXmlReaderSettings() { ReadMetadata = false });

			Assert.Throws<XmlException>(() => target.Read());
		}

		//Tested only on Nodes - code for parsing Tags is shared among functions parsing Node, Way and Relation
		[Fact]
		public void Read_ThrowsExceptionIfTagHasNotValue() {
			OsmXmlReader target = new OsmXmlReader(TestDataReader.OpenXml("osm-node-tag-without-value.osm"), new OsmXmlReaderSettings() { ReadMetadata = false });

			Assert.Throws<XmlException>(() => target.Read());
		}

		[Fact]
		public void Read_ThrowsExceptionIPieceOffMetadataIsMissingAndStrictModeIsTrue() {
			OsmXmlReader target = new OsmXmlReader(TestDataReader.OpenXml("osm-node-missing-timestamp.osm"), new OsmXmlReaderSettings() { ReadMetadata = true, StrictMode = true });
			Assert.Throws<XmlException>(() => target.Read());
		}

		[Fact]
		public void Read_DoesNotThrowExceptionIPieceOffMetadataIsMissingAndStrictModeIsFalse() {
			OsmXmlReader target = new OsmXmlReader(TestDataReader.OpenXml("osm-node-missing-timestamp.osm"), new OsmXmlReaderSettings() { ReadMetadata = true, StrictMode = false });
			target.Read();
		}

		#endregion

		#region Read() tests - nodes

		[Fact]
		public void Read_ThrowsExceptionIfNodeHasNotID() {
			OsmXmlReader target = new OsmXmlReader(TestDataReader.OpenXml("osm-node-without-id.osm"), new OsmXmlReaderSettings() { ReadMetadata = false });

			Assert.Throws<XmlException>(() => target.Read());
		}

		[Fact]
		public void Read_ThrowsExceptionIfNodeHasNotLat() {
			OsmXmlReader target = new OsmXmlReader(TestDataReader.OpenXml("osm-node-without-lat.osm"), new OsmXmlReaderSettings() { ReadMetadata = false });

			Assert.Throws<XmlException>(() => target.Read());
		}

		[Fact]
		public void Read_ThrowsExceptionIfNodeHasNotLon() {
			OsmXmlReader target = new OsmXmlReader(TestDataReader.OpenXml("osm-node-without-lon.osm"), new OsmXmlReaderSettings() { ReadMetadata = false });

			Assert.Throws<XmlException>(() => target.Read());
		}

		[Fact]
		public void Read_ReadsSimpleNode() {
			OsmXmlReader target = new OsmXmlReader(TestDataReader.OpenXml("osm-simple-node.osm"), new OsmXmlReaderSettings() { ReadMetadata = false });
			NodeInfo readNode = target.Read() as NodeInfo;

			this.CompareNodes(_node, readNode);
		}

		[Fact]
		public void Read_ReadsNodeWithUnknownElement() {
			OsmXmlReader target = new OsmXmlReader(TestDataReader.OpenXml("osm-node-with-tag-and-unknown-element.osm"), new OsmXmlReaderSettings() { ReadMetadata = false });
			NodeInfo readNode = target.Read() as NodeInfo;

			this.CompareNodes(_node, readNode);

			// nothing more left to read in the file
			Assert.Null(target.Read());
		}

		[Fact]
		public void Read_ReadsNodeWithTags() {
			OsmXmlReader target = new OsmXmlReader(TestDataReader.OpenXml("osm-node-with-tags.osm"), new OsmXmlReaderSettings() { ReadMetadata = false });
			NodeInfo readNode = target.Read() as NodeInfo;

			this.CompareNodes(_nodeTags, readNode);
		}

		[Fact]
		public void Read_ReadsNodeWithAllAttributes() {
			OsmXmlReader target = new OsmXmlReader(TestDataReader.OpenXml("osm-node-all-properties.osm"), new OsmXmlReaderSettings() { ReadMetadata = true });
			NodeInfo readNode = target.Read() as NodeInfo;

			this.CompareNodes(_nodeProperties, readNode);
		}

		#endregion

		#region Read() tests - way

		[Fact]
		public void Read_ThrowsExceptionIfWayHasNotID() {
			OsmXmlReader target = new OsmXmlReader(TestDataReader.OpenXml("osm-way-nd-without-ref.osm"), new OsmXmlReaderSettings() { ReadMetadata = false });

			Assert.Throws<XmlException>(() => target.Read());
		}

		[Fact]
		public void Read_ThrowsExceptionIfWayNDHasNotRef() {
			OsmXmlReader target = new OsmXmlReader(TestDataReader.OpenXml("osm-way-nd-without-ref.osm"), new OsmXmlReaderSettings() { ReadMetadata = false });

			Assert.Throws<XmlException>(() => target.Read());
		}

		[Fact]
		public void Read_ReadsWayWithoutNodes() {
			OsmXmlReader target = new OsmXmlReader(TestDataReader.OpenXml("osm-way-without-nodes.osm"), new OsmXmlReaderSettings() { ReadMetadata = false });
			WayInfo readWay = target.Read() as WayInfo;

			this.CompareWays(_wayWithoutNodes, readWay);
		}

		[Fact]
		public void Read_ReadsSimpleWay() {
			OsmXmlReader target = new OsmXmlReader(TestDataReader.OpenXml("osm-simple-way.osm"), new OsmXmlReaderSettings() { ReadMetadata = false });
			WayInfo readWay = target.Read() as WayInfo;

			this.CompareWays(_way, readWay);
		}

		[Fact]
		public void Read_ReadsWayWithTags() {
			OsmXmlReader target = new OsmXmlReader(TestDataReader.OpenXml("osm-way-with-tags.osm"), new OsmXmlReaderSettings() { ReadMetadata = false });
			WayInfo readWay = target.Read() as WayInfo;

			this.CompareWays(_wayTags, readWay);
		}

		[Fact]
		public void Read_ReadsWayWithUnknownElement() {
			OsmXmlReader target = new OsmXmlReader(TestDataReader.OpenXml("osm-way-with-tags-and-unknown-element.osm"), new OsmXmlReaderSettings() { ReadMetadata = false });
			WayInfo readWay = target.Read() as WayInfo;

			this.CompareWays(_wayTags, readWay);
		}

		[Fact]
		public void Read_ReadsWayWithAllAttributes() {
			OsmXmlReader target = new OsmXmlReader(TestDataReader.OpenXml("osm-way-all-properties.osm"), new OsmXmlReaderSettings() { ReadMetadata = true });
			WayInfo readWay = target.Read() as WayInfo;

			this.CompareWays(_wayProperties, readWay);
		}

		#endregion

		#region Read() tests - relation

		[Fact]
		public void Read_ThrowsExceptionIfRelationHasNotID() {
			OsmXmlReader target = new OsmXmlReader(TestDataReader.OpenXml("osm-relation-without-id.osm"), new OsmXmlReaderSettings() { ReadMetadata = false });

			Assert.Throws<XmlException>(() => target.Read());
		}

		[Fact]
		public void Read_ThrowsExceptionIfRelationMemberHasNotRef() {
			OsmXmlReader target = new OsmXmlReader(TestDataReader.OpenXml("osm-relation-member-without-ref.osm"), new OsmXmlReaderSettings() { ReadMetadata = false });

			Assert.Throws<XmlException>(() => target.Read());
		}

		[Fact]
		public void Read_ThrowsExceptionIfRelationMemberHasNotType() {
			OsmXmlReader target = new OsmXmlReader(TestDataReader.OpenXml("osm-relation-member-without-type.osm"), new OsmXmlReaderSettings() { ReadMetadata = false });

			Assert.Throws<XmlException>(() => target.Read());
		}

		[Fact]
		public void Read_ReadsRelationWithoutMembers() {
			OsmXmlReader target = new OsmXmlReader(TestDataReader.OpenXml("osm-relation-without-members.osm"), new OsmXmlReaderSettings() { ReadMetadata = false });
			RelationInfo readRelation = target.Read() as RelationInfo;

			this.CompareRelation(_relationWithoutMembers, readRelation);
		}

		[Fact]
		public void Read_ReadsRelationWithNodeMember() {
			OsmXmlReader target = new OsmXmlReader(TestDataReader.OpenXml("osm-relation-node.osm"), new OsmXmlReaderSettings() { ReadMetadata = false });
			RelationInfo readRelation = target.Read() as RelationInfo;

			this.CompareRelation(_relationNode, readRelation);
		}

		[Fact]
		public void Read_ReadsRelationWithWayMember() {
			OsmXmlReader target = new OsmXmlReader(TestDataReader.OpenXml("osm-relation-way.osm"), new OsmXmlReaderSettings() { ReadMetadata = false });
			RelationInfo readRelation = target.Read() as RelationInfo;

			this.CompareRelation(_relationWay, readRelation);
		}

		[Fact]
		public void Read_ReadsRelationWithRelationMember() {
			OsmXmlReader target = new OsmXmlReader(TestDataReader.OpenXml("osm-relation-relation.osm"), new OsmXmlReaderSettings() { ReadMetadata = false });
			RelationInfo readRelation = target.Read() as RelationInfo;

			this.CompareRelation(_relationRelation, readRelation);
		}

		[Fact]
		public void Read_ReadsRelationWithTags() {
			OsmXmlReader target = new OsmXmlReader(TestDataReader.OpenXml("osm-relation-with-tags.osm"), new OsmXmlReaderSettings() { ReadMetadata = false });
			RelationInfo readRelation = target.Read() as RelationInfo;

			this.CompareRelation(_relationTags, readRelation);
		}

		[Fact]
		public void Read_ReadsRelationWithTagsAndUnknownElement() {
			OsmXmlReader target = new OsmXmlReader(TestDataReader.OpenXml("osm-relation-with-tags-and-unknown-element.osm"), new OsmXmlReaderSettings() { ReadMetadata = false });
			RelationInfo readRelation = target.Read() as RelationInfo;

			this.CompareRelation(_relationTags, readRelation);
		}

		[Fact]
		public void Read_ReadsRelationWithAllProperties() {
			OsmXmlReader target = new OsmXmlReader(TestDataReader.OpenXml("osm-relation-all-properties.osm"), new OsmXmlReaderSettings() { ReadMetadata = true });
			RelationInfo readRelation = target.Read() as RelationInfo;

			this.CompareRelation(_relationProperties, readRelation);
		}

		#endregion

		#region Dispose() tests

		[Fact]
		public void Dispose_ClosesOutputStreamIfWritingToFiles() {
			string filename = "../../src/Tests.SpatialLite.Osm/Data/Xml/osm-real-file.osm";

			OsmXmlReader target = new OsmXmlReader(filename, new OsmXmlReaderSettings() { ReadMetadata = false });
			target.Dispose();

			FileStream testStream = null;
			testStream = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite);
			testStream.Dispose();
		}

		#endregion

		#region Compare*(Expected, actual)

		private void CompareNodes(NodeInfo expected, NodeInfo actual) {
			Assert.Equal(expected.ID, actual.ID);
			Assert.Equal(expected.Longitude, actual.Longitude);
			Assert.Equal(expected.Latitude, actual.Latitude);

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
