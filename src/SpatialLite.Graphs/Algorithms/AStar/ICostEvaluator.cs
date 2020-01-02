using SpatialLite.Graphs.Api;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpatialLite.Graphs.Algorithms.AStar {
    public interface ICostEvaluator<TVertex, TEdge> where TEdge : IEdge<TVertex> {
        double EvaluateCost(TEdge edge);
        double EstimateCost(TVertex source, TVertex destination);
    }
}
