using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

using SpatialLite.Gps.Geometries;

namespace Tests.SpatialLite.Gps.Geometries {
    public class GpxMetadataTests {
        #region Constructor() tests

        [Fact]
        public void Constructor_CreatesMetadataWithEmptyCollectionOfLinks() {
            GpxMetadata target = new GpxMetadataWrapper();

            Assert.Empty(target.Links);
        }

        #endregion
    }

    public class GpxMetadataWrapper : GpxMetadata {
    }
}
