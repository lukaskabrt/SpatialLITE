using System.Runtime.CompilerServices;

namespace SpatialLite.H3.Internals;

// Port of the H3 geo-to-cell algorithm from the Uber H3 library.
// Reference commit: 006ba2c73a944691777e6bbf08e3754bf5cc9212
// All static look-up tables live in H3IndexData; this class contains only
// the algorithm constants and computation methods.
internal static class LatLngConverter
{
    // ── Algorithm constants ───────────────────────────────────────────────────

    private const double M2Pi = 6.28318530717958647692528676655900576839433;
    private const double Epsilon = 0.0000000000000001; // 1e-16
    private const double MAp7RotRads = 0.333473172251832115336090755351601070065900389;
    private const double InvRes0UGnomonic = 2.61803398874989588842;
    private const double MSqrt7 = 2.6457513110645905905016157536392604257102;
    private const double MRsin60 = 1.1547005383792515290182975610039149112953; // 2/sqrt(3)
    private const double MPi180 = 0.0174532925199432957692369076848861271111;
    private const int MaxFaceCoord = 2;

    // H3 index initialiser: 0x7FFFFFFFFFFFUL — mode=0, res=0, bc=0, all 15 digits set to 7
    private const ulong H3Init = 35184372088831UL;

    // ── Public entry point ────────────────────────────────────────────────────

    internal static ulong LatLngToCell(double latDeg, double lngDeg, int resolution)
    {
        var latRad = latDeg * MPi180;
        var lngRad = lngDeg * MPi180;

        // Step 1: geo → face + hex2d
        GeoToHex2d(latRad, lngRad, resolution, out var face, out var v);

        // Step 2: hex2d → ijk
        Hex2dToCoordIjk(in v, out var ijk);

        // Step 3: faceijk → H3 index
        return FaceIjkToH3(face, ref ijk, resolution);
    }

