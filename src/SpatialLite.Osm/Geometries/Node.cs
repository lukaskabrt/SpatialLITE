using SpatialLite.Core.API;
using SpatialLite.Core.Geometries;

namespace SpatialLite.Osm.Geometries {
    /// <summary>
    /// Represents OSM node.
    /// </summary>
    public class Node : Point, IOsmGeometry {
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the Node class with specified ID.
		/// </summary>
		/// <param name="id">The  ID of the node.</param>
		public Node(long id)
			: this(id, Coordinate.Empty, new TagsCollection()) {
		}

		/// <summary>
		/// Initializes a new instance of the Node class with specified ID, Longitude and Latitude.
		/// </summary>
		/// <param name="id">The ID of the node.</param>
		/// <param name="longitude">The longitude of the Node.</param>
		/// <param name="latitude">The latitude of the Node.</param>
		public Node(long id, double longitude, double latitude)
			: this(id, new Coordinate(longitude, latitude), new TagsCollection()) {
		}

		/// <summary>
		/// Initializes a new instance of the Node class with specified ID, Longitude, Latitude and Tags.
		/// </summary>
		/// <param name="id">The ID of the node.</param>
		/// <param name="longitude">The longitude of the Node.</param>
		/// <param name="latitude">The latitude of the Node.</param>
		/// <param name="tags">The collection of tags associated with the Node.</param>
		public Node(long id, double longitude, double latitude, TagsCollection tags)
			: this(id, new Coordinate(longitude, latitude), tags) {
		}

		/// <summary>
		/// Initializes a new instance of the Node class with specified ID and Position.
		/// </summary>
		/// <param name="id">The ID of the node.</param>
		/// <param name="position">The position of the Node.</param>
		public Node(long id, Coordinate position)
			: this(id, position, new TagsCollection()) {
		}

		/// <summary>
		/// Initializes a new instance of the Node class with specified ID, Position and Tags.
		/// </summary>
		/// <param name="id">The ID of the node.</param>
		/// <param name="position">The position of the Node.</param>
		/// <param name="tags">The collection of tags associated with the Node.</param> 
		public Node(long id, Coordinate position, TagsCollection tags)
			: base(position) {
			this.ID = id;
			this.Tags = tags;
		}

		#endregion

		#region Public Static Methods

		/// <summary>
		/// Creates a new instance of the Node class with data from NodeInfo object
		/// </summary>
		/// <param name="info">NodeInfo object that contains data about node</param>
		/// <returns>Node object with data from specific NodeInfo object</returns>
		public static Node FromNodeInfo(NodeInfo info) {
			return new Node(info.ID, info.Longitude, info.Latitude, info.Tags) { Metadata = info.Metadata };
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets ID of the Node.
		/// </summary>
		public long ID { get; set; }

		/// <summary>
		/// Gets or sets the collection of tags associated with the Node.
		/// </summary>
		public TagsCollection Tags { get; set; }

		/// <summary>
		/// Gets or sets metadata of the Node.
		/// </summary>
		public EntityMetadata Metadata { get; set; }


		/// <summary>
		/// Gets type of this entity.
		/// </summary>
		public EntityType EntityType {
			get { return EntityType.Node; }
		}

		#endregion
	}
}
