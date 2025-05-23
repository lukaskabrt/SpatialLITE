using SpatialLite.Core.API;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace SpatialLite.Core.IO;

/// <summary>
/// Provides methods for writing Geomerty objects into WKT format.
/// </summary>
public class WktWriter : IDisposable
{

    private bool _disposed = false;
    private readonly TextWriter _writer;
    private readonly Stream _outputStream;

    /// <summary>
    /// Initializes a new instance of the WkbWriter class with specific settings.
    /// </summary>
    /// <param name="settings">The settings defining behaviour of the writer.</param>
    protected WktWriter(WktWriterSettings settings)
    {
        Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        Settings.IsReadOnly = true;
    }

    /// <summary>
    /// Initializes a new instance of the WktWriter class that writes geometries to stream with specific settings.
    /// </summary>
    /// <param name="stream">The stream to write geometries to.</param>
    /// <param name="settings">The settings defining behaviour of the writer.</param>
    public WktWriter(Stream stream, WktWriterSettings settings)
        : this(settings)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        _writer = new StreamWriter(stream);
    }

    /// <summary>
    /// Initializes a new instance of the WktWriter class that writes geometreis to file with specific settings.
    /// </summary>
    /// <param name="path">The path to the file to write geometries to.</param>
    /// <param name="settings">The settings defining behaviour of the writer.</param>
    public WktWriter(string path, WktWriterSettings settings)
        : this(settings)
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        _outputStream = new FileStream(path, FileMode.Create, FileAccess.Write);
        _writer = new StreamWriter(_outputStream);
    }

    /// <summary>
    /// Gets the settings that determines behaviour of this instance of the WktWriter.
    /// </summary>
    public WktWriterSettings Settings { get; private set; }

    /// <summary>
    /// Writes the Geometry object in the WKT format to the output.
    /// </summary>
    /// <param name="geometry">The geometry to write.</param>
    public void Write(IGeometry geometry)
    {
        Write(geometry, _writer);
    }

    /// <summary>
    /// Releases all resources used by the WktWriter.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Writes the Geometry object to the string in WKT format.
    /// </summary>
    /// <param name="geometry">The Geometry to be written.</param>
    /// <returns>The WKT representation of the Geometry.</returns>
    public static string WriteToString(IGeometry geometry)
    {
        StringWriter tw = new(CultureInfo.InvariantCulture);
        Write(geometry, tw);

        return tw.ToString();
    }

    /// <summary>
    /// Writes specified Geometry to the TextWriter in WKT format.
    /// </summary>
    /// <param name="geometry">The geometry to be written.</param>
    /// <param name="writer">The output Stream the Geometry will be written to.</param>
    protected static void Write(IGeometry geometry, TextWriter writer)
    {
        if (geometry is IPoint point)
        {
            AppendPointTaggedText(point, writer);
        }
        else if (geometry is ILineString lineString)
        {
            AppendLineStringTaggedText(lineString, writer);
        }
        else if (geometry is IPolygon polygon)
        {
            AppendPolygonTaggedText(polygon, writer);
        }
        else if (geometry is IMultiPoint multiPoint)
        {
            AppendMultiPointTaggedText(multiPoint, writer);
        }
        else if (geometry is IMultiLineString multiLineString)
        {
            AppendMultiLineStringTaggedText(multiLineString, writer);
        }
        else if (geometry is IMultiPolygon multiPolygon)
        {
            AppendMultiPolygonTaggetText(multiPolygon, writer);
        }
        else if (geometry is IGeometryCollection<IGeometry> geometryCollection)
        {
            AppendGeometryCollectionTaggedText(geometryCollection, writer);
        }
    }

    /// <summary>
    /// Converts the Coordinate to the format used in WKT and appends it to the output Stream.
    /// </summary>
    /// <param name="coordinate">The Coordinate to be converted.</param>
    /// <param name="writer">The output Stream to Appent WKT representation to.</param>
    private static void AppendCoordinate(Coordinate coordinate, TextWriter writer)
    {
        writer.Write(coordinate.X.ToString(CultureInfo.InvariantCulture));
        writer.Write(' ');
        writer.Write(coordinate.Y.ToString(CultureInfo.InvariantCulture));

        if (coordinate.Is3D)
        {
            writer.Write(' ');
            writer.Write(coordinate.Z.ToString(CultureInfo.InvariantCulture));
        }

        if (coordinate.IsMeasured)
        {
            writer.Write(' ');
            writer.Write(coordinate.M.ToString(CultureInfo.InvariantCulture));
        }
    }

    /// <summary>
    /// Converts the List of Coordinates to WKT format and appends it to output stream.
    /// </summary>
    /// <param name="coordinates">The list of coordinates to be converted.</param>
    /// <param name="writer">The output stream.</param>
    /// <param name="wrap">bool value indicating whether the list of coordinates should be enclosed in parathenes.</param>
    private static void AppendCoordinates(ICoordinateList coordinates, TextWriter writer, bool wrap)
    {
        if (coordinates.Count == 0)
        {
            return;
        }

        if (wrap)
        {
            writer.Write("(");
        }

        AppendCoordinate(coordinates[0], writer);

        for (int i = 1; i < coordinates.Count; i++)
        {
            writer.Write(", ");
            AppendCoordinate(coordinates[i], writer);
        }

        if (wrap)
        {
            writer.Write(")");
        }
    }

    /// <summary>
    /// Converts Point to WKT format and appends WKT representation to the output stream.
    /// </summary>
    /// <param name="point">The Point to be converted.</param>
    /// <param name="writer">The output Stream to append WKT representation to.</param>
    private static void AppendPointTaggedText(IPoint point, TextWriter writer)
    {
        writer.Write("point ");

        string dimension = GetDimensionText(point);
        if (string.IsNullOrEmpty(dimension) == false)
        {
            writer.Write(dimension);
            writer.Write(" ");
        }

        AppendPointText(point, writer);
    }

    /// <summary>
    /// Converts Point's content to WKT format and appends it to the output stream.
    /// </summary>
    /// <param name="point">The Point to be converted.</param>
    /// <param name="writer">The output Stream to append WKT representation to.</param>
    private static void AppendPointText(IPoint point, TextWriter writer)
    {
        if (point.Position.Equals(Coordinate.Empty))
        {
            writer.Write("empty");
        }
        else
        {
            writer.Write("(");
            AppendCoordinate(point.Position, writer);
            writer.Write(")");
        }
    }

    /// <summary>
    /// Converts LineString to WKT format and appends WKT representation to the output stream.
    /// </summary>
    /// <param name="linestring">The LineString to be converted.</param>
    /// <param name="writer">The output Stream to Append WKT representation to.</param>
    private static void AppendLineStringTaggedText(ILineString linestring, TextWriter writer)
    {
        writer.Write("linestring ");

        string dimension = GetDimensionText(linestring);
        if (string.IsNullOrEmpty(dimension) == false)
        {
            writer.Write(dimension);
            writer.Write(" ");
        }

        AppendLineStringText(linestring, writer);
    }

    /// <summary>
    /// Converts LineString's content to WKT and appends it to the output stream.
    /// </summary>
    /// <param name="linestring">The LineString to be converted.</param>
    /// <param name="writer">The output Stream to Append WKT representation to.</param>
    private static void AppendLineStringText(ILineString linestring, TextWriter writer)
    {
        if (linestring.Coordinates.Count == 0)
        {
            writer.Write("empty");
        }
        else
        {
            AppendCoordinates(linestring.Coordinates, writer, true);
        }
    }

    /// <summary>
    /// Converts Polygon to WKT format and appends WKT representation to the output stream.
    /// </summary>
    /// <param name="polygon">The Polygon to be converted.</param>
    /// <param name="writer">The output Stream to Append WKT representation to.</param>
    private static void AppendPolygonTaggedText(IPolygon polygon, TextWriter writer)
    {
        writer.Write("polygon ");

        string dimension = GetDimensionText(polygon);
        if (string.IsNullOrEmpty(dimension) == false)
        {
            writer.Write(dimension);
            writer.Write(" ");
        }

        AppendPolygonText(polygon, writer);
    }

    /// <summary>
    /// Converts Polygon's content to WKT format and appends WKT representation to the output stream.
    /// </summary>
    /// <param name="polygon">The Polygon to be converted.</param>
    /// <param name="writer">The output Stream to Append WKT representation to.</param>
    private static void AppendPolygonText(IPolygon polygon, TextWriter writer)
    {
        if (polygon.ExteriorRing.Count == 0)
        {
            writer.Write("empty");
        }
        else
        {
            writer.Write("(");
            AppendCoordinates(polygon.ExteriorRing, writer, true);

            if (polygon.InteriorRings.Count() > 0)
            {
                writer.Write(",");
                AppendCoordinates(polygon.InteriorRings.First(), writer, true);

                foreach (var ring in polygon.InteriorRings.Skip(1))
                {
                    writer.Write(",");
                    AppendCoordinates(ring, writer, true);
                }
            }

            writer.Write(")");
        }
    }

    /// <summary>
    /// Converts MultiPoint to WKT format and appends WKT representation to the output stream.
    /// </summary>
    /// <param name="multipoint">The MultiPoint to be converted.</param>
    /// <param name="writer">The output Stream to Append WKT representation to.</param>
    private static void AppendMultiPointTaggedText(IMultiPoint multipoint, TextWriter writer)
    {
        writer.Write("multipoint ");

        string dimension = GetDimensionText(multipoint);
        if (string.IsNullOrEmpty(dimension) == false)
        {
            writer.Write(dimension);
            writer.Write(" ");
        }

        AppendMultiPointText(multipoint, writer);
    }

    /// <summary>
    /// Converts MultiPoint's content to WKT format and appends WKT representation to the output stream.
    /// </summary>
    /// <param name="multipoint">The MultiPoint to be converted.</param>
    /// <param name="writer">The output Stream to Append WKT representation to.</param>
    private static void AppendMultiPointText(IMultiPoint multipoint, TextWriter writer)
    {
        if (multipoint.Geometries.Count() == 0)
        {
            writer.Write("empty");
        }
        else
        {
            writer.Write("(");

            if (multipoint.Geometries.Count() > 0)
            {
                AppendPointText(multipoint.Geometries.First(), writer);

                foreach (var point in multipoint.Geometries.Skip(1))
                {
                    writer.Write(",");
                    AppendPointText(point, writer);
                }
            }

            writer.Write(")");
        }
    }

    /// <summary>
    /// Converts MultiLineString to WKT format and appends WKT representation to the output stream.
    /// </summary>
    /// <param name="mls">The MultiLineString to be converted.</param>
    /// <param name="writer">The output Stream to Append WKT representation to.</param>
    private static void AppendMultiLineStringTaggedText(IMultiLineString mls, TextWriter writer)
    {
        writer.Write("multilinestring ");

        string dimension = GetDimensionText(mls);
        if (string.IsNullOrEmpty(dimension) == false)
        {
            writer.Write(dimension);
            writer.Write(" ");
        }

        AppendMultiLineStringText(mls, writer);
    }

    /// <summary>
    /// Converts MultiLineString's content to WKT format and appends WKT representation to the output stream.
    /// </summary>
    /// <param name="mls">The MultiLIneString to be converted.</param>
    /// <param name="writer">The output Stream to Append WKT representation to.</param>
    private static void AppendMultiLineStringText(IMultiLineString mls, TextWriter writer)
    {
        if (mls.Geometries.Count() == 0)
        {
            writer.Write("empty");
        }
        else
        {
            writer.Write("(");

            AppendLineStringText(mls.Geometries.First(), writer);

            foreach (var linestring in mls.Geometries.Skip(1))
            {
                writer.Write(",");
                AppendLineStringText(linestring, writer);
            }

            writer.Write(")");
        }
    }

    /// <summary>
    /// Converts MultiPolygon to WKT format and appends WKT representation to the output stream.
    /// </summary>
    /// <param name="mp">The Multipolygon to be converted.</param>
    /// <param name="writer">The output Stream to Append WKT representation to.</param>
    private static void AppendMultiPolygonTaggetText(IMultiPolygon mp, TextWriter writer)
    {
        writer.Write("multipolygon ");

        string dimension = GetDimensionText(mp);
        if (string.IsNullOrEmpty(dimension) == false)
        {
            writer.Write(dimension);
            writer.Write(" ");
        }

        AppendMultiPolygonText(mp, writer);
    }

    /// <summary>
    /// Converts MultiPolygon's content to WKT format and appends WKT representation to the output stream.
    /// </summary>
    /// <param name="multipolygon">The MultiPolygon to be converted.</param>
    /// <param name="writer">The output Stream to Append WKT representation to.</param>
    private static void AppendMultiPolygonText(IMultiPolygon multipolygon, TextWriter writer)
    {
        if (multipolygon.Geometries.Count() == 0)
        {
            writer.Write("empty");
        }
        else
        {
            writer.Write("(");

            AppendPolygonText(multipolygon.Geometries.First(), writer);

            foreach (var polygon in multipolygon.Geometries.Skip(1))
            {
                writer.Write(",");
                AppendPolygonText(polygon, writer);
            }

            writer.Write(")");
        }
    }

    /// <summary>
    /// Converts GeometryCollection to WKT format and appends WKT representation to the output stream.
    /// </summary>
    /// <param name="collection">The GeometryCollection be converted.</param>
    /// <param name="writer">The output Stream to Append WKT representation to.</param>
    private static void AppendGeometryCollectionTaggedText(IGeometryCollection<IGeometry> collection, TextWriter writer)
    {
        writer.Write("geometrycollection ");

        string dimension = GetDimensionText(collection);
        if (string.IsNullOrEmpty(dimension) == false)
        {
            writer.Write(dimension);
            writer.Write(" ");
        }

        AppendGeometryCollectionText(collection, writer);
    }

    /// <summary>
    /// Converts GeometryCollection's content to WKT format and appends WKT representation to the output stream.
    /// </summary>
    /// <param name="collection">The GeometryCollection to be converted.</param>
    /// <param name="writer">The output Stream to Append WKT representation to.</param>
    private static void AppendGeometryCollectionText(IGeometryCollection<IGeometry> collection, TextWriter writer)
    {
        if (collection.Geometries.Count() == 0)
        {
            writer.Write("empty");
        }
        else
        {
            writer.Write("(");
            Write(collection.Geometries.First(), writer);

            foreach (var geometry in collection.Geometries.Skip(1))
            {
                writer.Write(",");
                Write(geometry, writer);
            }

            writer.Write(")");
        }
    }

    /// <summary>
    /// Gets string that represents dimension of the geometry.
    /// </summary>
    /// <param name="geometry">The geometry to examine.</param>
    /// <returns>The string representing dimension of the geometry - empty string for 2D geometry, 'm' for measured geometry, 'z' for 3D geometry.</returns>
    private static string GetDimensionText(IGeometry geometry)
    {
        string dimension = string.Empty;

        if (geometry.Is3D)
        {
            dimension += "z";
        }

        if (geometry.IsMeasured)
        {
            dimension += "m";
        }

        return dimension;
    }

    /// <summary>
    /// Releases the unmanaged resources used by the WktWriter and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _writer.Dispose();

                if (_outputStream != null)
                {
                    _outputStream.Dispose();
                }
            }

            _disposed = true;
        }
    }
}
