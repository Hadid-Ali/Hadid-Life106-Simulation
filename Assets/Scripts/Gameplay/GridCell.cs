using System;
using UnityEngine;

public class GridCell : IEquatable<GridCell>
{
    private long _x;
    private long _y;

    public long X => _x;
    public long Y => _y;

    public GridCell(long x, long y)
    {
        _x = x;
        _y = y;
    }

    public bool Equals(GridCell other) => _x == other._x && _y == other._y;
    public override bool Equals(object obj) => obj is GridCell other && Equals(other);

    public override int GetHashCode()
    {
        unchecked
        {
            return (_x.GetHashCode() * 397) ^ _y.GetHashCode();
        }
    }

    public override string ToString() => $"{_x} {_y}";
}