namespace SpatialLite.H3.Internals;

internal static class LatLngConverter
{
    private const double MapP7RotRads = 0.333473172251832115;
    private const double Res0UGnomonic = 0.38196601125010500003;
    private const double Sqrt7 = 2.6457513110645905905;
    private const double Sin60 = 0.8660254037844386;
    private const double Epsilon = 1e-9;
    private const double TwoPi = 2.0 * Math.PI;
    private const ulong H3Init = (1UL << 59) | 0x1FFFFFFFFFFFUL;

    internal static ulong LatLngToCell(double latDeg, double lngDeg, int res)
    {
        if (res < 0 || res > 15)
        {
            throw new ArgumentOutOfRangeException(nameof(res), "Resolution must be between 0 and 15.");
        }

        double lat = latDeg * (Math.PI / 180.0);
        double lng = lngDeg * (Math.PI / 180.0);

        double cosLat = Math.Cos(lat);
        double vx = cosLat * Math.Cos(lng);
        double vy = cosLat * Math.Sin(lng);
        double vz = Math.Sin(lat);

        int face = 0;
        double bestSqd = double.MaxValue;
        for (int f = 0; f < 20; f++)
        {
            var (cx, cy, cz) = H3Tables.FaceCenterPoint[f];
            double sqd = ((vx - cx) * (vx - cx)) + ((vy - cy) * (vy - cy)) + ((vz - cz) * (vz - cz));
            if (sqd < bestSqd)
            {
                bestSqd = sqd;
                face = f;
            }
        }

        double r = Math.Acos(Math.Max(-1.0, Math.Min(1.0, 1.0 - (bestSqd / 2.0))));

        double x2d;
        double y2d;
        if (r < Epsilon)
        {
            x2d = 0.0;
            y2d = 0.0;
        }
        else
        {
            var (fcLat, fcLng) = H3Tables.FaceCenterGeo[face];
            double az = Math.Atan2(
                cosLat * Math.Sin(lng - fcLng),
                (Math.Cos(fcLat) * Math.Sin(lat)) - (Math.Sin(fcLat) * cosLat * Math.Cos(lng - fcLng)));

            double theta = PosAngle(H3Tables.FaceAxesAzRadsCII[face, 0] - PosAngle(az));
            if ((res & 1) == 1)
            {
                theta = PosAngle(theta - MapP7RotRads);
            }

            r = Math.Tan(r) / Res0UGnomonic;
            for (int i = 0; i < res; i++)
            {
                r *= Sqrt7;
            }

            x2d = r * Math.Cos(theta);
            y2d = r * Math.Sin(theta);
        }

        var ijk = Hex2dToCoordIjk(x2d, y2d);
        return FaceIjkToH3(face, ijk, res);
    }

    private static double PosAngle(double a)
    {
        double r = a % TwoPi;
        return r < 0 ? r + TwoPi : r;
    }

    private static CoordIjk Hex2dToCoordIjk(double x, double y)
    {
        double a1 = Math.Abs(x);
        double a2 = Math.Abs(y);

        double x2 = a2 / Sin60;
        double y2 = a1 - (a2 / (Sin60 * 2.0 / Math.Sqrt(3.0)));

        int m1;
        int m2;

        if (x2 + y2 > 1.0)
        {
            if (x2 > y2)
            {
                m1 = (int)Math.Round(x2 + (y2 / 2.0));
                m2 = (int)Math.Round(y2);
            }
            else
            {
                m1 = (int)Math.Round((x2 / 2.0) + y2);
                m2 = (int)Math.Round(x2);
            }
        }
        else
        {
            m1 = 0;
            m2 = 0;
        }

        double r1 = x - m1;
        double r2 = y - m2;

        int hi;
        int hj;

        if (Math.Abs(r1) >= Math.Abs(r2))
        {
            if (r1 >= 0.0)
            {
                if (r2 >= 0.0)
                {
                    hi = m1;
                    hj = m1 + m2;
                }
                else
                {
                    hi = m1 + m2;
                    hj = m1;
                }
            }
            else
            {
                if (r2 < 0.0)
                {
                    hi = -m1;
                    hj = -(m1 + m2);
                }
                else
                {
                    hi = -(m1 + m2);
                    hj = -m1;
                }
            }
        }
        else
        {
            if (r2 >= 0.0)
            {
                if (r1 >= 0.0)
                {
                    hi = m1 + m2;
                    hj = m2;
                }
                else
                {
                    hi = -m2;
                    hj = m1 + m2;
                }
            }
            else
            {
                if (r1 >= 0.0)
                {
                    hi = m2;
                    hj = -(m1 + m2);
                }
                else
                {
                    hi = -(m1 + m2);
                    hj = -m2;
                }
            }
        }

        var c = new CoordIjk(hi, hj, 0);
        c.NormalizeInPlace();
        return c;
    }

