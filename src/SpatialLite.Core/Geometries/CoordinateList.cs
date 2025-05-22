using SpatialLite.Core.API;
using System.Collections;
using System.Collections.Generic;

namespace SpatialLite.Core.Geometries;

/// <summary>
/// Represents read-write list of Coordinates.
/// </summary>
public class CoordinateList : ICoordinateList
{

    private readonly List<Coordinate> _storage;

    /// <summary>
    /// Initializes a new instance of the CoordinateList class, that is empty.
    /// </summary>
    public CoordinateList()
    {
        _storage = new List<Coordinate>();
    }

    /// <summary>
    /// Initializes a new instance of the CoordinateList class that contains coordinates from the given collection.
    /// </summary>
    /// <param name="coords">The collection whose elements are used to fill CoordinateList.</param>
    public CoordinateList(IEnumerable<Coordinate> coords)
    {
        _storage = new List<Coordinate>(coords);
    }

    /// <summary>
    /// Gets number of Coordinates in the list.
    /// </summary>
    public int Count
    {
        get
        {
            return _storage.Count;
        }
    }

    /// <summary>
    /// Gets or sets Coordinate at the given index.
    /// </summary>
    /// <param name="index">The zero-based index of the Coordinate to get or set.</param>
    /// <returns>The element at the specified index.</returns>
    public Coordinate this[int index]
    {
        get
        {
            return _storage[index];
        }
        set
        {
            _storage[index] = value;
        }
    }

    /// <summary>
    /// Adds Coordinate to the end of the list.
    /// </summary>
    /// <param name="coord">The Coordinate to add to the list.</param>
    public void Add(Coordinate coord)
    {
        _storage.Add(coord);
    }

    /// <summary>
    /// Adds collection of coordinates to the end of the list.
    /// </summary>
    /// <param name="coords">The collection of coordinates to add to the list.</param>
    public void Add(IEnumerable<Coordinate> coords)
    {
        _storage.AddRange(coords);
    }

    /// <summary>
    /// Insertes Coordinate to the list at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index at which coord should be inserted.</param>
    /// <param name="coord">The Coordinate to insert into list.</param>
    /// <remarks>If index equals the number of items in the list, then item is appended to the list.</remarks>
    public void Insert(int index, Coordinate coord)
    {
        _storage.Insert(index, coord);
    }

    /// <summary>
    /// Removes Coordinate at the specified index from the list.
    /// </summary>
    /// <param name="index">The zero-based index of the Coordinate to remove.</param>
    public void RemoveAt(int index)
    {
        _storage.RemoveAt(index);
    }

    /// <summary>
    /// Removes all Coordinates from the collection.
    /// </summary>
    public void Clear()
    {
        _storage.Clear();
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
