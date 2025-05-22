using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xunit;

using SpatialLite.Core.IO;

namespace Tests.SpatialLite.Core.IO
{
    public class WkbWriterSettingsTests
    {

        [Fact]
        public void Constructor__SetsDefaultValues()
        {
            WkbWriterSettings target = new WkbWriterSettings();

            Assert.Equal(BinaryEncoding.LittleEndian, target.Encoding);
        }

        [Fact]
        public void UseDenseFormatSetter_ThrowInvaldOperationExceptionIfSettingsIsReadOnly()
        {
            WkbWriterSettings target = new WkbWriterSettings();
            target.IsReadOnly = true;

            Assert.Throws<InvalidOperationException>(() => target.Encoding = BinaryEncoding.BigEndian);
        }
    }
}
