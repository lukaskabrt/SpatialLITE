using System;

using Xunit;

using SpatialLite.Osm.IO;

namespace Tests.SpatialLite.Osm.IO {
    public class OsmWriterSettingsTests {

		#region Constructor() tests

		[Fact]
		public void Constructor__CreatesSettingsWithDefaultValues() {
			OsmWriterSettings target = new OsmWriterSettings();

			Assert.True(target.WriteMetadata);
		}

		#endregion

		#region WriteMetadata property tests

		[Fact]
		public void WriteMetadataSetter_ThrowInvaldOperationExceptionIfSettingsIsReadOnly() {
			OsmWriterSettings target = new OsmWriterSettings();
			target.IsReadOnly = true;

			Assert.Throws<InvalidOperationException>(() => target.WriteMetadata = true);
		}

		#endregion


		#region ProgramName property tests

		[Fact]
		public void ProgramNameSetter_ThrowInvaldOperationExceptionIfSettingsIsReadOnly() {
			OsmWriterSettings target = new OsmWriterSettings();
			target.IsReadOnly = true;

			Assert.Throws<InvalidOperationException>(() => target.ProgramName = "TEST");
		}

		#endregion
	}
}
