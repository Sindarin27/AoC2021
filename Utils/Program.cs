using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Utils
{
    public static class Program
    {
        private static void Main()
        {
            Console.WriteLine("You probably shouldn't run this on its own...");
        }
        public static IEnumerable<T> ForEachInputLine<T>(Func<String, T> inputToOutput)
        {
            for (string input; (input = Console.ReadLine() ?? "") != "";)
            {
                yield return inputToOutput(input);
            }
        }
        public static void ForEachInputLine(Action<String> handleInput)
        {
            for (string input; (input = Console.ReadLine() ?? "") != "";)
            {
                handleInput(input);
            }
        }
        
        [Pure]
        public static IEnumerable<U> Scan<T, U>(this IEnumerable<T> input, Func<U, T, U> next, U state) {
            yield return state;
            foreach(T item in input) {
                state = next(state, item);
                yield return state;
            }
        }
        
        public static IEnumerable<T> ReadCommaSeparated<T>(Func<string,T> parser)
        {
            return Console.ReadLine()!.Split(',').Select(parser);
        }
        
        [Pure]
        public static int[] ReadCommaSeparatedCounts()
        {
            return ListToCounts(ReadCommaSeparated(int.Parse).ToList());
        }

        [Pure]
        public static int[] ListToCounts(List<int> list)
        {
            int[] counts = new int[list.Max()];
            foreach (int item in list)
            {
                counts[item]++;
            }

            return counts;
        }
    }
}