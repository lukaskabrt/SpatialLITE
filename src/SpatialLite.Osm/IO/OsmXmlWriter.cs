﻿using System;
using System.Text;
using System.IO;
using System.Xml;

using SpatialLite.Osm.Geometries;

namespace SpatialLite.Osm.IO;

/// <summary>
/// Represents an IOsmWriter, that can write OSM entities to XML format.
/// </summary>
public class OsmXmlWriter : IOsmWriter
{

    private System.Globalization.CultureInfo _culture = System.Globalization.CultureInfo.InvariantCulture;
    private Stream _output;
    private bool _ownsOutputStream;

    private XmlWriter _writer;
    private bool _isInsideOsm = false;
    private bool _disposed = false;

    // underlaying stream writer for writing to files
    private StreamWriter _streamWriter;

    /// <summary>
    /// Initializes a new instance of the OsmXmlWriter class that writes OSM entities to the specified stream.
    /// </summary>
    /// <param name="stream">The Stream to write OSM entities to.</param>
    /// <param name="settings">The settings defining behaviour of the writer.</param>
    public OsmXmlWriter(Stream stream, OsmWriterSettings settings)
    {
        this.Settings = settings;
        this.Settings.IsReadOnly = true;

        _output = stream;
        _ownsOutputStream = false;

        XmlWriterSettings writerSetting = new XmlWriterSettings();
        writerSetting.Indent = true;

        _writer = XmlWriter.Create(stream, writerSetting);
    }

    /// <summary>
    /// Initializes a new instance of the OsmXmlWriter class that writes OSM entities to the specified file.
    /// </summary>
    /// <param name="path">Path to the OSM file.</param>
    /// <param name="settings">The settings defining behaviour of the writer.</param>
    /// <remarks>If the file exists, it is overwritten, otherwise, a new file is created.</remarks>
    public OsmXmlWriter(string path, OsmWriterSettings settings)
    {
        this.Settings = settings;
        this.Settings.IsReadOnly = true;

        _output = new FileStream(path, FileMode.Create, FileAccess.Write);
        _ownsOutputStream = true;

        XmlWriterSettings writerSetting = new XmlWriterSettings();
        writerSetting.Indent = true;

        _streamWriter = new StreamWriter(_output, new UTF8Encoding(false));
        _writer = XmlWriter.Create(_streamWriter, writerSetting);
    }

    /// <summary>
    /// Gets settings used by this XmlWriter.
    /// </summary>
    public OsmWriterSettings Settings { get; private set; }

