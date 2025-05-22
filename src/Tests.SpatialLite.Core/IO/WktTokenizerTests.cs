using SpatialLite.Core.IO;
using System.IO;
using System.Linq;
using Xunit;

namespace Tests.SpatialLite.Core.IO;

public class WktTokenizerTests
{

    [Fact]
    private void Tokenize_String_ReturnsEmptyTokenForEmptyString()
    {
        string data = string.Empty;
        var tokens = WktTokenizer.Tokenize(data);

        Assert.Empty(tokens);
    }

    [Theory]
    [InlineData("stringTOOKEN", TokenType.STRING)]
    [InlineData(" ", TokenType.WHITESPACE)]
    [InlineData("\t", TokenType.WHITESPACE)]
    [InlineData("\n", TokenType.WHITESPACE)]
    [InlineData("\r", TokenType.WHITESPACE)]
    [InlineData("(", TokenType.LEFT_PARENTHESIS)]
    [InlineData(")", TokenType.RIGHT_PARENTHESIS)]
    [InlineData(",", TokenType.COMMA)]
    [InlineData("-123456780.9", TokenType.NUMBER)]
    private void Tokenize_String_CorrectlyRecognizesTokenTypes(string str, TokenType expectedType)
    {
        var tokens = WktTokenizer.Tokenize(str).ToArray();

        Assert.Single(tokens);

        WktToken t = tokens.First();
        Assert.Equal(expectedType, t.Type);
        Assert.Equal(str, t.Value);
    }

    [Fact]
    private void Tokenize_String_ProcessesComplexText()
    {
        string data = "point z (-10 -15 -100.1)";
        var tokens = WktTokenizer.Tokenize(data).ToArray();

        Assert.Equal(11, tokens.Length);

        WktToken t = tokens[0];
        Assert.Equal(TokenType.STRING, t.Type);
        Assert.Equal("point", t.Value);

        t = tokens[1];
        Assert.Equal(TokenType.WHITESPACE, t.Type);

        t = tokens[2];
        Assert.Equal(TokenType.STRING, t.Type);
        Assert.Equal("z", t.Value);

        t = tokens[3];
        Assert.Equal(TokenType.WHITESPACE, t.Type);

        t = tokens[4];
        Assert.Equal(TokenType.LEFT_PARENTHESIS, t.Type);

        t = tokens[5];
        Assert.Equal(TokenType.NUMBER, t.Type);
        Assert.Equal("-10", t.Value);

        t = tokens[6];
        Assert.Equal(TokenType.WHITESPACE, t.Type);

        t = tokens[7];
        Assert.Equal(TokenType.NUMBER, t.Type);
        Assert.Equal("-15", t.Value);

        t = tokens[8];
        Assert.Equal(TokenType.WHITESPACE, t.Type);

        t = tokens[9];
        Assert.Equal(TokenType.NUMBER, t.Type);
        Assert.Equal("-100.1", t.Value);

        t = tokens[10];
        Assert.Equal(TokenType.RIGHT_PARENTHESIS, t.Type);
    }

    [Fact]
    private void Tokenize_TextReader_ProcessesComplexText()
    {
        StringReader reader = new StringReader("point z (-10 -15 -100.1)");

        var tokens = WktTokenizer.Tokenize(reader).ToArray();

        Assert.Equal(11, tokens.Length);

        WktToken t = tokens[0];
        Assert.Equal(TokenType.STRING, t.Type);
        Assert.Equal("point", t.Value);

        t = tokens[1];
        Assert.Equal(TokenType.WHITESPACE, t.Type);

        t = tokens[2];
        Assert.Equal(TokenType.STRING, t.Type);
        Assert.Equal("z", t.Value);

        t = tokens[3];
        Assert.Equal(TokenType.WHITESPACE, t.Type);

        t = tokens[4];
        Assert.Equal(TokenType.LEFT_PARENTHESIS, t.Type);

        t = tokens[5];
        Assert.Equal(TokenType.NUMBER, t.Type);
        Assert.Equal("-10", t.Value);

        t = tokens[6];
        Assert.Equal(TokenType.WHITESPACE, t.Type);

        t = tokens[7];
        Assert.Equal(TokenType.NUMBER, t.Type);
        Assert.Equal("-15", t.Value);

        t = tokens[8];
        Assert.Equal(TokenType.WHITESPACE, t.Type);

        t = tokens[9];
        Assert.Equal(TokenType.NUMBER, t.Type);
        Assert.Equal("-100.1", t.Value);

        t = tokens[10];
        Assert.Equal(TokenType.RIGHT_PARENTHESIS, t.Type);
    }
}
