using System;
using System.Collections.Generic;

using Xunit;
using Moq;

using SpatialLite.Core;
using SpatialLite.Core.Geometries;
using SpatialLite.Osm.Geometries;
using SpatialLite.Osm;

namespace Tests.SpatialLite.Osm.Geometries {
    public class WayTests {
        List<Node> _nodes = new List<Node>(new Node[] { 
			new Node(1, 1.1, 2.2),
			new Node(2, 10.1, -20.2),
			new Node(3, -30.1, 40.2) });

        WayInfo _wayEmptyInfo = new WayInfo(10, new TagsCollection(), new List<long>(), new EntityMetadata());
        WayInfo _wayInfo = new WayInfo(10, new TagsCollection(), new long[] { 1, 2, 3 }, new EntityMetadata());

        IEntityCollection<IOsmGeometry> _nodesEntityCollection;

        public WayTests() {
            Mock<IEntityCollection<IOsmGeometry>> _nodesCollectionM = new Mock<IEntityCollection<IOsmGeometry>>();
            _nodesCollectionM.SetupGet(c => c[1, EntityType.Node]).Returns(_nodes[0]);
            _nodesCollectionM.SetupGet(c => c[2, EntityType.Node]).Returns(_nodes[1]);
            _nodesCollectionM.SetupGet(c => c[3, EntityType.Node]).Returns(_nodes[2]);
            _nodesEntityCollection = _nodesCollectionM.Object;
        }

        #region Constructor(ID) tests

        [Fact]
        public void Constructor_ID_CreatesNewEmptyWayAndInitializesProperties() {
            int id = 11;

            Way target = new Way(id);

            Assert.Equal(id, target.ID);
            Assert.Equal(SRIDList.WSG84, target.Srid);
            Assert.Empty(target.Nodes);
            Assert.Empty(target.Tags);
            Assert.Null(target.Metadata);
        }

        #endregion

        #region Constructor(ID, Nodes) tests

        [Fact]
        public void Constructor_ID_Nodes_CreatesWayAddsNodesAndInitializesProperties() {
            int id = 11;
            Way target = new Way(id, _nodes);

            Assert.Equal(id, target.ID);
            Assert.Equal(SRIDList.WSG84, target.Srid);
            Assert.Equal(_nodes.Count, target.Nodes.Count);
            for (int i = 0; i < _nodes.Count; i++) {
                Assert.Same(_nodes[i], target.Nodes[i]);
            }

            Assert.Empty(target.Tags);
            Assert.Null(target.Metadata);
        }
        #endregion

        #region Constructor(ID, Nodes, Tags) tests

        [Fact]
        public void Constructor_ID_Nodes_Tags_CreatesWayAddsNodesAndInitializesProperties() {
            int id = 11;
            TagsCollection tags = new TagsCollection();

            Way target = new Way(id, _nodes, tags);

            Assert.Equal(id, target.ID);
            Assert.Equal(SRIDList.WSG84, target.Srid);
            Assert.Equal(_nodes.Count, target.Nodes.Count);
            for (int i = 0; i < _nodes.Count; i++) {
                Assert.Same(_nodes[i], target.Nodes[i]);
            }

            Assert.Same(tags, target.Tags);
            Assert.Null(target.Metadata);
        }

        #endregion

        #region FromWayInfo(WayInfo, IEntityCollection<IOsmGeometry>, bool) tests

        [Fact]
        public void FromWayInfo_SetsProperties() {
            Way target = Way.FromWayInfo(_wayEmptyInfo, _nodesEntityCollection, true);

            Assert.Equal(_wayEmptyInfo.ID, target.ID);
            Assert.Same(_wayEmptyInfo.Tags, target.Tags);
            Assert.Same(_wayEmptyInfo.Metadata, target.Metadata);
            Assert.Empty(target.Nodes);
        }

        [Fact]
        public void FromWayInfo_SetsNodes() {
            Way target = Way.FromWayInfo(_wayInfo, _nodesEntityCollection, true);

            Assert.Equal(_wayInfo.Nodes.Count, target.Nodes.Count);
            for (int i = 0; i < _wayInfo.Nodes.Count; i++) {
                Assert.Equal(_wayInfo.Nodes[i], target.Nodes[i].ID);
            }
        }

        [Fact]
        public void FromWayInfo_ThrowsArgumentExceptionIfReferencedNodeIsNotInCollectionAndMissingNodesAsErrorsIsTrue() {
            _wayInfo.Nodes[0] = 10000;

            Assert.Throws<ArgumentException>(() => Way.FromWayInfo(_wayInfo, _nodesEntityCollection, true));
        }

        [Fact]
        public void FromWayInfo_ReturnsNullIfReferencedNodeIsNotInCollectionAndMissingNodesAsErrorsIsFalse() {
            _wayInfo.Nodes[0] = 10000;

            Assert.Null(Way.FromWayInfo(_wayInfo, _nodesEntityCollection, false));
        }

		[Fact]
		public void WhenWayIsInitializedFromWayInfo_CoordinatesReturnsNodesCoordinates() {
			Way target = Way.FromWayInfo(_wayInfo, _nodesEntityCollection, true);

			Assert.Equal(_wayInfo.Nodes.Count, target.Nodes.Count);

			Assert.Equal(_nodes[0].Position, target.Coordinates[0]);
			Assert.Equal(_nodes[0].Position, target.Coordinates[0]);
			Assert.Equal(_nodes[0].Position, target.Coordinates[0]);
		}

        #endregion

        #region Coordinates property tests

        [Fact]
        public void Coordinates_GetsPositionOfNodes() {
            int id = 11;
            Way target = new Way(id, _nodes);

            Assert.Equal(_nodes.Count, target.Coordinates.Count);
            for (int i = 0; i < _nodes.Count; i++) {
                Assert.Equal(_nodes[i].Position, target.Coordinates[i]);
            }
        }

        [Fact]
        public void Coordinates_GetsPositionOfNodesIfWayCastedToLineString() {
            int id = 11;
            Way way = new Way(id, _nodes);
            LineString target = (LineString)way;

            Assert.Equal(_nodes.Count, target.Coordinates.Count);
            for (int i = 0; i < _nodes.Count; i++) {
                Assert.Equal(_nodes[i].Position, target.Coordinates[i]);
            }
        }

        #endregion

        #region EntityType property tests

        [Fact]
        public void EntityType_Returns_Way() {
            Way target = new Way(10);

            Assert.Equal(EntityType.Way, target.EntityType);
        }

        #endregion
    }
}
