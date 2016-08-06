using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xunit;

using SpatialLite.Osm.Geometries;
using SpatialLite.Osm;

namespace Tests.SpatialLite.Osm {
	public class WayInfoTests {
		#region Constructor(ID, Tags, Nodes, EntityMetadata) tests

		[Fact]
		public void Constructor_PropertiesWithoutEntityDetails_SetsProperties() {
			int id = 45;
			TagsCollection tags = new TagsCollection();
			List<long> nodes = new List<long>();

			WayInfo target = new WayInfo(id, tags, nodes);

			Assert.Equal(EntityType.Way, target.EntityType);
			Assert.Equal(id, target.ID);
			Assert.Same(tags, target.Tags);
			Assert.Same(nodes, target.Nodes);
			Assert.Null(target.Metadata);
		}

		[Fact]
		public void Constructor_Properties_SetsProperties() {
			int id = 45;
			TagsCollection tags = new TagsCollection();
			List<long> nodes = new List<long>();
			EntityMetadata details = new EntityMetadata();

			WayInfo target = new WayInfo(id, tags, nodes, details);

			Assert.Equal(EntityType.Way, target.EntityType);
			Assert.Equal(id, target.ID);
			Assert.Same(tags, target.Tags);
			Assert.Same(nodes, target.Nodes);
			Assert.Same(details, target.Metadata);
		}

		#endregion

		#region Constructor(Way) tests

		[Fact]
		public void Constructor_Way_SetsProperties() {
			Way way = new Way(10, new Node[0], new TagsCollection()) { Metadata = new EntityMetadata() };

			WayInfo target = new WayInfo(way);

			Assert.Equal(way.ID, target.ID);
			Assert.Same(way.Tags, target.Tags);
			Assert.Same(way.Metadata, target.Metadata);
		}

		[Fact]
		public void Constructor_Way_SetsNodesReferences() {
			Way way = new Way(10, new Node[] {new Node(1), new Node(2), new Node(3)}, new TagsCollection()) { Metadata = new EntityMetadata() };

			WayInfo target = new WayInfo(way);

			Assert.Equal(way.Nodes.Count, target.Nodes.Count);
			for (int i = 0; i < way.Nodes.Count; i++) {
				Assert.Equal(way.Nodes[i].ID, target.Nodes[i]);
			}
		}

		[Fact]
		public void Constructor_Way_ThrowsExceptionIfWayIsNull() {
			Assert.Throws<ArgumentNullException>(() => new WayInfo(null));
		}

		#endregion
	}
}
