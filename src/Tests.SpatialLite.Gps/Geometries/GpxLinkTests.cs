using System;
using Xunit;
using SpatialLite.Gps.Geometries;

namespace Tests.SpatialLite.Gps.Geometries {
    public class GpxLinkTests {
        #region Constructor tests

        #region Constructor(Url) tests

        [Fact]
        public void Constructor_Url_SetsUrl() {
            Uri url = new Uri("http://spatial.litesolutions.net");

            GpxLink target = new GpxLink(url);

            Assert.Same(url, target.Url);
        }

        #endregion

        #endregion
    }
}
