using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Xunit;
using Moq;
using SpatialLite.Osm.IO;
using SpatialLite.Osm.Geometries;
using Tests.SpatialLite.Osm.Data;

namespace Tests.SpatialLite.Osm.Geometries {
    public class OsmGeometryDatabaseTests {
		Node[] _nodeData;
		Way[] _wayData;
		Relation[] _relationData;
		IOsmGeometry[] _data;

		public OsmGeometryDatabaseTests() {
			_nodeData = new Node[3];
			_nodeData[0] = new Node(1);
			_nodeData[1] = new Node(2);
			_nodeData[2] = new Node(3);

			_wayData = new Way[2];
			_wayData[0] = new Way(10, _nodeData);
			_wayData[1] = new Way(11, _nodeData.Skip(1));

			_relationData = new Relation[2];
			_relationData[0] = new Relation(100, new RelationMember[] { new RelationMember(_wayData[0], "way"), new RelationMember(_nodeData[0], "node") });
			_relationData[1] = new Relation(101, new RelationMember[] { new RelationMember(_relationData[0], "relation"), new RelationMember(_nodeData[0], "node") });

			_data = _nodeData.Concat<IOsmGeometry>(_wayData).Concat<IOsmGeometry>(_relationData).ToArray();
		}

		#region Constructor() tests

		[Fact]
		public void Constructor__CreatesEmptyDatabase() {
			OsmGeometryDatabase target = new OsmGeometryDatabase();

			Assert.Empty(target);
			Assert.Empty(target.Nodes);
			Assert.Empty(target.Ways);
			Assert.Empty(target.Relations);
		}

		#endregion

		#region Constructor(IEnumerable<T>) tests

		[Fact]
		public void Constructor_IEnumerable_CreatesCollectionWithSpecifiedItems() {
			OsmGeometryDatabase target = new OsmGeometryDatabase(_data);

			for (int i = 0; i < _data.Length; i++) {
				Assert.Contains(_data[i], target);
			}
		}

		[Fact]
		public void Constructor_IEnumerable_AddEnittiesToCorrextCollections() {
			OsmGeometryDatabase target = new OsmGeometryDatabase(_data);

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

		#region Load(IOsmReader, IgnoreMissing) tests

		[Fact]
		public void Load_LoadsNodes() {
			IOsmReader reader = new OsmXmlReader(TestDataReader.OpenOsmDB("osm-nodes.osm"), new OsmXmlReaderSettings() { ReadMetadata = true });
			OsmGeometryDatabase target = OsmGeometryDatabase.Load(reader, true);

			Assert.Equal(3, target.Nodes.Count);
			Assert.True(target.Nodes.Contains(1));
			Assert.True(target.Nodes.Contains(2));
			Assert.True(target.Nodes.Contains(3));
		}

		[Fact]
		public void Load_LoadsWay() {
			IOsmReader reader = new OsmXmlReader(TestDataReader.OpenOsmDB("osm-way.osm"), new OsmXmlReaderSettings() { ReadMetadata = true });
			OsmGeometryDatabase target = OsmGeometryDatabase.Load(reader, true);

			Assert.Equal(3, target.Nodes.Count);
			Assert.True(target.Nodes.Contains(1));
			Assert.True(target.Nodes.Contains(2));
			Assert.True(target.Nodes.Contains(3));

			Assert.Equal(1, target.Ways.Count);
			Assert.True(target.Ways.Contains(10));
		}

		[Fact]
		public void Load_LoadsRelation() {
			IOsmReader reader = new OsmXmlReader(TestDataReader.OpenOsmDB("osm-relation.osm"), new OsmXmlReaderSettings() { ReadMetadata = true });
			OsmGeometryDatabase target = OsmGeometryDatabase.Load(reader, true);

			Assert.Equal(1, target.Nodes.Count);
			Assert.True(target.Nodes.Contains(1));

			Assert.Equal(1, target.Relations.Count);
			Assert.True(target.Relations.Contains(100));
		}

		[Fact]
		public void Load_CanLoadRelationsWithReferenceToRelationsNotYetCreated() {
			IOsmReader reader = new OsmXmlReader(TestDataReader.OpenOsmDB("osm-relation-ref-other-relation.osm"), new OsmXmlReaderSettings() { ReadMetadata = true });
			OsmGeometryDatabase target = OsmGeometryDatabase.Load(reader, true);

			Assert.Equal(2, target.Relations.Count);
			Assert.True(target.Relations.Contains(100));
			Assert.True(target.Relations.Contains(101));

			Assert.Equal(101, target.Relations[100].Geometries[0].Member.ID);
		}

		[Fact]
		public void Load_ThrowsExceptionIfAllRelationReferencesAreNotResolvedAtTheEndOfLoadingAndIgnoreMissingIsFalse() {
			IOsmReader reader = new OsmXmlReader(TestDataReader.OpenOsmDB("osm-relation-invalid-ref.osm"), new OsmXmlReaderSettings() { ReadMetadata = true });
			Assert.Throws<ArgumentException>(() => OsmGeometryDatabase.Load(reader, false));
		}

		[Fact]
		public void Load_DoesNotThrowExceptionIfIgnoreMissingIsTrueAndWaysNodeIsMissing() {
			IOsmReader reader = new OsmXmlReader(TestDataReader.OpenOsmDB("osm-way-invalid-ref.osm"), new OsmXmlReaderSettings() { ReadMetadata = true });
			OsmGeometryDatabase target = null;
			target = OsmGeometryDatabase.Load(reader, true);

			Assert.Equal(2, target.Nodes.Count);
			Assert.Equal(0, target.Ways.Count);
		}

		[Fact]
		public void Load_DoesNotThrowExceptionIfIgnoreMissingIsTrueAndRelationMemberIsMissing() {
			IOsmReader reader = new OsmXmlReader(TestDataReader.OpenOsmDB("osm-relation-invalid-ref.osm"), new OsmXmlReaderSettings() { ReadMetadata = true });
			OsmGeometryDatabase target = null;
			target = OsmGeometryDatabase.Load(reader, true);

			Assert.Equal(1, target.Relations.Count);
		}

		#endregion

		#region Save(IOsmWriter) tests

		[Fact]
		public void Save_CallsIOsmWriterWriteForAllEntities() {
			List<IOsmGeometry> written = new List<IOsmGeometry>();
			Mock<IOsmWriter> writerM = new Mock<IOsmWriter>();

			writerM.Setup(w => w.Write(It.IsAny<IOsmGeometry>())).Callback<IOsmGeometry>((e) => written.Add(e));

			OsmGeometryDatabase target = new OsmGeometryDatabase(_data);
			target.Save(writerM.Object);

			Assert.Equal(target.Count, written.Count);
			foreach (var entity in target) {
				Assert.Contains(entity, written);
			}
		}

		#endregion
	}
}
