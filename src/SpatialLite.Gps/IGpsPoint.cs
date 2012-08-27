using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpatialLite.Core.API;

namespace SpatialLite.Gps {
    /// <summary>
    /// Represents location on the earth surface with timestamp that defines time when the point was recorded 
    /// </summary>
    public interface IGpsPoint : IPoint {
        /// <summary>
        /// Gets or sets time when the point was recorded.
        /// </summary>
        DateTime? Timestamp { get; set; }
    }
}
