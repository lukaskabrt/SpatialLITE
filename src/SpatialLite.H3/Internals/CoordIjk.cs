namespace SpatialLite.H3.Internals;

internal struct CoordIjk
{
    internal int I;
    internal int J;
    internal int K;

    internal CoordIjk(int i, int j, int k)
    {
        I = i;
        J = j;
        K = k;
    }

    internal readonly void Normalize(out CoordIjk result)
    {
        var r = new CoordIjk(I, J, K);
        r.NormalizeInPlace();
        result = r;
    }

    internal void NormalizeInPlace()
    {
        if (I < 0)
        {
            J -= I;
            K -= I;
            I = 0;
        }

        if (J < 0)
        {
            I -= J;
            K -= J;
            J = 0;
        }

        if (K < 0)
        {
            I -= K;
            J -= K;
            K = 0;
        }

        var min = Math.Min(I, Math.Min(J, K));
        if (min > 0)
        {
            I -= min;
            J -= min;
            K -= min;
        }
    }

    internal void UpAp7()
    {
        int i = I - K;
        int j = J - K;
        I = (int)Math.Round((3.0 * i - j) / 7.0);
        J = (int)Math.Round((i + 2.0 * j) / 7.0);
        K = 0;
        NormalizeInPlace();
    }

    internal void UpAp7r()
    {
        int i = I - K;
        int j = J - K;
        I = (int)Math.Round((2.0 * i + j) / 7.0);
        J = (int)Math.Round((3.0 * j - i) / 7.0);
        K = 0;
        NormalizeInPlace();
    }

    internal void DownAp7()
    {
        int ii = I;
        int jj = J;
        int kk = K;
        I = (3 * ii) + kk;
        J = ii + (3 * jj);
        K = jj + (3 * kk);
        NormalizeInPlace();
    }

    internal void DownAp7r()
    {
        int ii = I;
        int jj = J;
        int kk = K;
        I = (3 * ii) + jj;
        J = (3 * jj) + kk;
        K = ii + (3 * kk);
        NormalizeInPlace();
    }

    internal readonly int ToDigit()
    {
        var c = new CoordIjk(I, J, K);
        c.NormalizeInPlace();

        for (int d = 0; d < 7; d++)
        {
            var u = H3Tables.UnitVecs[d];
            if (c.I == u.I && c.J == u.J && c.K == u.K)
            {
                return d;
            }
        }

        return 7;
    }

    internal static CoordIjk Sub(in CoordIjk a, in CoordIjk b)
    {
        var r = new CoordIjk(a.I - b.I, a.J - b.J, a.K - b.K);
        r.NormalizeInPlace();
        return r;
    }
}
