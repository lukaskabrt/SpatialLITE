using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using SpatialLite.Core.API;
using SpatialLite.Core.Geometries;

namespace SpatialLite.Core.IO {
	/// <summary>
	/// Provides functions for reading and parsing geometries from WKB format.
	/// </summary>
	public class WkbReader : IDisposable {
		#region Private Fields

		private BinaryReader _inputReader;
		private FileStream _inputFileStream;
		private bool _disposed = false;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the WkbReader class that reads data from specific stream.
		/// </summary>
		/// <param name="input">The stream to read data from.</param>
		public WkbReader(Stream input) {
			if (input == null) {
				throw new ArgumentNullException("input", "Input stream cannot be null");
			}

			_inputReader = new BinaryReader(input);
		}

		/// <summary>
		/// Initializes a new instance of the WkbReader class that reads data from specific file.
		/// </summary>
		/// <param name="path">Path to the file to read data from.</param>
		public WkbReader(string path) {
			_inputFileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
			_inputReader = new BinaryReader(_inputFileStream);
		}

		#endregion

		#region Public Static Methods

		/// <summary>
		/// Parses data from the binnary array.
		/// </summary>
		/// <param name="wkb">The binary array with WKB serialized geometry.</param>
		/// <returns>Parsed geometry.</returns>
		/// <exception cref="WkbFormatException">Throws exception if wkb array does not contrains valid WKB geometry.</exception>
		public static Geometry Parse(byte[] wkb) {
			if (wkb == null) {
				throw new ArgumentNullException("wkb");
			}

			using (MemoryStream ms = new MemoryStream(wkb)) {
				using (BinaryReader reader = new BinaryReader(ms)) {
					if (reader.PeekChar() == -1) {
						return null;
					}

					try {
						BinaryEncoding encoding = (BinaryEncoding)reader.ReadByte();
						if (encoding == BinaryEncoding.BigEndian) {
							throw new NotSupportedException("Big endian encoding is not supprted in the current version of WkbReader.");
						}

						Geometry parsed = WkbReader.ReadGeometry(reader);

						return parsed;
					}
					catch (EndOfStreamException) {
						throw new WkbFormatException("End of stream reached before end of valid WKB geometry end.");
					}
				}
			}
		}

