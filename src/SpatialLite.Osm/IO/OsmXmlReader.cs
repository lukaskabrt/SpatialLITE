using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Sys = System.Xml;

using SpatialLite.Osm.Geometries;

namespace SpatialLite.Osm.IO.Xml {
	/// <summary>
	/// Represents a OsmReader, that can read OSM entities saved in the XML format.
	/// </summary>
	public class OsmXmlReader : IOsmReader {
		#region Private Fields

		private bool _disposed = false;
		private Sys.XmlReader _xmlReader;

		/// <summary>
		/// Underlaying stream to read data from
		/// </summary>
		private Stream _input;
		private bool _ownsInputStream = false;

		/// <summary>
		/// Contains bool value indicating whether XmlReader is inside osm element
		/// </summary>
		private bool _insideOsm;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the OsmXmlReader class that reads data from the specified file.
		/// </summary>
		/// <param name="path">Path to the .osm file.</param>
		/// <param name="settings">The OsmReaderSettings object that determines behaviour of XmlReader.</param>
		public OsmXmlReader(string path, OsmXmlReaderSettings settings) {
			_input = new FileStream(path, FileMode.Open, FileAccess.Read);
			_ownsInputStream = true;

			this.Settings = settings;
			this.Settings.IsReadOnly = true;
			this.InitializeReader();
		}