    /// <summary>
    /// Releases all resources used by the OsmXmlWriter.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Writes specified entity object in XML format to the underlaying stream.
    /// </summary>
    /// <param name="entity">Entity to write.</param>
    public void Write(IOsmGeometry entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity), "Entity cannot be null");
        }

        if (_isInsideOsm == false)
        {
            this.StartDocument();
        }

        switch (entity.EntityType)
        {
            case EntityType.Node: this.Write(new NodeInfo((Node)entity)); break;
            case EntityType.Way: this.Write(new WayInfo((Way)entity)); break;
            case EntityType.Relation: this.Write(new RelationInfo((Relation)entity)); break;
        }
    }

    /// <summary>
    /// Writes specified entity data-transfer object in XML format to the underlaying stream.
    /// </summary>
    /// <param name="info">Entity data-transfer object to write.</param>
    public void Write(IEntityInfo info)
    {
        if (this.Settings.WriteMetadata)
        {
            if (info.Metadata == null)
            {
                throw new ArgumentException("Entity doesn't contain metadata object, but writer was created with WriteMetadata setting.");
            }
        }

        if (_isInsideOsm == false)
        {
            this.StartDocument();
        }

        switch (info.EntityType)
        {
            case EntityType.Node: this.WriteNode((NodeInfo)info); break;
            case EntityType.Way: this.WriteWay((WayInfo)info); break;
            case EntityType.Relation: this.WriteRelation((RelationInfo)info); break;
        }
    }

    /// <summary>
    /// Causes any buffered data to be written to the underlaying storage.
    /// </summary>
    public void Flush()
    {
        _writer.Flush();
    }

    /// <summary>
    /// Writes &lt;osm&gt; start element to the output stream.
    /// </summary>
    private void StartDocument()
    {
        _writer.WriteStartElement("osm");
        if (string.IsNullOrEmpty(this.Settings.ProgramName) == false)
        {
            _writer.WriteAttributeString("generator", this.Settings.ProgramName);
        }

        _isInsideOsm = true;
    }

    /// <summary>
    /// Writes &lt;osm&gt; end element to the output stream.
    /// </summary>
    private void EndDocument()
    {
        _writer.WriteEndElement();
        _isInsideOsm = false;
    }

    /// <summary>
    /// Writes node to the output streram.
    /// </summary>
    /// <param name="info">The Node to be written.</param>
    private void WriteNode(NodeInfo info)
    {
        _writer.WriteStartElement("node");
        _writer.WriteAttributeString("lon", info.Longitude.ToString(_culture));
        _writer.WriteAttributeString("lat", info.Latitude.ToString(_culture));
        _writer.WriteAttributeString("id", info.ID.ToString(_culture));

        if (this.Settings.WriteMetadata)
        {
            this.WriteMetadata(info.Metadata);
        }

        this.WriteTags(info.Tags);

        _writer.WriteEndElement();
    }

    /// <summary>
    /// Writes way to the output stream
    /// </summary>
    /// <param name="info">The Way to be written</param>
    private void WriteWay(WayInfo info)
    {
        _writer.WriteStartElement("way");
        _writer.WriteAttributeString("id", info.ID.ToString(_culture));

        if (this.Settings.WriteMetadata)
        {
            this.WriteMetadata(info.Metadata);
        }

        this.WriteTags(info.Tags);

        for (int i = 0; i < info.Nodes.Count; i++)
        {
            _writer.WriteStartElement("nd");
            _writer.WriteAttributeString("ref", info.Nodes[i].ToString(_culture));
            _writer.WriteEndElement();
        }

        _writer.WriteEndElement();
    }

    /// <summary>
    /// Writes relation to the output stream.
    /// </summary>
    /// <param name="info">The relation to be written.</param>
    private void WriteRelation(RelationInfo info)
    {
        _writer.WriteStartElement("relation");
        _writer.WriteAttributeString("id", info.ID.ToString(_culture));

        if (this.Settings.WriteMetadata)
        {
            this.WriteMetadata(info.Metadata);
        }

        this.WriteTags(info.Tags);

        for (int i = 0; i < info.Members.Count; i++)
        {
            _writer.WriteStartElement("member");
            _writer.WriteAttributeString("ref", info.Members[i].Reference.ToString(_culture));
            _writer.WriteAttributeString("role", info.Members[i].Role);
            _writer.WriteAttributeString("type", info.Members[i].MemberType.ToString().ToLower());
            _writer.WriteEndElement();
        }

        _writer.WriteEndElement();
    }

    /// <summary>
    /// Writes collection of tags to the output stream.
    /// </summary>
    /// <param name="tags">The collection of tags to write.</param>
    private void WriteTags(TagsCollection tags)
    {
        foreach (var tag in tags)
        {
            _writer.WriteStartElement("tag");
            _writer.WriteAttributeString("k", tag.Key);
            _writer.WriteAttributeString("v", tag.Value);
            _writer.WriteEndElement();
        }
    }

    /// <summary>
    /// Writes detailed attributes of the OSM entity.
    /// </summary>
    /// <param name="medatata">The entity whose attributes to be written.</param>
    private void WriteMetadata(EntityMetadata medatata)
    {
        _writer.WriteAttributeString("version", medatata.Version.ToString(_culture));
        _writer.WriteAttributeString("changeset", medatata.Changeset.ToString(_culture));

        //HACK ? write only valid user data
        if (medatata.Uid > 0)
        {
            _writer.WriteAttributeString("uid", medatata.Uid.ToString(_culture));
            _writer.WriteAttributeString("user", medatata.User);
        }

        _writer.WriteAttributeString("visible", medatata.Visible.ToString().ToLower());
        _writer.WriteAttributeString("timestamp", medatata.Timestamp.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'"));
    }

    /// <summary>
    /// Releases the unmanaged resources used by the OsmXmlWriter and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    private void Dispose(bool disposing)
    {
        if (!this._disposed)
        {
            if (_writer != null)
            {
                _writer.Dispose();
            }

            if (disposing)
            {
                if (_streamWriter != null)
                {
                    _streamWriter.Dispose();
                }

                if (_ownsOutputStream)
                {
                    _output.Dispose();
                }
            }

            _disposed = true;
        }
    }
}
