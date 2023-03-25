namespace SpatialLite.Core.Api;

/// <summary>
/// Represents a collection of <see cref="IPolygon"/>.
/// </summary>
public interface IMultiPolygon : IGeometryCollection<IPolygon>
{
}
