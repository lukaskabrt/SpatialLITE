using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SpatialLite.Core.API;
using SpatialLite.Core.Geometries;

namespace SpatialLite.Osm.Geometries {
	/// <summary>
	/// Represents OSM way.
	/// </summary>
	public class Way : LineString, IOsmGeometry {
		#region Private Fields

		private WayCoordinateList _coordinatesAdapter;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the Way class with specified ID.
		/// </summary>
		/// <param name="id">The ID of the Way.</param>
		public Way(long id)
			: this(id, new Node[] { }, new TagsCollection()) {
		}

		/// <summary>
		/// Initializes a new instance of the Way class with specified ID and given Nodes.
		/// </summary>
		/// <param name="id">The ID of the Way.</param>
		/// <param name="nodes">The colection of Nodes to add to this Way.</param>
		public Way(long id, IEnumerable<Node> nodes)
			: this(id, nodes, new TagsCollection()) {
		}

		/// <summary>
		/// Initializes a new instance of the Way class with specified ID, Nodes and collection of tags.
		/// </summary>
		/// <param name="id">The ID of the Way.</param>
		/// <param name="nodes">The colection of Nodes to add to this Way.</param> 
		/// <param name="tags">The collection of tags associated with the way.</param>
		public Way(long id, IEnumerable<Node> nodes, TagsCollection tags)
			: base() {
			this.ID = id;
			this.Tags = tags;
			this.Nodes = new List<Node>(nodes);

			_coordinatesAdapter = new WayCoordinateList(this.Nodes);
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
		/// Gets collection of Nodes of this Way.
		/// </summary>
		public List<Node> Nodes { get; private set; }

		/// <summary>
		/// Gets the list of çoordinates of this Way.
		/// </summary>
		public override ICoordinateList Coordinates {
			get {
				return _coordinatesAdapter;
			}
		}

		/// <summary>
		/// Gets type of this entity.
		/// </summary>
		public EntityType EntityType {
			get { return EntityType.Way; }
		}

		#endregion

		#region Public Static Methods

		/// <summary>
		/// Creates a new instance of the Way class based on data from WayInfo object
		/// </summary>
		/// <param name="info">The WayInfo object that contains data about way</param>
		/// <param name="entities">The entities that can be referenced by way</param>
		/// <param name="throwOnMissing">bool value indicating whether references to the missing nodes should cause exception</param>
		/// <returns>The Way object created from WayInfo or null if referenced node is missing</returns>
		public static Way FromWayInfo(WayInfo info, IEntityCollection<IOsmGeometry> entities, bool throwOnMissing) {
			Way result = new Way(info.ID) { Tags = info.Tags, Metadata = info.Metadata };

			result.Nodes.Capacity = info.Nodes.Count;
			foreach (var nodeID in info.Nodes) {
				Node node = entities[nodeID, EntityType.Node] as Node;
				if (node != null) {
					result.Nodes.Add(node);
				}
				else {
					if (throwOnMissing) {
						throw new ArgumentException(string.Format("Referenced Node (ID = {0}) not found in entities collection.", nodeID), "info.ID");
					}

					return null;
				}
			}

			return result;
		}

		#endregion
	}
}
