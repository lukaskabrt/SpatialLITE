using System;
using SpatialLite.Gps.IO;
using Xunit;

namespace Tests.SpatialLite.Gps.IO
{
    public class GpxReaderSettingsTests
    {

        [Fact]
        public void Constructor__CreatesSettingsWithDefaultValues()
        {
            var target = new GpxReaderSettings();

            Assert.True(target.ReadMetadata);
        }

        [Fact]
        public void ReadMetadataSetter_ThrowInvaldOperationExceptionIfSettingsIsReadOnly()
        {
            var target = new GpxReaderSettings();
            target.IsReadOnly = true;

            Assert.Throws<InvalidOperationException>(() => target.ReadMetadata = true);
        }
    }
}
