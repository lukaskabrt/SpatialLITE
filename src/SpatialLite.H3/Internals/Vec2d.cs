namespace SpatialLite.H3.Internals;

internal readonly struct Vec2d
{
    internal Vec2d(double x, double y)
    {
        X = x;
        Y = y;
    }

    internal double X { get; }
    internal double Y { get; }
}
