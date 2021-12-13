using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static Utils.Program;

namespace Day13
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            // Read input points
            // We're using a hashset to easily be able to check if a point was in the same location
            HashSet<(int x, int y)> points = ForEachInputLine(input =>
            {
                int[] coords = input.Split(',').Select(int.Parse).ToArray();
                return (coords[0], coords[1]);
            }).ToHashSet();

            // Read folds
            List<(bool alongX, int index)> folds = ForEachInputLine(input =>
            {
                string[] parts = input.Split().Last().Split("=").ToArray();
                return (parts[0] == "x", int.Parse(parts[1]));
            }).ToList();

            // Count only points in first fold
            (bool alongX, int index) firstFold = folds.First();
            int pointCount = FoldPaper(points, firstFold.alongX, firstFold.index)
                .Trace(p => DrawPaper(p, true))
                .Count;
            Console.WriteLine($"Counted {pointCount} points after the first fold");

            // Calculate positions after folding
            foreach ((bool alongX, int index) in folds)
            {
                // Update points
                points = FoldPaper(points, alongX, index).Trace(p => DrawPaper(p, true));
            }
            
            // Final map
            DrawPaper(points, false);
        }

        private static HashSet<(int x, int y)> FoldPaper(HashSet<(int x, int y)> beforeFold, bool alongX, int index)
        {
            HashSet<(int x, int y)> newPoints = new();
            if (alongX)
            {
                foreach ((int x, int y) in beforeFold)
                {
                    // Point folds, calculate new point and check if it overlaps
                    // If no overlap, add
                    if (x > index)
                    {
                        (int x, int y) newPoint = (2 * index - x, y);
                        if (beforeFold.Contains(newPoint)) continue;
                        newPoints.Add(newPoint);
                    }
                    else newPoints.Add((x, y)); // Point not folded
                }
            }
            else
            {
                foreach ((int x, int y) in beforeFold)
                {
                    // Point folds, calculate new point and check if it overlaps
                    // If no overlap, add
                    if (y > index)
                    {
                        (int x, int y) newPoint = (x, 2 * index - y);
                        if (beforeFold.Contains(newPoint)) continue;
                        newPoints.Add(newPoint);
                    }
                    else newPoints.Add((x, y)); // Point not folded
                }
            }

            return newPoints;
        }
        private static void DrawPaper(HashSet<(int x, int y)> points, bool debug)
        {
#if(RELEASE)
            if (debug) return;
#endif
            int maxX = points.Select(point => point.x).Max();
            int maxY = points.Select(point => point.y).Max();
            bool[,] map = new bool[maxX + 1, maxY + 1];
            foreach ((int x, int y) in points)
            {
                map[x, y] = true;
            }

            for (int y = 0; y < map.GetLength(1); y++)
            {
                for (int x = 0; x < map.GetLength(0); x++)
                {
                    string character = map[x, y] ? "#" : " ";
                    if (debug) Debug.Write(character);
                    else Console.Write(character);
                }

                if (debug) Debug.WriteLine("");
                else Console.WriteLine("");
            }
        }
    }
}