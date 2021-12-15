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

        /// <summary>
        /// If the key exists in the dictionary, return the associated value.
        /// Otherwise, create the value using the provided method, insert into the dictionary, and return the value.
        /// </summary>
        /// <param name="dictionary">Dictionary to search and insert into</param>
        /// <param name="key">Key to find or insert at</param>
        /// <param name="valueGenerator">Method to create a value if none exists</param>
        /// <typeparam name="T">Type of index in dictionary</typeparam>
        /// <typeparam name="U">Type of value in dictionary</typeparam>
        /// <returns>Value associated with the key in the dictionary</returns>
        public static U GetOrCreate<T, U>(this IDictionary<T, U> dictionary, T key, Func<U> valueGenerator)
        {
            if (dictionary.ContainsKey(key)) return dictionary[key];
            else
            {
                U value = valueGenerator();
                dictionary.Add(key, value);
                return value;
            }
        }

        /// <summary>
        /// If the key exists in the dictionary, increment by given value.
        /// Otherwise, create key in dictionary with given value.
        /// </summary>
        public static void IncrementOrCreate<T>(this IDictionary<T, long> dictionary, T key, long amount)
        {
            if (dictionary.ContainsKey(key)) dictionary[key] += amount;
            else dictionary.Add(key, amount);
        }

        /// <summary>
        /// If the key exists in the dictionary, increment by given value.
        /// Otherwise, create key in dictionary with given value.
        /// </summary>
        public static void IncrementOrCreate<T>(this IDictionary<T, int> dictionary, T key, int amount)
        {
            if (dictionary.ContainsKey(key)) dictionary[key] += amount;
            else dictionary.Add(key, amount);
        }
        
        /// <summary>
        /// Create a reverse lookup version of the given dictionary
        /// From: https://stackoverflow.com/a/22595707
        /// </summary>
        public static Dictionary<TValue, TKey> CreateReverseLookup<TKey, TValue>(this IDictionary<TKey, TValue> source)
        {
            Dictionary<TValue, TKey> dictionary = new Dictionary<TValue, TKey>();
            foreach ((TKey key, TValue value) in source)
            {
                if(!dictionary.ContainsKey(value))
                    dictionary.Add(value, key);
            }
            return dictionary;
        } 

        /// <summary>
        /// Run the trace function on the element and return the element itself
        /// </summary>
        /// <param name="element">Element to run function on and return</param>
        /// <param name="trace">Function to run on element</param>
        /// <typeparam name="T">Type of element</typeparam>
        /// <returns>Element</returns>
        public static T Trace<T>(this T element, Action<T> trace)
        {
            trace(element);
            return element;
        }

        /// <summary>
        /// Returns itself
        /// </summary>
        /// <param name="element">Element to return</param>
        /// <typeparam name="T">Type of element</typeparam>
        /// <returns>Input</returns>
        [Pure] public static T Id<T>(T element) { return element; }


        /// <summary>
        /// It is common to read a "map" of bytes in AOC challenges. This method reads every character one by one,
        /// line by line, until it reaches the end. It will then create a 2D array of bytes based on the numbers in
        /// the input.
        /// </summary>
        /// <returns>2D array of the input</returns>
        public static byte[,] ReadMap()
        {
            return CreateRectangularArray(ForEachInputLine(input =>
            {
                return input.ToCharArray().Select(x => byte.Parse(x.ToString())).ToArray();
            }).ToList());
        } 
    }
}