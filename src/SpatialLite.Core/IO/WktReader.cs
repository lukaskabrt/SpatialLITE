using SpatialLite.Core.API;
using SpatialLite.Core.Geometries;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace SpatialLite.Core.IO;

/// <summary>
/// Provides functions for reading and parsing geometries from WKT format.
/// </summary>
public class WktReader : IDisposable
{

    private static readonly CultureInfo _invarianCulture = CultureInfo.InvariantCulture;

    private readonly TextReader _inputReader;
    private readonly FileStream _inputFileStream;
    private readonly WktTokensBuffer _tokens;

    private bool _disposed = false;

    /// <summary>
    /// Initializes a new instance of the WktReader class that reads data from specific stream.
    /// </summary>
    /// <param name="input">The stream to read data from.</param>
    public WktReader(Stream input)
    {
        if (input == null)
        {
            throw new ArgumentNullException(nameof(input), "Input stream cannot be null");
        }

        _inputReader = new StreamReader(input);
        _tokens = new WktTokensBuffer(WktTokenizer.Tokenize(_inputReader));
    }

    /// <summary>
    /// Initializes a new instance of the WktReader class that reads data from specific file.
    /// </summary>
    /// <param name="path">Path to the file to read data from.</param>
    public WktReader(string path)
    {
        _inputFileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        _inputReader = new StreamReader(_inputFileStream);
        _tokens = new WktTokensBuffer(WktTokenizer.Tokenize(_inputReader));
    }

    /// <summary>
    /// Parses a Geometry from WKT string.
    /// </summary>
    /// <param name="wkt">The string with WKT representation of a Geometry.</param>
    /// <returns>The parsed Geometry.</returns>
    public static Geometry Parse(string wkt)
    {
        WktTokensBuffer tokens = new WktTokensBuffer(WktTokenizer.Tokenize(wkt));
        return ParseGeometryTaggedText(tokens);
    }

