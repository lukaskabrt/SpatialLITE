using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xunit;

using SpatialLite.Core.IO;

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
