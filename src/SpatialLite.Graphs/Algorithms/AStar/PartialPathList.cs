using System.Collections.Generic;

namespace SpatialLite.Graphs.Algorithms.AStar {
    /// <summary>
    /// Represents a priority queue of the PartialPath, it is used as open list in the A* algorithm
    /// </summary>
    class PartialPathList<TVertex> : BinaryHeap<PartialPath<TVertex>> {
		Dictionary<TVertex, PartialPath<TVertex>> _paths;

		/// <summary>
		/// Creates a new instance of the PartialPathList
		/// </summary>
		public PartialPathList()
			: base() {
			_paths = new Dictionary<TVertex, PartialPath<TVertex>>();
		}

		/// <summary>
		/// Gets the PartialPath with the specified Position
		/// </summary>
		/// <param name="position">The Position of PartialPath</param>
		/// <returns></returns>
		public PartialPath<TVertex> this[TVertex position] {
			get {
				return _paths[position];
			}
		}

		/// <summary>
		/// Updates the PartialPath length
		/// </summary>
		/// <param name="item">The item to update</param>
		/// <param name="pathLength">The new length of the PartialPath</param>
		public void Update(PartialPath<TVertex> item, double pathLength) {
			base.Remove(item);

			item.Cost = pathLength;
			base.Add(item);
		}

		/// <summary>
		/// Adds a new item to the List
		/// </summary>
		/// <param name="item">The item to add</param>
		public new void Add(PartialPath<TVertex> item) {
			_paths.Add(item.End, item);
			base.Add(item);
		}

		/// <summary>
		/// Removes the item from the List
		/// </summary>
		/// <param name="item">The item to remove</param>
		/// <returns>true if item was removed, otherwise returns false</returns>
		public new bool Remove(PartialPath<TVertex> item) {
			_paths.Remove(item.End);
			return base.Remove(item);
		}

		/// <summary>
		/// Removes the shortest PartialPath from the list
		/// </summary>
		/// <returns>The shortest PartialPath object from the list</returns>
		public new PartialPath<TVertex> RemoveTop() {
			PartialPath<TVertex> result = base.RemoveTop();
			_paths.Remove(result.End);

			return result;
		}

		/// <summary>
		/// Determinates whether List contains the specified item
		/// </summary>
		/// <param name="item">The item to check</param>
		/// <returns>true if item is presented in the List, otherwise returns false</returns>
		public new bool Contains(PartialPath<TVertex> item) {
			return _paths.ContainsKey(item.End);
		}

		/// <summary>
		/// Determinates whether List contains the item with the specified Position
		/// </summary>
		/// <param name="item">The Position of the item</param>
		/// <returns>true if item is presented in the List, otherwise returns false</returns>
		public bool Contains(TVertex position) {
			return _paths.ContainsKey(position);
		}

		/// <summary>
		/// Removes all objects from the List
		/// </summary>
		public new void Clear() {
			base.Clear();
			_paths.Clear();
		}
	}
}
