namespace SpatialLite.H3.Internals;

internal readonly struct Vec3d
{
    internal readonly double X;
    internal readonly double Y;
    internal readonly double Z;

    internal Vec3d(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    internal double SquareDist(in Vec3d other)
    {
        var dx = X - other.X;
        var dy = Y - other.Y;
        var dz = Z - other.Z;
        return dx * dx + dy * dy + dz * dz;
    }
}
