namespace SpatialLite.Core.IO
{
    /// <summary>
    /// Represents type of the WKT token.
    /// </summary>
    internal enum TokenType
    {
        /// <summary>
        /// String token - it consists of a-zA-Z characters
        /// </summary>
        STRING,

        /// <summary>
        /// WHITESPACE token (' ', '\t', '\r', '\n').
        /// </summary>
        WHITESPACE,

        /// <summary>
        /// '(' token
        /// </summary>
        LEFT_PARENTHESIS,

        /// <summary>
        /// ')' token
        /// </summary>
        RIGHT_PARENTHESIS,

        /// <summary>
        /// ',' token
        /// </summary>
        COMMA,

        /// <summary>
        /// Number token - it consists of '-','0'-'9' and '.' characters
        /// </summary>
        NUMBER,

        /// <summary>
        /// Token that represents end of available data
        /// </summary>
        END_OF_DATA
    }
}
