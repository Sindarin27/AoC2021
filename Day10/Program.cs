using System;
using System.Collections.Generic;
using System.Linq;
using static Utils.Program;

namespace Day10
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            // Read all input to a simple list of strings
            List<string> strings = ForEachInputLine(s => s).ToList();
            
            
            // Allow easy lookup for all characters
            Dictionary<char, char> openingToClosing = new()
            {
                {'(', ')'},
                {'[', ']'},
                {'{', '}'},
                {'<', '>'},
            };
            Dictionary<char, int> invalidClosingToScore = new()
            {
                {')', 3},
                {']', 57},
                {'}', 1197},
                {'>', 25137},
            };
            Dictionary<char, int> uncompletedOpeningToScore = new()
            {
                {'(', 1},
                {'[', 2},
                {'{', 3},
                {'<', 4},
            };
            HashSet<char> closing = openingToClosing.Values.ToHashSet();
            
            // Perform bracket matching using stacks and calculate score.
            int syntaxScore = strings.Select(input => CalculateSyntaxScore(input, closing, openingToClosing, invalidClosingToScore)).Sum();

            // Print score
            Console.WriteLine($"Score of non-matching brackets: {syntaxScore}!");

            // Filter away invalid lines
            List<string> uncompleted = strings.Where(input => CalculateSyntaxScore(input, closing, openingToClosing, invalidClosingToScore) == 0).ToList();
            
            // Perform auto complete in the same way, save all scores
            List<int> autocompleteScores = uncompleted.Select(input => CalculateAutoCompleteScore(input, closing, uncompletedOpeningToScore)).ToList();
            
            // Find and print median score
            autocompleteScores.Sort();
            int autocompleteScore =  autocompleteScores[autocompleteScores.Count / 2];
            Console.WriteLine($"Score of uncompleted brackets: {autocompleteScore}!");
        }

        private static int CalculateSyntaxScore(string input, HashSet<char> closing, Dictionary<char, char> openingToClosing, Dictionary<char, int> invalidClosingToScore)
        {
            Stack<char> openingBrackets = new();
            foreach (char c in input)
            {
                if (closing.Contains(c))
                {
                    char opening = openingBrackets.Pop();
                    // Brackets match!
                    if (openingToClosing.ContainsKey(opening) && openingToClosing[opening] == c) continue;
                    // Uh oh... return score for line.
                    return invalidClosingToScore[c];
                }

                openingBrackets.Push(c);
            }

            return 0;
        }
        private static int CalculateAutoCompleteScore(string input, IReadOnlySet<char> closing, IReadOnlyDictionary<char, int> uncompletedOpeningToScore)
        {
            Stack<char> openingBrackets = new();
            foreach (char c in input)
            {
                if (closing.Contains(c))
                {
                    // We can assume brackets match now, so just pop
                    openingBrackets.Pop();
                }
                else
                {
                    openingBrackets.Push(c);
                }
            }

            int score = 0;
            while (openingBrackets.Count > 0)
            {
                char uncompletedOpening = openingBrackets.Pop();
                score *= 5;
                score += uncompletedOpeningToScore[uncompletedOpening];
            }

            return score;
        }
    }
}