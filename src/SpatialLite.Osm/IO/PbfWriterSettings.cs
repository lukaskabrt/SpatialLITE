using System;

namespace SpatialLite.Osm.IO;

/// <summary>
///  Contains settings that determine behaviour of the PbfWriter.
/// </summary>
public class PbfWriterSettings : OsmWriterSettings
{

    private bool _useDenseFormat;
    private CompressionMode _compression;

    /// <summary>
    /// Initializes a new instance of the PbfWriterSettings class with default values.
    /// </summary>
    public PbfWriterSettings()
        : base()
    {
        this.UseDenseFormat = true;
        this.Compression = CompressionMode.ZlibDeflate;
    }

    /// <summary>
    /// Gets or sets a value indicating whether PbfWriter should use dense format for serializing nodes.
    /// </summary>
    public bool UseDenseFormat
    {
        get
        {
            return _useDenseFormat;
        }
        set
        {
            if (this.IsReadOnly)
            {
                throw new InvalidOperationException("Cannot change the 'UseDenseFromat' property - PbfReaderSettings is read-only");
            }

            _useDenseFormat = value;
        }
    }

    /// <summary>
    /// Gets or sets a compression to be used by PbfWriter.
    /// </summary>
    public CompressionMode Compression
    {
        get
        {
            return _compression;
        }
        set
        {
            if (this.IsReadOnly)
            {
                throw new InvalidOperationException("Cannot change the 'Compression' property - PbfReaderSettings is read-only");
            }

            _compression = value;
        }
    }
}
