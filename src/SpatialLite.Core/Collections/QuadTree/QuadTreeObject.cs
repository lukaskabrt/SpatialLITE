using SpatialLite.Core.API;

namespace SpatialLite.Core.Collections.QuadTree {
    /// <summary>
    /// Used internally to attach an Owner to each object stored in the QuadTree
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class QuadTreeObject<T> where T : IGeometry
    {
        /// <summary>
        /// The wrapped data value
        /// </summary>
        public T Data {
            get;
            private set;
        }

        /// <summary>
        /// The QuadTreeNode that owns this object
        /// </summary>
        internal QuadTreeNode<T> Owner {
            get;
            set;
        }

        /// <summary>
        /// Wraps the data value
        /// </summary>
        /// <param name="data">The data value to wrap</param>
        public QuadTreeObject(T data) {
            Data = data;
        }
    }
}
