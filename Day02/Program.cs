using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using static Utils.Program;

namespace Day2
{
    class Program
    {
        static void Main(string[] args)
        {
            // First, read the input into an understandable format
            List<int> horizontalChanges = new List<int>();
            List<int> verticalChanges = new List<int>();

            ForEachInputLine(input =>
            {
                string[] words = input.Split(' ');
                string direction = words[0];
                int amount = int.Parse(words[1]);

                switch (direction)
                {
                    case "forward":
                        horizontalChanges.Add(amount);
                        verticalChanges.Add(0);
                        break;
                    case "up":
                        horizontalChanges.Add(0);
                        verticalChanges.Add(-amount);
                        break;
                    case "down":
                        horizontalChanges.Add(0);
                        verticalChanges.Add(amount);
                        break;
                }
            });
            
            // Calculate the final position
            int finalHorizontal = horizontalChanges.Sum();
            int finalVertical = verticalChanges.Sum();
            
            // Give answer to part 1
            Console.WriteLine($"Horizontal position * vertical depth = {finalHorizontal * finalVertical}");
            
            // Start calculating part 2
            // Horizontal position calculation stays the same

            IEnumerable<int> aim = verticalChanges.Scan((change, prevAim) => prevAim + change, 0);
            IEnumerable<int> aimVerticalChanges = horizontalChanges.Zip(aim, (horizontalChange, currentAim) => horizontalChange * currentAim);
            int finalAimVertical = aimVerticalChanges.Sum();
            
            Console.WriteLine($"Using Aim: Horizontal position * vertical depth = {finalHorizontal * finalAimVertical}");
        }
    }
}