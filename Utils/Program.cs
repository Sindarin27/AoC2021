using System;
using System.Collections.Generic;

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
        
        public static IEnumerable<U> Scan<T, U>(this IEnumerable<T> input, Func<U, T, U> next, U state) {
            yield return state;
            foreach(T item in input) {
                state = next(state, item);
                yield return state;
            }
        }
    }
}