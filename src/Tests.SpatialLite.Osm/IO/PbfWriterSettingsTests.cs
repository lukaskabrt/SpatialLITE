using SpatialLite.Osm.IO;
using System;
using Xunit;

namespace Tests.SpatialLite.Osm.IO;

public class PbfWriterSettingsTests
{
    [Fact]
    public void Constructor__SetsDefaultValues()
    {
        PbfWriterSettings target = new();

        Assert.True(target.UseDenseFormat);
        Assert.Equal(CompressionMode.ZlibDeflate, target.Compression);
        Assert.True(target.WriteMetadata);
    }

    [Fact]
    public void UseDenseFormatSetter_ThrowInvaldOperationExceptionIfSettingsIsReadOnly()
    {
        PbfWriterSettings target = new();
        target.IsReadOnly = true;

        Assert.Throws<InvalidOperationException>(() => target.UseDenseFormat = false);
    }

    [Fact]
    public void ConpressionSetter_ThrowInvaldOperationExceptionIfSettingsIsReadOnly()
    {
        PbfWriterSettings target = new();
        target.IsReadOnly = true;

        Assert.Throws<InvalidOperationException>(() => target.Compression = CompressionMode.ZlibDeflate);
    }
}