		/// <summary>
		/// Parses data from the binary array as given geometry type.
		/// </summary>
		/// <typeparam name="T">The Geometry type to be parsed.</typeparam>
		/// <param name="wkb">The binary array with WKB serialized geometry.</param>
		/// <returns>Parsed geometry.</returns>
		/// <exception cref="WkbFormatException">Throws exception if wkb array does not contrains valid WKB geometry of specific type.</exception>
		public static T Parse<T>(byte[] wkb) where T : Geometry {
			Geometry parsed = WkbReader.Parse(wkb);

			if (parsed != null) {
				T result = parsed as T;
				if (result == null) {
					throw new WkbFormatException("Input doesn't contain valid WKB representation of the specified geometry type.");
				}

				return result;
			}
			else {
				return null;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Releases all resources used by the ComponentLibrary.
		/// </summary>
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Read geometry in WKB format from the input.
		/// </summary>
		/// <returns>Parsed geometry or null if no other geometry is available.</returns>
		public Geometry Read() {
			if (_inputReader.PeekChar() == -1) {
				return null;
			}

			try {
				BinaryEncoding encoding = (BinaryEncoding)_inputReader.ReadByte();
				if (encoding == BinaryEncoding.BigEndian) {
					throw new NotSupportedException("Big endian encoding is not supprted in the current version of WkbReader.");
				}

				return WkbReader.ReadGeometry(_inputReader);
			}
			catch (EndOfStreamException) {
				throw new WkbFormatException("End of stream reached before end of valid WKB geometry end.");
			}
		}

		/// <summary>
		/// Read geometry in WKB format from the input.
		/// </summary>
		/// <typeparam name="T">The Geometry type to be parsed.</typeparam>
		/// <returns>Geometry obejct of specific type read from the input, or null if no other geometry is available.</returns>
		/// <exception cref="WkbFormatException">Throws exception if wkb array does not contrains valid WKB geometry of specific type.</exception>
		public T Read<T>() where T : Geometry {
			Geometry parsed = this.Read();

			if (parsed != null) {
				T result = parsed as T;
				if (result == null) {
					throw new WkbFormatException("Input doesn't contain valid WKB representation of the specified geometry type.");
				}

				return result;
			}
			else {
				return null;
			}
		}

		#endregion

		#region Private Static Methods

		/// <summary>
		/// Reads Coordinate from the BinaryReader.
		/// </summary>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="is3D">Bool value indicating whether Coordinate has Z value.</param>
		/// <param name="isMeasured">Bool value indicating whether Coordinate has M value.</param>
		/// <returns>Parsed Coordinate.</returns>
		private static Coordinate ReadCoordinate(BinaryReader reader, bool is3D, bool isMeasured) {
			double x = reader.ReadDouble();
			double y = reader.ReadDouble();
			double z = is3D ? reader.ReadDouble() : double.NaN;
			double m = isMeasured ? reader.ReadDouble() : double.NaN;

			return new Coordinate(x, y, z, m);
		}

		/// <summary>
		/// Reads a list of coordinates from the BinaryReader.
		/// </summary>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="is3D">Bool value indicating whether coordinates has Z value.</param>
		/// <param name="isMeasured">Bool value indicating whether coordinates has M value.</param>
		/// <returns>Parsed Coordinate.</returns>
		private static IEnumerable<Coordinate> ReadCoordinates(BinaryReader reader, bool is3D, bool isMeasured) {
			int pointCount = (int)reader.ReadUInt32();

			List<Coordinate> result = new List<Coordinate>(pointCount);
			for (int i = 0; i < pointCount; i++) {
				result.Add(WkbReader.ReadCoordinate(reader, is3D, isMeasured));
			}

			return result;
		}

		/// <summary>
		/// Reads Geometry from the reader.
		/// </summary>
		/// <param name="reader">The reader used to read data from input stream.</param>
		/// <returns>Geometry read from the input.</returns>
		private static Geometry ReadGeometry(BinaryReader reader) {
			WkbGeometryType geometryType = (WkbGeometryType)reader.ReadUInt32();

			bool is3D, isMeasured;
			WkbGeometryType basicType;
			WkbReader.GetGeometryTypeDetails(geometryType, out basicType, out is3D, out isMeasured);

			switch (basicType) {
				case WkbGeometryType.Point: return WkbReader.ReadPoint(reader, is3D, isMeasured);
				case WkbGeometryType.LineString: return WkbReader.ReadLineString(reader, is3D, isMeasured);
				case WkbGeometryType.Polygon: return WkbReader.ReadPolygon(reader, is3D, isMeasured);
				case WkbGeometryType.MultiPoint: return WkbReader.ReadMultiPoint(reader, is3D, isMeasured);
				case WkbGeometryType.MultiLineString: return WkbReader.ReadMultiLineString(reader, is3D, isMeasured);
				case WkbGeometryType.MultiPolygon: return WkbReader.ReadMultiPolygon(reader, is3D, isMeasured);
				case WkbGeometryType.GeometryCollection: return WkbReader.ReadGeometryCollection(reader, is3D, isMeasured);
				default: throw new WkbFormatException("Unknown geometry type.");
			}
		}

		/// <summary>
		/// Reads Point from the reader.
		/// </summary>
		/// <param name="reader">The reader used to read data from input stream.</param>
		/// <param name="is3D">bool value indicating whether point beeing read has Z-dimension.</param>
		/// <param name="isMeasured">bool value indicating whether point beeing read has M-value.</param>
		/// <returns>Point read from the input</returns>
		private static Point ReadPoint(BinaryReader reader, bool is3D, bool isMeasured) {
			Coordinate position = WkbReader.ReadCoordinate(reader, is3D, isMeasured);
			return new Point(position);
		}

		/// <summary>
		/// Reads LineString from the reader.
		/// </summary>
		/// <param name="reader">The reader used to read data from input stream.</param>
		/// <param name="is3D">bool value indicating whether linestring beeing read has Z-dimension.</param>
		/// <param name="isMeasured">bool value indicating whether linestring beeing read has M-value.</param>
		/// <returns>Linestring read from the input.</returns>
		private static LineString ReadLineString(BinaryReader reader, bool is3D, bool isMeasured) {
			IEnumerable<Coordinate> coordinates = WkbReader.ReadCoordinates(reader, is3D, isMeasured);
			return new LineString(coordinates);
		}

		/// <summary>
		/// Reads Polygon from the reader.
		/// </summary>
		/// <param name="reader">The reader used to read data from input stream.</param>
		/// <param name="is3D">bool value indicating whether polygon beeing read has Z-dimension.</param>
		/// <param name="isMeasured">bool value indicating whether polygon beeing read has M-value.</param>
		/// <returns>Polygon read from the input.</returns>
		private static Polygon ReadPolygon(BinaryReader reader, bool is3D, bool isMeasured) {
			int ringsCount = (int)reader.ReadUInt32();

			if (ringsCount == 0) {
				return new Polygon();
			}

			IEnumerable<Coordinate> exterior = WkbReader.ReadCoordinates(reader, is3D, isMeasured);
			Polygon result = new Polygon(new CoordinateList(exterior));

			for (int i = 1; i < ringsCount; i++) {
				IEnumerable<Coordinate> interior = WkbReader.ReadCoordinates(reader, is3D, isMeasured);
				result.InteriorRings.Add(new CoordinateList(interior));
			}

			return result;
		}

		/// <summary>
		/// Reads MultiLineString from the reader.
		/// </summary>
		/// <param name="reader">The reader used to read data from input stream.</param>
		/// <param name="is3D">bool value indicating whether multilinestring beeing read has Z-dimension.</param>
		/// <param name="isMeasured">bool value indicating whether multilinestring beeing read has M-value.</param>
		/// <returns>MultiLineString read from the input.</returns>
		private static MultiPoint ReadMultiPoint(BinaryReader reader, bool is3D, bool isMeasured) {
			int pointsCount = (int)reader.ReadUInt32();

			MultiPoint result = new MultiPoint();
			for (int i = 0; i < pointsCount; i++) {
				result.Geometries.Add(WkbReader.ReadPoint(reader, is3D, isMeasured));
			}

			return result;
		}

		/// <summary>
		/// Reads MultiLineString from the reader.
		/// </summary>
		/// <param name="reader">The reader used to read data from input stream.</param>
		/// <param name="is3D">bool value indicating whether multilinestring beeing read has Z-dimension.</param>
		/// <param name="isMeasured">bool value indicating whether multilinestring beeing read has M-value.</param>
		/// <returns>MultiLineString read from the input</returns>
		private static MultiLineString ReadMultiLineString(BinaryReader reader, bool is3D, bool isMeasured) {
			int pointsCount = (int)reader.ReadUInt32();

			MultiLineString result = new MultiLineString();
			for (int i = 0; i < pointsCount; i++) {
				result.Geometries.Add(WkbReader.ReadLineString(reader, is3D, isMeasured));
			}

			return result;
		}

		/// <summary>
		/// Reads MultiPolygon from the reader.
		/// </summary>
		/// <param name="reader">The reader used to read data from input stream.</param>
		/// <param name="is3D">bool value indicating whether MultiPolygon beeing read has Z-dimension.</param>
		/// <param name="isMeasured">bool value indicating whether MultiPolygon beeing read has M-value.</param>
		/// <returns>MultiPolygon read from the input.</returns>
		private static MultiPolygon ReadMultiPolygon(BinaryReader reader, bool is3D, bool isMeasured) {
			int pointsCount = (int)reader.ReadUInt32();

			MultiPolygon result = new MultiPolygon();
			for (int i = 0; i < pointsCount; i++) {
				result.Geometries.Add(WkbReader.ReadPolygon(reader, is3D, isMeasured));
			}

			return result;
		}

		/// <summary>
		/// Reads GeometryCollection from the reader.
		/// </summary>
		/// <param name="reader">The reader used to read data from input stream.</param>
		/// <param name="is3D">bool value indicating whether GeometryCollection beeing read has Z-dimension.</param>
		/// <param name="isMeasured">bool value indicating whether GeometryCollection beeing read has M-value.</param>
		/// <returns>GeometryCollection read from the input.</returns>
		private static GeometryCollection<Geometry> ReadGeometryCollection(BinaryReader reader, bool is3D, bool isMeasured) {
			int pointsCount = (int)reader.ReadUInt32();

			GeometryCollection<Geometry> result = new GeometryCollection<Geometry>();
			for (int i = 0; i < pointsCount; i++) {
				result.Geometries.Add(WkbReader.ReadGeometry(reader));
			}

			return result;
		}

		/// <summary>
		/// Gets details form the WkbGeometryType value.
		/// </summary>
		/// <param name="type">The value to be examined.</param>
		/// <param name="basicType">Outputs type striped of dimension information.</param>
		/// <param name="is3D">Outputs bool value indicating whether type represents 3D geometry.</param>
		/// <param name="isMeasured">Outputs bool value indicating whether type represents measured geometry.</param>
		private static void GetGeometryTypeDetails(WkbGeometryType type, out WkbGeometryType basicType, out bool is3D, out bool isMeasured) {
			is3D = ((int)type > 1000 && (int)type < 2000) || (int)type > 3000;
			isMeasured = (int)type > 2000;
			basicType = (WkbGeometryType)((int)type % 1000);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Releases the unmanaged resources used by the ComponentLibrary and optionally releases the managed resources.
		/// </summary>
		/// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
		private void Dispose(bool disposing) {
			if (!this._disposed) {
				if (disposing) {
					_inputReader.Dispose();

					if (_inputFileStream != null) {
						_inputFileStream.Dispose();
					}
				}

				_disposed = true;
			}
		}

		#endregion
	}
}
