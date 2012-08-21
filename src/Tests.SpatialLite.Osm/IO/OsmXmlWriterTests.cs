using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Xunit;

using SpatialLite.Osm.Geometries;
using System.Xml.Linq;
using SpatialLite.Osm.IO;
using SpatialLite.Osm;

namespace Tests.SpatialLite.Osm.IO {
	public class OsmXmlWriterTests {
		//resolution for default granularity
		private const double _resolution = 1E-07;

		private EntityMetadata _details;
		private NodeInfo _node, _nodeTags, _nodeProperties;
		private WayInfo _way, _wayTags, _wayProperties, _wayWithoutNodes;
		private RelationInfo _relationNode, _relationWay, _relationRelation, _relationNodeProperties, _relationTags;

		public OsmXmlWriterTests() {
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
			_relationNodeProperties = new RelationInfo(1, new TagsCollection(), new RelationMemberInfo[] { new RelationMemberInfo() { MemberType = EntityType.Node, Reference = 10, Role = "test" } }, _details);
		}

		#region Constructor(Stream, WriteDetails)

		[Fact]
		public void Constructor_StreamSettings_SetsSettingsAndMakesThemReadOnly() {
			MemoryStream stream = new MemoryStream();
			OsmWriterSettings settings = new OsmWriterSettings();
			using (OsmXmlWriter target = new OsmXmlWriter(stream, settings)) {
				Assert.Same(settings, target.Settings);
				Assert.True(target.Settings.IsReadOnly);
			}
		}

		#endregion

		#region Constructor(Path, Settings)

		[Fact]
		public void Constructor_PathSettings_SetsSettingsAndMakesThemReadOnly() {
			string path = "TestFiles\\xmlwriter-constructor-test.osm";
			OsmWriterSettings settings = new OsmWriterSettings();
			using (OsmXmlWriter target = new OsmXmlWriter(path, settings)) {
				Assert.Same(settings, target.Settings);
				Assert.True(target.Settings.IsReadOnly);
			}
		}

		[Fact]
		public void Constructor_PathSettings_CreatesOutputFile() {
			string filename = "TestFiles\\osmwriter-constructor-creates-output-test.pbf";
			File.Delete(filename);

			OsmWriterSettings settings = new OsmWriterSettings();
			using (OsmXmlWriter target = new OsmXmlWriter(filename, settings)) {
				;
			}

			Assert.True(File.Exists(filename));
		}

		#endregion

		#region Dispose() tests

		[Fact]
		public void Dispose_ClosesOutputStreamIfWritingToFiles() {
			string path = "TestFiles\\xmlwriter-closes-output-filestream-test.osm";
			File.Delete(path);

			OsmXmlWriter target = new OsmXmlWriter(path, new OsmWriterSettings());
			target.Dispose();

			FileStream testStream = null;
			Assert.DoesNotThrow(() => testStream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite));
			testStream.Dispose();
		}

		[Fact]
		public void Dispose_ClosesOutputStreamIfWritingToStream() {
			MemoryStream stream = new MemoryStream();

			OsmXmlWriter target = new OsmXmlWriter(stream, new OsmWriterSettings());
			target.Dispose();

			Assert.False(stream.CanRead);
		}

		#endregion

		#region Write(IEntityInfo) tests

		[Fact]
		public void Write_ThrowsArgumentExceptionIfWriteMetadataIsTrueButEntityDoesntHaveMetadata() {
			MemoryStream stream = new MemoryStream();

			using (OsmXmlWriter target = new OsmXmlWriter(stream, new OsmWriterSettings() { WriteMetadata = true })) {
				Assert.Throws<ArgumentException>(() => target.Write(_node));
			}
		}

		[Fact]
		public void Write_DoesNotThrowsExceptionIfMetadataContainsNullInsteadUsername() {
			MemoryStream stream = new MemoryStream();
			_nodeProperties.Metadata.User = null;

			using (OsmXmlWriter target = new OsmXmlWriter(stream, new OsmWriterSettings() { WriteMetadata = true })) {
				Assert.DoesNotThrow(() => target.Write(_nodeProperties));
			}
		}

		#endregion

		#region Write(NodeInfo) tests

		[Fact]
		public void Write_IEntityInfo_WritesNode() {
			MemoryStream stream = new MemoryStream();

			using (OsmXmlWriter target = new OsmXmlWriter(stream, new OsmWriterSettings() { WriteMetadata = false })) {
				target.Write(_node);
			}

			this.TestXmlOutput(stream, _node, false);
		}

		[Fact]
		public void Write_IEntityInfo_WritesNodeWithTags() {
			MemoryStream stream = new MemoryStream();

			using (OsmXmlWriter target = new OsmXmlWriter(stream, new OsmWriterSettings() { WriteMetadata = false })) {
				target.Write(_nodeTags);
			}

			this.TestXmlOutput(stream, _nodeTags, false);
		}