    // ── Internal algorithm steps ──────────────────────────────────────────────

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Vec3d GeoToVec3d(double latRad, double lngRad)
    {
        var r = Math.Cos(latRad);
        return new Vec3d(Math.Cos(lngRad) * r, Math.Sin(lngRad) * r, Math.Sin(latRad));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double PosAngleRads(double rads)
    {
        var tmp = rads < 0.0 ? rads + M2Pi : rads;
        if (rads >= M2Pi)
        {
            tmp -= M2Pi;
        }

        return tmp;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double GeoAzimuthRads(double lat1, double lng1, double lat2, double lng2)
        => Math.Atan2(
            Math.Cos(lat2) * Math.Sin(lng2 - lng1),
            Math.Cos(lat1) * Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(lng2 - lng1));

    private static void GeoToHex2d(double latRad, double lngRad, int res, out int face, out Vec2d v)
    {
        var p = GeoToVec3d(latRad, lngRad);

        // Find closest face
        face = 0;
        var minSqd = 5.0;
        for (var f = 0; f < 20; f++)
        {
            var sqd = H3IndexData.FaceCenterPoint[f].SquareDist(in p);
            if (sqd < minSqd)
            {
                face = f;
                minSqd = sqd;
            }
        }

        // Great circle distance: cos(r) = 1 - sqd/2
        var r = Math.Acos(1.0 - minSqd * 0.5);

        if (r < Epsilon)
        {
            v = new Vec2d(0.0, 0.0);
            return;
        }

        // Theta: CCW from CII i-axis
        var (fcLat, fcLng) = H3IndexData.FaceCenterGeo[face];
        var theta = PosAngleRads(
            H3IndexData.FaceAxesAzRadsCii[face, 0] - PosAngleRads(GeoAzimuthRads(fcLat, fcLng, latRad, lngRad)));

        if (res % 2 == 1) // Class III
        {
            theta = PosAngleRads(theta - MAp7RotRads);
        }

        // Gnomonic scaling and resolution scaling
        r = Math.Tan(r) * InvRes0UGnomonic;
        for (var i = 0; i < res; i++)
        {
            r *= MSqrt7;
        }

        v = new Vec2d(r * Math.Cos(theta), r * Math.Sin(theta));
    }

    private static void Hex2dToCoordIjk(in Vec2d v, out CoordIJK h)
    {
        // Port of H3's _hex2dToCoordIJK from coordijk.c
        var a1 = Math.Abs(v.X);
        var a2 = Math.Abs(v.Y);

        var x2 = a2 * MRsin60;
        var x1 = a1 + x2 / 2.0;

        var m1 = (int)x1;
        var m2 = (int)x2;

        var r1 = x1 - m1;
        var r2 = x2 - m2;

        int hi, hj;
        h = default;
        h.K = 0;

        if (r1 < 0.5)
        {
            if (r1 < 1.0 / 3.0)
            {
                if (r2 < (1.0 + r1) / 2.0)
                {
                    hi = m1;
                    hj = m2;
                }
                else
                {
                    hi = m1;
                    hj = m2 + 1;
                }
            }
            else
            {
                hj = r2 < 1.0 - r1 ? m2 : m2 + 1;
                hi = (1.0 - r1) <= r2 && r2 < 2.0 * r1 ? m1 + 1 : m1;
            }
        }
        else
        {
            if (r1 < 2.0 / 3.0)
            {
                hj = r2 < 1.0 - r1 ? m2 : m2 + 1;
                hi = (2.0 * r1 - 1.0) < r2 && r2 < 1.0 - r1 ? m1 : m1 + 1;
            }
            else
            {
                if (r2 < r1 / 2.0)
                {
                    hi = m1 + 1;
                    hj = m2;
                }
                else
                {
                    hi = m1 + 1;
                    hj = m2 + 1;
                }
            }
        }

        h.I = hi;
        h.J = hj;

        // Fold across x=0 axis
        if (v.X < 0.0)
        {
            if ((h.J % 2) == 0)
            {
                long axisi = h.J / 2;
                long diff = h.I - axisi;
                h.I = (int)(h.I - 2L * diff);
            }
            else
            {
                long axisi = (h.J + 1) / 2;
                long diff = h.I - axisi;
                h.I = (int)(h.I - (2L * diff + 1));
            }
        }

        // Fold across y=0 axis
        if (v.Y < 0.0)
        {
            h.I -= (2 * h.J + 1) / 2;
            h.J = -h.J;
        }

        h.Normalize();
    }

    private static ulong FaceIjkToH3(int face, ref CoordIJK ijk, int res)
    {
        // Initialize: mode=1, res=res, all digits=7 (INVALID)
        var h = H3Init;
        h = (h & ~(15UL << 59)) | (1UL << 59); // mode = 1
        h = (h & ~(15UL << 52)) | ((ulong)res << 52); // resolution

        if (res == 0)
        {
            if (ijk.I > MaxFaceCoord || ijk.J > MaxFaceCoord || ijk.K > MaxFaceCoord)
            {
                // Guard against numerical edge cases at face boundaries — mirrors the
                // H3_NULL sentinel return in the H3 C source (faceIjkToH3).
                // Unreachable for valid geographic inputs at resolution 0.
                return 0; // H3_NULL
            }

            var (bc0, _) = H3IndexData.FaceIjkBaseCells[face * 27 + ijk.I * 9 + ijk.J * 3 + ijk.K];
            h = (h & ~(127UL << 45)) | ((ulong)bc0 << 45);
            return h;
        }

        // Build digits from finest resolution up
        var faceIjk = ijk;
        for (var r = res - 1; r >= 0; r--)
        {
            var lastIjk = faceIjk;
            CoordIJK lastCenter;

            if ((r + 1) % 2 == 1) // Class III
            {
                faceIjk.UpAp7();
                lastCenter = faceIjk;
                lastCenter.DownAp7();
            }
            else // Class II
            {
                faceIjk.UpAp7r();
                lastCenter = faceIjk;
                lastCenter.DownAp7r();
            }

            var diff = CoordIJK.Subtract(in lastIjk, in lastCenter);
            diff.Normalize();
            var digit = diff.ToDigit();

            // H3_SET_INDEX_DIGIT(h, r+1, digit)
            var shift = (15 - (r + 1)) * 3;
            h = (h & ~(7UL << shift)) | ((ulong)digit << shift);
        }

        // faceIjk now holds the base cell IJK
        if (faceIjk.I > MaxFaceCoord || faceIjk.J > MaxFaceCoord || faceIjk.K > MaxFaceCoord)
        {
            // Guard against numerical edge cases at face boundaries — mirrors the
            // H3_NULL sentinel return in the H3 C source (faceIjkToH3).
            // Unreachable for valid geographic inputs.
            return 0; // H3_NULL
        }

        var (baseCell, numRots) = H3IndexData.FaceIjkBaseCells[face * 27 + faceIjk.I * 9 + faceIjk.J * 3 + faceIjk.K];
        h = (h & ~(127UL << 45)) | ((ulong)baseCell << 45);

        if (H3IndexData.IsBaseCellPentagon[baseCell])
        {
            // Force rotation out of missing K-axes sub-sequence
            if (H3LeadingNonZeroDigit(h, res) == 1) // K_AXES_DIGIT
            {
                var (_, _, _, _, _, cwOff0, cwOff1) = H3IndexData.BaseCellData[baseCell];
                if (cwOff0 == face || cwOff1 == face)
                {
                    h = H3Rotate60Cw(h, res);
                }
                else
                {
                    h = H3Rotate60Ccw(h, res);
                }
            }

            for (var i = 0; i < numRots; i++)
            {
                h = H3RotatePent60Ccw(h, res);
            }
        }
        else
        {
            for (var i = 0; i < numRots; i++)
            {
                h = H3Rotate60Ccw(h, res);
            }
        }

        return h;
    }

    // ── H3 index digit rotation helpers ──────────────────────────────────────

    private static int H3LeadingNonZeroDigit(ulong h, int res)
    {
        for (var r = 1; r <= res; r++)
        {
            var d = GetIndexDigit(h, r);
            if (d != 0)
            {
                return d;
            }
        }

        return 0; // CENTER_DIGIT
    }

    private static ulong H3Rotate60Ccw(ulong h, int res)
    {
        for (var r = 1; r <= res; r++)
        {
            var d = GetIndexDigit(h, r);
            h = SetIndexDigit(h, r, Rotate60Ccw(d));
        }

        return h;
    }

    private static ulong H3Rotate60Cw(ulong h, int res)
    {
        for (var r = 1; r <= res; r++)
        {
            var d = GetIndexDigit(h, r);
            h = SetIndexDigit(h, r, Rotate60Cw(d));
        }

        return h;
    }

    private static ulong H3RotatePent60Ccw(ulong h, int res)
    {
        var foundFirstNonZero = false;
        for (var r = 1; r <= res; r++)
        {
            h = SetIndexDigit(h, r, Rotate60Ccw(GetIndexDigit(h, r)));
            if (!foundFirstNonZero && GetIndexDigit(h, r) != 0)
            {
                foundFirstNonZero = true;
                if (H3LeadingNonZeroDigit(h, res) == 1) // K_AXES_DIGIT
                {
                    h = H3Rotate60Ccw(h, res);
                }
            }
        }

        return h;
    }

    // ── Digit rotation helpers ────────────────────────────────────────────────

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Rotate60Ccw(int digit) => H3IndexData.Rot60CcwMap[digit];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Rotate60Cw(int digit) => H3IndexData.Rot60CwMap[digit];

    // ── H3 index bit manipulation helpers ────────────────────────────────────

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetIndexDigit(ulong h, int r)
        => (int)((h >> ((15 - r) * 3)) & 7UL);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong SetIndexDigit(ulong h, int r, int digit)
    {
        var shift = (15 - r) * 3;
        return (h & ~(7UL << shift)) | ((ulong)digit << shift);
    }
}
