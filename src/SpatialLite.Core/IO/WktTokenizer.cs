using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace SpatialLite.Core.IO {
    /// <summary>
    /// Implemets tokenizer that splits wkt string to list of tokens.
    /// </summary>
    internal static class WktTokenizer {
        /// <summary>
        /// Splits text to tokens.
        /// </summary>
        /// <param name="text">The text to be splitted.</param>
        /// <returns>The list of parsed tokens of the input string.</returns>
        public static IEnumerable<WktToken> Tokenize(string text) {
            StringReader reader = new StringReader(text);
            return WktTokenizer.Tokenize(reader);
        }

        /// <summary>
        /// Processes text from TextReader and splits it into tokens.
        /// </summary>
        /// <param name="reader">The TextReader to read string from</param>
        /// <returns>The collection parsed tokens of the input string</returns>
        public static IEnumerable<WktToken> Tokenize(TextReader reader) {
            var buffer = new char[255];
            var bufferIndex = 0;

            if (reader.Peek() == -1) {
                yield break;
            }

            int b;
            var lastToken = TokenType.END_OF_DATA;
            while ((b = reader.Read()) != -1) {
                var ch = (char)b;
                var token = WktTokenizer.GetTokenType(ch);

                // tokens COMMA, LEFT_PARENTHESIS and RIGHT_PARENTHESIS can not be grupped together
                if ((token != lastToken && lastToken != TokenType.END_OF_DATA) || lastToken == TokenType.COMMA || lastToken == TokenType.LEFT_PARENTHESIS || lastToken == TokenType.RIGHT_PARENTHESIS) {
                    if (lastToken == TokenType.NUMBER) {
                        yield return new WktToken() { Type = lastToken, NumericValue = double.Parse(buffer.AsSpan(0, bufferIndex), provider: CultureInfo.InvariantCulture) };
                    } else {
                        yield return new WktToken() { Type = lastToken, TextValue = bufferIndex == 0 ? string.Empty : buffer.AsSpan(0, bufferIndex).ToString() };
                    }
                    bufferIndex = 0;
                }
                if (token == TokenType.NUMBER || token == TokenType.STRING) {
                    buffer[bufferIndex++] = ch;
                }
                lastToken = token;
            }

            if (lastToken == TokenType.NUMBER) {
                yield return new WktToken() { Type = lastToken, NumericValue = double.Parse(buffer.AsSpan(0, bufferIndex), provider: CultureInfo.InvariantCulture) };
            } else {
                yield return new WktToken() { Type = lastToken, TextValue = bufferIndex == 0 ? string.Empty : buffer.AsSpan(0, bufferIndex).ToString() };
            }
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
            }

            throw new Exception();
        }
    }
}
