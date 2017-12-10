using System;

using Xunit;

using SpatialLite.Osm.IO;

namespace Tests.SpatialLite.Osm.IO {
    public class OsmReaderSettingsTests {

		#region Constructor() tests

		[Fact]
		public void Constructor__CreatesSettingsWithDefaultValues() {
			OsmReaderSettings target = new OsmReaderSettings();

			Assert.True(target.ReadMetadata);
		}

		#endregion

		#region ReadMetadata property tests
	
		[Fact]
		public void ReadMetadataSetter_ThrowInvaldOperationExceptionIfSettingsIsReadOnly() {
			OsmReaderSettings target = new OsmReaderSettings();
			target.IsReadOnly = true;

			Assert.Throws<InvalidOperationException>(() => target.ReadMetadata = true);
		}

		#endregion
	}
}
