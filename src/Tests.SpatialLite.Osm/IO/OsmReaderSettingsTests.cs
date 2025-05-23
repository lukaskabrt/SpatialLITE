using SpatialLite.Osm.IO;
using System;
using Xunit;

namespace Tests.SpatialLite.Osm.IO;

public class OsmReaderSettingsTests
{
    [Fact]
    public void Constructor__CreatesSettingsWithDefaultValues()
    {
        OsmReaderSettings target = new();

        Assert.True(target.ReadMetadata);
    }

    [Fact]
    public void ReadMetadataSetter_ThrowInvaldOperationExceptionIfSettingsIsReadOnly()
    {
        OsmReaderSettings target = new();
        target.IsReadOnly = true;

        Assert.Throws<InvalidOperationException>(() => target.ReadMetadata = true);
    }
}
