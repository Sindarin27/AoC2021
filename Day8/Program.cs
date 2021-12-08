using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using static Utils.Program;

namespace Day8
{
    internal static class Program
    {
        private static readonly int LENGTH_LONGEST_STRING = "abcdefg".Count();
        private static readonly int LENGTH_SHORTEST_STRING = "cf".Count();
        private static void Main(string[] args)
        {
            // Read input
            // Indices 0-9 : "training" data
            // Indices 10-13 : "output" data
            List<string[]> segments = new List<string[]>();
            ForEachInputLine(input => { segments.Add(input.Replace(" | ", " ").Split()); });

            // Count number of output values that certainly correspond to a 1, 4, 7 or 8
            // Aka the number of strings on indices 10-13 that have length 2, 4, 3 or 7
            HashSet<int> lengthsToCount = new() {2, 4, 3, 7};
            int totalKnownCount = segments.Select(segment =>
            {
                // Count in one segment
                byte count = 0;
                for (int i = 10; i <= 13; i++)
                {
                    if (lengthsToCount.Contains(segment[i].Length)) count++;
                }

                return count;
            }).Sum(x => x); // Sum up counts of all segments

            Console.WriteLine($"Number of known digits is {totalKnownCount}");

            // Time to decode.
            // 0 : abcefg
            // 1 : cf
            // 2 : acdeg
            // 3 : acdfg
            // 4 : bcdf
            // 5 : abdfg
            // 6 : abdefg
            // 7 : acf
            // 8 : abcdefg
            // 9 : abcdfg
            // Might as well do it in parallel, no?
            ConcurrentBag<int> numbers = new();
            Parallel.ForEach(segments, segment =>
            {
#if(DEBUG) // As it turns out, every entry contains every number. That makes things a lot easier, as we can determine a simple search structure.
                bool containsOne = segment.Any(s => s.Length == 2);
                bool containsFour = segment.Any(s => s.Length == 4);
                bool containsSeven = segment.Any(s => s.Length == 3);
                bool containsEight = segment.Any(s => s.Length == 7);
                Debug.WriteLine($"Exists in entry: 1 - {containsOne}. 4 - {containsFour}. 7 - {containsSeven}. 8 - {containsEight}");
#endif
                // Put every TRAINING entry into a nice list sorted by their length
                List<string>[] entriesByLength = new List<string>[LENGTH_LONGEST_STRING - LENGTH_SHORTEST_STRING + 1];
                foreach (string entry in segment.Take(10))
                {
                    int index = LengthIndex(entry.Length);
                    if (entriesByLength[index] == null) entriesByLength[index] = new List<string>() {entry};
                    else entriesByLength[index].Add(entry);
                }

                // To start, let us get the known digits
                string one = entriesByLength[LengthIndex(2)].Single();
                string four = entriesByLength[LengthIndex(4)].Single();
                string seven = entriesByLength[LengthIndex(3)].Single();
                string eight = entriesByLength[LengthIndex(7)].Single();

                // Firstly, we can easily deduce the a signal by subtracting the 1 from the 7
                char aString = seven.Except(one).Single();

                // Next, let us find the 3. We can find this as it is the only entry of length 5 that contains both the c and the f
                // (which we know from the 1)
                string three = entriesByLength[LengthIndex(5)].Single(entry => entry.Intersect(one).Count() == one.Length);
                entriesByLength[LengthIndex(5)].Remove(three); // Clean up

                // By subtracting the 4 from the 3 and removing the a as well, we can find the g-string
                char gString = three.Except(four).Except(aString.ToString()).Single();

                // Now by subtracting the 7 from the 3 and removing the g as well, we can find the d-string
                char dString = three.Except(seven).Except(gString.ToString()).Single();

                // We can find the 0 as it is the only six-digit entry with no d-string
                string zero = entriesByLength[LengthIndex(6)].Find(s => !s.Contains(dString));
                entriesByLength[LengthIndex(6)].Remove(zero);

                // The 9 is the only remaining six-digit entry that contains the c AND the f
                string nine = entriesByLength[LengthIndex(6)].Single(entry => entry.Intersect(one).Count() == one.Length);
                entriesByLength[LengthIndex(6)].Remove(nine);

                // The 6 is the only remaining six-digit entry
                string six = entriesByLength[LengthIndex(6)].Single();

                // Using the 6, we can find the c-string by subtracting the 6 from the 1.
                char cString = one.Except(six).Single();

                // The only remaining five-digit entries are the 2 and the 5. Only the 2 contains a c.
                string two = entriesByLength[LengthIndex(5)].Find(s => s.Contains(cString));
                entriesByLength[LengthIndex(5)].Remove(two);
                string five = entriesByLength[LengthIndex(5)].Single();

                // Sort the letters in each string to allow easy lookup
                // Yes, this could be written better
                char[] zeroSort = zero!.ToCharArray();
                Array.Sort(zeroSort);
                char[] oneSort = one.ToCharArray();
                Array.Sort(oneSort);
                char[] twoSort = two!.ToCharArray();
                Array.Sort(twoSort);
                char[] threeSort = three.ToCharArray();
                Array.Sort(threeSort);
                char[] fourSort = four.ToCharArray();
                Array.Sort(fourSort);
                char[] fiveSort = five.ToCharArray();
                Array.Sort(fiveSort);
                char[] sixSort = six.ToCharArray();
                Array.Sort(sixSort);
                char[] sevenSort = seven.ToCharArray();
                Array.Sort(sevenSort);
                char[] eightSort = eight.ToCharArray();
                Array.Sort(eightSort);
                char[] nineSort = nine.ToCharArray();
                Array.Sort(nineSort);

                // Make a lookup table
                // Note that we cannot index on char arrays as arrays are saved by ref
                Dictionary<string, int> charToNum = new()
                {
                    {new string(zeroSort), 0},
                    {new string(oneSort), 1},
                    {new string(twoSort), 2},
                    {new string(threeSort), 3},
                    {new string(fourSort), 4},
                    {new string(fiveSort), 5},
                    {new string(sixSort), 6},
                    {new string(sevenSort), 7},
                    {new string(eightSort), 8},
                    {new string(nineSort), 9},
                };

                // Find the final number
                char[] first = segment[10].ToCharArray();
                char[] second = segment[11].ToCharArray();
                char[] third = segment[12].ToCharArray();
                char[] fourth = segment[13].ToCharArray();
                Array.Sort(first);
                Array.Sort(second);
                Array.Sort(third);
                Array.Sort(fourth);
                numbers.Add(
                    charToNum[new string(first)] * 1000 + 
                    charToNum[new string(second)] * 100 + 
                    charToNum[new string(third)] * 10 + 
                    charToNum[new string(fourth)]
                    );
            });

            int sum = numbers.Sum();
            Console.WriteLine($"The sum of all the numbers is {sum}");
        }

        [Pure]
        private static int LengthIndex(int length)
        {
            return length - LENGTH_SHORTEST_STRING;
        }
    }
}