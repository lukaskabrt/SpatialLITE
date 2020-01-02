using SpatialLite.Graphs.Algorithms.AStar;
using SpatialLite.Graphs.Api;
using System.Collections.Generic;
using System.Text;

namespace SpatialLite.Graphs.Algorithms {
	/// <summary>
	/// Represents class that is able to find path beteewn two points in the graph
	/// </summary>
	public class Astar<TVertex, TEdge> where TEdge : IEdge<TVertex> {
		private readonly IGraph<TVertex, TEdge> _graph;
		private readonly ICostEvaluator<TVertex, TEdge> _costEvaluator;

		PartialPathList<TVertex> _open;
		Dictionary<TVertex, PartialPath<TVertex>> _close;

		/// <summary>
		/// Creates a new instance of the pathfinder 
		/// </summary>
		/// <param name="graph">RoadGraph object that represents roads network</param>
		public Astar(IGraph<TVertex, TEdge> graph) {
			_graph = graph;

			_open = new PartialPathList<TVertex>();
			_close = new Dictionary<TVertex, PartialPath<TVertex>>();
		}

		/// <summary>
		/// Initializes internal properties before search
		/// </summary>
		/// <param name="from">The start point</param>
		/// <param name="to">The destination point</param>
		void Initialize(TVertex from, TVertex to) {
			_open.Clear();
			_close.Clear();

			// Add nodes reachable from the From point to the open list
			foreach (var edge in _graph.OutgoingEdges(from)) {
				PartialPath<TVertex> path = new PartialPath<TVertex>() {
					End = edge.Destination,
					Cost = _costEvaluator.EvaluateCost(edge),
					EstimatedCostToEnd = _costEvaluator.EstimateCost(edge.Destination, to)
				};

				if (_open.Contains(path)) {
					if (_open[path.End].Cost > path.Cost) {
						_open.Remove(_open[path.End]);
						_open.Add(path);
					}
				} else {
					_open.Add(path);
				}
			}
		}

		/// <summary>
		/// Builds result path
		/// </summary>
		/// <param name="lastPathPart"></param>
		/// <param name="from"></param>
		/// <returns></returns>
		List<TVertex> BuildPath(PartialPath<TVertex> lastPathPart, TVertex from) {
			var result = new List<TVertex>();

			while (lastPathPart.PreviousNode != null) {
				result.Add(lastPathPart.End);
				lastPathPart = _close[lastPathPart.PreviousNode];
			}
			result.Reverse();

			return result;
		}

		/// <summary>
		/// Finds path between From and To points
		/// </summary>
		/// <param name="from">The start point</param>
		/// <param name="to">The destination point</param>
		/// <param name="cost">Length of the path in meters</param>
		/// <returns>Path as list of PathSegments beteewn two point. If path wasn't found returns null.</returns>
		public List<TVertex> FindPath(TVertex from, TVertex to, ref double cost) {
			Initialize(from, to);

			while (_open.Count > 0) {
				PartialPath<TVertex> current = _open.RemoveTop();
				_close.Add(current.End, current);

				// Path found
				if (current.End.Equals(to)) {
					cost = current.Cost;
					var result = BuildPath(current, from);

					return result;
				}

				foreach (var link in _graph.OutgoingEdges(current.End)) {
					if (link.Source.Equals(current.End) == false) continue;

					double distance = _costEvaluator.EvaluateCost(link);

					if (_open.Contains(link.Destination)) {
						// Update previously found path in the open list (if this one is shorter)
						PartialPath<TVertex> p = _open[link.Destination];
						if (p.Cost > distance) {
							p.PreviousNode = current.End;
							_open.Update(p, distance);
						}
					} else if (_close.ContainsKey(link.Destination)) {
						// Update previously found path in the close list (if this one is shorter)
						if (_close[link.Destination].Cost > distance) {
							_close[link.Destination].Cost = distance;
							_close[link.Destination].End = current.End;
						}
					} else {
						// Expand path to new node
						PartialPath<TVertex> expanded = new PartialPath<TVertex>() {
							Cost = distance,
							EstimatedCostToEnd = _costEvaluator.EstimateCost(link.Destination, to),
							End = link.Destination, PreviousNode = current.End
						};
						_open.Add(expanded);
					}
				}
			}

			return null;
		}
	}
}
