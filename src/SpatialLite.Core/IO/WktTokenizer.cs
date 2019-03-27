﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace SpatialLite.Core.IO {
    /// <summary>
    /// Implemets tokenizer that splits wkt string to list of tokens.
    /// </summary>
    internal static class WktTokenizer {
        private static char[] pointText = new char[] { 'P', 'O', 'I', 'N', 'T' };
        private static char[] emptyText = new char[] { 'E', 'M', 'P', 'T', 'Y' };
        private static char[] polygonText = new char[] { 'P', 'O', 'L', 'Y', 'G', 'O', 'N' };
        private static char[] linestringText = new char[] { 'L', 'I', 'N', 'E', 'S', 'T', 'R', 'I', 'N', 'G' };
        private static char[] multipointText = new char[] { 'M', 'U', 'L', 'T', 'I', 'P', 'O', 'I', 'N', 'T' };
        private static char[] multipolygonText = new char[] { 'M', 'U', 'L', 'T', 'I', 'P', 'O', 'L', 'Y', 'G', 'O', 'N' };
        private static char[] multilinestringText = new char[] { 'M', 'U', 'L', 'T', 'I', 'L', 'I', 'N', 'E', 'S', 'T', 'R', 'I', 'N', 'G' };
        private static char[] geometrycollectionText = new char[] { 'G', 'E', 'O', 'M', 'E', 'T', 'R', 'Y', 'C', 'O', 'L', 'L', 'E', 'C', 'T', 'I', 'O', 'N' };

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
                    yield return CreateToken();
                    bufferIndex = 0;
                }
                if (token == TokenType.NUMBER || token == TokenType.STRING) {
                    buffer[bufferIndex++] = char.ToUpperInvariant(ch);
                }
                lastToken = token;
            }

            yield return CreateToken();

            WktToken CreateToken() {
                switch (lastToken) {
                    case TokenType.NUMBER:
                        return new WktToken() { Type = lastToken, NumericValue = double.Parse(buffer.AsSpan(0, bufferIndex), provider: CultureInfo.InvariantCulture) };
                    case TokenType.STRING:
                        return new WktToken() { Type = lastToken, TextValue = GetTextValue() };
                    default:
                        return new WktToken() { Type = lastToken };
                }
            }

            string GetTextValue() {
                var textBuffer = buffer.AsSpan(0, bufferIndex);

                switch(textBuffer.Length) {
                    case 1:
                        if (textBuffer[0] == 'Z') return "Z";
                        if (textBuffer[0] == 'M') return "M";
                        break;
                    case 2:
                        if (textBuffer[0] == 'Z' && textBuffer[1] == 'M') return "ZM";
                        break;
                    case 5:
                        if (textBuffer.SequenceEqual(pointText)) return "POINT";
                        if (textBuffer.SequenceEqual(emptyText)) return "EMPTY";
                        break;
                    case 7:
                        if (textBuffer.SequenceEqual(polygonText)) return "POLYGON";
                        break;
                    case 10:
                        if (textBuffer.SequenceEqual(linestringText)) return "LINESTRING";
                        if (textBuffer.SequenceEqual(multipointText)) return "MULTIPOINT";
                        break;
                    case 12:
                        if (textBuffer.SequenceEqual(multipolygonText)) return "MULTIPOLYGON";
                        break;
                    case 15:
                        if (textBuffer.SequenceEqual(multilinestringText)) return "MULTILINESTRING";
                        break;
                    case 18:
                        if (textBuffer.SequenceEqual(geometrycollectionText)) return "GEOMETRYCOLLECTION";
                        break;
                }

                return textBuffer.ToString();
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
