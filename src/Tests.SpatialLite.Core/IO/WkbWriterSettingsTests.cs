using SpatialLite.Core.IO;
using System;
using Xunit;

namespace Tests.SpatialLite.Core.IO;

public class WkbWriterSettingsTests
{

    [Fact]
    public void Constructor__SetsDefaultValues()
    {
        WkbWriterSettings target = new WkbWriterSettings();

        Assert.Equal(BinaryEncoding.LittleEndian, target.Encoding);
    }

    [Fact]
    public void UseDenseFormatSetter_ThrowInvaldOperationExceptionIfSettingsIsReadOnly()
    {
        WkbWriterSettings target = new WkbWriterSettings();
        target.IsReadOnly = true;

        Assert.Throws<InvalidOperationException>(() => target.Encoding = BinaryEncoding.BigEndian);
    }
}
