using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic;
using static Utils.Program;

namespace Day14
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            char[] polymer = Console.ReadLine()!.ToCharArray();
            Console.ReadLine(); // Read empty line
            
            // Read insertion rules
            Dictionary<(char firstLetter, char secondLetter), char> rules = ForEachInputLine(input =>
            {
                string[] parts = input.Split(" -> ");
                char[] inputChars = parts[0].ToCharArray();
                return (inputChars[0], inputChars[1], parts[1].Single());
            }).ToDictionary(rule => (rule.Item1, rule.Item2), rule => rule.Item3);

            // Aggregate over polymer with replacement rules
            char[] workPolymer = (char[])polymer.Clone();
            for (int i = 0; i < 10; i++)
            {
                workPolymer = AggregatePolymerOnce(workPolymer, rules);
                Debug.WriteLine(new string(workPolymer));
            }

            Dictionary<char,int> histogram = workPolymer.GroupBy(Id).ToDictionary(chars => chars.Key, chars => chars.Count());
            int max = histogram.Max(pair => pair.Value);
            int min = histogram.Min(pair => pair.Value);
            Console.WriteLine($"{max - min}");
            
            // Aggregate over polymer with replacement rules
            // DO NOT DO THIS IT IS SUPER SLOW
            // for (int i = 0; i < 40; i++)
            // {
            //     polymer = AggregatePolymerOnce(polymer, rules);
            //     Debug.WriteLine(new string(polymer));
            //     Console.WriteLine($"Finished iteration {i}");
            // }
            //
            // Dictionary<char,long> reverseHistogramExtra = polymer.GroupBy(Id).ToDictionary(chars => chars.Key, chars => chars.LongCount());
            // long maxExtra = reverseHistogramExtra.Max(pair => pair.Value);
            // long minExtra = reverseHistogramExtra.Min(pair => pair.Value);
            // Console.WriteLine($"{maxExtra - minExtra}");
            
            // Instead of that, we're going lanternfish mode.
            // Let us first generate a mapping from pairname to index
            int counter = 0;
            Dictionary<(char first, char last), int> pairToIndex = rules.ToDictionary(rule => rule.Key, _ => counter++);
            Dictionary<int, (char first, char last)> indexToPair = pairToIndex.CreateReverseLookup();
            
            // Let us reinterpret the rules to go from int to int instead
            Dictionary<int, (int, int)> rulesIndexed = new();
            foreach (KeyValuePair<(char firstLetter, char secondLetter),char> rule in rules)
            {
                (char inFirst, char inSecond) inputChars = rule.Key;
                (char outFirst, char outSecond) outputAChars = (inputChars.inFirst, rule.Value);
                (char outFirst, char outSecond) outputBChars = (rule.Value, inputChars.inSecond);
                int inputIndex = pairToIndex[inputChars];
                int outputAIndex = pairToIndex[outputAChars];
                int outputBIndex = pairToIndex[outputBChars];
                rulesIndexed.Add(inputIndex, (outputAIndex, outputBIndex));
            }
            
            // Let us now reinterpret the string as a bunch of pairs
            long[] polymerIndexed = new long[pairToIndex.Count];
            for (int i = 0; i < polymer.Length - 1; i++)
            {
                char first = polymer[i];
                char second = polymer[i + 1];
                int index = pairToIndex[(first, second)];
                polymerIndexed[index]++;
            }
            
            // Now, let us loop a bit more efficiently.
            // First, let us see if we get the same result for part 1
            for (int i = 0; i < 10; i++)
            {
                polymerIndexed = StepIndexedPolymerOnce(polymerIndexed, rulesIndexed);
            }

            Dictionary<char,long> histogramExtra = GetLetterCounts(polymerIndexed, indexToPair);
            long maxExtra = histogramExtra.Max(pair => pair.Value);
            long minExtra = histogramExtra.Min(pair => pair.Value);
            Console.WriteLine($"With improved method: {maxExtra - minExtra}");
            
            // Now for the grand finale! 30 more steps to go.
            for (int i = 0; i < 30; i++)
            {
                polymerIndexed = StepIndexedPolymerOnce(polymerIndexed, rulesIndexed);
            }

            histogramExtra = GetLetterCounts(polymerIndexed, indexToPair);
            maxExtra = histogramExtra.Max(pair => pair.Value);
            minExtra = histogramExtra.Min(pair => pair.Value);
            Console.WriteLine($"After 40 steps: {maxExtra - minExtra}");
        }

        private static char[] AggregatePolymerOnce(char[] polymer, IReadOnlyDictionary<(char, char), char> rules)
        {
            return polymer.Aggregate((new StringBuilder(""), ' '),
                ((StringBuilder s, char prev) acc, char next) =>
                {
                    var (s, prev) = acc;
                    if (rules.ContainsKey((prev, next)))
                    {
                        s.Append(rules[(prev, next)]);
                    }
                    s.Append(next);
                    return (s, next);
                }, acc => acc.Item1.ToString().ToCharArray());
        }

        private static long[] StepIndexedPolymerOnce(long[] polymer, IReadOnlyDictionary<int, (int, int)> rules)
        {
            long[] newPolymer = new long[polymer.Length];
            for (int index = 0; index < polymer.Length; index++)
            {
                long value = polymer[index];
                (int insertA, int insertB) = rules[index];
                newPolymer[insertA] += value;
                newPolymer[insertB] += value;
            }
            return newPolymer;
        }

        private static Dictionary<char, long> GetLetterCounts(long[] polymer, IReadOnlyDictionary<int, (char, char)> indexToPair)
        {
            Dictionary<char, long> histogram = new();
            for (int i = 0; i < polymer.Length; i++)
            {
                (char first, char last) = indexToPair[i];
                long value = polymer[i];
                histogram.IncrementOrCreate(first, value);
                histogram.IncrementOrCreate(last, value);
            }

            foreach (char histogramKey in histogram.Keys)
            {
                // Nearly every character should appear twice now, as it is included in 2 pairs
                // Except the first and last character. They only appear once.
                // By adding 1 before dividing, we count it anyway.
                histogram[histogramKey] = (histogram[histogramKey] + 1) / 2;
            }

            return histogram;
        }
    }
}