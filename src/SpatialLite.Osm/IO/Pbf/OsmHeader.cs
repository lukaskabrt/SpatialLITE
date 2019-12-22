using System.Collections.Generic;

using ProtoBuf;

namespace SpatialLite.Osm.IO.Pbf {
    /// <summary>
    /// Represents header message of the PBF file.
    /// </summary>
    [ProtoContract(Name = "HeaderBlock")]
	internal class OsmHeader {
		private IList<string> _optionalFeatures = new List<string>();
		private IList<string> _requiredFeatures = new List<string>();

		/// <summary>
		/// Gets or sets bounding box of the data in the file
		/// </summary>
		[ProtoMember(1, Name = "bbox", IsRequired = false)]
		public HeaderBBox BBox { get; set; }

		/// <summary>
		/// Gets or sets collection of optional features that parser could take advantege of
		/// </summary>
		[ProtoMember(5, Name = "optional_features")]
		public IList<string> OptionalFeatures {
			get { return _optionalFeatures; }
			set { _optionalFeatures = value; }
		}

		/// <summary>
		/// Gets or sets collection of required features that parser must suppor to process the file
		/// </summary>
		[ProtoMember(4, Name = "required_features")]
		public IList<string> RequiredFeatures {
			get { return _requiredFeatures; }
			set { _requiredFeatures = value; }
		}

		/// <summary>
		/// Gets or sets source of the data
		/// </summary>
		[ProtoMember(0x11, Name = "source", IsRequired = false)]
		public string Source { get; set; }

		/// <summary>
		/// Gets or sets identification of writing program
		/// </summary>
		[ProtoMember(0x10, Name = "writingprogram", IsRequired = false)]
		public string WritingProgram { get; set; }
	}
}
