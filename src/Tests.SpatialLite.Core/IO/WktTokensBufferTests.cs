using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xunit;

using SpatialLite.Core.IO;

namespace Tests.SpatialLite.Core.IO {
    public class WktTokensBufferTests {
        //WktToken[] _testData = new WktToken[] {
        //    new WktToken() { Type = TokenType.STRING, TextValue = "point" },
        //    new WktToken() { Type = TokenType.WHITESPACE, TextValue = " " },
        //    new WktToken() { Type = TokenType.LEFT_PARENTHESIS, TextValue = "(" }
        //};

        //#region Constructor() tests

        //[Fact]
        //public void Constructor__CreatesEmptyBuffer() {
        //    WktTokensBuffer target = new WktTokensBuffer(new WktToken[] { });

        //    var result = target.Peek(false);

        //    Assert.Equal(WktToken.EndOfDataToken, result);
        //}

        //#endregion

        //#region Constructor(IEnumerable<WktToken>) tests

        //[Fact]
        //public void Constructor_TextReader_CreatesBufferWithSpecificTokens() {
        //    WktTokensBuffer target = new WktTokensBuffer(_testData);

        //    for (int i = 0; i < _testData.Length; i++) {
        //        Assert.Equal(_testData[i], target.GetToken(false));
        //    }
        //}

        //#endregion

        //#region Peek(IgnoreWhitespace) tests

        //[Fact]
        //public void Peek_IgnoreWhitespace_GetsNextTokenFromBufferAndLeavesItThere() {
        //    WktTokensBuffer target = new WktTokensBuffer(new WktToken[] { _testData[0] });

        //    var result = target.Peek(false);
        //    var next = target.Peek(false);

        //    Assert.Equal(_testData[0], result);
        //    Assert.Equal(_testData[0], next);
        //}

        //[Fact]
        //public void Peek_IgnoreWhitespace_IgnoresWhitespacesBeforeTokenIfIgnoreTokenIsTrue() {
        //    WktTokensBuffer target = new WktTokensBuffer(new WktToken[] {
        //        new WktToken() { Type = TokenType.WHITESPACE, TextValue = " " },
        //        _testData[0]
        //    });

        //    var result = target.Peek(true);

        //    Assert.Equal(_testData[0], result);
        //}

        //[Fact]
        //public void Peek_IgnoreWhitespace_ReturnsWhitespaceIfIgnoreWhitespaceIsFalseAndNextTokenIsWhitespace() {
        //    WktToken whitespaceToken = new WktToken() { Type = TokenType.WHITESPACE, TextValue = " " };
        //    WktTokensBuffer target = new WktTokensBuffer(new WktToken[] {
        //        whitespaceToken,
        //        _testData[0]
        //    });

        //    var result = target.Peek(false);

        //    Assert.Equal(whitespaceToken, result);
        //}

        //[Fact]
        //public void Peek_IgnoreWhitespace_ReturnsEndOfDataTokenIfNoMoreTokensAreAvailable() {
        //    WktTokensBuffer target = new WktTokensBuffer(new WktToken[] { });

        //    var result = target.Peek(false);

        //    Assert.Equal(WktToken.EndOfDataToken, result);
        //}

        //[Fact]
        //public void Peek_IgnoreWhitespace_ReturnsEndOfDataTokenIfOnlyWhitespaceTokensAreAvailalbleAndIgnoreWhitespaceIsTrue() {
        //    WktToken whitespaceToken = new WktToken() { Type = TokenType.WHITESPACE, TextValue = " " };
        //    WktTokensBuffer target = new WktTokensBuffer(new WktToken[] { whitespaceToken });

        //    var result = target.Peek(true);

        //    Assert.Equal(WktToken.EndOfDataToken, result);
        //}

        //#endregion

        //#region GetToken(IgnoreWhitespace) tests

        //[Fact]
        //public void GetToken_IgnoreWhitespace_GetsNextTokenFromBufferAndRemoveIt() {
        //    WktTokensBuffer target = new WktTokensBuffer(new WktToken[] { _testData[0] });

        //    var result = target.GetToken(false);
        //    var next = target.GetToken(false);

        //    Assert.Equal(_testData[0], result);
        //    Assert.Equal(next, WktToken.EndOfDataToken);
        //}

        //[Fact]
        //public void GetToken_IgnoreWhitespace_IgnoresWhitespacesBeforeTokenIfIgnoreTokenIsTrue() {
        //    WktTokensBuffer target = new WktTokensBuffer(new WktToken[] {
        //        new WktToken() { Type = TokenType.WHITESPACE, TextValue = " " },
        //        _testData[0]
        //    });

        //    var result = target.GetToken(true);

        //    Assert.Equal(_testData[0], result);
        //}

        //[Fact]
        //public void GetToken_IgnoreWhitespace_ReturnsWhitespaceIfIgnoreWhitespaceIsFalseAndNextTokenIsWhitespace() {
        //    WktToken whitespaceToken = new WktToken() { Type = TokenType.WHITESPACE, TextValue = " " };
        //    WktTokensBuffer target = new WktTokensBuffer(new WktToken[] {
        //        whitespaceToken,
        //        _testData[0]
        //    });

        //    var result = target.GetToken(false);

        //    Assert.Equal(whitespaceToken, result);
        //}

        //[Fact]
        //public void GetToken_IgnoreWhitespace_ReturnsEndOfDataTokenIfNoMoreTokensAreAvailable() {
        //    WktTokensBuffer target = new WktTokensBuffer(new WktToken[] { });

        //    var result = target.GetToken(false);

        //    Assert.Equal(WktToken.EndOfDataToken, result);
        //}

        //[Fact]
        //public void GetToken_IgnoreWhitespace_ReturnsEndOfDataTokenIfOnlyWhitespaceTokensAreAvailalbleAndIgnoreWhitespaceIsTrue() {
        //    WktToken whitespaceToken = new WktToken() { Type = TokenType.WHITESPACE, TextValue = " " };
        //    WktTokensBuffer target = new WktTokensBuffer(new WktToken[] { whitespaceToken });

        //    var result = target.GetToken(true);
        //    var next = target.GetToken(true);

        //    Assert.Equal(WktToken.EndOfDataToken, result);
        //    Assert.Equal(WktToken.EndOfDataToken, next);
        //}

        //#endregion
    }
}
