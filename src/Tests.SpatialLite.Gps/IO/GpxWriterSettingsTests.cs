using System;
using SpatialLite.Gps.IO;
using Xunit;

namespace Tests.SpatialLite.Gps.IO {
    public class GpxWriterSettingsTests {
        #region Constructor() tests

        [Fact]
        public void Constructor__CreatesSettingsWithDefaultValues() {
            var target = new GpxWriterSettings();

            Assert.True(target.WriteMetadata);
        }

        #endregion

        #region WriteMetadata property tests

        [Fact]
        public void WriteMetadataSetter_ThrowInvaldOperationExceptionIfSettingsIsReadOnly() {
            var target = new GpxWriterSettings();
            target.IsReadOnly = true;

            Assert.Throws<InvalidOperationException>(() => target.WriteMetadata = true);
        }

        #endregion
        
        #region ReadMetadata property tests

        [Fact]
        public void GeneratorNameSetter_ThrowInvaldOperationExceptionIfSettingsIsReadOnly() {
            var target = new GpxWriterSettings();
            target.IsReadOnly = true;

            Assert.Throws<InvalidOperationException>(() => target.GeneratorName = "TEST");
        }

        #endregion

    }
}
