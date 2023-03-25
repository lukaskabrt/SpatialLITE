using System;
using System.Collections;
using System.Collections.Generic;

using SpatialLite.Core.Api;

namespace SpatialLite.Core.Geometries
{
    /// <summary>
    /// Represents read-write list of Coordinates.
    /// </summary>
    public class CoordinateSequence : ICoordinateSequence
    {

        private List<Coordinate> _storage;

        /// <summary>
        /// Initializes a new instance of the CoordinateList class, that is empty.
        /// </summary>
        public CoordinateSequence()
        {
            _storage = new List<Coordinate>();
        }

        /// <summary>
        /// Initializes a new instance of the CoordinateList class that contains coordinates from the given collection.
        /// </summary>
        /// <param name="coords">The collection whose elements are used to fill CoordinateList.</param>
        public CoordinateSequence(IEnumerable<Coordinate> coords)
        {
            _storage = new List<Coordinate>(coords);
        }

        /// <summary>
        /// Gets number of Coordinates in the list.
        /// </summary>
        public int Count => _storage.Count;

        /// <summary>
        /// Gets or sets Coordinate at the given index.
        /// </summary>
        /// <param name="index">The zero-based index of the Coordinate to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        public Coordinate this[int index]
        {
            get => _storage[index];
        }

        /// <summary>
        /// Returns an enumerator that iterates through the CoordinateList
        /// </summary>
        /// <returns>The Enumerator for the CoordinateList</returns>
        public IEnumerator<Coordinate> GetEnumerator()
        {
            return _storage.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the CoordinateList
        /// </summary>
        /// <returns>The Enumerator for the CoordinateList</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_storage).GetEnumerator();
        }
    }

    public class CoordinateSequenceAdapter<T> : ICoordinateSequence where T : IPoint
    {
        /// <summary>
        /// Initializes a new instance of the ReadOnlyCoordinateList class with specified source list fo nodes
        /// </summary>
        /// <param name="source">The list of Points to be used as source for this ReadOnlyCoordinateList</param>
        public CoordinateSequenceAdapter(IReadOnlyList<T> source)
        {
            this.Source = source;
        }

        /// <summary>
        /// Gets number of Coordinates in the list.
        /// </summary>
        public int Count
        {
            get
            {
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
        public Coordinate this[int index]
        {
            get
            {
                return this.Source[index].Position;
            }
            set
            {
                throw new NotSupportedException("This operation isn't supported - use Nodes property to modify coordinates.");
            }
        }

        /// <summary>
        /// Adds Coordinate to the end of the list.
        /// </summary>
        /// <param name="coord">The Coordinate to add to the list.</param>
        public void Add(Coordinate coord)
        {
            throw new NotSupportedException("This operation isn't supported - use Nodes property to modify coordinates.");
        }

        /// <summary>
        /// Adds collection of coordinates to the end of the list.
        /// </summary>
        /// <param name="coords">The collection of coordinates to add to the list.</param>
        public void Add(IEnumerable<Coordinate> coords)
        {
            throw new NotSupportedException("This operation isn't supported - use Nodes property to modify coordinates.");
        }

        /// <summary>
        /// Insertes Coordinate to the list at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which coord should be inserted.</param>
        /// <param name="coord">The Coordinate to insert into list.</param>
        /// <remarks>If index equals the number of items in the list, then item is appended to the list.</remarks>
        public void Insert(int index, Coordinate coord)
        {
            throw new NotSupportedException("This operation isn't supported - use Nodes property to modify coordinates.");
        }

        /// <summary>
        /// Removes Coordinate at the specified index from the list.
        /// </summary>
        /// <param name="index">The zero-based index of the Coordinate to remove.</param>
        public void RemoveAt(int index)
        {
            throw new NotSupportedException("This operation isn't supported - use Nodes property to modify coordinates.");
        }

        /// <summary>
        /// Removes all Coordinates from the collection.
        /// </summary>
        public void Clear()
        {
            throw new NotSupportedException("This operation isn't supported - use Nodes property to modify coordinates.");
        }

        /// <summary>
        /// Returns an enumerator that iterates through the CoordinateList
        /// </summary>
        /// <returns>The Enumerator for the CoordinateList</returns>
        public IEnumerator<Coordinate> GetEnumerator()
        {
            foreach (var node in this.Source)
            {
                yield return node.Position;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the CoordinateList
        /// </summary>
        /// <returns>The Enumerator for the CoordinateList</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
