namespace SpatialLite.Core.IO;

/// <summary>
/// Represents token of the wkt string.
/// </summary>
internal struct WktToken
{
    /// <summary>
    /// Special token that represents end of available data.
    /// </summary>
    public static WktToken EndOfDataToken = new() { Type = TokenType.END_OF_DATA, Value = string.Empty };

    /// <summary>
    /// The type of the tooken.
    /// </summary>
    public TokenType Type;

    /// <summary>
    /// The string value of the token.
    /// </summary>
    public string Value;
}
