using SpatialLite.H3;

namespace SpatialLite.UnitTests.H3;

public partial class H3CellTests
{
    public class GetResolution
    {
        [Theory]
        [InlineData(0x085283473fffffffUL, 5)]
        [InlineData(0x0821c37fffffffffUL, 2)]
        [InlineData(0x08001fffffffffffUL, 0)]
        [InlineData(0x08f2830828052d25UL, 15)]
        public void ReturnsExpectedResolution_IfKnownCell(ulong rawValue, int expectedResolution)
        {
            // Given
            var cell = new H3Cell(rawValue);

            // When
            var resolution = cell.GetResolution();

            // Then
            resolution.Should().Be(expectedResolution);
        }
    }

    public new class ToString
    {
        [Fact]
        public void ReturnsSameValueAsH3ToString_IfValidCell()
        {
            // Given
            var cell = new H3Cell(0x085283473fffffffUL);

            // When
            var result = cell.ToString();

            // Then
            result.Should().Be("85283473fffffff");
        }
    }

    public class Parse
    {
        [Theory]
        [InlineData("85283473fffffff", 0x085283473fffffffUL)]
        [InlineData("821c37fffffffff", 0x0821c37fffffffffUL)]
        public void ReturnsCellWithCorrectValue_IfValidHexString(string hex, ulong expectedValue)
        {
            // When
            var cell = H3Cell.Parse(hex);

            // Then
            cell.Value.Should().Be(expectedValue);
        }

        [Fact]
        public void ThrowsArgumentException_IfInvalidString()
        {
            // Given
            static void Act() => H3Cell.Parse("xyz");

            // When
            var exception = Record.Exception(Act);

            // Then
            exception.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void ThrowsArgumentException_IfNullString()
        {
            // Given
            static void Act() => H3Cell.Parse(null!);

            // When
            var exception = Record.Exception(Act);

            // Then
            exception.Should().BeOfType<ArgumentException>();
        }
    }

    public class TryParse
    {
        [Fact]
        public void ReturnsTrueAndCorrectCell_IfValidHexString()
        {
            // When
            var success = H3Cell.TryParse("85283473fffffff", out var cell);

            // Then
            success.Should().BeTrue();
            cell.Value.Should().Be(0x085283473fffffffUL);
        }

        [Theory]
        [InlineData("xyz")]
        [InlineData(null)]
        public void ReturnsFalse_IfInvalidOrNullString(string? input)
        {
            // Given
            // input provided by [InlineData]

            // When
            var success = H3Cell.TryParse(input, out _);

            // Then
            success.Should().BeFalse();
        }
    }
}
