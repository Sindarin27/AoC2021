using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using static Utils.Program;

namespace Day3
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            // Read input
            // First read an extra integer to determine the length of the bitstrings
            int stringLength = int.Parse(Console.ReadLine());
            
            // Now make an array. For each bit position, this will keep track of the amount of '1' bits in that position
            int[] positiveCount = new int[stringLength];
            // Also keep track of the total amount of bitstrings
            List<int> bitStrings = new List<int>();

            // Count bits
            ForEachInputLine(input =>
            {
                for (int bitPos = 0; bitPos < input.Length; bitPos++)
                {
                    if (input[bitPos] == '1') positiveCount[bitPos]++;
                }
                bitStrings.Add(Convert.ToInt32(input, 2));
            });

            // gamma rate is the most common bit every time. Create this bitstring as something we can actually use.
            List<bool> bits = positiveCount.Select(occurence => occurence > (bitStrings.Count / 2)).ToList();
            int gamma = bits.Aggregate<bool, int>(0, (acc, bit) => acc << 1 | (bit ? 1 : 0));
            int epsilon = bits.Aggregate<bool, int>(0, (acc, bit) => acc << 1 | (bit ? 0 : 1));
            
            // Calculate part 1
            Console.WriteLine($"Gamma = {Convert.ToString(gamma, 2)} ({gamma}), Epsilon = {Convert.ToString(epsilon, 2)} ({epsilon}). Product = {gamma * epsilon}");

            // Start filtering for part 2
            List<int> oxygenCandidates = new List<int>();
            List<int> co2Candidates = new List<int>();
            int position = stringLength - 1;
            foreach (int bitString in bitStrings)
            {
                if (bits[^(position + 1)] == ((bitString >> position & 0b1) == 1))
                {
                    oxygenCandidates.Add(bitString);
                }
                else
                {
                    co2Candidates.Add(bitString);
                }
            }
            Debug.WriteLine($"Most common bit: {bits[^position]}");
            Debug.WriteLine($"Oxy Candidates: ");
            oxygenCandidates.ForEach(oxy => Debug.WriteLine(Convert.ToString(oxy, 2)));
            Debug.WriteLine($"CO2 Candidates: ");
            co2Candidates.ForEach(co2 => Debug.WriteLine(Convert.ToString(co2, 2)));

            while (oxygenCandidates.Count > 1 || co2Candidates.Count > 1)
            {
                position--;
                if (oxygenCandidates.Count > 1)
                {
                    bool mostCommonBit = CalculateMostCommonBit(co2Candidates, position);
                    Debug.WriteLine($"Most common bit: {mostCommonBit}");
                    oxygenCandidates.RemoveAll(bitString => CalculateMostCommonBit(oxygenCandidates, position) != ((bitString >> position & 0b1) == 1));
                }
                Debug.WriteLine($"Oxy Candidates: ");
                oxygenCandidates.ForEach(oxy => Debug.WriteLine(Convert.ToString(oxy, 2)));
                
                if (co2Candidates.Count > 1)
                {
                    bool mostCommonBit = CalculateMostCommonBit(co2Candidates, position);
                    Debug.WriteLine($"Most common bit: {mostCommonBit}");
                    co2Candidates.RemoveAll(bitString => mostCommonBit == ((bitString >> position & 0b1) == 1));
                }
                Debug.WriteLine($"CO2 Candidates: ");
                co2Candidates.ForEach(co2 => Debug.WriteLine(Convert.ToString(co2, 2)));
            }

            int oxygen = oxygenCandidates.Single();
            int co2 = co2Candidates.Single();
            Console.WriteLine($"Oxygen = {Convert.ToString(oxygen, 2)} ({oxygen}), Co2 = {Convert.ToString(co2, 2)} ({co2}). Product = {oxygen * co2}");
        }

        private static bool CalculateMostCommonBit(IEnumerable<int> bitStrings, int position)
        {
            int[] counts = new int[2];
            foreach (int bitString in bitStrings)
            {
                counts[bitString >> position & 0b1]++;
            }

            return counts[1] >= counts[0];
        }
    }
}