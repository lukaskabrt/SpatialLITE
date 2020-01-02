using SpatialLite.Core.API;
using SpatialLite.Core.Collections.QuadTree;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SpatialLite.Core.Collections {

    /// <summary>
    /// A QuadTree Object that provides fast and efficient storage of objects in a world space.
    /// </summary>
    /// <typeparam name="T">Any object implementing IQuadStorable.</typeparam>
    public class QuadTree<T> : ICollection<T> where T : IGeometry {
        private readonly Dictionary<T, QuadTreeObject<T>> wrappedDictionary = new Dictionary<T, QuadTreeObject<T>>();

        // The root of this quad tree
        private readonly QuadTreeNode<T> quadTreeRoot;

        /// <summary>
        /// Creates a QuadTree for the specified area.
        /// </summary>
        /// <param name="rect">The area this QuadTree object will encompass.</param>
        public QuadTree(Envelope rect) {
            quadTreeRoot = new QuadTreeNode<T>(rect);
        }


        /// <summary>
        /// Creates a QuadTree for the specified area.
        /// </summary>
        /// <param name="x">The top-left position of the area rectangle.</param>
        /// <param name="y">The top-right position of the area rectangle.</param>
        /// <param name="width">The width of the area rectangle.</param>
        /// <param name="height">The height of the area rectangle.</param>
        public QuadTree(double x, double y, double width, double height) {
            quadTreeRoot = new QuadTreeNode<T>(new Envelope(x, x + width, y, y + height));
        }

        /// <summary>
        /// Gets the rectangle that bounds this QuadTree
        /// </summary>
        public Envelope QuadRect {
            get { return quadTreeRoot.QuadRect; }
        }

        /// <summary>
        /// Get the objects in this tree that intersect with the specified rectangle.
        /// </summary>
        /// <param name="rect">The rectangle to find objects in.</param>
        public List<T> GetObjects(Envelope rect) {
            return quadTreeRoot.GetObjects(rect);
        }


        /// <summary>
        /// Get the objects in this tree that intersect with the specified rectangle.
        /// </summary>
        /// <param name="rect">The rectangle to find objects in.</param>
        /// <param name="results">A reference to a list that will be populated with the results.</param>
        public void GetObjects(Envelope rect, ref List<T> results) {
            quadTreeRoot.GetObjects(rect, ref results);
        }


        /// <summary>
        /// Get all objects in this Quad, and it's children.
        /// </summary>
        public List<T> GetAllObjects() {
            return new List<T>(wrappedDictionary.Keys);
        }


        /// <summary>
        /// Moves the object in the tree
        /// </summary>
        /// <param name="item">The item that has moved</param>
        public bool Move(T item) {
            if (Contains(item)) {
                quadTreeRoot.Move(wrappedDictionary[item]);
                return true;
            } else {
                return false;
            }
        }

        ///<summary>
        ///Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
        ///</summary>
        ///
        ///<param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        ///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
        public void Add(T item) {
            QuadTreeObject<T> wrappedObject = new QuadTreeObject<T>(item);
            wrappedDictionary.Add(item, wrappedObject);
            quadTreeRoot.Insert(wrappedObject);
        }


        ///<summary>
        ///Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        ///</summary>
        ///
        ///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only. </exception>
        public void Clear() {
            wrappedDictionary.Clear();
            quadTreeRoot.Clear();
        }


        ///<summary>
        ///Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.
        ///</summary>
        ///
        ///<returns>
        ///true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.
        ///</returns>
        ///
        ///<param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        public bool Contains(T item) {
            return wrappedDictionary.ContainsKey(item);
        }


        ///<summary>
        ///Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
        ///</summary>
        ///
        ///<param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        ///<param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        ///<exception cref="T:System.ArgumentNullException"><paramref name="array" /> is null.</exception>
        ///<exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex" /> is less than 0.</exception>
        ///<exception cref="T:System.ArgumentException"><paramref name="array" /> is multidimensional.-or-<paramref name="arrayIndex" /> is equal to or greater than the length of <paramref name="array" />.-or-The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1" /> is greater than the available space from <paramref name="arrayIndex" /> to the end of the destination <paramref name="array" />.-or-Type <paramref name="T" /> cannot be cast automatically to the type of the destination <paramref name="array" />.</exception>
        public void CopyTo(T[] array, int arrayIndex) {
            wrappedDictionary.Keys.CopyTo(array, arrayIndex);
        }

        ///<summary>
        ///Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
        ///</summary>
        ///<returns>
        ///The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
        ///</returns>
        public int Count {
            get { return wrappedDictionary.Count; }
        }

        ///<summary>
        ///Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        ///</summary>
        ///
        ///<returns>
        ///true if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise, false.
        ///</returns>
        ///
        public bool IsReadOnly {
            get { return false; }
        }

        ///<summary>
        ///Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        ///</summary>
        ///
        ///<returns>
        ///true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.
        ///</returns>
        ///
        ///<param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        ///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
        public bool Remove(T item) {
            if (Contains(item)) {
                quadTreeRoot.Delete(wrappedDictionary[item], true);
                wrappedDictionary.Remove(item);
                return true;
            } else {
                return false;
            }
        }

        ///<summary>
        ///Returns an enumerator that iterates through the collection.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        ///</returns>
        ///<filterpriority>1</filterpriority>
        public IEnumerator<T> GetEnumerator() {
            return wrappedDictionary.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}
