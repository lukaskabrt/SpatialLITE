using System.Collections.Generic;
using System.Linq;
using System.IO;

using Xunit;
using Moq;

using SpatialLite.Osm;
using SpatialLite.Osm.IO;
using Tests.SpatialLite.Osm.Data;

namespace Tests.SpatialLite.Osm.Geometries {
    public class OsmEntityInfoDatabaseTests {
		NodeInfo[] _nodeData;
		WayInfo[] _wayData;
		RelationInfo[] _relationData;
		IEntityInfo[] _data;

		public OsmEntityInfoDatabaseTests() {
			_nodeData = new NodeInfo[3];
			_nodeData[0] = new NodeInfo(1, 10.1, 11.1, new TagsCollection());
			_nodeData[1] = new NodeInfo(2, 10.2, 11.2, new TagsCollection());
			_nodeData[2] = new NodeInfo(3, 10.3, 11.3, new TagsCollection());

			_wayData = new WayInfo[2];
			_wayData[0] = new WayInfo(10, new TagsCollection(), _nodeData.Select(n => n.ID).ToArray());
			_wayData[1] = new WayInfo(11, new TagsCollection(), _nodeData.Select(n => n.ID).Skip(1).ToArray());

			_relationData = new RelationInfo[2];
			_relationData[0] = new RelationInfo(100, new TagsCollection(), new RelationMemberInfo[] { new RelationMemberInfo() { Reference = 10, Role = "way" }, new RelationMemberInfo() { Reference = 1, Role = "node" } });
			_relationData[1] = new RelationInfo(101, new TagsCollection(), new RelationMemberInfo[] { new RelationMemberInfo() { Reference = 101, Role = "relation" }, new RelationMemberInfo() { Reference = 1, Role = "node" } });

			_data = _nodeData.Concat<IEntityInfo>(_wayData).Concat<IEntityInfo>(_relationData).ToArray();
		}

		#region Constructor() tests

		[Fact]
		public void Constructor__CreatesEmptyDatabase() {
			OsmEntityInfoDatabase target = new OsmEntityInfoDatabase();

			Assert.Empty(target);
			Assert.Empty(target.Nodes);
			Assert.Empty(target.Ways);
			Assert.Empty(target.Relations);
		}

		#endregion

		#region Constructor(IEnumerable<T>) tests

		[Fact]
		public void Constructor_IEnumerable_CreatesCollectionWithSpecifiedItems() {
			OsmEntityInfoDatabase target = new OsmEntityInfoDatabase(_data);

			for (int i = 0; i < _data.Length; i++) {
				Assert.Contains(_data[i], target);
			}
		}

		[Fact]
		public void Constructor_IEnumerable_AddEnittiesToCorrextCollections() {
			OsmEntityInfoDatabase target = new OsmEntityInfoDatabase(_data);

			for (int i = 0; i < _nodeData.Length; i++) {
				Assert.Contains(_nodeData[i], target.Nodes);
			}

			for (int i = 0; i < _wayData.Length; i++) {
				Assert.Contains(_wayData[i], target.Ways);
			}

			for (int i = 0; i < _relationData.Length; i++) {
				Assert.Contains(_relationData[i], target.Relations);
			}
		}

		#endregion

		#region Load(IOsmReader) tests

		[Fact]
		public void Load_IOsmReader_LoadsNodes() {
			IOsmReader reader = new OsmXmlReader(TestDataReader.OpenOsmDB("osm-nodes.osm"), new OsmXmlReaderSettings() { ReadMetadata = true });
			OsmEntityInfoDatabase target = OsmEntityInfoDatabase.Load(reader);

			Assert.Equal(3, target.Nodes.Count);
			Assert.True(target.Nodes.Contains(1));
			Assert.True(target.Nodes.Contains(2));
			Assert.True(target.Nodes.Contains(3));
		}

		[Fact]
		public void Load_IOsmReader_LoadsWay() {
			IOsmReader reader = new OsmXmlReader(TestDataReader.OpenOsmDB("osm-way.osm"), new OsmXmlReaderSettings() { ReadMetadata = true });
			OsmEntityInfoDatabase target = OsmEntityInfoDatabase.Load(reader);

			Assert.Equal(3, target.Nodes.Count);
			Assert.True(target.Nodes.Contains(1));
			Assert.True(target.Nodes.Contains(2));
			Assert.True(target.Nodes.Contains(3));

			Assert.Equal(1, target.Ways.Count);
			Assert.True(target.Ways.Contains(10));
		}

		[Fact]
		public void Load_IOsmReader_LoadsRelation() {
			IOsmReader reader = new OsmXmlReader(TestDataReader.OpenOsmDB("osm-relation.osm"), new OsmXmlReaderSettings() { ReadMetadata = true });
			OsmEntityInfoDatabase target = OsmEntityInfoDatabase.Load(reader);

			Assert.Equal(1, target.Nodes.Count);
			Assert.True(target.Nodes.Contains(1));

			Assert.Equal(1, target.Relations.Count);
			Assert.True(target.Relations.Contains(100));
		}

		#endregion

		#region Save(IOsmWriter) tests

		[Fact]
		public void Save_CallsIOsmWriterWriteForAllEntities() {
			List<IEntityInfo> written = new List<IEntityInfo>();
			Mock<IOsmWriter> writerM = new Mock<IOsmWriter>();

			writerM.Setup(w => w.Write(It.IsAny<IEntityInfo>())).Callback<IEntityInfo>((e) => written.Add(e));

			OsmEntityInfoDatabase target = new OsmEntityInfoDatabase(_data);
			target.Save(writerM.Object);

			Assert.Equal(target.Count, written.Count);
			foreach (var entity in target) {
				Assert.Contains(entity, written);
			}
		}

		#endregion
	}
}