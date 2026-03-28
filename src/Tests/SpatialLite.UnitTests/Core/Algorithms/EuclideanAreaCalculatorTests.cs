using SpatialLite.Contracts;
using SpatialLite.Core.Algorithms;

namespace SpatialLite.UnitTests.Core.Algorithms;

public class EuclideanAreaCalculatorTests
{
    private const int Precision = 6;

    private readonly EuclideanAreaCalculator _calculator = new();

    [Fact]
    public void CalculateArea_ReturnsArea_IfEnvelopeIsNonEmpty()
    {
        // Given
        var envelope = new Envelope(
        [
            new Coordinate(1, 2),
            new Coordinate(4, 6)
        ]);

        // When
        var area = _calculator.CalculateArea(envelope);

        // Then
        Assert.Equal(12.0, area, Precision);
    }

    [Fact]
    public void CalculateArea_ReturnsZero_IfEnvelopeIsEmpty()
    {
        // Given
        var envelope = Envelope.Empty;

        // When
        var area = _calculator.CalculateArea(envelope);

        // Then
        Assert.Equal(0.0, area, Precision);
    }

    [Theory]
    [InlineData(2, 1, 2, 5)]
    [InlineData(2, 1, 6, 1)]
    public void CalculateArea_ReturnsZero_IfEnvelopeIsCollapsedInOneDimension(double x1, double y1, double x2, double y2)
    {
        // Given
        var envelope = new Envelope(
        [
            new Coordinate(x1, y1),
            new Coordinate(x2, y2)
        ]);

        // When
        var area = _calculator.CalculateArea(envelope);

        // Then
        Assert.Equal(0.0, area, Precision);
    }
}