		/// <summary>
		/// Initializes a new instance of the OsmXmlReader class that reads data from the specified stream.
		/// </summary>
		/// <param name="stream">The stream with osm xml data.</param>
		/// <param name="settings">The OsmReaderSettings object that determines behaviour of XmlReader.</param>
		public OsmXmlReader(Stream stream, OsmXmlReaderSettings settings) {
			_input = stream;

			this.Settings = settings;
			this.Settings.IsReadOnly = true;
			this.InitializeReader();
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets OsmReaderSettings object that contains properties which determine behaviour of the reader.
		/// </summary>
		public OsmXmlReaderSettings Settings { get; private set; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Reads the next OSM entity from the stream.
		/// </summary>
		/// <returns>IEntityInfo object with information about entity read from the stream, or null if no more entities are available.</returns>
		public IEntityInfo Read() {
			IEntityInfo result = null;

			while (_xmlReader.NodeType != Sys.XmlNodeType.EndElement && result == null) {
				switch (_xmlReader.Name) {
					case "node":
						result = this.ReadNode();
						break;
					case "way":
						result = this.ReadWay();
						break;
					case "relation":
						result = this.ReadRelation();
						break;
					default:
						_xmlReader.Read();
						break;
				}
			}

			return result;
		}

		/// <summary>
		/// Releases all resources used by the OsmXmlReader.
		/// </summary>
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Reads Node from the currnt element in the _xmlReader.
		/// </summary>
		/// <returns>Information about parsed node.</returns>
		private NodeInfo ReadNode() {
			// id
			string attId = _xmlReader.GetAttribute("id");
			if (attId == null) {
				throw new Sys.XmlException("Attribute 'id' is missing.");
			}

			int nodeId = int.Parse(attId, System.Globalization.CultureInfo.InvariantCulture);

			// latitude
			string attLat = _xmlReader.GetAttribute("lat");
			if (attLat == null) {
				throw new Sys.XmlException("Attribute 'lat' is missing.");
			}

			double nodeLat = double.Parse(attLat, System.Globalization.CultureInfo.InvariantCulture);

			// longitude
			string attLon = _xmlReader.GetAttribute("lon");
			if (attLon == null) {
				throw new Sys.XmlException("Attribute 'lon'is missing.");
			}

			double nodeLon = double.Parse(attLon, System.Globalization.CultureInfo.InvariantCulture);

			EntityMetadata additionalInfo = null;
			if (this.Settings.ReadMetadata) {
				additionalInfo = this.ReadMetadata();
			}

			NodeInfo result = new NodeInfo(nodeId, nodeLat, nodeLon, new TagsCollection(), additionalInfo);

			if (_xmlReader.IsEmptyElement == false) {
				_xmlReader.Read();

				while (_xmlReader.NodeType != Sys.XmlNodeType.EndElement) {
					if (_xmlReader.NodeType == Sys.XmlNodeType.Element && _xmlReader.Name == "tag") {
						result.Tags.Add(this.ReadTag());
					}
					else {
						_xmlReader.Skip();
					}
				}
			}

			_xmlReader.Skip();
			return result;
		}

		/// <summary>
		/// Reads Way from the currnt element in the _xmlReader.
		/// </summary>
		/// <returns>Information about parsed way.</returns>
		private WayInfo ReadWay() {
			string attId = _xmlReader.GetAttribute("id");
			if (attId == null) {
				throw new Sys.XmlException("Attribute 'id' is missing.");
			}

			int wayId = int.Parse(attId, System.Globalization.CultureInfo.InvariantCulture);

			EntityMetadata additionalInfo = null;
			if (this.Settings.ReadMetadata) {
				additionalInfo = this.ReadMetadata();
			}

			WayInfo way = new WayInfo(wayId, new TagsCollection(), new List<int>(), additionalInfo);

			if (_xmlReader.IsEmptyElement == false) {
				_xmlReader.Read();

				while (_xmlReader.NodeType != Sys.XmlNodeType.EndElement) {
					switch (_xmlReader.NodeType) {
						case Sys.XmlNodeType.Element:
							switch (_xmlReader.Name) {
								case "nd":
									way.Nodes.Add(this.ReadWayNd());
									continue;
								case "tag":
									way.Tags.Add(this.ReadTag());
									continue;
								default:
									_xmlReader.Skip();
									continue;
							}

						default:
							_xmlReader.Skip();
							break;
					}
				}
			}

			_xmlReader.Skip();
			return way;
		}

		/// <summary>
		/// Reads Node reference from the currnt Way element in the _xmlReader.
		/// </summary>
		/// <returns>Reference to the node.</returns>
		private int ReadWayNd() {
			string attRef = _xmlReader.GetAttribute("ref");
			if (string.IsNullOrEmpty(attRef)) {
				throw new Sys.XmlException("Attribute 'ref' is missing.");
			}

			int nodeId = int.Parse(attRef, System.Globalization.CultureInfo.InvariantCulture);

			_xmlReader.Skip();

			return nodeId;
		}

		/// <summary>
		/// Reads Relation from the currnt element in the _xmlReader.
		/// </summary>
		/// <returns>Information about parsed relation.</returns>
		private RelationInfo ReadRelation() {
			string attId = _xmlReader.GetAttribute("id");
			if (attId == null) {
				throw new Sys.XmlException("Attribute 'id' is missing.");
			}

			int relationId = int.Parse(attId, System.Globalization.CultureInfo.InvariantCulture);

			EntityMetadata additionalInfo = null;
			if (this.Settings.ReadMetadata) {
				additionalInfo = this.ReadMetadata();
			}

			RelationInfo relation = new RelationInfo(relationId, new TagsCollection(), new List<RelationMemberInfo>(), additionalInfo);

			if (false == _xmlReader.IsEmptyElement) {
				_xmlReader.Read();

				while (_xmlReader.NodeType != Sys.XmlNodeType.EndElement) {
					switch (_xmlReader.NodeType) {
						case Sys.XmlNodeType.Element:
							switch (_xmlReader.Name) {
								case "member":
									relation.Members.Add(this.ReadRelationMember());
									continue;
								case "tag":
									relation.Tags.Add(this.ReadTag());
									continue;
								default:
									_xmlReader.Skip();
									continue;
							}

						default:
							_xmlReader.Skip();
							break;
					}
				}
			}

			_xmlReader.Skip();

			return relation;
		}

		/// <summary>
		/// Reads RelationMember from the current Relation in the _xmlReader.
		/// </summary>
		/// <returns>Information about parsed relation member</returns>
		private RelationMemberInfo ReadRelationMember() {
			string attType = _xmlReader.GetAttribute("type");
			if (string.IsNullOrEmpty(attType)) {
				throw new Sys.XmlException("Attribute 'type' is missing.");
			}

			string attRef = _xmlReader.GetAttribute("ref");
			if (string.IsNullOrEmpty(attRef)) {
				throw new Sys.XmlException("Attribute 'ref' is missing.");
			}

			int refId = int.Parse(attRef, System.Globalization.CultureInfo.InvariantCulture);

			string attRole = _xmlReader.GetAttribute("role");

			EntityType memberType;
			switch (attType) {
				case "node":
					memberType = EntityType.Node;
					break;
				case "way":
					memberType = EntityType.Way;
					break;
				case "relation":
					memberType = EntityType.Relation;
					break;
				default:
					throw new Sys.XmlException("Unknown relation member type");
			}

			_xmlReader.Skip();

			return new RelationMemberInfo() { MemberType = memberType, Reference = refId, Role = attRole };
		}

		/// <summary>
		/// Reads Tag from the current element in the _xmlReader.
		/// </summary>
		/// <returns>the parsed Tag.</returns>
		private Tag ReadTag() {
			string attK = _xmlReader.GetAttribute("k");
			if (attK == null) {
				throw new Sys.XmlException("Attribute 'k' is missing.");
			}

			string attV = _xmlReader.GetAttribute("v");
			if (attV == null) {
				throw new Sys.XmlException("Attribute 'v' is missing.");
			}

			_xmlReader.Skip();

			return new Tag(attK, attV);
		}

		/// <summary>
		/// Reads metadata of the osm entity
		/// </summary>
		/// <returns>Metadata of the entity read from the reader.</returns>
		private EntityMetadata ReadMetadata() {
			EntityMetadata result = new EntityMetadata();

			// version
			string attVersion = _xmlReader.GetAttribute("version");
			if (attVersion == null) {
				if (this.Settings.StrictMode) {
					throw new Sys.XmlException("Attribute 'version' is missing.");
				}
			}
			else {
				result.Version = int.Parse(attVersion, System.Globalization.CultureInfo.InvariantCulture);
			}

			// changeset
			string attChangeset = _xmlReader.GetAttribute("changeset");
			if (attChangeset == null) {
				if (this.Settings.StrictMode) {
					throw new Sys.XmlException("Attribute 'changeset' is missing.");
				}
			}
			else {
				result.Changeset = int.Parse(attChangeset, System.Globalization.CultureInfo.InvariantCulture);
			}

			// uid
			string attUid = _xmlReader.GetAttribute("uid");
			if (attUid == null) {
				if (this.Settings.StrictMode) {
					throw new Sys.XmlException("Attribute 'uid' is missing.");
				}
			}
			else {
				result.Uid = int.Parse(attUid, System.Globalization.CultureInfo.InvariantCulture);
			}

			// user		
			string attrUser = _xmlReader.GetAttribute("user");
			if (attrUser == null) {
				if (this.Settings.StrictMode) {
					throw new Sys.XmlException("Attribute 'user' is missing.");
				}
			}
			else {
				result.User = attrUser;
			}

			// visible
			string attVisible = _xmlReader.GetAttribute("visible");
			if (attVisible == null && this.Settings.StrictMode) {
				result.Visible = true;
			}
			else {
				result.Visible = bool.Parse(attVisible);
			}

			// timestamp
			string attTimestamp = _xmlReader.GetAttribute("timestamp");
			if (attTimestamp == null) {
				if (this.Settings.StrictMode) {
					throw new Sys.XmlException("Attribute 'timestamp' is missing.");
				}
			}
			else {
				result.Timestamp = DateTime.ParseExact(attTimestamp, "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", System.Globalization.CultureInfo.InvariantCulture);
			}

			return result;
		}

		/// <summary>
		/// Initializes internal properties
		/// </summary>
		private void InitializeReader() {
			Sys.XmlReaderSettings xmlReaderSettings = new Sys.XmlReaderSettings();
			xmlReaderSettings.IgnoreComments = true;
			xmlReaderSettings.IgnoreProcessingInstructions = true;
			xmlReaderSettings.IgnoreWhitespace = true;

			_xmlReader = Sys.XmlTextReader.Create(_input, xmlReaderSettings);

			_xmlReader.Read();
			while (_xmlReader.EOF == false && _insideOsm == false) {
				switch (_xmlReader.NodeType) {
					case Sys.XmlNodeType.Element:
						if (_xmlReader.Name != "osm") {
							throw new Sys.XmlException("Invalid xml root element. Expected <osm>.");
						}

						_insideOsm = true;
						break;

					default:
						_xmlReader.Read();
						break;
				}
			}
		}

		/// <summary>
		/// Releases the unmanaged resources used by the ComponentLibrary and optionally releases the managed resources.
		/// </summary>
		/// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
		private void Dispose(bool disposing) {
			if (!this._disposed) {
				if (disposing) {
					_xmlReader.Close();
					_input.Close();

					if (_ownsInputStream) {
						_input.Dispose();
					}
				}

				_disposed = true;
			}
		}
		#endregion
	}
}
