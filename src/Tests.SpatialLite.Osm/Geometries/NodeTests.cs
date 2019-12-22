using Xunit;

using SpatialLite.Core;
using SpatialLite.Core.API;
using SpatialLite.Osm;
using SpatialLite.Osm.Geometries;

namespace Tests.SpatialLite.Osm.Geometry {
    public class NodeTests {

		[Fact]
		public void Constructor_int_CreatesNodeAndInitializeProperties() {
			int id = 11;
			Node target = new Node(id);

			Assert.Equal(SRIDList.WSG84, target.Srid);
			Assert.Equal(Coordinate.Empty, target.Position);
			Assert.Equal(id, target.ID);
			Assert.NotNull(target.Tags);
			Assert.Null(target.Metadata);
		}

		[Fact]
		public void Constructor_int_double_double_CreatesNodeAndInitializeProperties() {
			int id = 11;
			Coordinate coord = new Coordinate(-15.6, 68.7);
			Node target = new Node(id, coord.X, coord.Y);

			Assert.Equal(SRIDList.WSG84, target.Srid);
			Assert.Equal(target.Position.X, coord.X);
			Assert.Equal(target.Position.Y, coord.Y);
			Assert.Equal(id, target.ID);
			Assert.NotNull(target.Tags);
			Assert.Null(target.Metadata);
		}

		[Fact]
		public void Constructor_int_double_double_Tags_CreatesNodeAndInitializesProperties() {
			int id = 11;
			Coordinate coord = new Coordinate(-15.6, 68.7);
			TagsCollection tags = new TagsCollection();

			Node target = new Node(id, coord.X, coord.Y, tags);
			
			Assert.Equal(SRIDList.WSG84, target.Srid);
			Assert.Equal(target.Position.X, coord.X);
			Assert.Equal(target.Position.Y, coord.Y);
			Assert.Equal(id, target.ID);
			Assert.Same(tags, target.Tags);
			Assert.Null(target.Metadata);
		}

		[Fact]
		public void Constructor_int_Coordinate_CreatesNodeAndInitializeProperties() {
			int id = 11;
			Coordinate coord = new Coordinate(-15.6, 68.7);
			Node target = new Node(id, coord);

			Assert.Equal(SRIDList.WSG84, target.Srid);
			Assert.Equal(id, target.ID);
			Assert.Equal(target.Position.X, coord.X);
			Assert.Equal(target.Position.Y, coord.Y);
			Assert.NotNull(target.Tags);
		}

		[Fact]
		public void Constructor_int_Coordinate_Tags_CreatesNodeAndInitializeProperties() {
			int id = 11;
			Coordinate coord = new Coordinate(-15.6, 68.7);
			TagsCollection tags = new TagsCollection();

			Node target = new Node(id, coord, tags);

			Assert.Equal(SRIDList.WSG84, target.Srid);
			Assert.Equal(id, target.ID);
			Assert.Equal(target.Position.X, coord.X);
			Assert.Equal(target.Position.Y, coord.Y);
			Assert.Same(tags, target.Tags);
		}

		[Fact]
		public void Constructor_NodeInfo_CreatesNodeFromNodeInfo() {
			NodeInfo info = new NodeInfo(1, 15.6, 20.4, new TagsCollection(), new EntityMetadata());

			Node target = Node.FromNodeInfo(info);

			Assert.Equal(info.ID, target.ID);
			Assert.Equal(info.Longitude, target.Position.X);
			Assert.Equal(info.Latitude, target.Position.Y);
			Assert.Same(info.Tags, target.Tags);
			Assert.Same(info.Metadata, target.Metadata);
		}

		[Fact]
		public void EntityType_Returns_Node() {
			Node target = new Node(1);

			Assert.Equal(EntityType.Node, target.EntityType);
		}
	}
}
