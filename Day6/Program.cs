using System;
using System.Collections.Generic;
using System.Linq;
using static Utils.Program;

namespace Day6
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            // Read input
            long[] fishes = ReadInitialFish();
            long fishesOnDay7 = 0, fishesOnDay8 = 0;

            for (int day = 0; ; day++)
            {
                long fishesOnDay6 = fishesOnDay7;
                fishesOnDay7 = fishesOnDay8;
                fishesOnDay8 = fishes[day % 7];
                fishes[day % 7] += fishesOnDay6;
                // Print answer to part 1
                if (day == 79) Console.WriteLine($"On day 80, there are {fishes.Sum() + fishesOnDay7 + fishesOnDay8} fish in total");
                if (day == 255) Console.WriteLine($"After 256 days, there are a  whopping {fishes.Sum() + fishesOnDay7 + fishesOnDay8} fish in total!");
                if (day > 255)
                {
                    try
                    {
                        long fishesInTotal = fishes.Sum() + fishesOnDay7 + fishesOnDay8;
                    }
                    catch (System.OverflowException)
                    {
                        Console.WriteLine($"The sea is overflowing with fish on day {day}!");
                        return;
                    }
                }
            }
        }

        private static long[] ReadInitialFish()
        {
            long[] fishes = new long[7];
            IEnumerable<byte> input = Console.ReadLine()!.Split(',').Select(byte.Parse);
            foreach (byte fish in input)
            {
                fishes[fish]++;
            }

            return fishes;
        }
    }
}