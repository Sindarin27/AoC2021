using System;
using System.Linq;
using static Utils.Program;
using System.Collections.Generic;

namespace Day15
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            // Read input
            byte[,] map = ReadMap();

            // Perform A* path finding
            PriorityQueue<int, int> test = new();
            Console.WriteLine($"");

            // TODO Do stuff for part 2
            Console.WriteLine($"");
        }

        private static int HeuristicCost(byte[,] map, (int x, int y) position)
        {
            return map.GetLength(0) + map.GetLength(1) - 2 - position.x - position.y;
        }
    }
}