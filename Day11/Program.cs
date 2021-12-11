using System;
using System.Linq;
using static Utils.Program;

namespace Day11
{
    internal static class Program
    {
        private const byte OCTOPUS_FLASH_ENERGY = 9;
        private static long flashes = 0, steps = 0, flashed = 0; // For once, I'll use a static variable
        private static void Main(string[] args)
        {
            // Read input
            byte[,] map = CreateRectangularArray(ForEachInputLine(input =>
            {
                return input.ToCharArray().Select(x => byte.Parse(x.ToString())).ToArray();
            }).ToList());

            // Calculate first 100 steps
            for (int i = 0; i < 100; i++)
            {
                DoStep(map);
            }
            Console.WriteLine($"Flashed {flashes} times!");

            while (flashed != map.GetLength(0) * map.GetLength(1))
            {
                flashed = 0;
                DoStep(map);
            }
            Console.WriteLine($"Synchronised at step {steps}");
        }

        private static void DoStep(byte[,] map)
        {
            steps++;
            for (int x = 0; x < map.GetLength(0); x++) for (int y = 0; y < map.GetLength(1); y++)
            {
                IncreaseOctopus(map, x, y);
            }
            ResetOctopi(map);
        }
        
        private static void IncreaseOctopus(byte[,] map, int x, int y)
        {
            // Increase octopus
            map[x, y]++;
            
            // If octopus at max, flash
            if (map[x, y] == OCTOPUS_FLASH_ENERGY + 1)
            {
                FlashOctopus(map, x, y);
            }
        }

        private static void FlashOctopus(byte[,] map, int x, int y)
        {
            // Count flash
            flashes++;

            // Find all neighbours inside array bounds
            int minX = Math.Max(x - 1, 0);
            int maxX = Math.Min(x + 1, map.GetLength(0) - 1);
            int minY = Math.Max(y - 1, 0);
            int maxY = Math.Min(y + 1, map.GetLength(1) - 1);
            
            // Increase all neighbouring octopi
            for (int octoX = minX; octoX <= maxX; octoX++) for (int octoY = minY; octoY <= maxY; octoY++)
                IncreaseOctopus(map, octoX, octoY);
        }

        // Reset all flashed octopi in the map
        public static void ResetOctopi(byte[,] map)
        {
            for (int x = 0; x < map.GetLength(0); x++) for (int y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y] > OCTOPUS_FLASH_ENERGY)
                {
                    map[x, y] = 0;
                    flashed++;
                }
            }
        }
    }
}