		[Fact]
		public void Write_IEntityInfo_WritesNodeWithMetadata() {
			MemoryStream stream = new MemoryStream();

			using (OsmXmlWriter target = new OsmXmlWriter(stream, new OsmWriterSettings() { WriteMetadata = true })) {
				target.Write(_nodeProperties);
			}

			this.TestXmlOutput(stream, _nodeProperties, true);
		}

		[Fact]
		public void Write_IEntityInfo_DoesntWriteNodeMetadataIfWriteMedataIsFalse() {
			MemoryStream stream = new MemoryStream();

			using (OsmXmlWriter target = new OsmXmlWriter(stream, new OsmWriterSettings() { WriteMetadata = false })) {
				target.Write(_nodeProperties);
			}

			stream = new MemoryStream(stream.GetBuffer());

			using (TextReader reader = new StreamReader(stream)) {
				string line = null;
				while ((line = reader.ReadLine()) != null) {
					Assert.DoesNotContain("timestamp", line);
				}
			}
		}

		#endregion

		#region Write(WayInfo) tests

		[Fact]
		public void Write_IEntityInfo_WritesWay() {
			MemoryStream stream = new MemoryStream();

			using (OsmXmlWriter target = new OsmXmlWriter(stream, new OsmWriterSettings() { WriteMetadata = false })) {
				target.Write(_way);
			}

			this.TestXmlOutput(stream, _way, false);
		}

		[Fact]
		public void Write_IEntityInfo_WritesWayWithTags() {
			MemoryStream stream = new MemoryStream();

			using (OsmXmlWriter target = new OsmXmlWriter(stream, new OsmWriterSettings() { WriteMetadata = false })) {
				target.Write(_wayTags);
			}

			this.TestXmlOutput(stream, _wayTags, false);
		}

		[Fact]
		public void Write_IEntityInfo_WritesWayWithMetadata() {
			MemoryStream stream = new MemoryStream();

			using (OsmXmlWriter target = new OsmXmlWriter(stream, new OsmWriterSettings() { WriteMetadata = true })) {
				target.Write(_wayProperties);
			}

			this.TestXmlOutput(stream, _wayProperties, true);
		}

		[Fact]
		public void Write_IEntityInfo_DoesntWriteWayMetadataIfWriteMedataIsFalse() {
			MemoryStream stream = new MemoryStream();

			using (OsmXmlWriter target = new OsmXmlWriter(stream, new OsmWriterSettings() { WriteMetadata = false })) {
				target.Write(_wayProperties);
			}

			stream = new MemoryStream(stream.GetBuffer());

			using (TextReader reader = new StreamReader(stream)) {
				string line = null;
				while ((line = reader.ReadLine()) != null) {
					Assert.DoesNotContain("timestamp", line);
				}
			}
		}

		#endregion

		#region Write(RelationInfo) tests

		[Fact]
		public void Write_IEntityInfo_WritesRelationWithNode() {
			MemoryStream stream = new MemoryStream();

			using (OsmXmlWriter target = new OsmXmlWriter(stream, new OsmWriterSettings() { WriteMetadata = false })) {
				target.Write(_relationNode);
			}

			this.TestXmlOutput(stream, _relationNode, false);
		}

		[Fact]
		public void Write_IEntityInfo_WritesRelationWithWay() {
			MemoryStream stream = new MemoryStream();

			using (OsmXmlWriter target = new OsmXmlWriter(stream, new OsmWriterSettings() { WriteMetadata = false })) {
				target.Write(_relationWay);
			}

			this.TestXmlOutput(stream, _relationWay, false);
		}

		[Fact]
		public void Write_IEntityInfo_WritesRelationWithRelation() {
			MemoryStream stream = new MemoryStream();

			using (OsmXmlWriter target = new OsmXmlWriter(stream, new OsmWriterSettings() { WriteMetadata = false })) {
				target.Write(_relationRelation);
			}

			this.TestXmlOutput(stream, _relationRelation, false);
		}

		[Fact]
		public void Write_IEntityInfo_WritesRelationWithTags() {
			MemoryStream stream = new MemoryStream();

			using (OsmXmlWriter target = new OsmXmlWriter(stream, new OsmWriterSettings() { WriteMetadata = false })) {
				target.Write(_relationTags);
			}

			this.TestXmlOutput(stream, _relationTags, false);
		}

		[Fact]
		public void Write_IEntityInfo_WritesRelationWithMetadata() {
			MemoryStream stream = new MemoryStream();

			using (OsmXmlWriter target = new OsmXmlWriter(stream, new OsmWriterSettings() { WriteMetadata = true })) {
				target.Write(_relationNodeProperties);
			}

			this.TestXmlOutput(stream, _relationNodeProperties, true);
		}

