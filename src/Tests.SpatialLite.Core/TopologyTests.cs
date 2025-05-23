using SpatialLite.Core;
using SpatialLite.Core.Algorithms;
using Xunit;

namespace Tests.SpatialLite.Core;

public class TopologyTests
{

    [Fact]
    public void Euclidean2D_GetTopologyInstanceWithEuclidean2DLocator()
    {
        Assert.IsType<Euclidean2DLocator>(Topology.Euclidean2D.GeometryLocator);
    }
}
