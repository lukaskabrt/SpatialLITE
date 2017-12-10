using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xunit;

using SpatialLite.Core.IO;

namespace Tests.SpatialLite.Core.IO {
	public class WkbWriterSettingsTests {
		#region Constructor() tests

		[Fact]
		public void Constructor__SetsDefaultValues() {
			WkbWriterSettings target = new WkbWriterSettings();

			Assert.Equal(BinaryEncoding.LittleEndian, target.Encoding);
		}

		#endregion

		#region Encoding property tests

		[Fact]
		public void UseDenseFormatSetter_ThrowInvaldOperationExceptionIfSettingsIsReadOnly() {
			WkbWriterSettings target = new WkbWriterSettings();
			target.IsReadOnly = true;

			Assert.Throws<InvalidOperationException>(() => target.Encoding = BinaryEncoding.BigEndian);
		}

		#endregion
	}
}
