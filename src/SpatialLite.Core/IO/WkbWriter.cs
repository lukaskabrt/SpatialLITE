using SpatialLite.Core.API;
using System;
using System.IO;
using System.Linq;

namespace SpatialLite.Core.IO;

/// <summary>
/// Provides function for writing geometry objects into WKB format.
/// </summary>
public class WkbWriter : IDisposable
{

    private bool _disposed = false;
    private readonly BinaryWriter _writer = null;
    private readonly Stream _output = null;

    /// <summary>
    /// Initializes a new instance of the WkbWriter class with specific settings.
    /// </summary>
    /// <param name="settings">The settings defining behaviour of the writer.</param>
    protected WkbWriter(WkbWriterSettings settings)
    {
        if (settings == null)
        {
            throw new ArgumentNullException(nameof(settings));
        }

        if (settings.Encoding == BinaryEncoding.BigEndian)
        {
            throw new NotSupportedException("BigEndian encoding is not supported in current version of the WkbWriter.");
        }

        Settings = settings;
        Settings.IsReadOnly = true;
    }

    /// <summary>
    /// Initializes a new instance of the WkbWriter class that writes geometries to a stream with specific settings.
    /// </summary>
    /// <param name="stream">The stream to write geometries to.</param>
    /// <param name="settings">The settings defining behaviour of the writer.</param>
    public WkbWriter(Stream stream, WkbWriterSettings settings)
        : this(settings)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        _writer = new BinaryWriter(stream);
    }

    /// <summary>
    /// Initializes a new instance of the WkbWriter class that writes geometreis to a file with specific settings.
    /// </summary>
    /// <param name="path">The path to the file to write geometrie to.</param>
    /// <param name="settings">The settings defining behaviour of the writer.</param>
    public WkbWriter(string path, WkbWriterSettings settings)
        : this(settings)
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        _output = new FileStream(path, FileMode.Create, FileAccess.Write);
        _writer = new BinaryWriter(_output);
    }

    /// <summary>
    /// Gets the settings that detedmine behaviour of this instance of the WkbWriter.
    /// </summary>
    public WkbWriterSettings Settings { get; private set; }

    /// <summary>
    /// Writes specified Geometry in the WKB format to a binary arrray using default WkbWriterSettings.
    /// </summary>
    /// <param name="geometry">The geometry to write.</param>
    /// <returns>The binary array with WKB representation of the Geometry.</returns>
    public static byte[] WriteToArray(IGeometry geometry)
    {
        using (MemoryStream dataStream = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(dataStream))
            {
                WkbWriterSettings defaultSettings = new WkbWriterSettings();

                WriteEncoding(writer, defaultSettings.Encoding);
                Write(geometry, writer);

                return dataStream.ToArray();
            }
        }
    }

    /// <summary>
    /// Releases all resources used by the WkbWriter.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Writes specified Geometry in the WKB format to the output.
    /// </summary>
    /// <param name="geometry">The geometry to write.</param>
    public void Write(IGeometry geometry)
    {
        WriteEncoding(_writer, Settings.Encoding);
        Write(geometry, _writer);
    }

    /// <summary>
    /// Writes the Encoding byte defined by <see cref="WkbWriterSettings.Encoding"/> property to the output stream.
    /// </summary>
    /// <param name="writer">The BinaryWriter to be used.</param>
    /// <param name="encoding">The encoding of binary data.</param>
    private static void WriteEncoding(BinaryWriter writer, BinaryEncoding encoding)
    {
        writer.Write((byte)encoding);
    }

    /// <summary>
    /// Writes geometry to the output using specified writer.
    /// </summary>
    /// <param name="geometry">The geometry to write.</param>
    /// <param name="writer">The BinaryWriter used to write geometry to the output.</param>
    private static void Write(IGeometry geometry, BinaryWriter writer)
    {
        if (geometry is IPoint)
        {
            WritePoint((IPoint)geometry, writer);
        }
        else if (geometry is ILineString)
        {
            WriteLineString((ILineString)geometry, writer);
        }
        else if (geometry is IPolygon)
        {
            WritePolygon((IPolygon)geometry, writer);
        }
        else if (geometry is IMultiPoint)
        {
            WriteMultiPoint((IMultiPoint)geometry, writer);
        }
        else if (geometry is IMultiLineString)
        {
            WriteMultiLineString((IMultiLineString)geometry, writer);
        }
        else if (geometry is IMultiPolygon)
        {
            WriteMultiPolygon((IMultiPolygon)geometry, writer);
        }
        else if (geometry is IGeometryCollection<IGeometry>)
        {
            WriteGeometryCollection((IGeometryCollection<IGeometry>)geometry, writer);
        }
    }

    /// <summary>
    /// Writes Coordinate to the output using specified writer.
    /// </summary>
    /// <param name="coordinate">The Coordinate to write.</param>
    /// <param name="writer">The BinaryWriter used to write geometry to the output.</param>
    private static void WriteCoordinate(Coordinate coordinate, BinaryWriter writer)
    {
        writer.Write(coordinate.X);
        writer.Write(coordinate.Y);

        if (coordinate.Is3D)
        {
            writer.Write(coordinate.Z);
        }

        if (coordinate.IsMeasured)
        {
            writer.Write(coordinate.M);
        }
    }

    /// <summary>
    /// Writes list of Coordinates to the output using specified writer.
    /// </summary>
    /// <param name="coordinates">The list od Coordinates to write.</param>
    /// <param name="writer">The BinaryWriter used to write geometry to the output.</param>
    private static void WriteCoordinates(ICoordinateList coordinates, BinaryWriter writer)
    {
        writer.Write((uint)coordinates.Count);

        for (int i = 0; i < coordinates.Count; i++)
        {
            WriteCoordinate(coordinates[i], writer);
        }
    }

    /// <summary>
    /// Writes Point to the output using specified writer.
    /// </summary>
    /// <param name="point">The Point to write.</param>
    /// <param name="writer">The BinaryWriter used to write geometry to the output.</param>
    private static void WritePoint(IPoint point, BinaryWriter writer)
    {
        if (point.Position.Equals(Coordinate.Empty))
        {
            writer.Write((uint)WkbGeometryType.GeometryCollection);
            writer.Write(0u);
        }
        else
        {
            writer.Write((uint)AdjustGeometryType(point, WkbGeometryType.Point));
            WriteCoordinate(point.Position, writer);
        }
    }

    /// <summary>
    /// Writes LineString to the output using specified writer.
    /// </summary>
    /// <param name="linestring">The LineString to write.</param>
    /// <param name="writer">The BinaryWriter used to write geometry to the output.</param>
    private static void WriteLineString(ILineString linestring, BinaryWriter writer)
    {
        writer.Write((uint)AdjustGeometryType(linestring, WkbGeometryType.LineString));
        WriteCoordinates(linestring.Coordinates, writer);
    }

    /// <summary>
    /// Writes Polygon to the output using specified writer.
    /// </summary>
    /// <param name="polygon">The Polygon to write.</param>
    /// <param name="writer">The BinaryWriter used to write geometry to the output.</param>
    private static void WritePolygon(IPolygon polygon, BinaryWriter writer)
    {
        writer.Write((uint)AdjustGeometryType(polygon, WkbGeometryType.Polygon));
        WritePolygonContent(polygon, writer);
    }

    /// <summary>
    /// Writes content of the Polygon to the output using specified writer.
    /// </summary>
    /// <param name="polygon">The Polygon to write.</param>
    /// <param name="writer">The BinaryWriter used to write geometry to the output.</param>
    private static void WritePolygonContent(IPolygon polygon, BinaryWriter writer)
    {
        if (polygon.ExteriorRing.Count == 0)
        {
            writer.Write(0u);
        }
        else
        {
            writer.Write((uint)(1 + polygon.InteriorRings.Count()));
            WriteCoordinates(polygon.ExteriorRing, writer);

            foreach (var ring in polygon.InteriorRings)
            {
                WriteCoordinates(ring, writer);
            }
        }
    }

    /// <summary>
    /// Writes MultiPoint to the output using specified writer.
    /// </summary>
    /// <param name="multipoint">The MultiPoint to write.</param>
    /// <param name="writer">The BinaryWriter used to write geometry to the output.</param>
    private static void WriteMultiPoint(IMultiPoint multipoint, BinaryWriter writer)
    {
        writer.Write((uint)AdjustGeometryType(multipoint, WkbGeometryType.MultiPoint));
        writer.Write((uint)multipoint.Geometries.Count());
        foreach (var point in multipoint.Geometries)
        {
            WriteCoordinate(point.Position, writer);
        }
    }

    /// <summary>
    /// Writes MultiLineString to the output using specified writer.
    /// </summary>
    /// <param name="multiLineString">The MultiLineString to write.</param>
    /// <param name="writer">The BinaryWriter used to write geometry to the output.</param>
    private static void WriteMultiLineString(IMultiLineString multiLineString, BinaryWriter writer)
    {
        writer.Write((uint)AdjustGeometryType(multiLineString, WkbGeometryType.MultiLineString));
        writer.Write((uint)multiLineString.Geometries.Count());
        foreach (var linestring in multiLineString.Geometries)
        {
            WriteCoordinates(linestring.Coordinates, writer);
        }
    }

    /// <summary>
    /// Writes MultiPolygon to the output using specified writer.
    /// </summary>
    /// <param name="multiPolygon">The MultiPolygon to write.</param>
    /// <param name="writer">The BinaryWriter used to write geometry to the output.</param>
    private static void WriteMultiPolygon(IMultiPolygon multiPolygon, BinaryWriter writer)
    {
        writer.Write((uint)AdjustGeometryType(multiPolygon, WkbGeometryType.MultiPolygon));
        writer.Write((uint)multiPolygon.Geometries.Count());
        foreach (var polygon in multiPolygon.Geometries)
        {
            WritePolygonContent(polygon, writer);
        }
    }

    /// <summary>
    /// Writes GeometryCollection to the output using specified writer.
    /// </summary>
    /// <param name="collection">The GeometryCollection to write.</param>
    /// <param name="writer">The BinaryWriter used to write geometry to the output.</param>
    private static void WriteGeometryCollection(IGeometryCollection<IGeometry> collection, BinaryWriter writer)
    {
        writer.Write((uint)AdjustGeometryType(collection, WkbGeometryType.GeometryCollection));
        writer.Write((uint)collection.Geometries.Count());
        foreach (var geometry in collection.Geometries)
        {
            Write(geometry, writer);
        }
    }

    /// <summary>
    /// Adjust WkbGeometryType according to the Geometry object dimensions.
    /// </summary>
    /// <param name="geometry">The geometry object.</param>
    /// <param name="baseType">WkbGeometryType for the 2D, non-measured version of the geometry object.</param>
    /// <returns>The WkbGeometryType of the geometry object.</returns>
    private static WkbGeometryType AdjustGeometryType(IGeometry geometry, WkbGeometryType baseType)
    {
        WkbGeometryType result = baseType;

        if (geometry.Is3D)
        {
            result += 1000;
        }

        if (geometry.IsMeasured)
        {
            result += 2000;
        }

        return result;
    }

    /// <summary>
    /// Releases the unmanaged resources used by the WkbWriter and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _writer.Dispose();

                if (_output != null)
                {
                    _output.Dispose();
                }
            }

            _disposed = true;
        }
    }
}
