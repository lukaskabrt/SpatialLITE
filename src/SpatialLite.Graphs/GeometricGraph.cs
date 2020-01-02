using SpatialLite.Core.API;
using SpatialLite.Core.Collections;
using SpatialLite.Graphs.Api;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpatialLite.Graphs {
    public class GeometricGraph<TVertex, TEdge> : IGraph<TVertex, TEdge>
        where TVertex : IGeometry
        where TEdge : IGeometry, IEdge<TVertex> {

        private IGraph<TVertex, TEdge> _graph;
        private QuadTree<TVertex> _verticesIndex;
        private QuadTree<TEdge> _edgesIndex;

        public GeometricGraph(IGraph<TVertex, TEdge> graph, Envelope bounds) {
            _graph = graph;
            _verticesIndex = new QuadTree<TVertex>(bounds);
            _edgesIndex = new QuadTree<TEdge>(bounds);
        }

        public IEnumerable<TVertex> Vertices => _graph.Vertices;

        public IEnumerable<TEdge> Edges => _graph.Edges;

        public bool AddEdge(TEdge edge) {
            if (_graph.AddEdge(edge)) {
                _edgesIndex.Add(edge);
                return true;
            }

            return false;
        }

        public bool AddVertex(TVertex vertex) {
            if (_graph.AddVertex(vertex)) {
                _verticesIndex.Add(vertex);
                return true;
            }

            return false;
        }

        public void Clear() {
            _edgesIndex.Clear();
            _verticesIndex.Clear();

            _graph.Clear();
        }

        public bool HasEdge(TVertex firstVertex, TVertex secondVertex) => _graph.HasEdge(firstVertex, secondVertex);

        public bool HasVertex(TVertex vertex) => _graph.HasVertex(vertex);

        public IEnumerable<TEdge> IncomingEdges(TVertex vertex) => _graph.IncomingEdges(vertex);

        public IEnumerable<TEdge> OutgoingEdges(TVertex vertex) => _graph.OutgoingEdges(vertex);

        public bool RemoveEdge(TEdge edge) {
            if (_graph.RemoveEdge(edge)) {
                _edgesIndex.Remove(edge);
                return true;
            }

            return false;
        }

        public bool RemoveVertex(TVertex vertex) {
            if (_graph.RemoveVertex(vertex)) {
                _verticesIndex.Remove(vertex);
                return true;
            }

            return false;
        }

        public IEnumerable<TVertex> GetVertices(Envelope rect) {
            return _verticesIndex.GetObjects(rect);
        }

        public IEnumerable<TEdge> GetEdges(Envelope rect) {
            return _edgesIndex.GetObjects(rect);
        }
    }
}
