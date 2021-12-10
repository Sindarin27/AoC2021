using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Day1
{
    class Program
    {
        static void Main(string[] args)
        {
            List<int> sonarRecordings = new();
            for (string input; (input = Console.ReadLine() ?? "") != "";)
            {
                sonarRecordings.Add(int.Parse(input));
            }
            Console.WriteLine($"Depth increased {CalculateDepthChanges(sonarRecordings).increasing} times");
            Console.WriteLine($"With sliding window, increased {CalculateDepthChangesSlidingWindow(sonarRecordings).increasing} times");
        }

        static (int increasing, int decreasing) CalculateDepthChanges(IList<int> depthRecordings)
        {
            int increasing = 0, decreasing = 0;
            
            // We start at index 1, as there is no previous measurement for i=0
            // This also guarantees there is always a value at index i-1
            for (int i = 1; i < depthRecordings.Count; i++)
            {
                if (depthRecordings[i] > depthRecordings[i - 1]) increasing++;
                // Can also be the same depth, so do not automatically assume decreasing
                else if (depthRecordings[i] < depthRecordings[i - 1]) decreasing++;
            }

            return (increasing, decreasing);
        }

        static (int increasing, int decreasing) CalculateDepthChangesSlidingWindow(IList<int> depthRecordings)
        {
            FixedSizedQueue slidingWindow = new FixedSizedQueue(3);
            int increasing = 0, decreasing = 0;

            foreach (int depthRecording in depthRecordings)
            {
                if (slidingWindow.Count < 3)
                {
                    // Sliding window not full, only enqueue
                    slidingWindow.Enqueue(depthRecording);
                    continue;
                }
                
                // Update sliding window
                int previousWindowValue = slidingWindow.Sum;
                slidingWindow.Enqueue(depthRecording);
                int newWindowValue = slidingWindow.Sum;

                // Check new values
                if (newWindowValue > previousWindowValue) increasing++;
                else if (newWindowValue < previousWindowValue) decreasing++;
            }

            return (increasing, decreasing);
        }
        
        
        // Fixed size queue that automatically removes old values
        // From https://stackoverflow.com/a/10299662
        // Adapted to always keep track of sum of values
        public class FixedSizedQueue : ConcurrentQueue<int>
        {
            private readonly object _syncObject = new object();
            public int Sum { get; private set; }
            public int Size { get; private set; }

            public FixedSizedQueue(int size)
            {
                Size = size;
            }

            public new void Enqueue(int obj)
            {
                base.Enqueue(obj);
                Sum += obj;
                lock (_syncObject)
                {
                    while (Count > Size)
                    {
                        TryDequeue(out int outObj);
                        Sum -= outObj;
                    }
                }
            }
        }
    }
}