		[Fact]
		public void Write_IEntityInfo_DoesntWriteRelationMetadataIfWriteMedataIsFalse() {
			MemoryStream stream = new MemoryStream();

			using (OsmXmlWriter target = new OsmXmlWriter(stream, new OsmWriterSettings() { WriteMetadata = false })) {
				target.Write(_relationNodeProperties);
			}

			stream = new MemoryStream(stream.GetBuffer());

			using (TextReader reader = new StreamReader(stream)) {
				string line = null;
				while ((line = reader.ReadLine()) != null) {
					Assert.DoesNotContain("timestamp", line);
				}
			}
		}

		#endregion

		#region Write(IOsmGeometry)

		[Fact]
		public void Write_IOsmGeometry_WritesNode() {
			Node node = new Node(1, 11.1, 12.1);

			MemoryStream stream = new MemoryStream();

			using (OsmXmlWriter target = new OsmXmlWriter(stream, new OsmWriterSettings() { WriteMetadata = false })) {
				target.Write(node);
			}

			this.TestXmlOutput(stream, new NodeInfo(node), false);
		}

		[Fact]
		public void Write_IOsmGeometry_WritesWay() {
			Way way = new Way(10, new Node[] { new Node(1), new Node(2), new Node(3) });

			MemoryStream stream = new MemoryStream();

			using (OsmXmlWriter target = new OsmXmlWriter(stream, new OsmWriterSettings() { WriteMetadata = false })) {
				target.Write(way);
			}

			this.TestXmlOutput(stream, new WayInfo(way), false);
		}

		[Fact]
		public void Write_IOsmGeometry_WritesRelation() {
			Relation relation = new Relation(100, new RelationMember[] { new RelationMember(new Node(1), "test-role") });

			MemoryStream stream = new MemoryStream();

			using (OsmXmlWriter target = new OsmXmlWriter(stream, new OsmWriterSettings() { WriteMetadata = false })) {
				target.Write(relation);
			}

			this.TestXmlOutput(stream, new RelationInfo(relation), false);
		}

		[Fact]
		public void Write_IOsmGeometry_ThrowsExceptionIfEntityIsNull() {
			MemoryStream stream = new MemoryStream();

			using (OsmXmlWriter target = new OsmXmlWriter(stream, new OsmWriterSettings() { WriteMetadata = false })) {
				IOsmGeometry entity = null;
				Assert.Throws<ArgumentNullException>(() => target.Write(entity));
			}
		}

		#endregion

		private void TestXmlOutput(MemoryStream xmlStream, IEntityInfo expected, bool readMetadata) {
			if (xmlStream.CanSeek) {
				xmlStream.Seek(0, SeekOrigin.Begin);
			}
			else {
				xmlStream = new MemoryStream(xmlStream.GetBuffer());
			}

			OsmXmlReader reader = new OsmXmlReader(xmlStream, new OsmXmlReaderSettings() { ReadMetadata = readMetadata });
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

		private void CheckNode(XElement element) {
			Assert.Equal("15", element.Attribute("id").Value);
			Assert.Equal("46.8", element.Attribute("lon").Value);
			Assert.Equal("-15.6", element.Attribute("lat").Value);

			XElement tagE = element.Elements("tag").Single();
			Assert.Equal("source", tagE.Attribute("k").Value);
			Assert.Equal("survey", tagE.Attribute("v").Value);
		}

		private void CheckWay(XElement element) {
			Assert.Equal("25", element.Attribute("id").Value);

			var nodesElement = element.Elements("nd");
			Assert.Equal(3, nodesElement.Count());
			Assert.Equal("11", nodesElement.ToList()[0].Attribute("ref").Value);
			Assert.Equal("12", nodesElement.ToList()[1].Attribute("ref").Value);
			Assert.Equal("13", nodesElement.ToList()[2].Attribute("ref").Value);

			XElement tagE = element.Elements("tag").Single();
			Assert.Equal("source", tagE.Attribute("k").Value);
			Assert.Equal("survey", tagE.Attribute("v").Value);
		}

		private void CheckRelation(XElement element) {
			Assert.Equal("25", element.Attribute("id").Value);

			XElement tagE = element.Elements("tag").Single();
			Assert.Equal("source", tagE.Attribute("k").Value);
			Assert.Equal("survey", tagE.Attribute("v").Value);
		}

		private void CheckOsmDetails(EntityMetadata details, XElement element) {
			if (details == null) {
				Assert.Null(element.Attribute("version"));
				Assert.Null(element.Attribute("changeset"));
				Assert.Null(element.Attribute("uid"));
				Assert.Null(element.Attribute("user"));
				Assert.Null(element.Attribute("visible"));
				Assert.Null(element.Attribute("timestamp"));
			}
			else {
				Assert.Equal("2", element.Attribute("version").Value);
				Assert.Equal("123", element.Attribute("changeset").Value);
				Assert.Equal("4587", element.Attribute("uid").Value);
				Assert.Equal("username", element.Attribute("user").Value);
				Assert.Equal("true", element.Attribute("visible").Value);
				Assert.Equal("2011-01-20T14:00:04Z", element.Attribute("timestamp").Value);
			}
		}
	}
}
