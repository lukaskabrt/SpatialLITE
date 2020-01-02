using System;
using System.Collections.Generic;
using System.Text;

namespace SpatialLite.Graphs.Api {
        public interface IEdge<TVertex> {
            /// <summary>
            /// Gets the source vertex.
            /// </summary>
            TVertex Source { get; }

            /// <summary>
            /// Gets the destination vertex.
            /// </summary>
            TVertex Destination { get; }
        }
}
