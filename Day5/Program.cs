using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using static Utils.Program;

namespace Day5
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            List<((int x, int y) start, (int x, int y) end)> ventLines = new();
            
            // Read input
            ForEachInputLine(input =>
            {
                int[] parts = input.Replace(" -> ", ",").Split(',').Select(int.Parse).ToArray();
                ventLines.Add(((parts[0], parts[1]), (parts[2], parts[3])));
            });

            // Calculate size and initialize accumulator array
            int maxX = ventLines.Select(tuple => Math.Max(tuple.start.x, tuple.end.x)).Max();
            int maxY = ventLines.Select(tuple => Math.Max(tuple.start.y, tuple.end.y)).Max();
            int[,] accumulator = new int[maxX+1, maxY+1];

            // Add in the horizontal and vertical lines
            foreach (var ventLine in ventLines)
            {
                FillLine(ventLine, accumulator);
            }

            // Count and print day 1
            Console.WriteLine($"Counted {CountDoubles(accumulator)} squares where there's too many vents!");

            // Add in the diagonal lines
            foreach (var ventLine in ventLines)
            {
                FillDiagonalLine(ventLine, accumulator);
            }
            
            // Count and print day 2
            Console.WriteLine($"Added the diagonals! Counted {CountDoubles(accumulator)} now");
        }

        private static int CountDoubles(int[,] accumulator)
        {
            return accumulator.Cast<int>().Count(count => count > 1);
        }

        private static void FillLine(((int x1, int y1) start, (int x2, int y2) end) points, int[,] accumulator)
        {

            ((int x1, int y1), (int x2, int y2)) = points;
            if (x1 != x2 && y1 != y2) return; // Do not yet consider diagonal lines
            if (x1 == x2)
            {
                int yStart = Math.Min(y1, y2);
                int yEnd = Math.Max(y1, y2);
                for (int y = yStart; y <= yEnd; y++)
                {
                    accumulator[x1, y]++;
                }
            }
            else
            {
                int xStart = Math.Min(x1, x2);
                int xEnd = Math.Max(x1, x2);
                for (int x = xStart; x <= xEnd; x++)
                {
                    accumulator[x, y1]++;
                }
            }
        }

        private static void FillDiagonalLine(((int x1, int y1) start, (int x2, int y2) end) points, int[,] accumulator)
        {
            ((int x1, int y1), (int x2, int y2)) = points;
            if (x1 == x2 || y1 == y2) return; // We only consider diagonal lines now

            int xStart, xEnd, yStart, yMultiplier; // y multiplier used for backwards diagonal lines
            if (x1 < x2)
            {
                xStart = x1;
                xEnd = x2;
                yStart = y1;
                yMultiplier = y1 < y2 ? 1 : -1;
            }
            else
            {
                xStart = x2;
                xEnd = x1;
                yStart = y2;
                yMultiplier = y2 < y1 ? 1 : -1;
            }

            int length = xEnd - xStart;

            for (int offset = 0; offset <= length; offset++)
            {
                accumulator[xStart + offset, yStart + offset * yMultiplier]++;
            }
        }
    }
}