using System.Collections.Generic;

namespace SpatialLite.Core.IO;

/// <summary>
/// Represents collection of WktToken obejcts with specialized methods to access it's items.
/// </summary>
internal class WktTokensBuffer : IEnumerable<WktToken>
{

    private readonly List<WktToken> _buffer;

    /// <summary>
    /// Initializes a new instance of the WktTokenBuffer class that is empty.
    /// </summary>
    public WktTokensBuffer()
    {
        _buffer = new List<WktToken>();
    }

    /// <summary>
    /// Initializes a new instance of the WktTokensBuffer class and fills it with specified tokens.
    /// </summary>
    /// <param name="tokens">Tokens to add to the buffer.</param>
    public WktTokensBuffer(IEnumerable<WktToken> tokens)
    {
        _buffer = new List<WktToken>(tokens);
    }

    /// <summary>
    /// Gets number of items in the collection
    /// </summary>
    public int Count
    {
        get
        {
            return _buffer.Count;
        }
    }

    /// <summary>
    /// Adds specific token to the end of tokens tokens
    /// </summary>
    /// <param name="item">The WktToken to add to the tokens</param>
    public void Add(WktToken item)
    {
        _buffer.Add(item);
    }

    /// <summary>
    /// Adds tokens from specific collection to the ent of tokens
    /// </summary>
    /// <param name="items">The collection whose items to add to this collection</param>
    public void Add(IEnumerable<WktToken> items)
    {
        _buffer.AddRange(items);
    }

    /// <summary>
    /// Removes all tokens from the collection
    /// </summary>
    public void Clear()
    {
        _buffer.Clear();
    }

    /// <summary>
    /// Gets next token from the tokens
    /// </summary>
    /// <param name="ignoreWhitespace">bool value indicating whether whitespace tokens should be ignored</param>
    /// <returns>Next token form the tokens</returns>
    public WktToken Peek(bool ignoreWhitespace)
    {
        if (Count == 0)
        {
            return WktToken.EndOfDataToken;
        }

        WktToken t = _buffer[0];
        if (t.Type == TokenType.WHITESPACE && ignoreWhitespace)
        {
            int index = 0;
            while (index < _buffer.Count && t.Type == TokenType.WHITESPACE)
            {
                t = _buffer[index++];
            }

            if (t.Type == TokenType.WHITESPACE)
            {
                return WktToken.EndOfDataToken;
            }

            return t;
        }

        return t;
    }

    /// <summary>
    /// Gets next token from the tokens and proceeds to the next token
    /// </summary>
    /// <param name="ignoreWhitespace">bool value indicating whether whitespace tokens should be ignored</param>
    /// <returns>Next token form the tokens</returns>
    public WktToken GetToken(bool ignoreWhitespace)
    {
        if (ignoreWhitespace)
        {
            while (_buffer.Count > 0 && _buffer[0].Type == TokenType.WHITESPACE)
            {
                _buffer.RemoveAt(0);
            }
        }

        if (_buffer.Count == 0)
        {
            return WktToken.EndOfDataToken;
        }

        WktToken result = _buffer[0];
        _buffer.RemoveAt(0);

        return result;
    }

    /// <summary>
    /// Returns an enumerator that iterates through the items of collection
    /// </summary>
    /// <returns>An enumerator that iterates through the collection</returns>
    public IEnumerator<WktToken> GetEnumerator()
    {
        return _buffer.GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through the items of collection
    /// </summary>
    /// <returns>An enumerator that iterates through the collection</returns>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
