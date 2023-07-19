using System;

namespace Qupcakery
{
    public static class DataCompression
    {
        public static int CompressPuzzle(Puzzle puzzle)
        {
            int puzzleSize = puzzle.Size;
            int res = 0;

            for (int i = 0; i < puzzleSize; i++)
            {
                res += puzzle.entries[i].Item1; // Cake
                res = res << 3;
                res += puzzle.entries[i].Item2; // Order
                res = res << 2;
            }

            // Use last 2 bit to store puzzle size
            res += puzzleSize;

            return res;
        }

        public static int[,] DecompressPuzzle(int compressedPuzzle)
        {
            int puzzleSize = compressedPuzzle & 3;
            int[,] puzzle = new int[puzzleSize, 2];
            compressedPuzzle = compressedPuzzle >> 2;

            int cake, order;
            for (int i = puzzleSize - 1; i >= 0; i--)
            {
                order = compressedPuzzle & 7;
                puzzle[i, 1] = order;

                compressedPuzzle = compressedPuzzle >> 3;
                cake = compressedPuzzle & 3;
                puzzle[i, 0] = cake;

                compressedPuzzle = compressedPuzzle >> 2;
            }
            return puzzle;
        }

        public static long CompressPuzzleSolution(int[,] solution, int puzzleSize)
        {
            long res = 0;
            for (int i = 0; i < puzzleSize; i++)
            {
                for (int j = 0; j < Constants.MaxGatePerBelt; j++)
                {
                    res += solution[i, j];
                    res = res << 3;
                }
            }
            res += puzzleSize;
            return res;
        }

        public static int[,] DecrompressPuzzleSolution(long compressedSolution)
        {
            int puzzleSize = (int)compressedSolution & 3;
            compressedSolution = compressedSolution >> 3;
            int[,] solution = new int[Constants.MaxBeltPerBatch, Constants.MaxGatePerBelt];
            for (int i = Constants.MaxBeltPerBatch - 1; i >= 0; i--)
            {
                for (int j = Constants.MaxGatePerBelt - 1; j >= 0; j--)
                {
                    solution[i, j] = (int)compressedSolution & 7;
                    compressedSolution = compressedSolution >> 3;
                }
            }
            return solution;
        }
    }
}

