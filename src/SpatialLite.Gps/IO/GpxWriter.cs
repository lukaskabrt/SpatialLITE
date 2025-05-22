using SpatialLite.Gps.Geometries;
using System;
using System.IO;
using System.Text;
using System.Xml;

namespace SpatialLite.Gps.IO;

/// <summary>
/// Implements data writer that can write GPX data to streams and files.
/// </summary>
public class GpxWriter : IDisposable, IGpxWriter
{
    private readonly System.Globalization.CultureInfo _invariantCulture = System.Globalization.CultureInfo.InvariantCulture;

    private readonly XmlWriter _xmlWriter;
    private readonly StreamWriter _streamWriter;
    private bool _disposed = false;

    /// <summary>
    /// Initializes a new instance of the GpxWriter class that writes GPX entities to the specified stream.
    /// </summary>
    /// <param name="stream">The Stream to write GPX entities to.</param>
    /// <param name="settings">The settings defining behaviour of the writer.</param>
    public GpxWriter(Stream stream, GpxWriterSettings settings)
    {
        Settings = settings;
        settings.IsReadOnly = true;

        XmlWriterSettings writerSetting = new();
        writerSetting.Indent = true;

        _streamWriter = new StreamWriter(stream, new UTF8Encoding(false));
        _xmlWriter = XmlWriter.Create(_streamWriter, writerSetting);

        StartDocument();
    }

    /// <summary>
    /// Initializes a new instance of the GpxWriter class that writes GPX entities to the specified file.
    /// </summary>
    /// <param name="path">Path to the GPX file.</param>
    /// <param name="settings">The settings defining behaviour of the writer.</param>
    /// <remarks>If the file exists, it is overwritten, otherwise, a new file is created.</remarks>
    public GpxWriter(string path, GpxWriterSettings settings)
    {
        Settings = settings;
        settings.IsReadOnly = true;

        XmlWriterSettings writerSetting = new();
        writerSetting.Indent = true;

        var fileStream = new FileStream(path, FileMode.Create);
        _streamWriter = new StreamWriter(fileStream, new UTF8Encoding(false));
        _xmlWriter = XmlWriter.Create(_streamWriter, writerSetting);

        StartDocument();
    }

    /// <summary>
    /// Creates root element in the output stream
    /// </summary>
    private void StartDocument()
    {
        _xmlWriter.WriteStartDocument();
        _xmlWriter.WriteStartElement("gpx", "http://www.topografix.com/GPX/1/1");
        _xmlWriter.WriteAttributeString("version", "1.1");
        _xmlWriter.WriteAttributeString("creator", Settings.GeneratorName ?? "SpatialLite");
    }

    /// <summary>
    /// Gets settings of the writer
    /// </summary>
    public GpxWriterSettings Settings { get; private set; }

    /// <summary>
    /// Writes the given waypoint to the output stream
    /// </summary>
    /// <param name="waypoint">The waypoint to write</param>
    public void Write(GpxPoint waypoint)
    {
        WritePoint(waypoint, "wpt");
    }

    /// <summary>
    /// Writes the given route to the output stream
    /// </summary>
    /// <param name="route">The route to write</param>
    public void Write(GpxRoute route)
    {
        _xmlWriter.WriteStartElement("rte");

        for (int i = 0; i < route.Points.Count; i++)
        {
            WritePoint(route.Points[i], "rtept");
        }
        if (Settings.WriteMetadata)
        {
            WriteTrackMetadata(route.Metadata);
        }

        _xmlWriter.WriteEndElement();
    }

    /// <summary>
    /// Writes the given track to the output stream
    /// </summary>
    /// <param name="track">The track to write</param>
    public void Write(GpxTrack track)
    {
        _xmlWriter.WriteStartElement("trk");

        for (int i = 0; i < track.Geometries.Count; i++)
        {
            _xmlWriter.WriteStartElement("trkseg");

            for (int ii = 0; ii < track.Geometries[i].Points.Count; ii++)
            {
                WritePoint(track.Geometries[i].Points[ii], "trkpt");
            }

            _xmlWriter.WriteEndElement();
        }

        if (Settings.WriteMetadata)
        {
            WriteTrackMetadata(track.Metadata);
        }

        _xmlWriter.WriteEndElement();
    }

