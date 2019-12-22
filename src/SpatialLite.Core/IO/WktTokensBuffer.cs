using System.Collections.Generic;
using System.IO;

namespace SpatialLite.Core.IO {
    /// <summary>
    /// Represents collection of WktToken obejcts with specialized methods to access it's items.
    /// </summary>
    internal class WktTokensBuffer {
        private WktTokenizer _tokenizer;

        private WktToken _current = WktToken.EndOfDataToken;
        private WktToken _next = WktToken.EndOfDataToken;

        /// <summary>
        /// Initializes a new instance of the WktTokensBuffer class.
        /// </summary>
        /// <param name="tokenizer"></param>
        public WktTokensBuffer(WktTokenizer tokenizer) {
            _tokenizer = tokenizer;
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
            _next = _tokenizer.GetNextToken();
        }
    }
}
