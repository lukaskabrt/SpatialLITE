using System;
using System.Buffers;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace SpatialLite.Core.IO {
    /// <summary>
    /// Implemets tokenizer that splits wkt string to list of tokens.
    /// </summary>
    internal class WktTokenizer {
        private static char[] pointText = new char[] { 'P', 'O', 'I', 'N', 'T' };
        private static char[] emptyText = new char[] { 'E', 'M', 'P', 'T', 'Y' };
        private static char[] polygonText = new char[] { 'P', 'O', 'L', 'Y', 'G', 'O', 'N' };
        private static char[] linestringText = new char[] { 'L', 'I', 'N', 'E', 'S', 'T', 'R', 'I', 'N', 'G' };
        private static char[] multipointText = new char[] { 'M', 'U', 'L', 'T', 'I', 'P', 'O', 'I', 'N', 'T' };
        private static char[] multipolygonText = new char[] { 'M', 'U', 'L', 'T', 'I', 'P', 'O', 'L', 'Y', 'G', 'O', 'N' };
        private static char[] multilinestringText = new char[] { 'M', 'U', 'L', 'T', 'I', 'L', 'I', 'N', 'E', 'S', 'T', 'R', 'I', 'N', 'G' };
        private static char[] geometrycollectionText = new char[] { 'G', 'E', 'O', 'M', 'E', 'T', 'R', 'Y', 'C', 'O', 'L', 'L', 'E', 'C', 'T', 'I', 'O', 'N' };

        private TextReader _reader;

        private char _current;
        private TokenType _currentToken;

        /// <summary>
        /// Initializes a new instance of the tokenizer, that process a string.
        /// </summary>
        /// <param name="text">The text to be splitted.</param>
        public WktTokenizer(string text) : this(new StringReader(text)) {
        }

        /// <summary>
        /// Initializes a new instance of the tokenizer, that process test from a TextReader.
        /// </summary>
        /// <param name="reader">The TextReader to read string from</param>
        public WktTokenizer(TextReader reader) {
            _reader = reader;
            _current = (char)_reader.Read();
            _currentToken = _current == -1 ? TokenType.END_OF_DATA : GetTokenType(_current);
        }

        public WktToken GetNextToken() {
            if (_currentToken == TokenType.NUMBER || _currentToken == TokenType.STRING) {
                var buffer = ArrayPool<char>.Shared.Rent(255);
                var bufferIndex = 0;
                buffer[bufferIndex++] = char.ToUpperInvariant(_current);

                var token = TokenType.END_OF_DATA;
                char ch;
                do {
                    ch = (char)_reader.Read();
                    token = GetTokenType(ch);

                    if (token == _currentToken) {
                        buffer[bufferIndex++] = char.ToUpperInvariant(ch);
                    }
                } while (token != TokenType.END_OF_DATA && _currentToken == token);

                var result = CreateToken(_currentToken, buffer.AsSpan(0, bufferIndex));
                ArrayPool<char>.Shared.Return(buffer);

                _current = ch;
                _currentToken = token;

                return result;
            } else if (_currentToken == TokenType.WHITESPACE) {
                var token = TokenType.END_OF_DATA;
                char ch;
                do {
                    ch = (char)_reader.Read();
                    token = GetTokenType(ch);
                } while (token == TokenType.WHITESPACE);

                _current = ch;
                _currentToken = token;

                return new WktToken() { Type = TokenType.WHITESPACE };
            } else if (_currentToken == TokenType.COMMA || _currentToken == TokenType.LEFT_PARENTHESIS || _currentToken == TokenType.RIGHT_PARENTHESIS) {
                var result = new WktToken() { Type = _currentToken };

                _current = (char)_reader.Read();
                _currentToken = GetTokenType(_current);

                return result;
            } else {
                return WktToken.EndOfDataToken;
            }
        }

        private static WktToken CreateToken(TokenType tokenType, Span<char> buffer) {
            switch (tokenType) {
                case TokenType.NUMBER:
                    return new WktToken() { Type = TokenType.NUMBER, NumericValue = double.Parse(buffer, provider: CultureInfo.InvariantCulture) };
                case TokenType.STRING:
                    return new WktToken() { Type = TokenType.STRING, TextValue = GetTextValue(buffer) };
            }

            throw new ArgumentOutOfRangeException("tokenType");
        }

        private static string GetTextValue(Span<char> buffer) {
            switch (buffer.Length) {
                case 1:
                    if (buffer[0] == 'Z') return "Z";
                    if (buffer[0] == 'M') return "M";
                    break;
                case 2:
                    if (buffer[0] == 'Z' && buffer[1] == 'M') return "ZM";
                    break;
                case 5:
                    if (buffer.SequenceEqual(pointText)) return "POINT";
                    if (buffer.SequenceEqual(emptyText)) return "EMPTY";
                    break;
                case 7:
                    if (buffer.SequenceEqual(polygonText)) return "POLYGON";
                    break;
                case 10:
                    if (buffer.SequenceEqual(linestringText)) return "LINESTRING";
                    if (buffer.SequenceEqual(multipointText)) return "MULTIPOINT";
                    break;
                case 12:
                    if (buffer.SequenceEqual(multipolygonText)) return "MULTIPOLYGON";
                    break;
                case 15:
                    if (buffer.SequenceEqual(multilinestringText)) return "MULTILINESTRING";
                    break;
                case 18:
                    if (buffer.SequenceEqual(geometrycollectionText)) return "GEOMETRYCOLLECTION";
                    break;
            }

            return buffer.ToString();
        }

        /// <summary>
        /// Gets type of the token.
        /// </summary>
        /// <param name="ch">Character to get type.</param>
        /// <returns>the type of token for ch.</returns>
        /// <remarks>In WKT can characters be divided into token types without context - every character is member of thne one token type only.</remarks>
        private static TokenType GetTokenType(char ch) {
            if ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z')) {
                return TokenType.STRING;
            } else if (ch == ' ' || ch == '\t' || ch == '\r' || ch == '\n') {
                return TokenType.WHITESPACE;
            } else if (ch == ',') {
                return TokenType.COMMA;
            } else if (ch == '(') {
                return TokenType.LEFT_PARENTHESIS;
            } else if (ch == ')') {
                return TokenType.RIGHT_PARENTHESIS;
            } else if (ch == '-' || ch == '.' || (ch >= '0' && ch <= '9')) {
                return TokenType.NUMBER;
            } else if (ch == '\uffff') {
                return TokenType.END_OF_DATA;
            }

            throw new Exception();
        }
    }
}
