using System;
using System.Collections.Generic;
using System.Text;

namespace SpatialLite.Graphs.Api {
    public interface IGraph<TVertex, TEdge> where TEdge : IEdge<TVertex> {
        /// <summary>
        /// Returns the list of Vertices.
        /// </summary>
        IEnumerable<TVertex> Vertices { get; }

        /// <summary>
        /// An enumerable collection of edges.
        /// </summary>
        IEnumerable<TEdge> Edges { get; }

        /// <summary>
        /// Get all incoming edges from vertex
        /// </summary>
        IEnumerable<TEdge> IncomingEdges(TVertex vertex);

        /// <summary>
        /// Get all outgoing edges from vertex
        /// </summary>
        IEnumerable<TEdge> OutgoingEdges(TVertex vertex);

        /// <summary>
        /// Connects two vertices together.
        /// </summary>
        bool AddEdge(TEdge edge);

        /// <summary>
        /// Deletes an edge, if exists, between two vertices.
        /// </summary>
        bool RemoveEdge(TEdge edge);

        /// <summary>
        /// Adds a new vertex to graph.
        /// </summary>
        bool AddVertex(TVertex vertex);

        /// <summary>
        /// Removes the specified vertex from graph.
        /// </summary>
        bool RemoveVertex(TVertex vertex);

        /// <summary>
        /// Checks whether two vertices are connected (there is an edge between firstVertex & secondVertex)
        /// </summary>
        bool HasEdge(TVertex firstVertex, TVertex secondVertex);

        /// <summary>
        /// Determines whether this graph has the specified vertex.
        /// </summary>
        bool HasVertex(TVertex vertex);

        /// <summary>
        /// Clear this graph.
        /// </summary>
        void Clear();
    }
}
