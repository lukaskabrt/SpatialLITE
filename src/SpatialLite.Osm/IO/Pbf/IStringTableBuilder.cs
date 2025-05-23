namespace SpatialLite.Osm.IO.Pbf;

/// <summary>
/// Defines function for object that can be used to build StringTables.
/// </summary>
internal interface IStringTableBuilder
{
    /// <summary>
    /// Gets index of the string in StringTable being constructed.
    /// </summary>
    /// <param name="str">The string to locate in StringTable.</param>
    /// <returns>Index of the string in the StringTable.</returns>
    uint GetStringIndex(string str);

    /// <summary>
    /// Creates a StringTable object with data from StringTable object.
    /// </summary>
    /// <returns>The StringTable object with data from the IStringBuilder.</returns>
    StringTable BuildStringTable();
}
