using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static Utils.Program;

namespace Day12
{
    internal static class Program
    {
        private static readonly Dictionary<string, Node> Nodes = new();
        private static void Main(string[] args)
        {
            // Read input
            ForEachInputLine(input =>
            {
                // Parse names
                string[] parts = input.Split("-");
                string startName = parts[0];
                string endName = parts[1];

                // Get or create nodes
                Node start = Nodes.GetOrCreate(startName, () => new Node(startName));
                Node end = Nodes.GetOrCreate(endName, () => new Node(endName));

                // Create connections
                start.AddConnection(end);
            });

            // Count while visiting small caves only once
            Console.WriteLine($"There are {CountAllPaths()} paths to get through the cave");

            // Count while visiting small caves twice
            Console.WriteLine($"There are {CountAllPaths(true)} paths to get through the cave if we got some extra time.");
        }

        private static int CountAllPaths(bool allowDoubleVisits = false)
        {
            int pathCount = 0;
            Node start = Nodes["start"];
            Queue<(List<Node> path, bool visitedAnythingTwice)> front = new();
            front.Enqueue((new List<Node> {start}, false));

            while (front.Count > 0)
            {
                (List<Node> path, bool visitedAnythingTwice) = front.Dequeue();
                Node current = path.Last();

                // If we reached the end, remove from front
                if (current.name == "end")
                {
                    pathCount++;
#if(DEBUG)
                    foreach (Node node in path)
                    {
                        Debug.Write(node.name);
                        if (node.name != "end") Debug.Write(",");
                    }

                    Debug.WriteLine("");
#endif
                    continue;
                }

                // Add all neighbours that are either a big cave, or that have not yet appeared in the current path
                foreach (Node neighbour in current.connections.Where(
                    neighbour => neighbour.isBig
                                 || !path.Contains(neighbour)
                                 || allowDoubleVisits && !visitedAnythingTwice && neighbour.name != "start"
                                 ))
                {
                    if (!neighbour.isBig && path.Contains(neighbour))
                    {
                        front.Enqueue((new List<Node>(path) {neighbour}, true));
                    }
                    else
                    {
                        front.Enqueue((new List<Node>(path) {neighbour}, visitedAnythingTwice));
                    }
                }
            }

            Debug.WriteLine($"Finished counting {pathCount} paths with allow double visits {allowDoubleVisits}");
            return pathCount;
        }
    }
}