using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Utils.Program;

namespace Day7
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            // Read input
            List<int> input = ReadCommaSeparated(int.Parse).ToList();

            // For part 1, calculate median
            input.Sort();
            int median = input[(int)Math.Round(input.Count / 2f)];
            int fuelCost = input.Select(x => Math.Abs(x - median)).Sum();
            Console.WriteLine($"Aligning at {median} takes the least fuel, at {fuelCost} units!");

            // For part 2, calculate the same with another distance measure
            // I'm not sure of the optimal method, so let's just do brute force
            int[] values = new int[input.Max()];
            Parallel.For(0, values.Length, i =>
            {
                int totalScore = input.Select(x => SumOfNumbersBelow(Math.Abs(x - i))).Sum();
                values[i] = totalScore;
            });

            int fuelCostWithSums = values.Min();
            Console.WriteLine($"Aligning at [discarded] takes the least fuel, at {fuelCostWithSums} units!");
        }

        /// <summary>
        /// Calculate the sum of numbers in the range [0,n]
        /// </summary>
        private static int SumOfNumbersBelow(int n)
        {
            // n or n+1 is always even, and odd * even = even, thus we can safely divide by integer 2
            return n * (n + 1) / 2;
        }
    }
}