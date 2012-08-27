using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpatialLite.Gps.Geometries {
    /// <summary>
    /// Represents a link to an external resource (Web page, digital photo, video clip, etc) with additional information.
    /// </summary>
    public class GpxLink {
        #region Public properties

        /// <summary>
        /// Gets the URL of the link
        /// </summary>
        public Uri Url { get; set; }

        /// <summary>
        /// Gets the text of the hyperlink
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets the mime type of the linked content
        /// </summary>
        public string Type { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the GpxLink with given url
        /// </summary>
        /// <param name="url">The url of the link</param>
        public GpxLink(Uri url) {
            Url = url;
        }

        #endregion
    }
}