    /// <summary>
    /// Parses a Geometry of specific type from the WKT string.
    /// </summary>
    /// <typeparam name="T">The type of the Geometry to be parsed.</typeparam>
    /// <param name="wkt">The string with WKT representation of a Geometry.</param>
    /// <returns>The parsed Geometry of given type.</returns>
    public static T Parse<T>(string wkt) where T : Geometry
    {
        Geometry parsed = Parse(wkt);

        if (parsed != null)
        {
            if (parsed is not T result)
            {
                throw new WktParseException("Input doesn't contain valid WKB representation of the specified geometry type.");
            }

            return result;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Reads next geometry from the input.
    /// </summary>
    /// <returns>The geometry object read from the reader or null if no more geometries are available.</returns>
    public Geometry Read()
    {
        return ParseGeometryTaggedText(_tokens);
    }

    /// <summary>
    /// Reads next geometry from the input.
    /// </summary>
    /// <typeparam name="T">The type of Geometry to be parsed.</typeparam>
    /// <returns>The geometry object of specific type read from the reader or null if no more geometries are available.</returns>
    public T Read<T>() where T : Geometry
    {
        Geometry parsed = ParseGeometryTaggedText(_tokens);

        if (parsed != null)
        {
            if (parsed is not T result)
            {
                throw new WktParseException("Input doesn't contain valid WKB representation of the specified geometry type.");
            }

            return result;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Releases all resources used by the ComponentLibrary.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Parses a geometry tagged text.
    /// </summary>
    /// <param name="tokens">The list of tokens.</param>
    /// <returns>A geometry specified by tokens.</returns>
    private static Geometry ParseGeometryTaggedText(WktTokensBuffer tokens)
    {
        WktToken t = tokens.Peek(true);

        if (t.Type == TokenType.STRING)
        {
            if (t.Value.ToUpperInvariant() == "POINT")
            {
                return ParsePointTaggedText(tokens);
            }
            else if (t.Value.ToUpperInvariant() == "LINESTRING")
            {
                return ParseLineStringTaggedText(tokens);
            }
            else if (t.Value.ToUpperInvariant() == "POLYGON")
            {
                return ParsePolygonTaggedText(tokens);
            }
            else if (t.Value.ToUpperInvariant() == "MULTIPOINT")
            {
                return ParseMultiPointTaggedText(tokens);
            }
            else if (t.Value.ToUpperInvariant() == "MULTILINESTRING")
            {
                return ParseMultiLineStringTaggedText(tokens);
            }
            else if (t.Value.ToUpperInvariant() == "MULTIPOLYGON")
            {
                return ParseMultiPolygonTaggedText(tokens);
            }
            else if (t.Value.ToUpperInvariant() == "GEOMETRYCOLLECTION")
            {
                return ParseGeometryCollectionTaggedText(tokens);
            }
        }

        if (t.Type == TokenType.END_OF_DATA)
        {
            return null;
        }

        throw new WktParseException(string.Format("Invalid geometry type '{0}'", t.Value));
    }

    /// <summary>
    /// Parses a point tagged text.
    /// </summary>
    /// <param name="tokens">The list of tokens.</param>
    /// <returns>A point specified by tokens.</returns>
    /// <remarks><![CDATA[Point tagged text format: <point tagged text> ::=  point {z}{m} <point text>]]></remarks>
    private static Point ParsePointTaggedText(WktTokensBuffer tokens)
    {
        Expect("point", tokens);
        Expect(TokenType.WHITESPACE, tokens);

        bool is3D = false;
        bool isMeasured = false;

        WktToken t = tokens.Peek(true);
        if (TryParseDimensions(t, out is3D, out isMeasured))
        {
            tokens.GetToken(true);
            Expect(TokenType.WHITESPACE, tokens);
        }

        return ParsePointText(tokens, is3D, isMeasured);
    }

    /// <summary>
    /// Parses a point tagged text.
    /// </summary>
    /// <param name="tokens">The list of tokens.</param>
    /// <param name="is3D">bool value indicating whether point being parsed has z-coordinate.</param>
    /// <param name="isMeasured">bool value indicating whether point beeing parsed has m-value.</param>
    /// <returns>A point specified by tokens</returns>
    /// <remarks><![CDATA[<empty set> | <left paren> <point> <right paren> ]]></remarks>
    private static Point ParsePointText(WktTokensBuffer tokens, bool is3D, bool isMeasured)
    {
        WktToken t = tokens.Peek(true);

        if (t.Type == TokenType.STRING && t.Value.ToUpperInvariant() == "EMPTY")
        {
            tokens.GetToken(true);
            return new Point();
        }

        Expect(TokenType.LEFT_PARENTHESIS, tokens);
        Point result = new Point(ParseCoordinate(tokens, is3D, isMeasured));
        Expect(TokenType.RIGHT_PARENTHESIS, tokens);

        return result;
    }

    /// <summary>
    /// Parses multiple Points separated by comma.
    /// </summary>
    /// <param name="tokens">The list of tokens.</param>
    /// <param name="is3D">bool value indicating whether point being parsed had z-coordinate.</param>
    /// <param name="isMeasured">bool value indicating whether point being parsed has m-value.</param>
    /// <returns>A list of point specified by tokens.</returns>
    private static List<Point> ParsePoints(WktTokensBuffer tokens, bool is3D, bool isMeasured)
    {
        List<Point> points = new List<Point>();

        points.Add(ParsePointText(tokens, is3D, isMeasured));

        WktToken t = tokens.Peek(true);
        while (t.Type == TokenType.COMMA)
        {
            tokens.GetToken(true);
            points.Add(ParsePointText(tokens, is3D, isMeasured));
            t = tokens.Peek(true);
        }

        return points;
    }

    /// <summary>
    /// Parses a coordinate.
    /// </summary>
    /// <param name="tokens">The list of tokens.</param>
    /// <param name="is3D">bool value indicating whether coordinate being parsed had z-coordinate.</param>
    /// <param name="isMeasured">bool value indicating whether coordinate beeing parsed has m-value.</param>
    /// <returns>A coordinate specified by tokens.</returns>
    /// <remarks><![CDATA[<x> <y> {<z>} {<m>}]]></remarks>
    private static Coordinate ParseCoordinate(WktTokensBuffer tokens, bool is3D, bool isMeasured)
    {
        WktToken t = Expect(TokenType.NUMBER, tokens);
        double x = double.Parse(t.Value, _invarianCulture);

        Expect(TokenType.WHITESPACE, tokens);

        t = Expect(TokenType.NUMBER, tokens);
        double y = double.Parse(t.Value, _invarianCulture);

        double z = double.NaN;
        double m = double.NaN;

        if (is3D)
        {
            Expect(TokenType.WHITESPACE, tokens);

            t = Expect(TokenType.NUMBER, tokens);
            z = double.Parse(t.Value, _invarianCulture);
        }

        if (isMeasured)
        {
            Expect(TokenType.WHITESPACE, tokens);

            t = Expect(TokenType.NUMBER, tokens);
            m = double.Parse(t.Value, _invarianCulture);
        }

        return new Coordinate(x, y, z, m);
    }

    /// <summary>
    /// Parses a series of coordinate.
    /// </summary>
    /// <param name="tokens">The list of tokens.</param>
    /// <param name="is3D">bool value indicating whether coordinate being parsed had z-coordinate.</param>
    /// <param name="isMeasured">bool value indicating whether coordinate beeing parsed has m-value.</param>
    /// <returns>A list of coordinates specified by tokens.</returns>
    private static IEnumerable<Coordinate> ParseCoordinates(WktTokensBuffer tokens, bool is3D, bool isMeasured)
    {
        List<Coordinate> coordinates = new List<Coordinate>();

        coordinates.Add(ParseCoordinate(tokens, is3D, isMeasured));

        WktToken t = tokens.Peek(true);
        while (t.Type == TokenType.COMMA)
        {
            tokens.GetToken(true);
            Expect(TokenType.WHITESPACE, tokens);
            coordinates.Add(ParseCoordinate(tokens, is3D, isMeasured));
            t = tokens.Peek(true);
        }

        return coordinates;
    }

    /// <summary>
    /// Parses a linestring tagged text.
    /// </summary>
    /// <param name="tokens">The list of tokens.</param>
    /// <returns>A linestring specified by tokens.</returns>
    /// <remarks><![CDATA[<linestring tagged text> ::=  linestring {z}{m} <linestring text>]]></remarks>
    private static LineString ParseLineStringTaggedText(WktTokensBuffer tokens)
    {
        Expect("linestring", tokens);
        Expect(TokenType.WHITESPACE, tokens);

        bool is3D = false;
        bool isMeasured = false;

        WktToken t = tokens.Peek(true);
        if (TryParseDimensions(t, out is3D, out isMeasured))
        {
            tokens.GetToken(true);
            Expect(TokenType.WHITESPACE, tokens);
        }

        return ParseLineStringText(tokens, is3D, isMeasured);
    }

    /// <summary>
    /// Parses a linestring text.
    /// </summary>
    /// <param name="tokens">The list of tokens.</param>
    /// <param name="is3D">bool value indicating whether linestring being parsed has z-coordinate.</param>
    /// <param name="isMeasured">bool value indicating whether linestring being parsed has m-value.</param>
    /// <returns>A linestring specified by tokens.</returns>
    /// <remarks><![CDATA[<empty set> | <left paren> <point> {<comma> <point>}* <right paren>]]></remarks>
    private static LineString ParseLineStringText(WktTokensBuffer tokens, bool is3D, bool isMeasured)
    {
        WktToken t = tokens.Peek(true);

        if (t.Type == TokenType.STRING && t.Value.ToUpperInvariant() == "EMPTY")
        {
            tokens.GetToken(true);
            return new LineString();
        }

        Expect(TokenType.LEFT_PARENTHESIS, tokens);
        IEnumerable<Coordinate> coords = ParseCoordinates(tokens, is3D, isMeasured);
        Expect(TokenType.RIGHT_PARENTHESIS, tokens);

        return new LineString(coords);
    }

    /// <summary>
    /// Parses multiple LineStrings separated by comma.
    /// </summary>
    /// <param name="tokens">The list of tokens.</param>
    /// <param name="is3D">bool value indicating whether linestring being parsed had z-coordinate.</param>
    /// <param name="isMeasured">bool value indicating whether linestring being parsed has m-value.</param>
    /// <returns>A list of linestrings specified by tokens.</returns>
    private static List<LineString> ParseLineStrings(WktTokensBuffer tokens, bool is3D, bool isMeasured)
    {
        List<LineString> linestrigns = new List<LineString>();

        linestrigns.Add(ParseLineStringText(tokens, is3D, isMeasured));

        WktToken t = tokens.Peek(true);
        while (t.Type == TokenType.COMMA)
        {
            tokens.GetToken(true);
            linestrigns.Add(ParseLineStringText(tokens, is3D, isMeasured));
            t = tokens.Peek(true);
        }

        return linestrigns;
    }

    /// <summary>
    /// Parses a polygon tagged text.
    /// </summary>
    /// <param name="tokens">The list of tokens.</param>
    /// <returns>A polygon specified by tokens.</returns>
    /// <remarks><![CDATA[<polygon tagged text> ::=  polygon {z}{m} <polygon text>]]></remarks>
    private static Polygon ParsePolygonTaggedText(WktTokensBuffer tokens)
    {
        Expect("polygon", tokens);
        Expect(TokenType.WHITESPACE, tokens);

        bool is3D = false;
        bool isMeasured = false;

        WktToken t = tokens.Peek(true);
        if (TryParseDimensions(t, out is3D, out isMeasured))
        {
            tokens.GetToken(true);
            Expect(TokenType.WHITESPACE, tokens);
        }

        return ParsePolygonText(tokens, is3D, isMeasured);
    }

    /// <summary>
    /// Parses a polygon text.
    /// </summary>
    /// <param name="tokens">The list of tokens.</param>
    /// <param name="is3D">bool value indicating whether polygon being parsed has z-coordinate.</param>
    /// <param name="isMeasured">bool value indicating whether polygon being parsed has m-value.</param>
    /// <returns>A polygon specified by tokens.</returns>
    /// <remarks><![CDATA[<empty set> | <left paren> <linestring text> {<comma> <linestring text>}* <right paren>]]></remarks>
    private static Polygon ParsePolygonText(WktTokensBuffer tokens, bool is3D, bool isMeasured)
    {
        WktToken t = tokens.Peek(true);

        if (t.Type == TokenType.STRING && t.Value.ToUpperInvariant() == "EMPTY")
        {
            tokens.GetToken(true);
            return new Polygon();
        }

        Expect(TokenType.LEFT_PARENTHESIS, tokens);

        IEnumerable<LineString> linestrings = ParseLineStrings(tokens, is3D, isMeasured);
        Polygon result = new Polygon(linestrings.First().Coordinates);

        foreach (var inner in linestrings.Skip(1))
        {
            result.InteriorRings.Add(inner.Coordinates);
        }

        Expect(TokenType.RIGHT_PARENTHESIS, tokens);

        return result;
    }

    /// <summary>
    /// Parses series of polygons.
    /// </summary>
    /// <param name="tokens">The list of tokens.</param>
    /// <param name="is3D">bool value indicating whether polygon being parsed has z-coordinate.</param>
    /// <param name="isMeasured">bool value indicating whether polygon being parsed has m-value.</param>
    /// <returns>A list of polygons specified by tokens.</returns>
    private static IEnumerable<Polygon> ParsePolygons(WktTokensBuffer tokens, bool is3D, bool isMeasured)
    {
        List<Polygon> polygons = new List<Polygon>();

        polygons.Add(ParsePolygonText(tokens, is3D, isMeasured));

        WktToken t = tokens.Peek(true);
        while (t.Type == TokenType.COMMA)
        {
            tokens.GetToken(true);
            polygons.Add(ParsePolygonText(tokens, is3D, isMeasured));
            t = tokens.Peek(true);
        }

        return polygons;
    }

    /// <summary>
    /// Parses a multilinestring tagged text.
    /// </summary>
    /// <param name="tokens">The list of tokens.</param>
    /// <returns>A multilinestring specified by tokens.</returns>
    /// <remarks><![CDATA[<multilinestring tagged text> ::=  multilinestring {z}{m} <multilinestring text>  ]]></remarks>
    private static MultiLineString ParseMultiLineStringTaggedText(WktTokensBuffer tokens)
    {
        Expect("multilinestring", tokens);
        Expect(TokenType.WHITESPACE, tokens);

        bool is3D = false;
        bool isMeasured = false;

        WktToken t = tokens.Peek(true);
        if (TryParseDimensions(t, out is3D, out isMeasured))
        {
            tokens.GetToken(true);
            Expect(TokenType.WHITESPACE, tokens);
        }

        return ParseMultiLineStringText(tokens, is3D, isMeasured);
    }

    /// <summary>
    /// Parses a multilinestring text.
    /// </summary>
    /// <param name="tokens">The list of tokens.</param>
    /// <param name="is3D">bool value indicating whether multilinestring being parsed has z-coordinate.</param>
    /// <param name="isMeasured">bool value indicating whether multilinestring being parsed has m-value.</param>
    /// <returns>A multilinestring specified by tokens.</returns>
    /// <remarks><![CDATA[<multilinestring text> ::= <empty set> | <left paren><linestring text> {<comma> <linestring text>}* <right paren>]]></remarks>
    private static MultiLineString ParseMultiLineStringText(WktTokensBuffer tokens, bool is3D, bool isMeasured)
    {
        WktToken t = tokens.Peek(true);

        if (t.Type == TokenType.STRING && t.Value.ToUpperInvariant() == "EMPTY")
        {
            tokens.GetToken(true);
            return new MultiLineString();
        }

        Expect(TokenType.LEFT_PARENTHESIS, tokens);
        MultiLineString result = new MultiLineString(ParseLineStrings(tokens, is3D, isMeasured));
        Expect(TokenType.RIGHT_PARENTHESIS, tokens);

        return result;
    }

    /// <summary>
    /// Parses a multipoint tagged text.
    /// </summary>
    /// <param name="tokens">The list of tokens.</param>
    /// <returns>A multipoint specified by tokens.</returns>
    /// <remarks><![CDATA[<multipoint tagged text> ::=  multipoint {z}{m} <multipoint text>  ]]></remarks>
    private static MultiPoint ParseMultiPointTaggedText(WktTokensBuffer tokens)
    {
        Expect("multipoint", tokens);
        Expect(TokenType.WHITESPACE, tokens);

        bool is3D = false;
        bool isMeasured = false;

        WktToken t = tokens.Peek(true);
        if (TryParseDimensions(t, out is3D, out isMeasured))
        {
            tokens.GetToken(true);
            Expect(TokenType.WHITESPACE, tokens);
        }

        return ParseMultiPointText(tokens, is3D, isMeasured);
    }

    /// <summary>
    /// Parses a multipoint text.
    /// </summary>
    /// <param name="tokens">The list of tokens.</param>
    /// <param name="is3D">bool value indicating whether multipoint being parsed has z-coordinate.</param>
    /// <param name="isMeasured">bool value indicating whether multipoint being parsed has m-value.</param>
    /// <returns>A multipoint specified by tokens.</returns>
    /// <remarks><![CDATA[<multipoint text> ::= <empty set> | <left paren><point text> {<comma> <point text>}* <right paren>]]></remarks>
    private static MultiPoint ParseMultiPointText(WktTokensBuffer tokens, bool is3D, bool isMeasured)
    {
        WktToken t = tokens.Peek(true);

        if (t.Type == TokenType.STRING && t.Value.ToUpperInvariant() == "EMPTY")
        {
            tokens.GetToken(true);
            return new MultiPoint();
        }

        Expect(TokenType.LEFT_PARENTHESIS, tokens);
        MultiPoint result = new MultiPoint(ParsePoints(tokens, is3D, isMeasured));
        Expect(TokenType.RIGHT_PARENTHESIS, tokens);

        return result;
    }

    /// <summary>
    /// Parses a multipolyugon tagged text.
    /// </summary>
    /// <param name="tokens">The list of tokens.</param>
    /// <returns>A multipolygon specified by tokens.</returns>
    /// <remarks><![CDATA[<multipolygon tagged text> ::=  multipolygon {z}{m} <multipolygon text>]]></remarks>
    private static MultiPolygon ParseMultiPolygonTaggedText(WktTokensBuffer tokens)
    {
        Expect("multipolygon", tokens);
        Expect(TokenType.WHITESPACE, tokens);

        bool is3D = false;
        bool isMeasured = false;

        WktToken t = tokens.Peek(true);
        if (TryParseDimensions(t, out is3D, out isMeasured))
        {
            tokens.GetToken(true);
            Expect(TokenType.WHITESPACE, tokens);
        }

        return ParseMultiPolygonText(tokens, is3D, isMeasured);
    }

    /// <summary>
    /// Parses a multipolygon text.
    /// </summary>
    /// <param name="tokens">The list of tokens.</param>
    /// <param name="is3D">bool value indicating whether multipolygon being parsed has z-coordinate.</param>
    /// <param name="isMeasured">bool value indicating whether multipolygon being parsed has m-value.</param>
    /// <returns>A multipolygon specified by tokens.</returns>
    /// <remarks><![CDATA[<multipolygon text> ::= <empty set> | <left paren> <polygon text> {<comma> <polygon text>}* <right paren>]]></remarks>
    private static MultiPolygon ParseMultiPolygonText(WktTokensBuffer tokens, bool is3D, bool isMeasured)
    {
        WktToken t = tokens.Peek(true);

        if (t.Type == TokenType.STRING && t.Value.ToUpperInvariant() == "EMPTY")
        {
            tokens.GetToken(true);
            return new MultiPolygon();
        }

        Expect(TokenType.LEFT_PARENTHESIS, tokens);
        MultiPolygon result = new MultiPolygon(ParsePolygons(tokens, is3D, isMeasured));
        Expect(TokenType.RIGHT_PARENTHESIS, tokens);

        return result;
    }

    /// <summary>
    /// Parses a multipolyugon tagged text.
    /// </summary>
    /// <param name="tokens">The list of tokens.</param>
    /// <returns>A GeometryCollection specified by tokens.</returns>
    /// <remarks><![CDATA[<GeometryCollection tagged text> ::=  GeometryCollection {z}{m} <GeometryCollection text>]]></remarks>
    private static GeometryCollection<Geometry> ParseGeometryCollectionTaggedText(WktTokensBuffer tokens)
    {
        Expect("geometrycollection", tokens);
        Expect(TokenType.WHITESPACE, tokens);

        bool is3D = false;
        bool isMeasured = false;

        WktToken t = tokens.Peek(true);
        if (TryParseDimensions(t, out is3D, out isMeasured))
        {
            tokens.GetToken(true);
            Expect(TokenType.WHITESPACE, tokens);
        }

        return ParseGeometryCollectionText(tokens, is3D, isMeasured);
    }

    /// <summary>
    /// Parses a GeometryCollection text.
    /// </summary>
    /// <param name="tokens">The list of tokens.</param>
    /// <param name="is3D">bool value indicating whether GeometryCollection being parsed has z-coordinate.</param>
    /// <param name="isMeasured">bool value indicating whether GeometryCollection being parsed has m-value.</param>
    /// <returns>A GeometryCollection specified by tokens.</returns>
    /// <remarks><![CDATA[<GeometryCollection text> ::= <empty set> | <left paren> <geometry tagged text> {<comma> <geometry tagged text>}* <right paren>]]></remarks>
    private static GeometryCollection<Geometry> ParseGeometryCollectionText(WktTokensBuffer tokens, bool is3D, bool isMeasured)
    {
        WktToken t = tokens.Peek(true);

        if (t.Type == TokenType.STRING && t.Value.ToUpperInvariant() == "EMPTY")
        {
            tokens.GetToken(true);
            return new GeometryCollection<Geometry>();
        }

        Expect(TokenType.LEFT_PARENTHESIS, tokens);

        GeometryCollection<Geometry> result = new GeometryCollection<Geometry>();
        result.Geometries.Add(ParseGeometryTaggedText(tokens));

        t = tokens.Peek(true);
        while (t.Type == TokenType.COMMA)
        {
            tokens.GetToken(true);
            result.Geometries.Add(ParseGeometryTaggedText(tokens));
            t = tokens.Peek(true);
        }

        Expect(TokenType.RIGHT_PARENTHESIS, tokens);

        return result;
    }

    /// <summary>
    /// Retrieves the next token from the tokens and checks if it has expected type.
    /// </summary>
    /// <param name="type">Expected type of token.</param>
    /// <param name="tokens">The list of tokens.</param>
    /// <returns>The head of the tokens queue.</returns>
    private static WktToken Expect(TokenType type, WktTokensBuffer tokens)
    {
        bool ignoreWhitespace = type != TokenType.WHITESPACE;

        WktToken t = tokens.GetToken(ignoreWhitespace);

        if (t.Type != type)
        {
            string expected = string.Empty;
            switch (type)
            {
                case TokenType.WHITESPACE: expected = " "; break;
                case TokenType.LEFT_PARENTHESIS: expected = "("; break;
                case TokenType.RIGHT_PARENTHESIS: expected = ")"; break;
                case TokenType.STRING: expected = "STRING"; break;
                case TokenType.NUMBER: expected = "NUMBER"; break;
            }

            throw new WktParseException(string.Format("Expected '{0}' but encountered '{1}'", expected, t.Value));
        }

        return t;
    }

    /// <summary>
    /// Retrieves the next token from tokens and checks if it is TokenType.STRING and if it has correct value.
    /// </summary>
    /// <param name="value">Expected string value of the token.</param>
    /// <param name="tokens">The list of tokens.</param>
    /// <returns>The head of the tokens queue.</returns>
    /// <remarks>String comparsion is not case-sensitive.</remarks>
    private static WktToken Expect(string value, WktTokensBuffer tokens)
    {
        WktToken t = tokens.GetToken(true);

        if (t.Type != TokenType.STRING || string.Equals(value, t.Value, StringComparison.OrdinalIgnoreCase) == false)
        {
            throw new WktParseException(string.Format("Expected '{0}' but encountered '{1}", value, t.Value));
        }

        return t;
    }

    /// <summary>
    /// Examines the given token and tries to parse it as WKT dimensions text.
    /// </summary>
    /// <param name="token">The WKT token to parse.</param>
    /// <param name="is3D">Parameter that specified whether dimension text contains {z}.</param>
    /// <param name="isMeasured">Parameter that specified whether dimension text contains {m}.</param>
    /// <returns>true if token was successfuly parsed as WKT dimension text, oterwise returns false.</returns>
    private static bool TryParseDimensions(WktToken token, out bool is3D, out bool isMeasured)
    {
        is3D = false;
        isMeasured = false;

        if (token.Type == TokenType.STRING)
        {
            if (token.Value.ToUpperInvariant() == "Z")
            {
                is3D = true;
            }
            else if (token.Value.ToUpperInvariant() == "M")
            {
                isMeasured = true;
            }
            else if (token.Value.ToUpperInvariant() == "ZM")
            {
                is3D = true;
                isMeasured = true;
            }

            return is3D || isMeasured;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Releases the unmanaged resources used by the ComponentLibrary and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _inputReader.Dispose();

                if (_inputFileStream != null)
                {
                    _inputFileStream.Dispose();
                }
            }

            _disposed = true;
        }
    }
}
