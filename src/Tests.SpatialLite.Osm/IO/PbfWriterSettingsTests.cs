using System;

using Xunit;

using SpatialLite.Osm.IO;

namespace Tests.SpatialLite.Osm.IO {
    public class PbfWriterSettingsTests {

		#region Constructor() tests

		[Fact]
		public void Constructor__SetsDefaultValues() {
			PbfWriterSettings target = new PbfWriterSettings();

			Assert.Equal(true, target.UseDenseFormat);
			Assert.Equal(CompressionMode.ZlibDeflate, target.Compression);
			Assert.Equal(true, target.WriteMetadata);
		}

		#endregion

		#region UseDenseFormat property tests

		[Fact]
		public void UseDenseFormatSetter_ThrowInvaldOperationExceptionIfSettingsIsReadOnly() {
			PbfWriterSettings target = new PbfWriterSettings();
			target.IsReadOnly = true;

			Assert.Throws<InvalidOperationException>(() => target.UseDenseFormat = false);
		}

		#endregion

		#region Compression property tests

		[Fact]
		public void ConpressionSetter_ThrowInvaldOperationExceptionIfSettingsIsReadOnly() {
			PbfWriterSettings target = new PbfWriterSettings();
			target.IsReadOnly = true;

			Assert.Throws<InvalidOperationException>(() => target.Compression = CompressionMode.ZlibDeflate);
		}

		#endregion
	}
}
