using SpatialLite.Gps.Geometries;
using Xunit;

namespace Tests.SpatialLite.Gps.Geometries;

public class GpxMetadataTests
{

    [Fact]
    public void Constructor_CreatesMetadataWithEmptyCollectionOfLinks()
    {
        GpxMetadata target = new GpxMetadataWrapper();

        Assert.Empty(target.Links);
    }
}

public class GpxMetadataWrapper : GpxMetadata
{
}
