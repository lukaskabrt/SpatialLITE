using FluentAssertions;
using FluentAssertions.Execution;
using SpatialLite.Core.Api;

namespace Tests.SpatialLite.Core.FluentAssertions
{
    public static class EnvelopeExtensions
    {
        public static void ShouldHaveBounds(this Envelope2D envelope, double minX, double maxX, double minY, double maxY)
        {
            using (new AssertionScope())
            {
                envelope.MinX.Should().Be(minX);
                envelope.MaxX.Should().Be(maxX);
                envelope.MinY.Should().Be(minY);
                envelope.MaxY.Should().Be(maxY);
            }
        }

        public static void ShouldHaveSameBounds(this Envelope2D envelope, Envelope2D expected)
        {
            using (new AssertionScope())
            {
                envelope.MinX.Should().Be(expected.MinX);
                envelope.MaxX.Should().Be(expected.MaxX);
                envelope.MinY.Should().Be(expected.MinY);
                envelope.MaxY.Should().Be(expected.MaxY);
            }
        }
    }
}
