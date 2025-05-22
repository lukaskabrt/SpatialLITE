namespace SpatialLite.Core.IO
{
    /// <summary>
    /// Defines WkbGeometryType.
    /// </summary>
    internal enum WkbGeometryType : uint
    {
        Point = 1,
        LineString = 2,
        Polygon = 3,
        Triangle = 17,
        MultiPoint = 4,
        MultiLineString = 5,
        MultiPolygon = 6,
        GeometryCollection = 7,
        PolyhedralSurface = 15,
        TIN = 16,

        PointZ = 1001,
        LineStringZ = 1002,
        PolygonZ = 1003,
        Trianglez = 1017,
        MultiPointZ = 1004,
        MultiLineStringZ = 1005,
        MultiPolygonZ = 1006,
        GeometryCollectionZ = 1007,
        PolyhedralSurfaceZ = 1015,
        TINZ = 1016,

        PointM = 2001,
        LineStringM = 2002,
        PolygonM = 2003,
        TriangleM = 2017,
        MultiPointM = 2004,
        MultiLineStringM = 2005,
        MultiPolygonM = 2006,
        GeometryCollectionM = 2007,
        PolyhedralSurfaceM = 2015,
        TINM = 2016,

        PointZM = 3001,
        LineStringZM = 3002,
        PolygonZM = 3003,
        TriangleZM = 3017,
        MultiPointZM = 3004,
        MultiLineStringZM = 3005,
        MultiPolygonZM = 3006,
        GeometryCollectionZM = 3007,
        PolyhedralSurfaceZM = 3015,
        TinZM = 3016
    }
}
