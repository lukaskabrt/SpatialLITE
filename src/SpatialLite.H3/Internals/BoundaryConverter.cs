using SpatialLite.Contracts;

namespace SpatialLite.H3.Internals;

internal static class BoundaryConverter
{
    private const double Sin60 = 0.8660254037844386;
    private const double Res0UGnomonic = 0.38196601125010500003;
    private const double Sqrt7 = 2.6457513110645905905;
    private const double TwoPi = 2.0 * Math.PI;
    private const double Epsilon = 1e-9;

    internal static IReadOnlyList<Coordinate> CellToBoundary(ulong h, int res, bool isPentagon)
    {
        int baseCell = (int)((h >> 45) & 0x7F);
        var bcd = H3Tables.BaseCellData[baseCell];
        int face = bcd.Face;
        var ijk = new CoordIjk(bcd.I, bcd.J, bcd.K);

        for (int r = 1; r <= res; r++)
        {
            int d = (int)((h >> ((15 - r) * 3)) & 7);
            if ((r & 1) == 1)
            {
                ijk.DownAp7();
            }
            else
            {
                ijk.DownAp7r();
            }

            var uv = H3Tables.UnitVecs[d];
            ijk.I += uv.I;
            ijk.J += uv.J;
            ijk.K += uv.K;
            ijk.NormalizeInPlace();
        }

        int adjRes = res + 1;
        bool isClassIII = (res & 1) == 1;

        var scaledIjk = new CoordIjk(ijk.I * 3, ijk.J * 3, ijk.K * 3);
        scaledIjk.NormalizeInPlace();
        if (isClassIII)
        {
            scaledIjk.DownAp7r();
        }
        else
        {
            scaledIjk.DownAp7();
        }

        var boundary = new List<Coordinate>(6);
        for (int v = 0; v < 6; v++)
        {
            if (isPentagon && v == 0)
            {
                continue;
            }

            var uv = H3Tables.UnitVecs[v + 1];
            var vertexIjk = new CoordIjk(
                scaledIjk.I + uv.I,
                scaledIjk.J + uv.J,
                scaledIjk.K + uv.K);
            vertexIjk.NormalizeInPlace();
            var (lat, lng) = FaceIjkToLatLng(face, vertexIjk, adjRes);
            boundary.Add(new Coordinate(lng, lat));
        }

        return boundary;
    }

    private static (double Lat, double Lng) FaceIjkToLatLng(int face, CoordIjk ijk, int adjRes)
    {
        int ri = ijk.I - ijk.K;
        int rj = ijk.J - ijk.K;
        double x2d = ri - (0.5 * rj);
        double y2d = rj * Sin60;

        double r = Math.Sqrt((x2d * x2d) + (y2d * y2d));

        if (r < Epsilon)
        {
            var fc = H3Tables.FaceCenterGeo[face];
            return (fc.Lat * (180.0 / Math.PI), fc.Lng * (180.0 / Math.PI));
        }

        double theta = Math.Atan2(y2d, x2d);

        r /= 3.0;
        if ((adjRes & 1) == 0)
        {
            r /= Sqrt7;
        }

        r *= Res0UGnomonic;
        for (int i = 0; i < adjRes; i++)
        {
            r /= Sqrt7;
        }

        r = Math.Atan(r);

        theta = PosAngle(H3Tables.FaceAxesAzRadsCII[face, 0] - theta);

        var (fcLat, fcLng) = H3Tables.FaceCenterGeo[face];
        double lat = Math.Asin(
            (Math.Sin(fcLat) * Math.Cos(r)) + (Math.Cos(fcLat) * Math.Sin(r) * Math.Cos(theta)));
        double lng = fcLng + Math.Atan2(
            Math.Sin(theta) * Math.Sin(r) * Math.Cos(fcLat),
            Math.Cos(r) - (Math.Sin(fcLat) * Math.Sin(lat)));

        lng = lng % TwoPi;
        if (lng < -Math.PI)
        {
            lng += TwoPi;
        }
        else if (lng > Math.PI)
        {
            lng -= TwoPi;
        }

        return (lat * (180.0 / Math.PI), lng * (180.0 / Math.PI));
    }

    private static double PosAngle(double a)
    {
        double r = a % TwoPi;
        return r < 0 ? r + TwoPi : r;
    }
}
