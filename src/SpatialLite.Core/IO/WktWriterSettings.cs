namespace SpatialLite.Core.IO;

/// <summary>
/// Specifies settings that determine behaviour of the WkbWriter.
/// </summary>
public class WktWriterSettings
{

    /// <summary>
    /// Initializes a new instance of the WkbWriterSettings class with default values.
    /// </summary>
    public WktWriterSettings()
        : base()
    {
    }

    /// <summary>
    /// Gets a value indicating whether properties of this OsmWriterSettings instance can be changed.
    /// </summary>
    protected internal bool IsReadOnly { get; internal set; }
}
