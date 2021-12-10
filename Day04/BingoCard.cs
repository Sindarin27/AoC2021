using System;
using System.Collections.Generic;
using System.Linq;

namespace Day4
{
    public class BingoCard
    {
        // Make lookup table of unchecked numbers so we can find the right coordinates in O(1)
        private Dictionary<byte, (byte x, byte y)> numbers;
        private bool[,] hasBeenChecked = new bool[5,5];
        public BingoCard(byte[,] cardNumbers)
        {
            if (cardNumbers.GetLength(0) != 5 || cardNumbers.GetLength(1) != 5) 
                throw new ArgumentException("Bingo card should be 5x5");
            numbers = new Dictionary<byte, (byte x, byte y)>();
            for (byte x = 0; x < 5; x++)
            for (byte y = 0; y < 5; y++)
                numbers.Add(cardNumbers[x,y], (x,y));
        }

        /// <summary>
        /// Check off a number on the bingo card
        /// </summary>
        /// <param name="number">number to check off</param>
        /// <returns>card score if bingo, null if no bingo</returns>
        public int? Check(byte number)
        {
            if (!numbers.ContainsKey(number)) return null;
            (byte x, byte y) = numbers[number];
            numbers.Remove(number); // Remove number from unchecked list
            hasBeenChecked[x, y] = true; // Remember it has been checked

            return BingoInRow(y) || BingoInColumn(x) ? CalculateScore(number) : null;
        }

        // Calculate score of the card based on the winning number
        private int CalculateScore(byte winningNumber)
        {
            return numbers.Keys.Sum(x => x) * winningNumber;
        }

        // Check whether we have bingo in row y
        private bool BingoInRow(byte y)
        {
            for (byte x = 0; x < 5; x++)
            {
                if (!hasBeenChecked[x, y]) return false;
            }

            return true;
        }

        // Check whether we have bingo in column x
        private bool BingoInColumn(byte x)
        {
            for (byte y = 0; y < 5; y++)
            {
                if (!hasBeenChecked[x, y]) return false;
            }

            return true;
        }
    }
}