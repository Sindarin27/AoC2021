using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Utils.Program;

namespace Day4
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            // Read numbers
            Queue<byte> bingoNumbersCalled = new(Console.ReadLine()!.Split(',').Select(byte.Parse).ToList());
            Console.ReadLine(); // Read empty line

            // Read bingo cards
            bool stop = false;
            List<BingoCard> cards = new();
            while (!stop)
            {
                byte layer = 0;
                byte[,] cardNumbers = new byte[5, 5];
                
                // Each card is separated by an empty line, and ForEachInputLine reads until the next empty line.
                // So each time this gets called it will read exactly one bingo card.
                ForEachInputLine(input =>
                {
                    // Read bingo card line
                    List<byte> nums = input.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(byte.Parse).ToList();
                    for (int i = 0; i < 5; i++) cardNumbers[layer, i] = nums[i];
                    layer++;
                });

                // Got an empty input, which means someone double tapped enter so we're finished
                if (layer == 0)
                {
                    stop = true;
                    continue;
                }
                
                // Make the bingo card and save it
                cards.Add(new BingoCard(cardNumbers));
            }

            int score = -1;
            while (bingoNumbersCalled.Count > 0)
            {
                byte numberCalled = bingoNumbersCalled.Dequeue();
                for (int i = cards.Count - 1; i >= 0; i--)
                {
                    BingoCard card = cards[i];
                    int? maybeScore = card.Check(numberCalled);
                    if (!maybeScore.HasValue) continue;
                    cards.RemoveAt(i);
                    score = maybeScore.Value;
                    // Also check the number on all other cards for star 2
                    cards.RemoveAll(otherCard => otherCard.Check(numberCalled).HasValue);
                    goto FOUND_WINNING;
                }
            }
            FOUND_WINNING:

            // Print the score of the winning card
            Console.WriteLine($"BINGO! Card score: {score}");

            // Filter away all the boards that win, to find the score of the last board
            int losingScore = -1;
            while (bingoNumbersCalled.Count > 0 && cards.Count > 1)
            {
                byte numberCalled = bingoNumbersCalled.Dequeue();
                cards.RemoveAll(card => card.Check(numberCalled).HasValue); // Remove all cards that win
            }

            BingoCard losingCard = cards.Single();
            while (bingoNumbersCalled.Count > 0)
            {
                byte numberCalled = bingoNumbersCalled.Dequeue();
                int? maybeScore = losingCard.Check(numberCalled);
                if (maybeScore.HasValue)
                {
                    losingScore = maybeScore.Value;
                    break;
                }
            }

            // Print losing score
            Console.WriteLine($"Found the way to lose! With a score of {losingScore}");
        }
    }
}