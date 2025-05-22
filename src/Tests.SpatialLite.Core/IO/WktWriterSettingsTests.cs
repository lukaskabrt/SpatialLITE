using SpatialLite.Core.IO;
using Xunit;

namespace Tests.SpatialLite.Core.IO;

public class WktWriterSettingsTests
{

    [Fact]
    public void Constructor__SetsDefaultValues()
    {
        WktWriterSettings target = new WktWriterSettings();

        Assert.False(target.IsReadOnly);
    }
}
