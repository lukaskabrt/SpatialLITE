using SpatialLite.Osm.IO;
using System;
using Xunit;

namespace Tests.SpatialLite.Osm.IO;

public class OsmWriterSettingsTests
{
    [Fact]
    public void Constructor__CreatesSettingsWithDefaultValues()
    {
        OsmWriterSettings target = new();

        Assert.True(target.WriteMetadata);
    }

    [Fact]
    public void WriteMetadataSetter_ThrowInvaldOperationExceptionIfSettingsIsReadOnly()
    {
        OsmWriterSettings target = new();
        target.IsReadOnly = true;

        Assert.Throws<InvalidOperationException>(() => target.WriteMetadata = true);
    }

    [Fact]
    public void ProgramNameSetter_ThrowInvaldOperationExceptionIfSettingsIsReadOnly()
    {
        OsmWriterSettings target = new();
        target.IsReadOnly = true;

        Assert.Throws<InvalidOperationException>(() => target.ProgramName = "TEST");
    }
}