    /// <summary>
    /// Releases all resources used by the GpxWriter.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Writes the given point to the output stream
    /// </summary>
    /// <param name="point">The point to be written</param>
    /// <param name="pointElementName">The name of the XML element the point is to be written to</param>
    private void WritePoint(GpxPoint point, string pointElementName)
    {
        _xmlWriter.WriteStartElement(pointElementName);
        _xmlWriter.WriteAttributeString("lat", point.Position.Y.ToString(_invariantCulture));
        _xmlWriter.WriteAttributeString("lon", point.Position.X.ToString(_invariantCulture));

        if (!double.IsNaN(point.Position.Z))
        {
            _xmlWriter.WriteElementString("ele", point.Position.Z.ToString(_invariantCulture));
        }

        if (point.Timestamp != null)
        {
            _xmlWriter.WriteElementString("time", point.Timestamp.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", _invariantCulture));
        }

        if (Settings.WriteMetadata)
        {
            WritePointMetadata(point.Metadata);
        }

        _xmlWriter.WriteEndElement();
    }

    /// <summary>
    /// Writes content of the GpxEntityDetails class to the current position of output stream
    /// </summary>
    /// <param name="metadata">A GpxEntityDetails object to be written</param>
    private void WriteTrackMetadata(GpxTrackMetadata metadata)
    {
        if (metadata != null)
        {
            if (metadata.Name != null)
                _xmlWriter.WriteElementString("name", metadata.Name);
            if (metadata.Comment != null)
                _xmlWriter.WriteElementString("cmt", metadata.Comment);
            if (metadata.Description != null)
                _xmlWriter.WriteElementString("desc", metadata.Description);
            if (metadata.Source != null)
                _xmlWriter.WriteElementString("src", metadata.Source);
            foreach (var link in metadata.Links)
            {
                WriteLink(link);
            }
            if (metadata.Comment != null)
                _xmlWriter.WriteElementString("type", metadata.Type);
        }
    }

    /// <summary>
    /// Writes content of the GpxPointDetails class to the current position of output stream
    /// </summary>
    /// <param name="metadata">A GpxEntityDetails object to be written</param>
    private void WritePointMetadata(GpxPointMetadata metadata)
    {
        if (metadata != null)
        {
            if (metadata.Name != null)
            {
                _xmlWriter.WriteElementString("name", metadata.Name);
            }
            if (metadata.Comment != null)
            {
                _xmlWriter.WriteElementString("cmt", metadata.Comment);
            }
            if (metadata.Description != null)
            {
                _xmlWriter.WriteElementString("desc", metadata.Description);
            }
            if (metadata.Source != null)
            {
                _xmlWriter.WriteElementString("src", metadata.Source);
            }

            foreach (var link in metadata.Links)
            {
                WriteLink(link);
            }

            if (metadata.MagVar.HasValue)
            {
                _xmlWriter.WriteElementString("magvar", metadata.MagVar.Value.ToString(_invariantCulture));
            }
            if (metadata.GeoidHeight.HasValue)
            {
                _xmlWriter.WriteElementString("geoidheight", (metadata.GeoidHeight ?? 0).ToString(_invariantCulture));
            }
            if (metadata.Symbol != null)
            {
                _xmlWriter.WriteElementString("sym", metadata.Symbol);
            }
            if (metadata.Fix.HasValue)
            {
                _xmlWriter.WriteElementString("fix", GpxFixHelper.GpsFixToString(metadata.Fix.Value));
            }
            if (metadata.SatellitesCount.HasValue)
            {
                _xmlWriter.WriteElementString("sat", metadata.SatellitesCount.Value.ToString(_invariantCulture));
            }
            if (metadata.Hdop.HasValue)
            {
                _xmlWriter.WriteElementString("hdop", metadata.Hdop.Value.ToString(_invariantCulture));
            }
            if (metadata.Vdop.HasValue)
            {
                _xmlWriter.WriteElementString("vdop", metadata.Vdop.Value.ToString(_invariantCulture));
            }
            if (metadata.Pdop.HasValue)
            {
                _xmlWriter.WriteElementString("pdop", metadata.Pdop.Value.ToString(_invariantCulture));
            }
            if (metadata.AgeOfDgpsData.HasValue)
            {
                _xmlWriter.WriteElementString("ageofdgpsdata", metadata.AgeOfDgpsData.Value.ToString(_invariantCulture));
            }
            if (metadata.DgpsId.HasValue)
            {
                _xmlWriter.WriteElementString("dgpsid", metadata.DgpsId.Value.ToString(_invariantCulture));
            }
        }
    }

    /// <summary>
    /// Writes the given link to the output stream
    /// </summary>
    /// <param name="link">The link to be written</param>
    private void WriteLink(GpxLink link)
    {
        _xmlWriter.WriteStartElement("link");
        _xmlWriter.WriteAttributeString("href", link.Url.OriginalString);

        if (link.Text != null)
            _xmlWriter.WriteElementString("text", link.Text);
        if (link.Type != null)
            _xmlWriter.WriteElementString("type", link.Type);

        _xmlWriter.WriteEndElement();
    }

    /// <summary>
    /// Releases the unmanaged resources used by the GpxWriter and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _xmlWriter.Dispose();

                if (_streamWriter != null)
                    _streamWriter.Dispose();
            }

            _disposed = true;
        }
    }
}
