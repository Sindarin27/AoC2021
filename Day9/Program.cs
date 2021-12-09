using System;
using System.Collections.Generic;
using System.Linq;
using static Utils.Program;

namespace Day9
{
    internal static class Program
    {
        /// Offsets to check for each position
        private static readonly (int x, int y)[] Offsets = {(1, 0), (-1, 0), (0, 1), (0, -1)};
        private static void Main(string[] args)
        {
            // Read input
            byte[,] map = CreateRectangularArray(ForEachInputLine(input =>
            {
                return input.ToCharArray().Select(x => byte.Parse(x.ToString())).ToArray();
            }).ToList());

            // Find sum of all local minima
            List<(int x, int y)> localMinima = FindLocalMinima(map, Offsets).ToList();
            int sumOfLocalMinima = localMinima.Select(coords => map[coords.x, coords.y] + 1).Sum();
            Console.WriteLine($"Sum of risk levels = {sumOfLocalMinima} (with {localMinima.Count} lowest points)");
            
            // Find all basin sizes
            List<int> sizes = localMinima.Select(coords => FindBasinSize(map, coords, Offsets)).ToList();
            sizes.Sort(); sizes.Reverse(); // Sort descending
            int productOfBiggestThreeBasins = sizes.Take(3).Aggregate<int>((x, y) => x * y);
            Console.WriteLine($"The three largest basins have a total area of {productOfBiggestThreeBasins}");
        }

        /// <summary>
        /// Find all local minima in the array according to the provided list of neighbours
        /// </summary>
        /// <param name="array">Array to find local minima in</param>
        /// <param name="offsets">List of offsets for neighbours</param>
        /// <returns>All coordinates of local minima inside the array</returns>
        private static IEnumerable<(int x, int y)> FindLocalMinima(byte[,] array, IList<(int x, int y)> offsets)
        {
            for (int x = 0; x < array.GetLength(0); x++) for (int y = 0; y < array.GetLength(1); y++)
            {
                byte value = array[x, y];
                // If no neighbour exists for which the value is equal or default, this is a local minima.
                if (!offsets.Any(offset => value >= array.GetIndexOrDefault(x + offset.x, y + offset.y, byte.MaxValue))) 
                    yield return (x, y);
            }
        }

        // Find the size of a basin starting at the lowest point
        private static int FindBasinSize(byte[,] array, (int x, int y) lowestPoint, IList<(int x, int y)> offsets)
        {
            // Perform flood fill on the basin to get the size
            Queue<(int x, int y)> front = new();
            HashSet<(int x, int y)> seen = new();
            front.Enqueue(lowestPoint);
            seen.Add(lowestPoint);
            int size = 0;

            while (front.Count > 0)
            {
                (int x, int y) = front.Dequeue();
                if (array.IsOutOfBounds(x, y)) continue;
                if (array[x, y] == 9) continue;
                size++;
                foreach ((int xOffset, int yOffset) in offsets)
                {
                    (int x, int y) coords = (x + xOffset, y + yOffset);
                    if (seen.Contains(coords)) continue;
                    front.Enqueue(coords);
                    seen.Add(coords);
                }
            }

            return size;
        }
    }
}