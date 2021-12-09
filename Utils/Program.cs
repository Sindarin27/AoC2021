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
        
        /// <summary>
        /// From a list of 1d arrays, create a 2d array
        /// From https://stackoverflow.com/a/9775057
        /// </summary>
        /// <param name="arrays">List of 1D arrays</param>
        /// <typeparam name="T">Type of data in array</typeparam>
        /// <returns>2D array</returns>
        /// <exception cref="ArgumentException">Arguments are invalid, for example when not all arrays have the same length</exception>
        public static T[,] CreateRectangularArray<T>(IList<T[]> arrays)
        {
            if (arrays.Count == 0) return new T[0, 0]; 
            int minorLength = arrays[0].Length;
            T[,] ret = new T[arrays.Count, minorLength];
            for (int i = 0; i < arrays.Count; i++)
            {
                T[] array = arrays[i];
                if (array.Length != minorLength)
                {
                    throw new ArgumentException
                        ("All arrays must be the same length");
                }
                for (int j = 0; j < minorLength; j++)
                {
                    ret[i, j] = array[j];
                }
            }
            return ret;
        }

        /// <summary>
        /// Get value at the index of 2D array, return default value if index is out of bounds
        /// </summary>
        /// <param name="array">Array to get value out of</param>
        /// <param name="x">First coordinate of index</param>
        /// <param name="y">Second coordinate of index</param>
        /// <param name="defaultValue">Value to return when index is out of bounds</param>
        /// <typeparam name="T">Type of values</typeparam>
        /// <returns>Item at array[x,y] if inside bounds, defaultValue if out of bounds</returns>
        public static T GetIndexOrDefault<T>(this T[,] array, int x, int y, T defaultValue)
        {
            if (IsOutOfBounds(array, x, y)) return defaultValue;
            return array[x, y];
        }

        /// <summary>
        /// Check whether the given index is inside the bounds of the array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool IsOutOfBounds<T>(this T[,] array, int x, int y)
        {
            return x < 0 || x >= array.GetLength(0) || y < 0 || y >= array.GetLength(1);
        }
    }
}