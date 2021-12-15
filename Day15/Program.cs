using System;
using System.Linq;
using static Utils.Program;
using System.Collections.Generic;
using System.Diagnostics;

namespace Day15
{
    internal static class Program
    {
        private static readonly List<(int xOffset, int yOffset)> offsets = new List<(int, int)>()
        {
            (0,1),
            (0,-1),
            (-1,0),
            (1,0),
        };
        
        private static void Main(string[] args)
        {
            // Read input
            byte[,] map = ReadMap();

            // Perform A* path finding
            State? goalState = DoAStarAndGetCost(map);
            Console.WriteLine(goalState == null ? "Wtf no solution" : $"Path to goal costs {goalState.Value.costSoFar}");

            // First generate bigger map
            int ogWidth = map.GetLength(0);
            int ogHeight = map.GetLength(1);
            int mapWidth = ogWidth * 5;
            int mapHeight = ogHeight * 5;
            byte[,] bigMap = new byte[mapWidth, mapHeight];
            for (int x = 0; x < ogWidth; x++) for (int y = 0; y < ogHeight; y++)
            {
                byte mapValue = map[x, y];
                for (int tileX = 0; tileX < 5; tileX++)
                for (int tileY = 0; tileY < 5; tileY++)
                {
                    byte tileValue = (byte)(mapValue + tileX + tileY);
                    if (tileValue > 9) tileValue -= 9;
                    bigMap[x + ogWidth * tileX, y + ogHeight * tileY] = tileValue;
                }
            }
            
            #if(DEBUG)
            for (int y = 0; y < mapHeight; y++) {
                for (int x = 0; x < mapWidth; x++)
                {
                    Debug.Write(bigMap[x, y]);
                }
                Debug.WriteLine("");
            }
            #endif
            
            // Time for A* again
            State? bigGoalState = DoAStarAndGetCost(bigMap);
            Console.WriteLine(bigGoalState == null ? "Wtf no solution" : $"Path to goal on big map costs {bigGoalState.Value.costSoFar}");
            
            Console.WriteLine($"");
        }

        private static State? DoAStarAndGetCost(byte[,] map)
        {
            PriorityQueue<State, int> front = new();
            front.Enqueue(new State((0, 0)), 0);
            (int x, int y) goal = (map.GetLength(0) - 1, map.GetLength(1) - 1);
            HashSet<(int x, int y)> positionsVisited = new();

            State? goalState = null;
            
            while (front.Count > 0)
            {
                State toExpand = front.Dequeue();
                if (toExpand.currentPos == goal)
                {
                    goalState = toExpand;
                    break;
                }

                if (positionsVisited.Contains(toExpand.currentPos)) continue;
                positionsVisited.Add(toExpand.currentPos);
                foreach (State successor in toExpand.GenerateSuccessors(map))
                {
                    front.Enqueue(successor, HeuristicCost(map, successor.currentPos) + successor.costSoFar);
                }
            }

            return goalState;
        }

        private static int HeuristicCost(byte[,] map, (int x, int y) position)
        {
            return map.GetLength(0) + map.GetLength(1) - 2 - position.x - position.y;
        }

        private struct State
        {
            public State((int x, int y) initialPos) : this()
            {
                path = new List<(int x, int y)>
                    {initialPos};
                currentPos = initialPos;
                costSoFar = 0; // Initial position cost does not count
            }
            public State(State parent, (int x, int y) nextPos, byte[,] map)
            {
                path = new List<(int x, int y)>(parent.path) {nextPos};
                currentPos = nextPos;
                costSoFar = parent.costSoFar + map[nextPos.x, nextPos.y];
            }

            public IEnumerable<State> GenerateSuccessors(byte[,] map)
            {
                (int x, int y) = currentPos;
                foreach ((int xOffset, int yOffset) in offsets)
                {
                    if (map.IsOutOfBounds(x + xOffset, y + yOffset)) continue;
                    (int x, int y) pos = (x + xOffset, y + yOffset);
                    if (this.path.Contains(pos)) continue; // Prevent cycles
                    yield return new State(this, pos, map);
                }
            }
            
            public List<(int x, int y)> path;
            public int costSoFar;
            public (int x, int y) currentPos;
        }
    }
}