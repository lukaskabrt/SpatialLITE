using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SpatialLite.Core.IO {
	/// <summary>
	/// Implemets tokenizer that splits wkt string to list of tokens.
	/// </summary>
	internal static class WktTokenizer {
		#region Public Static Methods

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
			StringBuilder stringBuffer = new StringBuilder();

			if (reader.Peek() == -1) {
				yield break;
			}

			char ch = (char)reader.Read();
			stringBuffer.Append(ch);
			TokenType lastToken = WktTokenizer.GetTokenType(ch);

			while (reader.Peek() != -1) {
				ch = (char)reader.Read();
				TokenType token = WktTokenizer.GetTokenType(ch);

				// tokens COMMA, LEFT_PARENTHESIS and RIGHT_PARENTHESIS can not be grupped together
				if ((token != lastToken) || token == TokenType.COMMA || token == TokenType.LEFT_PARENTHESIS || token == TokenType.RIGHT_PARENTHESIS) {
					yield return new WktToken() { Type = lastToken, Value = stringBuffer.ToString() };

					stringBuffer.Clear();
					lastToken = token;
				}

				stringBuffer.Append(ch);
			}

			yield return new WktToken() { Type = lastToken, Value = stringBuffer.ToString() };
		}

		#endregion

		#region Private Static Methods

		/// <summary>
		/// Gets type of the token.
		/// </summary>
		/// <param name="ch">Character to get type.</param>
		/// <returns>the type of token for ch.</returns>
		/// <remarks>In WKT can characters be divided into token types without context - every character is member of thne one token type only.</remarks>
		private static TokenType GetTokenType(char ch) {
			if ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z')) {
				return TokenType.STRING;
			}
			else if (ch == ' ' || ch == '\t' || ch == '\r' || ch == '\n') {
				return TokenType.WHITESPACE;
			}
			else if (ch == ',') {
				return TokenType.COMMA;
			}
			else if (ch == '(') {
				return TokenType.LEFT_PARENTHESIS;
			}
			else if (ch == ')') {
				return TokenType.RIGHT_PARENTHESIS;
			}
			else if (ch == '-' || ch == '.' || (ch >= '0' && ch <= '9')) {
				return TokenType.NUMBER;
			}

			throw new Exception();
		}

		#endregion
	}
}
