using SpatialLite.Graphs.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpatialLite.Graphs {
    public class DirectedSparseGraph<TVertex, TEdge> : IGraph<TVertex, TEdge> where TEdge : IEdge<TVertex> {
        protected Dictionary<TVertex, List<TEdge>> _adjacencyList;

        public IEnumerable<TVertex> Vertices => _adjacencyList.Keys;

        public IEnumerable<TEdge> Edges => _adjacencyList.SelectMany(e => e.Value);

        public bool AddEdge(TEdge edge) {
            if (!this.HasVertex(edge.Source) || !this.HasVertex(edge.Destination)) {
                return false;
            }

            _adjacencyList[edge.Source].Add(edge);
            return true;
        }

        public bool AddVertex(TVertex vertex) {
            if (this.HasVertex(vertex)) {
                return false;
            }

            _adjacencyList.Add(vertex, new List<TEdge>());
            return true;
        }

        public virtual void Clear() {
            _adjacencyList.Clear();
        }

        public virtual bool HasEdge(TVertex source, TVertex destination) {
            return
                _adjacencyList.ContainsKey(source) &&
                _adjacencyList[source].Any(e => e.Destination.Equals(destination));
        }

        public virtual bool HasVertex(TVertex vertex) {
            return _adjacencyList.ContainsKey(vertex);
        }

        public virtual IEnumerable<TEdge> IncomingEdges(TVertex vertex) {
            if (!this.HasVertex(vertex)) {
                throw new KeyNotFoundException("Vertex doesn't belong to graph.");
            }

            foreach (var adjacent in _adjacencyList) {
                foreach (var edge in adjacent.Value) {
                    if (edge.Destination.Equals(vertex)) {
                        yield return edge;
                    }
                }
            }
        }

        public virtual IEnumerable<TEdge> OutgoingEdges(TVertex vertex) {
            if (!_adjacencyList.TryGetValue(vertex, out var edges)) {
                throw new KeyNotFoundException("Vertex doesn't belong to graph.");
            }

            return edges;
        }

        public virtual bool RemoveEdge(TEdge edge) {
            // Check existence of nodes and non-existence of edge
            if (!this.HasEdge(edge.Source, edge.Destination)) {
                return false; }

            var sourceEdges = _adjacencyList[edge.Source];
            foreach (var sourceEdge in sourceEdges.ToList()) {
                if (sourceEdge.Destination.Equals(sourceEdge.Destination)) {
                    sourceEdges.Remove(sourceEdge);
                }
            }
            
            return true;
        }

        public virtual bool RemoveVertex(TVertex vertex) {
            if (!this.HasVertex(vertex)) {
                return false;
            }

            _adjacencyList.Remove(vertex);
            foreach (var adjacent in _adjacencyList) {
                if (adjacent.Value.Any(e => e.Destination.Equals(vertex))) {
                    foreach (var edge in adjacent.Value.ToList()) {
                        if (edge.Destination.Equals(vertex)) {
                            adjacent.Value.Remove(edge);
                        }
                    }
                }
            }

            return true;
        }
    }
}
