using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpatialLite.Core.API;

namespace SpatialLite.Core.Geometries {
    /// <summary>
    /// Provides read-only ICoordinateList wrapper around collection of points
    /// </summary>
    public class ReadOnlyCoordinateList<T> : ICoordinateList where T : IPoint {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the ReadOnlyCoordinateList class with specified source list fo nodes
        /// </summary>
        /// <param name="source">The list of Points to be used as source for this ReadOnlyCoordinateList</param>
        public ReadOnlyCoordinateList(IReadOnlyList<T> source) {
            this.Source = source;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets number of Coordinates in the list.
        /// </summary>
        public int Count {
            get {
                return this.Source.Count;
            }
        }

        /// <summary>
        /// Gets the List of Points used as source for this ReadOnlyCoordinateList.
        /// </summary>
        public IReadOnlyList<T> Source { get; private set; }

        /// <summary>
        /// Gets or sets Coordinate at the given index.
        /// </summary>
        /// <param name="index">The zero-based index of the Coordinate to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        public Coordinate this[int index] {
            get {
                return this.Source[index].Position;
            }
            set {
                throw new NotSupportedException("This operation isn't supported - use Nodes property to modify coordinates.");
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds Coordinate to the end of the list.
        /// </summary>
        /// <param name="coord">The Coordinate to add to the list.</param>
        public void Add(Coordinate coord) {
            throw new NotSupportedException("This operation isn't supported - use Nodes property to modify coordinates.");
        }

        /// <summary>
        /// Adds collection of coordinates to the end of the list.
        /// </summary>
        /// <param name="coords">The collection of coordinates to add to the list.</param>
        public void Add(IEnumerable<Coordinate> coords) {
            throw new NotSupportedException("This operation isn't supported - use Nodes property to modify coordinates.");
        }

        /// <summary>
        /// Insertes Coordinate to the list at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which coord should be inserted.</param>
        /// <param name="coord">The Coordinate to insert into list.</param>
        /// <remarks>If index equals the number of items in the list, then item is appended to the list.</remarks>
        public void Insert(int index, Coordinate coord) {
            throw new NotSupportedException("This operation isn't supported - use Nodes property to modify coordinates.");
        }

        /// <summary>
        /// Removes Coordinate at the specified index from the list.
        /// </summary>
        /// <param name="index">The zero-based index of the Coordinate to remove.</param>
        public void RemoveAt(int index) {
            throw new NotSupportedException("This operation isn't supported - use Nodes property to modify coordinates.");
        }

        /// <summary>
        /// Removes all Coordinates from the collection.
        /// </summary>
        public void Clear() {
            throw new NotSupportedException("This operation isn't supported - use Nodes property to modify coordinates.");
        }

        /// <summary>
        /// Returns an enumerator that iterates through the CoordinateList
        /// </summary>
        /// <returns>The Enumerator for the CoordinateList</returns>
        public IEnumerator<Coordinate> GetEnumerator() {
            foreach (var node in this.Source) {
                yield return node.Position;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the CoordinateList
        /// </summary>
        /// <returns>The Enumerator for the CoordinateList</returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        public void Apply(ICoordinateFilter filter) {
            for (int i = 0; i < this.Source.Count; i++) {
                this.Source[i].Apply(filter);
            }
        }

        #endregion
    }
}
