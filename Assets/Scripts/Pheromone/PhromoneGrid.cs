using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PhromoneGrid
{
    #region Data Types

    private class PheromoneCompareByAttraction : IComparer<Pheromone>
    {
        public Vector3 From = Vector3.zero;
        public float Limit = 0;

        public int Compare(Pheromone a, Pheromone b)
        {
            return a.Attraction(From, Limit).CompareTo(b.Attraction(From, Limit));
        }
    }

    #endregion

    public PhromoneGrid(int width, int height, float squareSize, float strengthEpsilon = 1e-6f)
    {
        grid = new LinkedList<Pheromone>[width, height];

        SquareSize = squareSize;
        StrengthEpsilon = strengthEpsilon;
    }

    public int Width { get { return grid.GetLength(0); } }

    public int Height { get { return grid.GetLength(1); } }

    public float SquareSize { get; private set; }

    public float StrengthEpsilon { get; private set; }

    private LinkedList<Pheromone>[,] grid;
    private PheromoneCompareByAttraction pheromoneCompareByAttraction = new PheromoneCompareByAttraction();
    private List<Pheromone> pheromones = new List<Pheromone>();

    public void Add(Pheromone pheromone)
    {
        Point p = WorldToGridPosition(pheromone.Position);
        if (grid[p.x, p.y] == null)
            grid[p.x, p.y] = new LinkedList<Pheromone>();

        grid[p.x, p.y].AddFirst(pheromone);
    }

    public List<Pheromone> Search(Vector3 from, float radius)
    {
        pheromones.Clear();
        Point p = WorldToGridPosition(from);
        int offset = Mathf.CeilToInt(radius / SquareSize);
        for (int i = -offset; i <= offset; ++i)
            for (int j = -offset; j <= offset; ++j)
            {
                int x = p.x + i;
                int y = p.y + j;
                if (IsPointInBounds(x, y))
                {
                    var list = grid[x, y];
                    if (list != null)
                    {
                        var node = list.First;
                        while (node != null)
                        {
                            var next = node.Next;
                            var value = node.Value;
                            if (value.Strength < StrengthEpsilon)
                            {
                                list.Remove(node);
                            }
                            else
                            {
                                pheromones.Add(value);
                            }
                            node = next;
                        }
                    }
                }
            }

        pheromoneCompareByAttraction.From = from;
        pheromoneCompareByAttraction.Limit = radius;
        pheromones.Sort(pheromoneCompareByAttraction);
        return pheromones;
    }

    private bool IsPointInBounds(int x, int y)
    {
        return (x >= 0 && x < this.Width
             && y >= 0 && y < this.Height);
    }

    private bool IsPointInBounds(Point p)
    {
        return IsPointInBounds(p.x, p.y);
    }

    private Point WorldToGridPosition(Vector3 position)
    {
        return new Point((int)(position.x / SquareSize), (int)(position.x / SquareSize));
    }

    private Vector3 GridToWorldPosition(Point position)
    {
        return new Vector3(position.x * SquareSize, 0, position.y * SquareSize);
    }

    /*
    // Midpoint circle algorithm
    void drawcircle(int x0, int y0, int radius)
    {
        int x = radius;
        int y = 0;
        int err = 0;

        while (x >= y)
        {
            line(Point((x0 - x, y0 + y), Point(x0 + x, y0 + y));
            line(Point(x0 - y, y0 + x), Point(x0 + y, y0 + x));


            line(Point(x0 - y, y0 - x), Point(x0 + y, y0 - x));

            line(Point(x0 - x, y0 - y), Point(x0 + x, y0 - y));

            y += 1;
            err += 1 + 2 * y;
            if (2 * (err - x) + 1 > 0)
            {
                x -= 1;
                err += 1 - 2 * x;
            }
        }
    }
    */
}
