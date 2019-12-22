using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SpatialLite.Core;
using SpatialLite.Core.API;
using SpatialLite.Core.Algorithms;

using Xunit;
using Moq;

namespace Tests.SpatialLite.Core {
	public class TopologyTests {

		[Fact]
		public void Euclidean2D_GetTopologyInstanceWithEuclidean2DLocator() {
			Assert.IsType<Euclidean2DLocator>(Topology.Euclidean2D.GeometryLocator);
		}

	}
}
