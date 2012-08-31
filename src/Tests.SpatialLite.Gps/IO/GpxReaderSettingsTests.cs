using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpatialLite.Gps.IO;
using Xunit;

namespace Tests.SpatialLite.Gps.IO {
    public class GpxReaderSettingsTests {
        #region Constructor() tests

        [Fact]
        public void Constructor__CreatesSettingsWithDefaultValues() {
            var target = new GpxReaderSettings();

            Assert.Equal(true, target.ReadMetadata);
        }

        #endregion

        #region ReadMetadata property tests

        [Fact]
        public void ReadMetadataSetter_ThrowInvaldOperationExceptionIfSettingsIsReadOnly() {
            var target = new GpxReaderSettings();
            target.IsReadOnly = true;

            Assert.Throws<InvalidOperationException>(() => target.ReadMetadata = true);
        }

        #endregion
    }
}
