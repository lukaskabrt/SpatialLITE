using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpatialLite.Core.IO {
	/// <summary>
	/// Represents exception that can occur during WKT parsing.
	/// </summary>
	public class WktParseException : Exception {
		/// <summary>
		/// Initializes a new instance of the WktParseException class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public WktParseException(string message)
			: base(message) {
		}
	}
}
