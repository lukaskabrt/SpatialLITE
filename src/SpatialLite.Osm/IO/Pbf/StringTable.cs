using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpatialLite.Osm.IO.Pbf;

/// <summary>
/// Stores all strings for Primitive block.
/// </summary>
[ProtoContract(Name = "StringTable")]
public class StringTable
{

    private List<byte[]> _s = new List<byte[]>();

    /// <summary>
    /// Gets or sets collection of strings serialized as byte array.
    /// </summary>
    [ProtoMember(1, Name = "s", DataFormat = DataFormat.Default)]
    public List<byte[]> Storage
    {
        get { return _s; }
        set { _s = value; }
    }

    /// <summary>
    /// Gets or sets string at specified position.
    /// </summary>
    /// <param name="index">The index of the string.</param>
    /// <returns>string at specified position.</returns>
    [ProtoIgnore]
    public string this[int index]
    {
        get
        {
            if (index >= Storage.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return Encoding.UTF8.GetString(Storage[index], 0, Storage[index].Length);
        }
        set
        {
            if (index >= Storage.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            Storage[index] = Encoding.UTF8.GetBytes(value);
        }
    }

    /// <summary>
    /// Gets or sets string at specified position.
    /// </summary>
    /// <param name="index">The index of the string.</param>
    /// <returns>string at specified position.</returns>
    [ProtoIgnore]
    public string this[uint index]
    {
        get
        {
            return this[(int)index];
        }
        set
        {
            this[(int)index] = value;
        }
    }
}
