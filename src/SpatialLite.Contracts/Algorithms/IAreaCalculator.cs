namespace SpatialLite.Contracts.Algorithms;

/// <summary>
/// Provides methods for calculating area.
/// </summary>
public interface IAreaCalculator
{
    /// <summary>
    /// Calculates area of an envelope.
    /// </summary>
    /// <param name="envelope">The envelope to compute area for.</param>
    /// <returns>
    /// Area in square coordinate units.
    /// </returns>
    public double CalculateArea(Envelope envelope);
}
