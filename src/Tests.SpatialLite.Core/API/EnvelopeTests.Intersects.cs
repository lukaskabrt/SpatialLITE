using FluentAssertions;
using SpatialLite.Core.Api;
using Xunit;

namespace Tests.SpatialLite.Core.API
{
    public partial class EnvelopeTests
    {
        /* Intersects(Envelope) */

        public static TheoryData<Envelope2D> IntersectingEnvelopes => new()
        {
            { new Envelope2D(new[] { new Coordinate(2, 2), new Coordinate(0, 0) }) },
            { new Envelope2D(new[] { new Coordinate(2, -2), new Coordinate(0, 0) }) },
            { new Envelope2D(new[] { new Coordinate(-2, 2), new Coordinate(0, 0) }) },
            { new Envelope2D(new[] { new Coordinate(-2, -2), new Coordinate(0, 0) }) }
        };

        [Theory]
        [MemberData(nameof(IntersectingEnvelopes))]
        public void Intersects_ReturnsTrueForIntersectingEnvelopes(Envelope2D other)
        {
            var target = new Envelope2D(new[] { new Coordinate(1, 1), new Coordinate(-1, -1) });

            target.Intersects(other).Should().BeTrue();
        }

        [Fact]
        public void Intersects_ReturnsTrueIfTargetEnvelopeCoversOtherEnvelope()
        {
            var target = new Envelope2D(new[] { new Coordinate(10, 20), new Coordinate(-10, -20) });
            var other = new Envelope2D(new[] { new Coordinate(1, 2), new Coordinate(-1, -2) });

            target.Intersects(other).Should().BeTrue();
        }

        [Fact]
        public void Intersects_ReturnsTrueIfOtherEnvelopeCoversTargetEnvelope()
        {
            var target = new Envelope2D(new[] { new Coordinate(1, 2), new Coordinate(-1, -2) });
            var other = new Envelope2D(new[] { new Coordinate(10, 20), new Coordinate(-10, -20) });

            target.Intersects(other).Should().BeTrue();
        }

        [Fact]
        public void Intersects_ReturnsTrueIfBothEnvelopesAreSame()
        {
            var target = new Envelope2D(new[] { new Coordinate(1, 2), new Coordinate(-1, -2) });
            var other = new Envelope2D(new[] { new Coordinate(1, 2), new Coordinate(-1, -2) });

            target.Intersects(other).Should().BeTrue();
        }

        public static TheoryData<Envelope2D> TouchingEnvelopes => new()
        {
            { new Envelope2D(new[] { new Coordinate(2, 2), new Coordinate(1, 1) }) },
            { new Envelope2D(new[] { new Coordinate(2, -2), new Coordinate(1, -1) }) },
            { new Envelope2D(new[] { new Coordinate(-2, 2), new Coordinate(-1, 1) }) },
            { new Envelope2D(new[] { new Coordinate(-2, -2), new Coordinate(-1, -1) }) }
        };

        [Theory]
        [MemberData(nameof(TouchingEnvelopes))]
        public void Intersects_ReturnsTrueForTouchingEnvelopes(Envelope2D other)
        {
            var target = new Envelope2D(new[] { new Coordinate(1, 1), new Coordinate(-1, -1) });

            target.Intersects(other).Should().BeTrue();
        }

        [Fact]
        public void Intersects_ReturnsFalseIfTargetEnvelopeIsEmpty()
        {
            var other = new Envelope2D(new[] { new Coordinate(1, 2), new Coordinate(-1, -2) });

            Envelope2D.Empty.Intersects(other).Should().BeFalse();
        }

        [Fact]
        public void Intersects_ReturnsFalseIfOtherEnvelopeIsEmpty()
        {
            var target = new Envelope2D(new[] { new Coordinate(1, 2), new Coordinate(-1, -2) });

            target.Intersects(Envelope2D.Empty).Should().BeFalse();
        }

        public static TheoryData<Envelope2D> NonIntersectingEnvelopes => new()
        {
            { new Envelope2D(new[] { new Coordinate(20, 20), new Coordinate(10, 10) }) },
            { new Envelope2D(new[] { new Coordinate(20, -20), new Coordinate(10, -10) }) },
            { new Envelope2D(new[] { new Coordinate(-20, 20), new Coordinate(-10, 10) }) },
            { new Envelope2D(new[] { new Coordinate(-20, -20), new Coordinate(-10, -10) }) }
        };

        [Theory]
        [MemberData(nameof(NonIntersectingEnvelopes))]
        public void Intersects_ReturnsFalseForNonIntersectingEnvelopes(Envelope2D other)
        {
            var target = new Envelope2D(new[] { new Coordinate(1, 1), new Coordinate(-1, -1) });

            target.Intersects(other).Should().BeFalse();
        }
    }
}
