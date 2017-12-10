using System.Collections.Generic;
using SpatialLite.Gps.Geometries;
using Xunit;

namespace Tests.SpatialLite.Gps.Geometries {
    public class GpxFixHelperTests {
        public static IEnumerable<object[]> GpsFixes {
            get {
                yield return new object[] { GpsFix.Dgps, "dgps" };
                yield return new object[] { GpsFix.Fix2D, "2d" };
                yield return new object[] { GpsFix.Fix3D, "3d" };
                yield return new object[] { GpsFix.None, "none" };
                yield return new object[] { GpsFix.Pps, "pps" };
            }
        }

        #region ParseGpsFix tests

        [Theory]
        [MemberData(nameof(GpsFixes))]
        public void ParseGpsFix_ValidGpsFixes_ParsesStringValue(GpsFix expectedValue, string s) {
            GpsFix? parsedValue = GpxFixHelper.ParseGpsFix(s);

            Assert.Equal(expectedValue, parsedValue);
        }

        [Fact]
        public void ParseGpsFix_InvalidValue_ReturnsNull() {
            string invalidValue = "some other string";
            GpsFix? parsedValue = GpxFixHelper.ParseGpsFix(invalidValue);

            Assert.Null(parsedValue);
        }

        #endregion

        #region GpsFixToString tests

        [Theory]
        [MemberData(nameof(GpsFixes))]
        public void GpsFixToString_GpsFixes_ConvertsToStringEquivalents(GpsFix fix, string expectedValue) {
            string strValue = GpxFixHelper.GpsFixToString(fix);

            Assert.Equal(expectedValue, strValue);
        }

        #endregion

    }
}
