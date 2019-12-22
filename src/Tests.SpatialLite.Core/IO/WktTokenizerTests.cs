using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xunit;
using Xunit.Extensions;

using SpatialLite.Core.IO;
using System.IO;
using System.Globalization;

namespace Tests.SpatialLite.Core.IO {
    public class WktTokenizerTests {
        #region Tokenize(string) tests

        [Fact]
        void Tokenize_String_ReturnsEmptyTokenForEmptyString() {
            string data = string.Empty;
            var tokenizer = new WktTokenizer(data);

            var token = tokenizer.GetNextToken();

            Assert.Equal(WktToken.EndOfDataToken, token);
        }

        [Theory]
        [InlineData("STRINGTOOKEN", TokenType.STRING)]
        [InlineData(" ", TokenType.WHITESPACE)]
        [InlineData("\t", TokenType.WHITESPACE)]
        [InlineData("\n", TokenType.WHITESPACE)]
        [InlineData("\r", TokenType.WHITESPACE)]
        [InlineData("(", TokenType.LEFT_PARENTHESIS)]
        [InlineData(")", TokenType.RIGHT_PARENTHESIS)]
        [InlineData(",", TokenType.COMMA)]
        [InlineData("-123456780.9", TokenType.NUMBER)]
        void Tokenize_String_CorrectlyRecognizesTokenTypes(string str, TokenType expectedType) {
            var tokenizer = new WktTokenizer(str);

            var token = tokenizer.GetNextToken();

            Assert.Equal(expectedType, token.Type);
            if (token.Type == TokenType.STRING) {
                Assert.Equal(str, token.TextValue);
            }
            if (token.Type == TokenType.NUMBER) {
                Assert.Equal(str, token.NumericValue.ToString(CultureInfo.InvariantCulture));
            }
        }

        [Fact]
        void Tokenize_String_ProcessesComplexText() {
            var tokenizer = new WktTokenizer("point z (-10 -15 -100.1)");

            var tokens = this.ReadTokensToList(tokenizer);

            Assert.Equal(11, tokens.Count);

            WktToken t = tokens[0];
            Assert.Equal(TokenType.STRING, t.Type);
            Assert.Equal("POINT", t.TextValue);

            t = tokens[1];
            Assert.Equal(TokenType.WHITESPACE, t.Type);

            t = tokens[2];
            Assert.Equal(TokenType.STRING, t.Type);
            Assert.Equal("Z", t.TextValue);

            t = tokens[3];
            Assert.Equal(TokenType.WHITESPACE, t.Type);

            t = tokens[4];
            Assert.Equal(TokenType.LEFT_PARENTHESIS, t.Type);

            t = tokens[5];
            Assert.Equal(TokenType.NUMBER, t.Type);
            Assert.Equal(-10, t.NumericValue);

            t = tokens[6];
            Assert.Equal(TokenType.WHITESPACE, t.Type);

            t = tokens[7];
            Assert.Equal(TokenType.NUMBER, t.Type);
            Assert.Equal(-15, t.NumericValue);

            t = tokens[8];
            Assert.Equal(TokenType.WHITESPACE, t.Type);

            t = tokens[9];
            Assert.Equal(TokenType.NUMBER, t.Type);
            Assert.Equal(-100.1, t.NumericValue);

            t = tokens[10];
            Assert.Equal(TokenType.RIGHT_PARENTHESIS, t.Type);
        }

        #endregion

        #region Tokenize(TextReader) tests

        [Fact]
        void Tokenize_TextReader_ProcessesComplexText() {
            var tokenizer = new WktTokenizer(new StringReader("point z (-10 -15 -100.1)"));

            var tokens = this.ReadTokensToList(tokenizer);

            Assert.Equal(11, tokens.Count);

            WktToken t = tokens[0];
            Assert.Equal(TokenType.STRING, t.Type);
            Assert.Equal("POINT", t.TextValue);

            t = tokens[1];
            Assert.Equal(TokenType.WHITESPACE, t.Type);

            t = tokens[2];
            Assert.Equal(TokenType.STRING, t.Type);
            Assert.Equal("Z", t.TextValue);

            t = tokens[3];
            Assert.Equal(TokenType.WHITESPACE, t.Type);

            t = tokens[4];
            Assert.Equal(TokenType.LEFT_PARENTHESIS, t.Type);

            t = tokens[5];
            Assert.Equal(TokenType.NUMBER, t.Type);
            Assert.Equal(-10, t.NumericValue);

            t = tokens[6];
            Assert.Equal(TokenType.WHITESPACE, t.Type);

            t = tokens[7];
            Assert.Equal(TokenType.NUMBER, t.Type);
            Assert.Equal(-15, t.NumericValue);

            t = tokens[8];
            Assert.Equal(TokenType.WHITESPACE, t.Type);

            t = tokens[9];
            Assert.Equal(TokenType.NUMBER, t.Type);
            Assert.Equal(-100.1, t.NumericValue);

            t = tokens[10];
            Assert.Equal(TokenType.RIGHT_PARENTHESIS, t.Type);
        }

        #endregion

        private List<WktToken> ReadTokensToList(WktTokenizer tokenizer) {
            var result = new List<WktToken>();

            var token = tokenizer.GetNextToken();
            while (token.Type != TokenType.END_OF_DATA) {
                result.Add(token);
                token = tokenizer.GetNextToken();
            }

            return result;
        }
    }
}
