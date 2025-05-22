using SpatialLite.Core.IO;
using System;
using System.Linq;
using Xunit;

namespace Tests.SpatialLite.Core.IO;

public class WktTokensBufferTests
{
    private readonly WktToken[] _testData = new WktToken[] {new() {Type = TokenType.STRING, Value = "point"}, new() {Type = TokenType.WHITESPACE, Value=" "},
        new() {Type = TokenType.LEFT_PARENTHESIS, Value = "("}};

    [Fact]
    public void Constructor__CreatesEmptyBuffer()
    {
        WktTokensBuffer target = new();

        Assert.Empty(target);
    }

    [Fact]
    public void Constructor_TextReader_CreatesBufferWithSpecificTokens()
    {
        WktTokensBuffer target = new(_testData);

        Assert.Equal(_testData.Length, target.Count());
        for (int i = 0; i < _testData.Length; i++)
        {
            Assert.Equal(_testData[i], target.ToArray()[i]);
        }
    }

    [Fact]
    public void Count_GetsNumberOfItemsInBufffer()
    {
        WktTokensBuffer target = new();
        target.Add(_testData);

        Assert.Equal(_testData.Length, target.Count);
    }

    [Fact]
    public void Add_WktToken_AddsItemToTheCollection()
    {
        WktTokensBuffer target = new();
        target.Add(_testData[0]);

        Assert.Single(target);
        Assert.Contains(_testData[0], target);
    }

    [Fact]
    public void Add_IEnumerable_AddsItemsToTheCollection()
    {
        WktTokensBuffer target = new();
        target.Add(_testData);

        Assert.Equal(_testData.Length, target.Count());
        for (int i = 0; i < _testData.Length; i++)
        {
            Assert.Equal(_testData[i], target.ToArray()[i]);
        }
    }

    [Fact]
    public void Clear_RemovesAllItemsFromCollection()
    {
        WktTokensBuffer target = new();
        target.Add(_testData);

        target.Clear();

        Assert.Empty(target);
    }

    [Fact]
    public void Peek_IgnoreWhitespace_GetsNextTokenFromBufferAndLeavesItThere()
    {
        WktTokensBuffer target = new();
        target.Add(_testData[0]);

        var result = target.Peek(false);

        Assert.Equal(_testData[0], result);
        Assert.Contains(_testData[0], target);
    }

    [Fact]
    public void Peek_IgnoreWhitespace_IgnoresWhitespacesBeforeTokenIfIgnoreTokenIsTrue()
    {
        WktTokensBuffer target = new();
        target.Add(new WktToken() { Type = TokenType.WHITESPACE, Value = " " });
        target.Add(new WktToken() { Type = TokenType.WHITESPACE, Value = " " });
        target.Add(_testData[0]);

        var result = target.Peek(true);

        Assert.Equal(_testData[0], result);
        Assert.Equal(3, target.Count);
    }

    [Fact]
    public void Peek_IgnoreWhitespace_ReturnsWhitespaceIfIgnoreWhitespaceIsFalseAndNextTokenIsWhitespace()
    {
        WktToken whitespaceToken = new() { Type = TokenType.WHITESPACE, Value = " " };
        WktTokensBuffer target = new();
        target.Add(whitespaceToken);
        target.Add(_testData[0]);

        var result = target.Peek(false);

        Assert.Equal(whitespaceToken, result);
        Assert.Equal(2, target.Count);
    }

    [Fact]
    public void Peek_IgnoreWhitespace_ReturnsEndOfDataTokenIfNoMoreTokensAreAvailable()
    {
        WktTokensBuffer target = new();

        var result = target.Peek(false);

        Assert.Equal(WktToken.EndOfDataToken, result);
    }

    [Fact]
    public void Peek_IgnoreWhitespace_ReturnsEndOfDataTokenIfOnlyWhitespaceTokensAreAvailalbleAndIgnoreWhitespaceIsTrue()
    {
        WktToken whitespaceToken = new() { Type = TokenType.WHITESPACE, Value = " " };
        WktTokensBuffer target = new();
        target.Add(whitespaceToken);

        var result = target.Peek(true);

        Assert.Equal(WktToken.EndOfDataToken, result);
    }

    [Fact]
    public void GetToken_IgnoreWhitespace_GetsNextTokenFromBufferAndRemoveIt()
    {
        WktTokensBuffer target = new();
        target.Add(_testData[0]);

        var result = target.GetToken(false);

        Assert.Equal(_testData[0], result);
        Assert.DoesNotContain(_testData[0], target);
    }

    [Fact]
    public void GetToken_IgnoreWhitespace_IgnoresWhitespacesBeforeTokenIfIgnoreTokenIsTrue()
    {
        WktTokensBuffer target = new();
        target.Add(new WktToken() { Type = TokenType.WHITESPACE, Value = " " });
        target.Add(new WktToken() { Type = TokenType.WHITESPACE, Value = " " });
        target.Add(_testData[0]);

        var result = target.GetToken(true);

        Assert.Equal(_testData[0], result);
        Assert.Equal(0, target.Count);
    }

    [Fact]
    public void GetToken_IgnoreWhitespace_ReturnsWhitespaceIfIgnoreWhitespaceIsFalseAndNextTokenIsWhitespace()
    {
        WktToken whitespaceToken = new() { Type = TokenType.WHITESPACE, Value = " " };
        WktTokensBuffer target = new();
        target.Add(whitespaceToken);
        target.Add(_testData[0]);

        var result = target.GetToken(false);

        Assert.Equal(whitespaceToken, result);
        Assert.Equal(1, target.Count);
    }

    [Fact]
    public void GetToken_IgnoreWhitespace_ReturnsEndOfDataTokenIfNoMoreTokensAreAvailable()
    {
        WktTokensBuffer target = new();

        var result = target.GetToken(false);

        Assert.Equal(WktToken.EndOfDataToken, result);
    }

    [Fact]
    public void GetToken_IgnoreWhitespace_ReturnsEndOfDataTokenIfOnlyWhitespaceTokensAreAvailalbleAndIgnoreWhitespaceIsTrue()
    {
        WktToken whitespaceToken = new() { Type = TokenType.WHITESPACE, Value = " " };
        WktTokensBuffer target = new();
        target.Add(whitespaceToken);

        var result = target.GetToken(true);

        Assert.Equal(WktToken.EndOfDataToken, result);
        Assert.Empty(target);
    }
}
