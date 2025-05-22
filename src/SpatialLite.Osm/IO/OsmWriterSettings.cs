using System;

namespace SpatialLite.Osm.IO
{
    /// <summary>
    /// Contains settings that determine behaviour of the OsmWriterWriter.
    /// </summary>
    public class OsmWriterSettings
    {

        bool _writeMetadata = true;
        string _programName = "SpatialLITE";

        /// <summary>
        /// Initializes a new instance of the OsmReaderSettings class with default values.
        /// </summary>
        public OsmWriterSettings()
        {
            this.WriteMetadata = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether OsmWriter should write entity metadata.
        /// </summary>
        public bool WriteMetadata
        {
            get
            {
                return _writeMetadata;
            }
            set
            {
                if (this.IsReadOnly)
                {
                    throw new InvalidOperationException("Cannot change the 'WriteMetadata' property - OsmWriterSettings is read-only.");
                }

                _writeMetadata = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the program that will be save to the output file.
        /// </summary>
        public string ProgramName
        {
            get
            {
                return _programName;
            }
            set
            {
                if (this.IsReadOnly)
                {
                    throw new InvalidOperationException("Cannot change the 'ProgramName' property - OsmWriterSettings is read-only.");
                }

                _programName = value;
            }
        }

        /// <summary>
        /// Gets or sets value indicating whether properties of the current OsmWriterSettings instance can be changed.
        /// </summary>
        protected internal bool IsReadOnly { get; internal set; }
    }
}
