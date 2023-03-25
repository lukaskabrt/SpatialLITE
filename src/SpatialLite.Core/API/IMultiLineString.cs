namespace SpatialLite.Core.Api;

/// <summary>
/// Represents a collection of <see cref="ILineString"/>.
/// </summary>
public interface IMultiLineString : IGeometryCollection<ILineString>
{
}
