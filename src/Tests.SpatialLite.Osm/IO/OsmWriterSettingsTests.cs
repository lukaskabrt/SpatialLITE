using System;

using Xunit;

using SpatialLite.Osm.IO;

namespace Tests.SpatialLite.Osm.IO
{
    public class OsmWriterSettingsTests
    {
        [Fact]
        public void Constructor__CreatesSettingsWithDefaultValues()
        {
            OsmWriterSettings target = new OsmWriterSettings();

            Assert.True(target.WriteMetadata);
        }

        [Fact]
        public void WriteMetadataSetter_ThrowInvaldOperationExceptionIfSettingsIsReadOnly()
        {
            OsmWriterSettings target = new OsmWriterSettings();
            target.IsReadOnly = true;

            Assert.Throws<InvalidOperationException>(() => target.WriteMetadata = true);
        }

        [Fact]
        public void ProgramNameSetter_ThrowInvaldOperationExceptionIfSettingsIsReadOnly()
        {
            OsmWriterSettings target = new OsmWriterSettings();
            target.IsReadOnly = true;

            Assert.Throws<InvalidOperationException>(() => target.ProgramName = "TEST");
        }
    }
}
