using SpatialLite.Contracts;
using SpatialLite.Contracts.Algorithms;

namespace SpatialLite.Core.Algorithms;

/// <summary>
/// Provides methods for calculating area in 2D Euclidean space.
/// </summary>
public class EuclideanAreaCalculator : IAreaCalculator
{
    /// <summary>
    /// Calculates area of an envelope.
    /// </summary>
    /// <param name="envelope">The envelope to compute area for.</param>
    /// <returns>
    /// Area in square coordinate units.
    /// Returns <c>0</c> for empty or collapsed envelopes.
    /// </returns>
    public double CalculateArea(Envelope envelope)
    {
        if (envelope.IsEmpty)
        {
            return 0;
        }

        return envelope.Width * envelope.Height;
    }
}
