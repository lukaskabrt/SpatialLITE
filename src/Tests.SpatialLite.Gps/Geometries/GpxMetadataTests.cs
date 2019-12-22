using Xunit;

using SpatialLite.Gps.Geometries;

namespace Tests.SpatialLite.Gps.Geometries {
    public class GpxMetadataTests {

        [Fact]
        public void Constructor_CreatesMetadataWithEmptyCollectionOfLinks() {
            GpxMetadata target = new GpxMetadataWrapper();

            Assert.Empty(target.Links);
        }
    }

    public class GpxMetadataWrapper : GpxMetadata {
    }
}
