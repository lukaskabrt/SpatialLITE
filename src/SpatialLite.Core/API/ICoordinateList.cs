using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpatialLite.Core.API {
	/// <summary>
	/// Defines properties and methods for collection of coordinates.
	/// </summary>
	public interface ICoordinateList : IEnumerable<Coordinate> {
		/// <summary>
		/// Gets number of Coordinates in the list
		/// </summary>
		int Count { get; }

		/// <summary>
		/// Gets or sets Coordinate at the given index.
		/// </summary>
		/// <param name="index">The zero-based index of the Coordinate to get or set.</param>
		/// <returns>The element at the specified index.</returns>
		Coordinate this[int index] { get; set; }

		/// <summary>
		/// Adds Coordinate to the end of the list.
		/// </summary>
		/// <param name="coord">The Coordinate to add to the list</param>
		void Add(Coordinate coord);

		/// <summary>
		/// Adds collection of coordinates to the end of the list.
		/// </summary>
		/// <param name="coords">The collection of coordinates to add to the list</param>
		void Add(IEnumerable<Coordinate> coords);

		/// <summary>
		/// Insertes Coordinate to the list at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which coord should be inserted.</param>
		/// <param name="coord">The Coordinate to insert into list.</param>
		/// <remarks>If index equals the number of items in the list, then item is appended to the list.</remarks>
		void Insert(int index, Coordinate coord);
		
		/// <summary>
		/// Removes Coordinate at the specified index from the list.
		/// </summary>
		/// <param name="index">The zero-based index of the Coordinate to remove.</param>
		void RemoveAt(int index);

		/// <summary>
		/// Removes all Coordinates from the collection.
		/// </summary>
		void Clear();
	}
}