    private static int Rotate60Ccw(int d) => d switch
    {
        1 => 5,
        5 => 4,
        4 => 6,
        6 => 2,
        2 => 3,
        3 => 1,
        _ => d,
    };

    private static int Rotate60Cw(int d) => d switch
    {
        1 => 3,
        3 => 2,
        2 => 6,
        6 => 4,
        4 => 5,
        5 => 1,
        _ => d,
    };

    private static ulong H3Rotate60Ccw(ulong h, int res)
    {
        for (int r = 1; r <= res; r++)
        {
            h = SetDigit(h, r, Rotate60Ccw(GetDigit(h, r)));
        }

        return h;
    }

    private static ulong H3Rotate60Cw(ulong h, int res)
    {
        for (int r = 1; r <= res; r++)
        {
            h = SetDigit(h, r, Rotate60Cw(GetDigit(h, r)));
        }

        return h;
    }

    private static ulong H3RotatePent60Ccw(ulong h, int res)
    {
        bool foundFirst = false;
        for (int r = 1; r <= res; r++)
        {
            h = SetDigit(h, r, Rotate60Ccw(GetDigit(h, r)));
            if (!foundFirst && GetDigit(h, r) != 0)
            {
                foundFirst = true;
                if (LeadingNonZeroDigit(h, res) == 1)
                {
                    h = H3Rotate60Ccw(h, res);
                }
            }
        }

        return h;
    }

    private static int LeadingNonZeroDigit(ulong h, int res)
    {
        for (int r = 1; r <= res; r++)
        {
            int d = GetDigit(h, r);
            if (d != 0)
            {
                return d;
            }
        }

        return 0;
    }

    private static int GetDigit(ulong h, int r) => (int)((h >> ((15 - r) * 3)) & 7UL);

    private static ulong SetDigit(ulong h, int r, int d)
    {
        int shift = (15 - r) * 3;
        return (h & ~(7UL << shift)) | ((ulong)d << shift);
    }

    private static ulong FaceIjkToH3(int face, CoordIjk ijk, int res)
    {
        ulong h = H3Init;
        h = (h & ~(0xFUL << 52)) | ((ulong)res << 52);

        if (res == 0)
        {
            if (ijk.I > 2 || ijk.J > 2 || ijk.K > 2)
            {
                return 0;
            }

            var (bc0, _) = H3Tables.GetFaceIjkBaseCell(face, ijk.I, ijk.J, ijk.K);
            h = (h & ~(0x7FUL << 45)) | ((ulong)bc0 << 45);
            return h;
        }

        int fi = ijk.I;
        int fj = ijk.J;
        int fk = ijk.K;

        for (int rr = res - 1; rr >= 0; rr--)
        {
            var lastIjk = new CoordIjk(fi, fj, fk);
            var cur = new CoordIjk(fi, fj, fk);
            CoordIjk lastCenter;

            if (((rr + 1) & 1) == 1)
            {
                cur.UpAp7();
                lastCenter = new CoordIjk(cur.I, cur.J, cur.K);
                lastCenter.DownAp7();
            }
            else
            {
                cur.UpAp7r();
                lastCenter = new CoordIjk(cur.I, cur.J, cur.K);
                lastCenter.DownAp7r();
            }

            fi = cur.I;
            fj = cur.J;
            fk = cur.K;

            var diff = CoordIjk.Sub(lastIjk, lastCenter);
            int digit = diff.ToDigit();
            h = SetDigit(h, rr + 1, digit);
        }

        if (fi > 2 || fj > 2 || fk > 2)
        {
            return 0;
        }

        var (baseCell, numRots) = H3Tables.GetFaceIjkBaseCell(face, fi, fj, fk);
        h = (h & ~(0x7FUL << 45)) | ((ulong)baseCell << 45);

        var bcd = H3Tables.BaseCellData[baseCell];
        if (bcd.IsPentagon)
        {
            if (LeadingNonZeroDigit(h, res) == 1)
            {
                if (face == bcd.CwOff0 || face == bcd.CwOff1)
                {
                    h = H3Rotate60Cw(h, res);
                }
                else
                {
                    h = H3Rotate60Ccw(h, res);
                }
            }

            for (int i = 0; i < numRots; i++)
            {
                h = H3RotatePent60Ccw(h, res);
            }
        }
        else
        {
            for (int i = 0; i < numRots; i++)
            {
                h = H3Rotate60Ccw(h, res);
            }
        }

        return h;
    }
}
