using System;
using System.Collections.Generic;

namespace SpatialLite.Graphs.Algorithms.AStar {
    /// <summary>
    /// Represents partial Path in the AStar pathfinder
    /// </summary>
    class PartialPath<TVertex> : IComparer<PartialPath<TVertex>>, IComparable {
		public TVertex End;
		public TVertex PreviousNode;
		public double Cost;
		public double EstimatedCostToEnd;

		/// <summary>
		/// Determines whether Obj equals this PartialPath
		/// </summary>
		/// <param name="obj">The object to compare</param>
		/// <returns>true if Obj is PartialPath and CurrentPosition are the same, otherwise false</returns>
		public override bool Equals(object obj) {
			PartialPath<TVertex> other = obj as PartialPath<TVertex>;
			if (other != null) {
				return this.End.Equals(other.End);
			} else
				return false;
		}

		/// <summary>
		/// Determines whether Other equals this PartialPath
		/// </summary>
		/// <param name="other">The PartialPath to compare with the current PartialPath</param>
		/// <returns>true if CurrentPosition are the same, otherwise false</returns>
		public bool Equals(PartialPath<TVertex> other) {
			return this.End.Equals(other.End);
		}

		/// <summary>
		/// Returns a hash code for the current PartialPath
		/// </summary>
		/// <returns>A hash code for the current Segment.</returns>
		public override int GetHashCode() {
			return this.End.GetHashCode();
		}

		#region IComparer<Path> Members

		public int Compare(PartialPath<TVertex> x, PartialPath<TVertex> y) {
			double totalLength = x.Cost + x.EstimatedCostToEnd;

			return totalLength.CompareTo(y.Cost + y.EstimatedCostToEnd);
		}

		#endregion

		#region IComparable Members

		public int CompareTo(object obj) {
			if (obj is PartialPath<TVertex>) {
				PartialPath<TVertex> other = (PartialPath<TVertex>)obj;
				return Compare(this, other);
			}
			return 0;
		}

		#endregion
	}
}
