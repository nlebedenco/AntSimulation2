using UnityEngine;

public struct Point
{
    public int x;
    public int y;

    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override string ToString()
    {
        return string.Format("({0}, {1})", x, y);
    }

    public override int GetHashCode()
    {
        return this.x.GetHashCode() ^ this.y.GetHashCode() << 2;
    }

    public override bool Equals(object other)
    {
        if (!(other is Point))
            return false;
        Point p = (Point)other;
        if (this.x.Equals(p.x))
            return this.y.Equals(p.y);
        return false;
    }

    public static Point operator +(Point a, Point b)
    {
        return new Point(a.x + b.x, a.y + b.y);
    }

    public static Point operator -(Point a)
    {
        return new Point(-a.x, -a.y);
    }

    public static Point operator -(Point a, Point b)
    {
        return new Point(a.x - b.x, a.y - b.y);
    }

    public static Point operator *(int i, Point a)
    {
        return (a * i);
    }

    public static Point operator *(Point a, int i)
    {
        return new Point(a.x * i, a.y * i);
    }

    public static Point operator /(Point a, int i)
    {
        return new Point(a.x / i, a.y / i);
    }

    public static bool operator ==(Point lhs, Point rhs)
    {
        return lhs.x == rhs.x && lhs.y == rhs.y;
    }

    public static bool operator !=(Point lhs, Point rhs)
    {
        return lhs.x != rhs.x || lhs.y != rhs.y;
    }

    public static implicit operator Point(Vector2 v)
    {
        return new Point((int)v.x, (int)v.y);
    }

    public static implicit operator Vector2(Point p)
    {
        return new Vector2(p.x, p.y);
    }
}
