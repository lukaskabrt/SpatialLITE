namespace SpatialLite.H3.Internals;

internal struct CoordIJK
{
    internal int I;
    internal int J;
    internal int K;

    internal CoordIJK(int i, int j, int k)
    {
        I = i;
        J = j;
        K = k;
    }

    // _ijkNormalize: make all components non-negative with minimum component = 0
    internal void Normalize()
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

    // _upAp7: go one aperture-7 class II parent (CCW rotation)
    internal void UpAp7()
    {
        var i = I - K;
        var j = J - K;
        I = (int)Math.Round((3 * i - j) * (1.0 / 7.0), MidpointRounding.AwayFromZero);
        J = (int)Math.Round((i + 2 * j) * (1.0 / 7.0), MidpointRounding.AwayFromZero);
        K = 0;
        Normalize();
    }

    // _upAp7r: go one aperture-7 class III parent (CW rotation)
    internal void UpAp7r()
    {
        var i = I - K;
        var j = J - K;
        I = (int)Math.Round((2 * i + j) * (1.0 / 7.0), MidpointRounding.AwayFromZero);
        J = (int)Math.Round((3 * j - i) * (1.0 / 7.0), MidpointRounding.AwayFromZero);
        K = 0;
        Normalize();
    }

    // _downAp7: class III finer resolution children
    internal void DownAp7()
    {
        // iVec={3,0,1}, jVec={1,3,0}, kVec={0,1,3}
        var newI = 3 * I + 1 * J + 0 * K;
        var newJ = 0 * I + 3 * J + 1 * K;
        var newK = 1 * I + 0 * J + 3 * K;
        I = newI;
        J = newJ;
        K = newK;
        Normalize();
    }

    // _downAp7r: class II finer resolution children
    internal void DownAp7r()
    {
        // iVec={3,1,0}, jVec={0,3,1}, kVec={1,0,3}
        var newI = 3 * I + 0 * J + 1 * K;
        var newJ = 1 * I + 3 * J + 0 * K;
        var newK = 0 * I + 1 * J + 3 * K;
        I = newI;
        J = newJ;
        K = newK;
        Normalize();
    }

    // _ijkRotate60ccw: rotate 60 degrees counter-clockwise
    // iVec={1,1,0}, jVec={0,1,1}, kVec={1,0,1}
    internal void Rotate60Ccw()
    {
        var newI = I + K;
        var newJ = I + J;
        var newK = J + K;
        I = newI;
        J = newJ;
        K = newK;
        Normalize();
    }

    // _ijkRotate60cw: rotate 60 degrees clockwise
    // iVec={1,0,1}, jVec={1,1,0}, kVec={0,1,1}
    internal void Rotate60Cw()
    {
        var newI = I + J;
        var newJ = J + K;
        var newK = I + K;
        I = newI;
        J = newJ;
        K = newK;
        Normalize();
    }

    // _unitIjkToDigit: convert normalized unit vector to direction digit (0-7)
    internal readonly int ToDigit()
    {
        var c = this;
        c.Normalize();
        return (c.I, c.J, c.K) switch
        {
            (0, 0, 0) => 0,
            (0, 0, 1) => 1,
            (0, 1, 0) => 2,
            (0, 1, 1) => 3,
            (1, 0, 0) => 4,
            (1, 0, 1) => 5,
            (1, 1, 0) => 6,
            _ => 7,
        };
    }

    internal static CoordIJK Subtract(in CoordIJK a, in CoordIJK b)
        => new(a.I - b.I, a.J - b.J, a.K - b.K);
}
