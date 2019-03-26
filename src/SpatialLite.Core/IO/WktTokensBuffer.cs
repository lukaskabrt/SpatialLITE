using System.Collections.Generic;

namespace SpatialLite.Core.IO {
    /// <summary>
    /// Represents collection of WktToken obejcts with specialized methods to access it's items.
    /// </summary>
    internal class WktTokensBuffer {
        private IEnumerator<WktToken> _buffer;

        private WktToken _current = WktToken.EndOfDataToken;
        private WktToken _next = WktToken.EndOfDataToken;

        /// <summary>
        /// Initializes a new instance of the WktTokensBuffer class and fills it with specified tokens.
        /// </summary>
        /// <param name="tokens">Tokens to add to the buffer.</param>
        public WktTokensBuffer(IEnumerable<WktToken> tokens) {
            _buffer = tokens.GetEnumerator();
            Advance();
            Advance();
        }

        /// <summary>
        /// Gets next token from the tokens
        /// </summary>
        /// <param name="ignoreWhitespace">bool value indicating whether whitespace tokens should be ignored</param>
        /// <returns>Next token form the tokens</returns>
        public WktToken Peek(bool ignoreWhitespace) {
            if (!ignoreWhitespace) {
                return _current;
            }

            if (_current.Type != TokenType.WHITESPACE) {
                return _current;
            }

            return _next;
        }

        /// <summary>
        /// Gets next token from the tokens and proceeds to the next token
        /// </summary>
        /// <param name="ignoreWhitespace">bool value indicating whether whitespace tokens should be ignored</param>
        /// <returns>Next token form the tokens</returns>
        public WktToken GetToken(bool ignoreWhitespace) {
            if (ignoreWhitespace && _current.Type == TokenType.WHITESPACE) {
                Advance();
            }

            var result = _current;
            Advance();
            return result;
        }

        private void Advance() {
            _current = _next;
            if(_buffer.MoveNext()) {
                _next = _buffer.Current;
            } else {
                _next = WktToken.EndOfDataToken;
            }
        }
    }
}
