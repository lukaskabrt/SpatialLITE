using SpatialLite.Gps.Geometries;
using System;
using Xunit;

namespace Tests.SpatialLite.Gps.Geometries;

public class GpxLinkTests
{

    [Fact]
    public void Constructor_Url_SetsUrl()
    {
        Uri url = new Uri("http://spatial.litesolutions.net");

        GpxLink target = new GpxLink(url);

        Assert.Same(url, target.Url);
    }
}
